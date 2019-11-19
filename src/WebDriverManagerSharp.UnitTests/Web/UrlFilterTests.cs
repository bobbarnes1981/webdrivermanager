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
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Web;

    [TestFixture]
    public class UrlFilterTests
    {
        private Mock<ILogger> loggerMock;
        private Mock<IFileStorage> storageMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<ILogger>();
            storageMock = new Mock<IFileStorage>();
        }

        [Test]
        public void TestFilterByArch()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

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
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

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
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

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
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

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
        public void TestFilterByArchNull()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = null;

            Assert.Throws<System.ArgumentNullException>(() => urlFilter.FilterByArch(uris, Architecture.X64, false));
        }

        [Test]
        public void TestFilterByOs()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

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
        public void TestFilterByOsNull()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = null;

            Assert.Throws<System.ArgumentNullException>(() => urlFilter.FilterByOs(uris, OperatingSystem.WIN.ToString()));
        }

        [Test]
        public void TestFilterByDistroNull()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = null;

            Assert.Throws<System.ArgumentNullException>(() => urlFilter.FilterByDistro(uris, "version"));
        }

        [Test]
        public void TestFilterByDistroNoDirectories()
        {
            storageMock.Setup(x => x.DirectoryExists("/etc")).Returns(false);
            storageMock.Setup(x => x.FileExists("/proc/version")).Returns(false);

            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://ubuntu/warty"),
                new System.Uri("http://ubuntu/breezy"),
                new System.Uri("http://ubuntu/dapper"),
            };

            List<System.Uri> output = urlFilter.FilterByDistro(uris, "version");

            Assert.That(output.Count, Is.EqualTo(uris.Count));
        }

        [Test]
        public void TestFilterByDistroEtcExists()
        {
            storageMock.Setup(x => x.DirectoryExists("/etc")).Returns(true);
            storageMock.Setup(x => x.FileExists("/proc/version")).Returns(false);
            storageMock.Setup(x => x.GetFileNames("/etc", "*-release")).Returns(new string[] { "/etc/os-release" });
            storageMock.Setup(x => x.ReadAllLines("/etc/os-release")).Returns(new string[] { "NAME=\"Ubuntu\"", "VERSION=\"16.04 LTS (Xenial Xerus)\"", "ID=ubuntu", "ID_LIKE=debian", "PRETTY_NAME=\"Ubuntu 16.04 LTS\"", "VERSION_ID=\"16.04\"", "HOME_URL=\"http://www.ubuntu.com/\"", "SUPPORT_URL=\"http://help.ubuntu.com/\"", "BUG_REPORT_URL=\"http://bugs.launchpad.net/ubuntu/\"", "UBUNTU_CODENAME=xenial" });

            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://ubuntu/1.2.3/warty"),
                new System.Uri("http://ubuntu/1.2.3/breezy"),
                new System.Uri("http://ubuntu/1.2.3/dapper"),
                new System.Uri("http://ubuntu/1.2.3/xenial"),
            };

            List<System.Uri> output = urlFilter.FilterByDistro(uris, "1.2.3");

            Assert.That(output.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestFilterByDistroVersionExists()
        {
            storageMock.Setup(x => x.DirectoryExists("/etc")).Returns(false);
            storageMock.Setup(x => x.FileExists("/proc/version")).Returns(true);
            storageMock.Setup(x => x.ReadAllLines("/proc/version")).Returns(new string[] { "NAME=\"Ubuntu\"", "VERSION=\"16.04 LTS (Xenial Xerus)\"", "ID=ubuntu", "ID_LIKE=debian", "PRETTY_NAME=\"Ubuntu 16.04 LTS\"", "VERSION_ID=\"16.04\"", "HOME_URL=\"http://www.ubuntu.com/\"", "SUPPORT_URL=\"http://help.ubuntu.com/\"", "BUG_REPORT_URL=\"http://bugs.launchpad.net/ubuntu/\"", "UBUNTU_CODENAME=xenial" });

            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://ubuntu/1.2.3/warty"),
                new System.Uri("http://ubuntu/1.2.3/breezy"),
                new System.Uri("http://ubuntu/1.2.3/dapper"),
                new System.Uri("http://ubuntu/1.2.3/xenial"),
            };

            List<System.Uri> output = urlFilter.FilterByDistro(uris, "1.2.3");

            Assert.That(output.Count, Is.EqualTo(1));
        }

        [Test]
        public void FilterByIgnoredVersionsNull()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = null;

            Assert.Throws<System.ArgumentNullException>(() => urlFilter.FilterByIgnoredVersions(uris, "1.2.3.4"));
        }

        [Test]
        public void FilterByIgnoredVersionsNone()
        {
            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = null;

            Assert.Throws<System.ArgumentNullException>(() => urlFilter.FilterByIgnoredVersions(uris));
        }

        [Test]
        public void FilterByIgnoredVersionsEmpty()
        {
            loggerMock.Setup(x => x.IsTraceEnabled()).Returns(true);

            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = new List<System.Uri>();

            List<System.Uri> output = urlFilter.FilterByIgnoredVersions(uris);

            Assert.That(output.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestFilterByIgnoreVersions()
        {
            loggerMock.Setup(x => x.IsTraceEnabled()).Returns(true);

            UrlFilter urlFilter = new UrlFilter(loggerMock.Object, storageMock.Object);

            List<System.Uri> uris = new List<System.Uri>
            {
                new System.Uri("http://website/1.2.3.4"),
                new System.Uri("http://website/1.2.3.5"),
                new System.Uri("http://website/1.2.4.4"),
                new System.Uri("http://website/1.3.3.4"),
                new System.Uri("http://website/2.2.3.4"),
            };

            List<System.Uri> output = urlFilter.FilterByIgnoredVersions(uris, "1.2.3.4", "1.3.3.4");

            Assert.That(output.Count, Is.EqualTo(3));
        }
    }
}
