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
using System.IO;

namespace WebDriverManager.Tests.Test
{
    /**
     * Test for ignore versions.
     * 
     * @since 1.7.2
     */
    [TestFixture(OperatingSystem.WIN)]
    [TestFixture(OperatingSystem.LINUX)]
    [TestFixture(OperatingSystem.MAC)]
    public class ForceOsTest
    {
        private readonly ILogger log = Logger.GetLogger();

        public OperatingSystem operatingSystem;

        //@InjectMocks
        public Downloader downloader;

        [SetUp]
        public void setup()
        {
            //initMocks(this);
            new DirectoryInfo(downloader.getTargetPath()).Delete(true);

        }

        [TearDown]
        public void teardown()
        {
            new DirectoryInfo(downloader.getTargetPath()).Delete(true);
        }

        [Test]
        public void testForceOs()
        {
            WebDriverManager.chromedriver().operatingSystem(operatingSystem).setup();
            FileInfo binary = new FileInfo(WebDriverManager.chromedriver().getBinaryPath());
            log.Debug("OS {0} - binary path {1}", operatingSystem, binary);
            Assert.True(binary.Exists);
        }
    }
}