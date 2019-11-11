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
using System;
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

        public static string serverPort;

        [OneTimeSetUp]
        public static void startServer()
        {
            serverPort = getFreePort();
            log.Debug("Test is starting WebDriverManager server at port {0}", serverPort);

            WebDriverManager.main(new string[] { "server", serverPort });
        }

        [TestCase("chromedriver", "chromedriver")] // + EXT
        [TestCase("firefoxdriver", "geckodriver")] // + EXT
        [TestCase("operadriver", "operadriver")] // + EXT
        [TestCase("phantomjs", "phantomjs")] // + EXT
        [TestCase("edgedriver", "msedgedriver.exe")]
        [TestCase("iedriver", "IEDriverServer.exe")]
        [TestCase("chromedriver?os=WIN", "chromedriver.exe")]
        [TestCase("chromedriver?os=LINUX&chromeDriverVersion=2.41&forceCache=true", "chromedriver")]
        public void testServer(string path, string driver)
        {
            throw new NotImplementedException();
            //string serverUrl = string.Format("http://localhost:%s/%s", serverPort, path);
            //OkHttpClient client = new OkHttpClient();
            //Request request = new Request.Builder().url(serverUrl).build();

            //// Assert response
            //Response response = client.newCall(request).execute();
            //Assert.True(response.isSuccessful());

            //// Assert attachment
            //string attachment = string.Format("attachment; filename=\"%s\"", driver);

            //List<string> headers = response.headers().values("Content-Disposition");
            //log.Debug("Assessing {} ... {} should contain {}", driver, headers, attachment);
            //Assert.True(headers.Contains(attachment));
        }

        public static string getFreePort()
        {
            //using (ServerSocket socket = new ServerSocket(0))
            //{
            //    return socket.getLocalPort();
            //}
            throw new NotImplementedException();
        }
    }
}