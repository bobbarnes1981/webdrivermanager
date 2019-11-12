/*
 * (C) Copyright 2016 Boni Garcia (http://bonigarcia.github.io/)
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
 * limitations under the License..
 *
 */
using System.Collections.Generic;
using System.IO;

namespace WebDriverManagerSharp
{

    /**
     * Manager for PhantomJs.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.0
     */
    public class PhantomJsDriverManager : WebDriverManager
    {
        protected override DriverManagerType? GetDriverManagerType()
        {
            return DriverManagerType.PHANTOMJS;
        }

        protected override string GetDriverName()
        {
            return "phantomjs";
        }

        protected override string GetDriverVersion()
        {
            return Config().GetPhantomjsDriverVersion();
        }

        protected override System.Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().GetPhantomjsDriverUrl());
        }

        protected override System.Uri GetMirrorUrl()
        {
            return Config().GetPhantomjsDriverMirrorUrl();
        }

        protected override string GetExportParameter()
        {
            return Config().GetPhantomjsDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().SetPhantomjsDriverVersion(version);
        }

        protected override void SetDriverUrl(System.Uri url)
        {
            Config().SetPhantomjsDriverUrl(url);
        }

        protected override List<System.Uri> GetDrivers()// throws IOException
        {
            System.Uri driverUrl = GetDriverUrl();
            Log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());
            return getDriversFromMirror(driverUrl);
        }

        protected override string GetCurrentVersion(System.Uri url, string driverName)
        {
            string file = url.GetFile();
            file = url.GetFile().SubstringJava(file.LastIndexOf(SLASH), file.Length);
            int matchIndex = file.IndexOf(driverName);
            string currentVersion = file
                    .SubstringJava(matchIndex + driverName.Length + 1, file.Length);
            int dashIndex = currentVersion.IndexOf('-');

            if (dashIndex != -1)
            {
                string beta = currentVersion.SubstringJava(dashIndex + 1, dashIndex + 1 + BETA.Length);
                if (beta.Equals(BETA, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    dashIndex = currentVersion.IndexOf('-', dashIndex + 1);
                }
                currentVersion = dashIndex != -1
                        ? currentVersion.SubstringJava(0, dashIndex)
                        : "";
            }
            else
            {
                currentVersion = "";
            }

            return currentVersion;
        }

        public override string PreDownload(string target, string version)
        {
            int iSeparator = target.IndexOf(version) - 1;
            int iDash = target.LastIndexOf(version) + version.Length;
            int iPoint = target.LastIndexOf(".tar") != -1
                    ? target.LastIndexOf(".tar")
                    : target.LastIndexOf(".zip");
            target = target.SubstringJava(0, iSeparator + 1)
                    + target.SubstringJava(iDash + 1, iPoint)
                    + target.SubstringJava(iSeparator);
            target = target.Replace("beta-", "");
            return target;
        }

        public override FileInfo PostDownload(FileInfo archive)
        {
            Log.Trace("PhantomJS package name: {0}", archive);

            DirectoryInfo extractFolder = GetFolderFilter(archive.Directory)[0];
            Log.Trace("PhantomJS extract folder (to be deleted): {0}", extractFolder);

            DirectoryInfo binFolder = new DirectoryInfo(extractFolder.FullName + Path.DirectorySeparatorChar + "bin");
            // Exception for older version of PhantomJS
            int binaryIndex = 0;
            if (!binFolder.Exists)
            {
                binFolder = extractFolder;
                binaryIndex = 3;
            }

            Log.Trace("PhantomJS bin folder: {0} (index {1})", binFolder, binaryIndex);

            FileInfo phantomjs = binFolder.GetFiles()[binaryIndex];
            Log.Trace("PhantomJS binary: {0}", phantomjs);

            FileInfo target = new FileInfo(Path.Combine(archive.Directory.FullName, phantomjs.Name));
            Log.Trace("PhantomJS target: {0}", target);

            downloader.RenameFile(phantomjs, target);
            downloader.DeleteFolder(extractFolder);
            return target;
        }

        protected override string GetBrowserVersion()
        {
            return null;
        }
    }
}