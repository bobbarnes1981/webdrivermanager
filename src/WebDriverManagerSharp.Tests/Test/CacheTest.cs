/*
 * (C) Copyright 2016 Boni Garcia (http://bonigarcia.github.io/)
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
using System.Reflection;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Test for driver cache.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.5
     */
    [TestFixture(DriverManagerType.CHROME, "2.27", Architecture.X32)] // MAC/OSX should be X64
    [TestFixture(DriverManagerType.OPERA, "0.2.2", Architecture.X64)]
    [TestFixture(DriverManagerType.PHANTOMJS, "2.1.1", Architecture.X64)]
    [TestFixture(DriverManagerType.FIREFOX, "0.17.0", Architecture.X64)]
    public class CacheTest
    {
        public DriverManagerType driverManagerType;

        public string driverVersion;

        public Architecture architecture;

        public CacheTest(DriverManagerType driverManagerType, string driverVersion, Architecture architecture)
        {
            this.driverManagerType = driverManagerType;
            this.driverVersion = driverVersion;
            this.architecture = architecture;
        }

        [SetUp]
        [TearDown]
        public void cleanCache()
        {
            new DirectoryInfo(new Downloader(driverManagerType).GetTargetPath()).Delete(true);
        }

        [Test]
        public void testCache()
        {
            WebDriverManager browserManager = WebDriverManager.GetInstance(driverManagerType);
            browserManager.ForceCache().ForceDownload().Architecture(architecture).Version(driverVersion).Setup();

            MethodInfo method = typeof(WebDriverManager).GetMethod("getDriverFromCache", BindingFlags.NonPublic | BindingFlags.Instance);
            FileInfo driverInCachePath = (FileInfo)method.Invoke(browserManager, new object[] { driverVersion, architecture, new Config().getOs() });

            Assert.That(driverInCachePath, Is.Not.Null);
        }
    }
}