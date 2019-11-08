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

using System.Collections.Generic;
using System.IO;

namespace WebDriverManager
{
    /**
     * Manager for Chrome.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class ChromeDriverManager : WebDriverManager
    {
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
            return Config().getChromeDriverVersion();
        }

        protected override System.Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().getChromeDriverUrl());
        }

        protected override System.Uri GetMirrorUrl()
        {
            return Config().getChromeDriverMirrorUrl();
        }

        protected override string GetExportParameter()
        {
            return Config().getChromeDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().setChromeDriverVersion(version);
        }

        protected override void SetDriverUrl(System.Uri url)
        {
            Config().setChromeDriverUrl(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<System.Uri> GetDrivers()
        {
            System.Uri mirrorUrl = GetMirrorUrl();
            if (mirrorUrl != null && Config().isUseMirror())
            {
                return getDriversFromMirror(mirrorUrl);
            }
            else
            {
                return getDriversFromXml(GetDriverUrl());
            }
        }

        protected override string getCurrentVersion(System.Uri url, string driverName)
        {
            if (Config().isUseMirror())
            {
                int i = url.GetFile().LastIndexOf(SLASH);
                int j = url.GetFile().SubstringJava(0, i).LastIndexOf(SLASH) + 1;
                return url.GetFile().SubstringJava(j, i);
            }
            else
            {
                return base.getCurrentVersion(url, driverName);
            }
        }

        protected override string GetBrowserVersion()
        {
            string[] programFilesEnvs = { getProgramFilesEnv(), "LOCALAPPDATA", getOtherProgramFilesEnv() };
            return getDefaultBrowserVersion(programFilesEnvs, "\\\\Google\\\\Chrome\\\\Application\\\\chrome.exe", "google-chrome", "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome", "--version", GetDriverManagerType().ToString());
        }

        protected override string getLatestVersion()
        {
            string url = Config().getChromeDriverUrl() + "LATEST_RELEASE";
            if (config.isUseMirror())
            {
                url = Config().getChromeDriverMirrorUrl() + "LATEST_RELEASE";
            }
            string version = null;
            try
            {
                Stream response = httpClient.executeHttpGet(new System.Uri(url)).Content.ReadAsStreamAsync().Result;
                using (StreamReader reader = new StreamReader(response))
                {
                    version = reader.ReadToEnd();
                }
            }
            catch (System.Exception e)
            {
                log.Warn("Exception reading {0} to get latest version of {1}", url, GetDriverName(), e);
            }
            if (version != null)
            {
                log.Debug("Latest version of {0} according to {1} is {2}", GetDriverName(), url, version);
            }
            return version;
        }

    }
}