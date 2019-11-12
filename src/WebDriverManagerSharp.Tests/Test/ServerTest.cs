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

using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Test using wdm server.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public class ServerTest
    {
        private static ILogger log = Logger.GetLogger();

        public static string EXT = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";

        public static int serverPort;

        [OneTimeSetUp]
        public static void startServer()
        {
            serverPort = getFreePort();
            log.Debug("Test is starting WebDriverManager server at port {0}", serverPort);

            WebDriverManager.main(new string[] { "server", serverPort.ToString() });
        }

        [TestCase("chromedriver", "chromedriver.exe")]
        [TestCase("firefoxdriver", "geckodriver.exe")]
        [TestCase("operadriver", "operadriver.exe")]
        [TestCase("phantomjs", "phantomjs.exe")]
        [TestCase("edgedriver", "msedgedriver.exe")]
        [TestCase("iedriver", "IEDriverServer.exe")]
        [TestCase("chromedriver?os=WIN", "chromedriver.exe")]
        [TestCase("chromedriver?os=LINUX&chromeDriverVersion=2.41&forceCache=true", "chromedriver")]
        public void testServer(string path, string driver)
        {
            string serverUrl = string.Format("http://localhost:{0}/{1}", serverPort, path);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl);

            // Assert response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Assert attachment
            string attachment = string.Format("attachment; filename=\"{0}\"", driver);

            string[] headers = response.Headers.GetValues("Content-Disposition");
            log.Debug("Assessing {0} ... {1} should contain {2}", driver, headers, attachment);
            Assert.That(headers.Where(h => h == attachment).Count, Is.GreaterThan(0));
        }

        public static int getFreePort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}