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

    /**
     * Manager for Chrome.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class ChromeDriverManager : WebDriverManager
    {
        public ChromeDriverManager(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
            : base(config, shell, preferences, logger, fileStorage)
        {
        }

        protected override DriverManagerType? GetDriverManagerType()
        {
            return DriverManagerType.CHROME;
        }

        protected override string GetDriverName()
        {
            return "chromedriver";
        }

        protected override string GetDriverVersion()
        {
            return Config().GetChromeDriverVersion();
        }

        protected override Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().GetChromeDriverUrl());
        }

        protected override Uri GetMirrorUrl()
        {
            return Config().GetChromeDriverMirrorUrl();
        }

        protected override string GetExportParameter()
        {
            return Config().GetChromeDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().SetChromeDriverVersion(version);
        }

        protected override void SetDriverUrl(Uri url)
        {
            Config().SetChromeDriverUrl(url);
        }

        /// <summary>
        /// Get the driver Uris for Chrome
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<Uri> GetDrivers()
        {
            Uri mirrorUrl = GetMirrorUrl();
            if (mirrorUrl != null && Config().IsUseMirror())
            {
                return getDriversFromMirror(mirrorUrl);
            }
            else
            {
                return getDriversFromXml(GetDriverUrl());
            }
        }

        protected override string GetCurrentVersion(Uri url, string driverName)
        {
            if (Config().IsUseMirror())
            {
                int i = url.GetFile().LastIndexOf(SLASH, StringComparison.OrdinalIgnoreCase);
                int j = url.GetFile().SubstringJava(0, i).LastIndexOf(SLASH, StringComparison.OrdinalIgnoreCase) + 1;
                return url.GetFile().SubstringJava(j, i);
            }
            else
            {
                return base.GetCurrentVersion(url, driverName);
            }
        }

        protected override string GetBrowserVersion()
        {
            string[] programFilesEnvs = { getProgramFilesEnv(), "LOCALAPPDATA", getOtherProgramFilesEnv() };
            return GetDefaultBrowserVersion(programFilesEnvs, "\\\\Google\\\\Chrome\\\\Application\\\\chrome.exe", "google-chrome", "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome", "--version", GetDriverManagerType().ToString());
        }

        protected override string getLatestVersion()
        {
            string url;
            if (Config().IsUseMirror())
            {
                url = Config().GetChromeDriverMirrorUrl().Append("LATEST_RELEASE").ToString();
            }
            else
            {
                url = Config().GetChromeDriverUrl().Append("LATEST_RELEASE").ToString();
            }

            string version = null;
            try
            {
                Stream response = HttpClient.ExecuteHttpGet(new Uri(url));
                using (StreamReader reader = new StreamReader(response))
                {
                    version = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Log.Warn("Exception reading {0} to get latest version of {1}", url, GetDriverName(), e);
            }

            if (version != null)
            {
                Log.Debug("Latest version of {0} according to {1} is {2}", GetDriverName(), url, version);
            }

            return version;
        }
    }
}