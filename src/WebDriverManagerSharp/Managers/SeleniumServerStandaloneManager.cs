/*
 * (C) Copyright 2018 Boni Garcia (http://bonigarcia.github.io/)
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
     * Manager for selenium-server-standalone.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.1
     */
    public class SeleniumServerStandaloneManager : WebDriverManager
    {
        public SeleniumServerStandaloneManager(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
            : base(config, shell, preferences, logger, fileStorage)
        {
        }

        protected override DriverManagerType? GetDriverManagerType()
        {
            return DriverManagerType.SELENIUM_SERVER_STANDALONE;
        }

        protected override string GetDriverName()
        {
            return "selenium-server-standalone";
        }

        protected override string GetDriverVersion()
        {
            return Config().GetSeleniumServerStandaloneVersion();
        }

        protected override Uri GetDriverUrl()
        {
            return Config().GetSeleniumServerStandaloneUrl();
        }

        protected override string GetExportParameter()
        {
            return null;
        }

        protected override void SetDriverVersion(string version)
        {
            Config().SetSeleniumServerStandaloneVersion(version);
        }

        protected override void SetDriverUrl(Uri url)
        {
            Config().SetSeleniumServerStandaloneUrl(url);
        }

        public override FileInfo PostDownload(FileInfo archive)
        {
            return archive;
        }

        protected override string GetBrowserVersion()
        {
            return null;
        }

        /// <summary>
        /// Get driver Uris for Selenium Standalone
        /// </summary>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        protected override List<Uri> GetDrivers()
        {
            return getDriversFromXml(GetDriverUrl());
        }
    }
}