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

namespace WebDriverManagerSharp.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Web;

    /**
     * Manager for PhantomJs.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.0
     */
    public class PhantomJsDriverManager : WebDriverManager
    {
        public PhantomJsDriverManager(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
            : base(config, shell, preferences, logger, fileStorage)
        {
        }

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

        protected override Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().GetPhantomjsDriverUrl());
        }

        protected override Uri GetMirrorUrl()
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

        /// <summary>
        /// Get the driver Uris for PhantomJS
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<Uri> GetDrivers()
        {
            Uri driverUrl = GetDriverUrl();
            Log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());
            return getDriversFromMirror(driverUrl);
        }

        protected override string GetCurrentVersion(Uri url, string driverName)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (driverName == null)
            {
                throw new ArgumentNullException(nameof(driverName));
            }

            string file = url.GetFile();
            file = url.GetFile().SubstringJava(file.LastIndexOf(SLASH, StringComparison.OrdinalIgnoreCase), file.Length);
            int matchIndex = file.IndexOf(driverName, StringComparison.OrdinalIgnoreCase);
            string currentVersion = file
                    .SubstringJava(matchIndex + driverName.Length + 1, file.Length);
            int dashIndex = currentVersion.IndexOf('-');

            if (dashIndex != -1)
            {
                string beta = currentVersion.SubstringJava(dashIndex + 1, dashIndex + 1 + BETA.Length);
                if (beta.Equals(BETA, StringComparison.InvariantCultureIgnoreCase))
                {
                    dashIndex = currentVersion.IndexOf('-', dashIndex + 1);
                }

                currentVersion = dashIndex != -1
                        ? currentVersion.SubstringJava(0, dashIndex)
                        : string.Empty;
            }
            else
            {
                currentVersion = string.Empty;
            }

            return currentVersion;
        }

        public override string PreDownload(string target, string version)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            int versionPathPreSeparator = target.IndexOf(version, StringComparison.OrdinalIgnoreCase) - 1;
            int versionFilePostSeparator = target.LastIndexOf(version, StringComparison.OrdinalIgnoreCase) + version.Length;
            int fileExtensionStart = target.LastIndexOf(".tar", StringComparison.OrdinalIgnoreCase) != -1
                    ? target.LastIndexOf(".tar", StringComparison.OrdinalIgnoreCase)
                    : target.LastIndexOf(".zip", StringComparison.OrdinalIgnoreCase);

            string driverFolderPath = target.SubstringJava(0, versionPathPreSeparator + 1);
            string osAndArch = target.SubstringJava(versionFilePostSeparator + 1, fileExtensionStart);
            string fileName = target.SubstringJava(versionPathPreSeparator);
            target = driverFolderPath + osAndArch + fileName;

            target = target.Replace("beta-", string.Empty);

            return target;
        }

        public override FileInfo PostDownload(FileInfo archive)
        {
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

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

            Downloader.RenameFile(phantomjs, target);
            Downloader.DeleteFolder(extractFolder);
            return target;
        }

        protected override string GetBrowserVersion()
        {
            return null;
        }
    }
}