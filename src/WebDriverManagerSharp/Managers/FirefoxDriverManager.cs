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
 * limitations under the License.
 *
 */

namespace WebDriverManagerSharp.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;

    /**
     * Manager for Firefox.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.5.0
     */
    public class FirefoxDriverManager : WebDriverManager
    {
        public FirefoxDriverManager(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
            : base(config, shell, preferences, logger, fileStorage)
        {
        }

        protected override DriverManagerType? GetDriverManagerType()
        {
            return DriverManagerType.FIREFOX;
        }

        protected override string GetDriverName()
        {
            return "geckodriver";
        }

        protected override string GetDriverVersion()
        {
            return Config().GetFirefoxDriverVersion();
        }

        protected override Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().GetFirefoxDriverUrl());
        }

        protected override Uri GetMirrorUrl()
        {
            return Config().GetFirefoxDriverMirrorUrl();
        }

        protected override string GetExportParameter()
        {
            return Config().GetFirefoxDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().SetFirefoxDriverVersion(version);
        }

        protected override void SetDriverUrl(Uri url)
        {
            Config().SetFirefoxDriverUrl(url);
        }

        /// <summary>
        /// Get the driver Uris for Firefox
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<Uri> GetDrivers()
        {
            return getDriversFromGitHub();
        }

        protected override string GetCurrentVersion(Uri url, string driverName)
        {
            string currentVersion = url.GetFile().SubstringJava(
                    url.GetFile().IndexOf('-') + 1, url.GetFile().LastIndexOf('-'));
            if (currentVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                currentVersion = currentVersion.SubstringJava(1);
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
            int extensionStart = target.LastIndexOf(".zip", StringComparison.OrdinalIgnoreCase);
            int tarGzExtensionStart = target.LastIndexOf(".tar.gz", StringComparison.OrdinalIgnoreCase);
            int gzExtensionStart = target.LastIndexOf(".gz", StringComparison.OrdinalIgnoreCase);

            if (tarGzExtensionStart != -1)
            {
                extensionStart = tarGzExtensionStart;
            }
            else if (gzExtensionStart != -1)
            {
                extensionStart = gzExtensionStart;
            }

            string driverFolderPath = target.SubstringJava(0, versionPathPreSeparator + 1);
            string archAndOs = target.SubstringJava(versionFilePostSeparator + 1, extensionStart).ToLower(CultureInfo.InvariantCulture);
            string fileName = target.SubstringJava(versionPathPreSeparator);

            target = driverFolderPath + archAndOs + fileName;

            return target;
        }

        protected override string GetBrowserVersion()
        {
            string[] programFilesEnvs = { getProgramFilesEnv() };
            return GetDefaultBrowserVersion(programFilesEnvs, "\\\\Mozilla Firefox\\\\firefox.exe", "firefox", "/Applications/Firefox.app/Contents/MacOS/firefox", "-v", GetDriverManagerType().ToString());
        }
    }
}