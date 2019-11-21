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

namespace WebDriverManagerSharp
{
    using System.Globalization;
    using System.IO;
    using Nancy;
    using Nancy.Responses;
    using WebDriverManagerSharp.Logging;

    /**
     * WebDriverManager server.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public class Server : NancyModule
    {
        private readonly ILogger log = Resolver.Resolve<ILogger>();

        public Server()
        {
            this.Get["/{driverName}"] = parameters => handleRequest("GET", parameters.driverName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <param name="requestMethod"></param>
        /// <param name="driverName"></param>
        /// <returns></returns>
        private Response handleRequest(string requestMethod, string driverName)
        {
            log.Info("Server request: {0} {1}", requestMethod, driverName);

            WebDriverManager driverManager = createDriverManager(driverName);
            if (driverManager != null)
            {
                return resolveDriver(driverManager);
            }

            return new Response()
            {
                StatusCode = HttpStatusCode.NotFound
            };
        }

        private WebDriverManager createDriverManager(string driverName)
        {
            WebDriverManager outDriver;
            switch (driverName)
            {
                case "chromedriver":
                    outDriver = WebDriverManager.ChromeDriver();
                    break;
                case "firefoxdriver":
                    outDriver = WebDriverManager.FirefoxDriver();
                    break;
                case "edgedriver":
                    outDriver = WebDriverManager.EdgeDriver();
                    break;
                case "iedriver":
                    outDriver = WebDriverManager.IEDriver();
                    break;
                case "operadriver":
                    outDriver = WebDriverManager.OperaDriver();
                    break;
                case "phantomjs":
                    outDriver = WebDriverManager.PhantomJS();
                    break;
                case "selenium-server-standalone":
                    outDriver = WebDriverManager.SeleniumServerStandalone();
                    break;
                default:
                    log.Warn("Unknown option {0}", driverName);
                    outDriver = null;
                    break;
            }

            return outDriver;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driverManager"></param>
        /// <returns></returns>
        /// <exception cref="IOException" />
        private Response resolveDriver(WebDriverManager driverManager)
        {
            // Query string (for configuration parameters)
            DynamicDictionary queryParamMap = this.Context.Request.Query;
            if (queryParamMap != null)
            {
                log.Info("Server query string for configuration {0}", queryParamMap);
                foreach (string key in queryParamMap.Keys)
                {
                    string configKey = "wdm." + key;
                    string configValue = queryParamMap[key];
                    log.Trace("\t{0} = {1}", configKey, configValue);
                    System.Environment.SetEnvironmentVariable(configKey, configValue);
                }
            }

            // Resolve driver
            driverManager.Config().SetAvoidExport(true);
            driverManager.Config().SetAvoidAutoVersion(true);
            driverManager.Setup();
            FileInfo binary = new FileInfo(driverManager.GetBinaryPath());
            string binaryVersion = driverManager.GetDownloadedVersion();
            string binaryName = binary.Name;
            string binaryLength = binary.Length.ToString(CultureInfo.InvariantCulture);

            // Clear configuration
            foreach (string key in queryParamMap.Keys)
            {
                System.Environment.SetEnvironmentVariable("wdm." + key, null);
            }

            // Response
            using (FileStream stream = File.OpenRead(binary.FullName))
            {
                StreamResponse response = new StreamResponse(() => stream, MimeTypes.GetMimeType(binary.FullName))
                {
                    StatusCode = HttpStatusCode.OK
                };
                response.Headers["Content-Disposition"] = "attachment; filename=\"" + binaryName + "\"";

                log.Info("Server response: {0} {1} ({2} bytes)", binaryName, binaryVersion, binaryLength);
                return response;
            }
        }
    }
}