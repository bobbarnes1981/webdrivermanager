/*		
 * (C) Copyright 2017 Boni Garcia (http://bonigarcia.github.io/)		
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
using System.IO;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Test for proxy with mock server.
     * 
     * @since 1.7.2
     */
    public class MockProxyTest
    {
        private readonly ILogger log = Logger.GetLogger();

        //@InjectMocks
        private Downloader downloader;

        //private ClientAndProxy proxy;
        private readonly int proxyPort;

        [SetUp]
        public void Setup()
        {
            DirectoryInfo wdmCache = new DirectoryInfo(downloader.GetTargetPath());
            log.Debug("Cleaning local cache {0}", wdmCache);
            wdmCache.Delete(true);

            //using (ServerSocket serverSocket = new ServerSocket(0))
            //{
            //    proxyPort = serverSocket.getLocalPort();
            //}
            log.Debug("Starting mock proxy on port {0}", proxyPort);
            //proxy = startClientAndProxy(proxyPort);
        }

        [TearDown]
        public void Teardown()
        {
            log.Debug("Stopping mock proxy on port {0}", proxyPort);
            //proxy.stop();
        }

        [Test]
        public void TestMockProx()
        {
            WebDriverManager.ChromeDriver().Proxy("localhost:" + proxyPort).ProxyUser("")
                        .ProxyPass("")
                        .DriverRepositoryUrl(
                                new Uri("https://chromedriver.storage.googleapis.com/"))
                        .Setup();
            FileInfo binary = new FileInfo(WebDriverManager.ChromeDriver().GetBinaryPath());
            Assert.True(binary.Exists);
        }
    }
}