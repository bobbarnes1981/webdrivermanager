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

using System.Collections.Generic;

namespace WebDriverManagerSharp
{
    /**
     * Manager for Firefox.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.5.0
     */
    public class FirefoxDriverManager : WebDriverManager
    {
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
            return Config().getFirefoxDriverVersion();
        }

        protected override System.Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().getFirefoxDriverUrl());
        }

        protected override System.Uri GetMirrorUrl()
        {
            return Config().getFirefoxDriverMirrorUrl();
        }

        protected override string GetExportParameter()
        {
            return Config().getFirefoxDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().setFirefoxDriverVersion(version);
        }

        protected override void SetDriverUrl(System.Uri url)
        {
            Config().setFirefoxDriverUrl(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<System.Uri> GetDrivers()
        {
            return getDriversFromGitHub();
        }

        protected override string getCurrentVersion(System.Uri url, string driverName)
        {
            string currentVersion = url.GetFile().SubstringJava(
                    url.GetFile().IndexOf('-') + 1, url.GetFile().LastIndexOf('-'));
            if (currentVersion.StartsWith("v"))
            {
                currentVersion = currentVersion.SubstringJava(1);
            }
            return currentVersion;
        }

        public override string preDownload(string target, string version)
        {
            int iSeparator = target.IndexOf(version) - 1;
            int iDash = target.LastIndexOf(version) + version.Length;
            int iPoint = target.LastIndexOf(".zip");
            int iPointTazGz = target.LastIndexOf(".tar.gz");
            int iPointGz = target.LastIndexOf(".gz");

            if (iPointTazGz != -1)
            {
                iPoint = iPointTazGz;
            }
            else if (iPointGz != -1)
            {
                iPoint = iPointGz;
            }

            target = target.SubstringJava(0, iSeparator + 1)
                    + target.SubstringJava(iDash + 1, iPoint).ToLower()
                    + target.SubstringJava(iSeparator);
            return target;
        }

        protected override string GetBrowserVersion()
        {
            string[] programFilesEnvs = { getProgramFilesEnv() };
            return getDefaultBrowserVersion(programFilesEnvs, "\\\\Mozilla Firefox\\\\firefox.exe", "firefox", "/Applications/Firefox.app/Contents/MacOS/firefox", "-v", GetDriverManagerType().ToString());
        }

    }
}