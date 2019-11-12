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

using NUnit.Framework;
using System.IO;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Test with Internet Explorer browser.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class IExplorerTest
    {
        private readonly ILogger log = Logger.GetLogger();

        [Test]
        public void testIExplorerLatest()
        {
            WebDriverManager.IEDriver().OperatingSystem(OperatingSystem.WIN).Setup();
            assertIEDriverBinary();
        }

        [Test]
        public void testIExplorerVersion()
        {
            WebDriverManager.IEDriver().OperatingSystem(OperatingSystem.WIN).Version("3.11").Setup();
            assertIEDriverBinary();
        }

        private void assertIEDriverBinary()
        {
            FileInfo binary = new FileInfo(WebDriverManager.IEDriver().GetBinaryPath());
            log.Debug("Binary path for IEDriverServer {0}", binary);
            Assert.True(binary.Exists);
        }
    }
}