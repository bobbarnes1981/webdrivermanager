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

using System.Collections.Generic;
using System.IO;
using WebDriverManagerSharp.FakeJavalin;

namespace WebDriverManagerSharp
{

    /**
     * WebDriverManager server.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public class Server
    {

        private readonly ILogger log = Logger.GetLogger();

        public Server(int port)
        {
            Javalin app = Javalin.Create().Start(port);

            app.Get("/chromedriver", handleRequest);
            app.Get("/firefoxdriver", handleRequest);
            app.Get("/edgedriver", handleRequest);
            app.Get("/iedriver", handleRequest);
            app.Get("/operadriver", handleRequest);
            app.Get("/phantomjs", handleRequest);
            app.Get("/selenium-server-standalone", handleRequest);

            log.Info("WebDriverManager server listening on port {0}", port);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <param name="ctx"></param>
        private void handleRequest(Context ctx)
        {
            string requestMethod = ctx.Method();
            string requestPath = ctx.Path();
            log.Info("Server request: {0} {1}", requestMethod, requestPath);

            WebDriverManager driverManager = createDriverManager(requestPath);
            if (driverManager != null)
            {
                resolveDriver(ctx, driverManager);
            }
        }

        private WebDriverManager createDriverManager(string requestPath)
        {
            WebDriverManager outDriver;
            switch (requestPath.SubstringJava(1))
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
                    log.Warn("Unknown option {0}", requestPath);
                    outDriver = null;
                    break;
            }
            return outDriver;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="driverManager"></param>
        /// <exception cref="IOException" />
        private void resolveDriver(Context ctx, WebDriverManager driverManager)
        {
            // Query string (for configuration parameters)
            Dictionary<string, List<string>> queryParamMap = ctx.QueryParamMap();
            if (queryParamMap != null)
            {
                log.Info("Server query string for configuration {0}", queryParamMap);
                foreach (KeyValuePair<string, List<string>> entry in queryParamMap)
                {
                    string configKey = "wdm." + entry.Key;
                    string configValue = entry.Value[0];
                    log.Trace("\t{0} = {1}", configKey, configValue);
                    System.Environment.SetEnvironmentVariable(configKey, configValue);
                }
            }

            // Resolve driver
            driverManager.Config().setAvoidExport(true);
            driverManager.Config().setAvoidAutoVersion(true);
            driverManager.Setup();
            FileInfo binary = new FileInfo(driverManager.GetBinaryPath());
            string binaryVersion = driverManager.GetDownloadedVersion();
            string binaryName = binary.FullName;
            string binaryLength = binary.Length.ToString();

            // Response
            ctx.Res.SetHeader("Content-Disposition", "attachment; filename=\"" + binaryName + "\"");
            using (FileStream stream = File.OpenRead(binary.FullName))
            {
                ctx.Result(stream);
            }
            log.Info("Server response: {0} {1} ({2} bytes)", binaryName, binaryVersion, binaryLength);

            // Clear configuration
            foreach (string key in queryParamMap.Keys)
            {
                System.Environment.SetEnvironmentVariable("wdm." + key, null);
            }
        }
    }
}