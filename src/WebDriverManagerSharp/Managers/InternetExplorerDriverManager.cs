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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;

    /**
     * Manager for Internet Explorer.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class InternetExplorerDriverManager : WebDriverManager
    {
        public InternetExplorerDriverManager(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
            : base(config, shell, preferences, logger, fileStorage)
        {
        }

        protected override DriverManagerType? GetDriverManagerType()
        {
            return DriverManagerType.IEXPLORER;
        }

        protected override string GetDriverName()
        {
            return "IEDriverServer";
        }

        protected override string GetDriverVersion()
        {
            return Config().GetInternetExplorerDriverVersion();
        }

        protected override System.Uri GetDriverUrl()
        {
            return Config().GetInternetExplorerDriverUrl();
        }

        protected override System.Uri GetMirrorUrl()
        {
            return null;
        }

        protected override string GetExportParameter()
        {
            return Config().GetInternetExplorerDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().SetInternetExplorerDriverVersion(version);
        }

        protected override void SetDriverUrl(System.Uri url)
        {
            Config().SetInternetExplorerDriverUrl(url);
        }

        /// <summary>
        /// Get the driver Uris for Internet Explorer
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<System.Uri> GetDrivers()
        {
            return getDriversFromXml(GetDriverUrl());
        }

        protected override string GetBrowserVersion()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string[] programFilesEnvs = { getProgramFilesEnv() };
                string ieVersion = GetDefaultBrowserVersion(programFilesEnvs, "\\\\Internet Explorer\\\\IExplore.exe", string.Empty, string.Empty, string.Empty, GetDriverManagerType().ToString());
                string browserVersionOutput;
                if (ieVersion != null)
                {
                    browserVersionOutput = ieVersion;
                    Log.Debug("Internet Explorer version {0} found", browserVersionOutput);
                    return browserVersionOutput;
                }
            }

            return null;
        }
    }
}