/*
 * (C) Copyright 2017 Boni Garcia (http://bonigarcia.github.io/)
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

using System;
using System.Collections.Generic;
using System.IO;

namespace WebDriverManager
{
    /**
     * System.Uri filtering logic.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.7.2
     */
    public class UrlFilter
    {
        private readonly ILogger log = Logger.GetLogger();

        public List<Uri> filterByOs(List<Uri> list, string osName)
        {
            log.Trace("System.Uris before filtering by OS ({0}): {1}", osName, list);
            List<Uri> outList = new List<Uri>();

            foreach (Uri url in list)
            {
                foreach (OperatingSystem os in System.Enum.GetValues(typeof(OperatingSystem)))
                {
                    if (((osName.Contains(os.ToString())
                            && url.GetFile().ToUpper().Contains(os.ToString()))
                            || (osName.Equals("mac", StringComparison.InvariantCultureIgnoreCase)
                                    && url.GetFile().ToLower().Contains("osx")))
                            && !outList.Contains(url))
                    {
                        outList.Add(url);
                    }
                }
            }

            log.Trace("System.Uris after filtering by OS ({0}): {1}", osName, outList);
            return outList;
        }

        public List<System.Uri> filterByArch(List<System.Uri> list, Architecture arch, bool forcedArch)
        {
            log.Trace("System.Uris before filtering by architecture ({0}): {1}", arch, list);
            List<System.Uri> outList = new List<System.Uri>(list);

            if ((forcedArch || outList.Count > 1) && arch != null)
            {
                foreach (System.Uri url in list)
                {
                    if (!url.GetFile().Contains("x86")
                            && !url.GetFile().Contains("64")
                            && !url.GetFile().Contains("i686")
                            && !url.GetFile().Contains("32"))
                    {
                        continue;
                    }

                    if (!url.GetFile().Contains(arch.ToString()))
                    {
                        outList.Remove(url);
                    }
                }
            }

            log.Trace("System.Uris after filtering by architecture ({0}): {1}", arch, outList);

            if (outList.Count == 0 && !forcedArch && list.Count != 0)
            {
                outList = new List<System.Uri>() { list[list.Count - 1] };
                log.Trace("Empty System.Uri list after filtering by architecture ... using last candidate: {0}", outList);
            }
            return outList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="version"></param>
        /// <exception cref="IOException" />
        /// <returns></returns>
        public List<System.Uri> filterByDistro(List<System.Uri> list, string version)
        {
            string distro = getDistroName();
            log.Trace("System.Uris before filtering by Linux distribution ({0}): {1}", distro, list);
            List<System.Uri> outList = new List<System.Uri>(list);

            foreach (System.Uri url in list)
            {
                if (url.GetFile().Contains(version) && !url.GetFile().Contains(distro))
                {
                    outList.Remove(url);
                }
            }

            log.Trace("System.Uris after filtering by Linux distribution ({0}): {1}", distro, outList);
            return outList;
        }

        public List<System.Uri> filterByIgnoredVersions(List<System.Uri> list, params string[] ignoredVersions)
        {
            if (log.IsTraceEnabled())
            {
                log.Trace("System.Uris before filtering by ignored versions ({0}): {1}", ignoredVersions.ToStringJava(), list);
            }
            List<System.Uri> outList = new List<System.Uri>(list);

            foreach (System.Uri url in list)
            {
                foreach (string s in ignoredVersions)
                {
                    if (url.GetFile().Contains(s))
                    {
                        log.Info("Ignoring version {0}", s);
                        outList.Remove(url);
                    }
                }
            }

            if (log.IsTraceEnabled())
            {
                log.Trace("System.Uris after filtering by ignored versions ({0}): {1}", ignoredVersions.ToStringJava(), outList);
            }
            return outList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        private static string getDistroName()
        {
            string outString = "";
            string key = "UBUNTU_CODENAME";
            DirectoryInfo dir = new DirectoryInfo(Path.DirectorySeparatorChar + "etc");
            FileInfo[] fileList = Array.Empty<FileInfo>();
            if (dir.Exists)
            {
                fileList = dir.GetFiles("*-release");
            }
            FileInfo fileVersion = new FileInfo(Path.Combine(Path.DirectorySeparatorChar + "proc", "version"));
            if (fileVersion.Exists)
            {
                fileList = fileList.CopyOf(fileList.Length + 1);
                fileList[fileList.Length - 1] = fileVersion;
            }
            foreach (FileInfo f in fileList)
            {
                //if (f.isDirectory()) {
                //    continue;
                //}
                string[] lines = File.ReadAllLines(f.FullName);
                foreach (string line in lines)
                {
                    if (line.Contains(key))
                    {
                        int beginIndex = key.Length;
                        outString = line.SubstringJava(beginIndex + 1);
                    }
                }
            }
            return outString;
        }
    }
}