/*
 * (C) Copyright 2015 Boni Garcia (http://bonigarcia.github.io/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebDriverManager
{
    /**
     * Manager for Microsoft Edge.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.3.0
     */
    public class EdgeDriverManager : WebDriverManager
    {
        protected override DriverManagerType? GetDriverManagerType()
        {
            return DriverManagerType.EDGE;
        }

        protected override string GetDriverName()
        {
            return "msedgedriver";
        }

        protected override string GetDriverVersion()
        {
            return Config().getEdgeDriverVersion();
        }

        protected override System.Uri GetDriverUrl()
        {
            return Config().getEdgeDriverUrl();
        }

        protected override System.Uri GetMirrorUrl()
        {
            return null;
        }

        protected override string GetExportParameter()
        {
            return Config().getEdgeDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().setEdgeDriverVersion(version);
        }

        protected override void SetDriverUrl(System.Uri url)
        {
            Config().setEdgeDriverUrl(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<System.Uri> GetDrivers()
        {
            listVersions = new List<string>();
            List<System.Uri> urlList = new List<System.Uri>();

            System.Uri driverUrl = GetDriverUrl();
            log.Debug("Reading {0} to find out the latest version of Edge driver", driverUrl);

            using (StreamReader inStream = new StreamReader(httpClient.executeHttpGet(driverUrl).Content.ReadAsStreamAsync().Result))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(inStream.ReadToEnd());

                HtmlNodeCollection downloadLink = doc.DocumentNode.SelectNodes("ul.driver-downloads li.driver-download > a");
                HtmlNodeCollection versionParagraph = doc.DocumentNode.SelectNodes("ul.driver-downloads li.driver-download p.driver-download__meta");

                log.Trace("[Original] Download links:\n{0}", downloadLink);
                log.Trace("[Original] Version paragraphs:\n{0}", versionParagraph);

                // Remove non-necessary paragraphs and links elements
                List<HtmlNode> versionParagraphClean = new List<HtmlNode>();
                for (int i = 0; i < versionParagraph.Count(); i++)
                {
                    HtmlNode element = versionParagraph[i];
                    if (element.InnerText.ToLower().StartsWith("version"))
                    {
                        versionParagraphClean.Add(element);
                    }
                }

                log.Trace("[Clean] Download links:\n{0}", downloadLink);
                log.Trace("[Clean] Version paragraphs:\n{0}", versionParagraphClean);

                int shiftLinks = versionParagraphClean.Count() - downloadLink.Count();
                log.Trace("The difference between the size of versions and links is {0}", shiftLinks);

                for (int i = 0; i < versionParagraphClean.Count(); i++)
                {
                    HtmlNode paragraph = versionParagraphClean[i];
                    string[] version = paragraph.InnerText.Split(' ');
                    string v = version[1];
                    listVersions.Add(v);

                    if (isChromiumBased(v))
                    {
                        // Edge driver version 75 and above
                        int childIndex = 0;
                        if (Config().getOs().Equals(OperatingSystem.MAC.ToString()))
                        {
                            childIndex = 2;
                        }
                        else if (Config().getArchitecture() == Architecture.X64)
                        {
                            childIndex = 1;
                        }
                        urlList.Add(new System.Uri(paragraph.ChildNodes[childIndex].Attributes["href"].Value));
                    }
                    else
                    {
                        // Older versions
                        if (!v.Equals("version", System.StringComparison.InvariantCultureIgnoreCase))
                        {
                            urlList.Add(new System.Uri(downloadLink[i - shiftLinks].Attributes["href"].Value));
                        }
                    }
                }

                log.Trace("Edge driver System.Uri list {0}", urlList);
                return urlList;
            }
        }

        public override List<string> getVersions()
        {
            httpClient = new HttpClient(Config());
            try
            {
                GetDrivers();
                listVersions.Sort(new VersionComparator());
                return listVersions;
            }
            catch (IOException e)
            {
                throw new WebDriverManagerException(e);
            }
        }

        protected override List<System.Uri> checkLatest(List<System.Uri> list, string driver)
        {
            log.Trace("Checking the lastest version of {0} with System.Uri list {1}", driver, list);
            List<System.Uri> outList = new List<System.Uri>();
            var iterator = listVersions.GetEnumerator();
            versionToDownload = iterator.Current;
            //outList.Add(iterator.Current);
            throw new System.NotImplementedException();
            log.Info("Latest version of Edge driver is {0}", versionToDownload);
            return outList;
        }

        public override string preDownload(string target, string version)
        {
            if (isChromiumBased(version))
            {
                int iVersion = target.IndexOf(version);
                if (iVersion != -1)
                {
                    target = target.SubstringJava(0, iVersion)
                            + Config().getArchitecture().ToString().ToLower()
                            + Files.Separator + target.SubstringJava(iVersion);
                }
            }
            log.Trace("Pre-download in EdgeDriver -- target={0}, version={1}", target,
                    version);
            return target;
        }

        public override FileInfo postDownload(FileInfo archive)
        {
            List<FileInfo> listFiles = archive.Directory.GetFiles().ToList();
            List<FileInfo>.Enumerator iterator = listFiles.GetEnumerator();
            FileInfo file = null;
            while (iterator.MoveNext())
            {
                file = iterator.Current;
                if (file.FullName.Contains(GetDriverName()))
                {
                    return file;
                }
            }
            return file;
        }

        protected override string GetBrowserVersion()
        {
            if (OsHelper.IsWindows())
            {
                string[] programFilesEnvs = { getProgramFilesEnv() };
                string msedgeVersion = getDefaultBrowserVersion(
                        programFilesEnvs,
                        "\\\\Microsoft\\\\Edge Dev\\\\Application\\\\msedge.exe",
                        "", "", "--version", GetDriverManagerType().ToString());
                string browserVersionOutput;
                if (msedgeVersion != null)
                {
                    browserVersionOutput = msedgeVersion;
                    log.Debug("Edge Dev (based on Chromium) version {0} found", browserVersionOutput);
                }
                else
                {
                    browserVersionOutput = Shell.runAndWait("powershell",
                            "get-appxpackage Microsoft.MicrosoftEdge");
                }
                if (!string.IsNullOrEmpty(browserVersionOutput))
                {
                    return Shell.getVersionFromPowerShellOutput(browserVersionOutput);
                }
            }
            return null;
        }

        private bool isChromiumBased(string version)
        {
            long countDot = version.Count(c => c == '.');
            log.Trace("Edge driver version {0} ({1} dots)", version, countDot);
            return countDot > 1;
        }
    }
}