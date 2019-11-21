/*
 * (C) Copyright 2019 Robert barnes
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

namespace WebDriverManagerSharp
{
    using Nancy.Hosting.Self;
    using System;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Logging;

    public class CommandLineInterface
    {
        public static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            ILogger logger = Resolver.Resolve<ILogger>();

            string validBrowsers = "chrome|firefox|opera|edge|phantomjs|iexplorer|selenium_server_standalone";
            if (args.Length <= 0)
            {
                logCliError(logger, validBrowsers);
            }
            else
            {
                string arg = args[0];
                if (arg.Equals("server", StringComparison.InvariantCultureIgnoreCase))
                {
                    startServer(logger, args);
                }
                else if (arg.Equals("clear-preferences", StringComparison.InvariantCultureIgnoreCase))
                {
                    Resolver.Resolve<IPreferences>().Clear();
                }
                else
                {
                    resolveLocal(logger, validBrowsers, arg);
                }
            }
        }

        private static void resolveLocal(ILogger logger, string validBrowsers, string arg)
        {
            logger.Info("using WebDriverManagerSharp to resolve {0}", arg);
            try
            {
                DriverManagerType driverManagerType = (DriverManagerType)Enum.Parse(typeof(DriverManagerType), arg, true);
                WebDriverManager wdm = WebDriverManager.GetInstance(driverManagerType).AvoidExport().TargetPath(".").ForceDownload();
                if (arg.Equals("edge", StringComparison.InvariantCultureIgnoreCase) || arg.Equals("iexplorer", StringComparison.InvariantCultureIgnoreCase))
                {
                    wdm.OperatingSystem(Enums.OperatingSystem.WIN);
                }

                wdm.AvoidOutputTree().Setup();
            }
            catch (Exception)
            {
                logger.Error("Driver for {0} not found (valid browsers {1})", arg, validBrowsers);
            }
        }

        private static void startServer(ILogger logger, string[] args)
        {
            int port;
            if (args.Length < 2 || !int.TryParse(args[1], out port))
            {
                port = Resolver.Resolve<IConfig>().GetServerPort();
            }

            new NancyHost(
                new HostConfiguration()
                {
                    UrlReservations = new UrlReservations()
                    {
                        CreateAutomatically = true
                    }
                },
                new Uri("http://localhost:" + port)).Start();

            logger.Info("WebDriverManager server listening on port {0}", port);
        }

        private static void logCliError(ILogger logger, string validBrowsers)
        {
            logger.Error("There are 3 options to run WebDriverManager CLI");
            logger.Error("1. WebDriverManager used to resolve binary drivers locally:");
            logger.Error("\tWebDriverManager browserName");
            logger.Error("\t(where browserName={0})", validBrowsers);

            logger.Error("2. WebDriverManager as a server:");
            logger.Error("\tWebDriverManager server <port>");
            logger.Error("\t(where default port is 4041)");

            logger.Error("3. To clear previously resolved driver versions (as Java preferences):");
            logger.Error("\tWebDriverManager clear-preferences");
        }
    }
}
