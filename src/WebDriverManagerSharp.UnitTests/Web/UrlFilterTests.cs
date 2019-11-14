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

namespace WebDriverManager.UnitTests.Web
{
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Web;

    [TestFixture]
    public class UrlFilterTests
    {
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<ILogger>();
        }

        [Test]
        public void TestFilterByArch()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://website.com/x86/driver.exe"),
                new System.Uri("http://website.com/64/driver.exe"),
                new System.Uri("http://website.com/i686/driver.exe"),
                new System.Uri("http://website.com/32/driver.exe"),
            };

            List<System.Uri> x32List = urlFilter.FilterByArch(uris, Architecture.X32, false);

            Assert.That(x32List.Count, Is.EqualTo(1));

            List<System.Uri> x64List = urlFilter.FilterByArch(uris, Architecture.X64, false);

            Assert.That(x64List.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestFilterByArchNone()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://website.com/x86/driver.exe"),
                new System.Uri("http://website.com/none/driver.exe"),
                new System.Uri("http://website.com/i686/driver.exe"),
                new System.Uri("http://website.com/32/driver.exe"),
            };

            List<System.Uri> x64List = urlFilter.FilterByArch(uris, Architecture.X64, false);

            Assert.That(x64List.Count, Is.EqualTo(1));
            Assert.That(x64List[0].ToString(), Is.EqualTo("http://website.com/none/driver.exe"));
        }

        [Test]
        public void TestFilterByArchOnlyOne()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://website.com/64/driver.exe"),
            };

            List<System.Uri> x64List = urlFilter.FilterByArch(uris, Architecture.X64, false);

            Assert.That(x64List.Count, Is.EqualTo(1));
            Assert.That(x64List[0].ToString(), Is.EqualTo("http://website.com/64/driver.exe"));
        }

        [Test]
        public void TestFilterByArchTwoNonMatching()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://website.com/i686/driver.exe"),
                new System.Uri("http://website.com/32/driver.exe"),
            };

            List<System.Uri> x64List = urlFilter.FilterByArch(uris, Architecture.X64, false);

            Assert.That(x64List.Count, Is.EqualTo(1));
            Assert.That(x64List[0].ToString(), Is.EqualTo("http://website.com/32/driver.exe"));
        }

        [Test]
        public void TestFilterByOs()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://website.com/win/driver.exe"),
                new System.Uri("http://website.com/mac/driver.exe"),
                new System.Uri("http://website.com/osx/driver.exe"),
                new System.Uri("http://website.com/linux/driver.exe"),
            };

            List<System.Uri> winList = urlFilter.FilterByOs(uris, OperatingSystem.WIN.ToString());

            Assert.That(winList.Count, Is.EqualTo(1));

            List<System.Uri> macList = urlFilter.FilterByOs(uris, OperatingSystem.MAC.ToString());

            Assert.That(macList.Count, Is.EqualTo(2));

            List<System.Uri> linuxList = urlFilter.FilterByOs(uris, OperatingSystem.LINUX.ToString());

            Assert.That(linuxList.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestFilterByDistro()
        {
            // TODO:
        }

        [Test]
        public void TestFilterByIgnoreVersions()
        {
            // TODO:
        }
    }
}
