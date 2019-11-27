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

namespace WebDriverManagerSharp.UnitTests.Managers
{
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http.Headers;
    using System.Text;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Managers;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Logging;
    using Autofac;
    using WebDriverManagerSharp.Web;

    [TestFixture]
    public class PhantomJsDriverManagerTests : BaseManagerTests
    {
        [Test]
        public void GetVersions()
        {
            Uri driverUrl = new Uri("https://fake.bitbucket.org/ariya/phantomjs/downloads/");
            configMock.Setup(x => x.GetPhantomjsDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");

            string fakeHtml = "<a class=\"execute\" rel=\"nofollow\" href=\"/ariya/phantomjs/downloads/phantomjs-2.5.0-beta2-windows.zip\">phantomjs-2.5.0-beta2-windows.zip</a>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));

            List<string> versions = WebDriverManager.PhantomJS().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetVersionsNoHyphen()
        {
            Uri driverUrl = new Uri("https://fake.bitbucket.org/ariya/phantomjs/downloads/");
            configMock.Setup(x => x.GetPhantomjsDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");

            string fakeHtml = "<a class=\"execute\" rel=\"nofollow\" href=\"/ariya/phantomjs/downloads/phantomjs.zip\">phantomjs.zip</a>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));

            List<string> versions = WebDriverManager.PhantomJS().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetVersionsNullUrl()
        {
            Uri driverUrl = null;
            configMock.Setup(x => x.GetPhantomjsDriverUrl()).Returns(driverUrl);

            Resolver.OverriddenRegistrations += (builder) =>
            {
                builder.RegisterType<PhantomJsDriverManagerAccessor>();
            };

            PhantomJsDriverManagerAccessor manager = Resolver.Resolve<PhantomJsDriverManagerAccessor>();

            Assert.Throws<ArgumentNullException>(() => manager.GetCurrentVersion(null, ""));
        }

        [Test]
        public void GetVersionsNullDriverName()
        {
            Uri driverUrl = null;
            configMock.Setup(x => x.GetPhantomjsDriverUrl()).Returns(driverUrl);

            Resolver.OverriddenRegistrations += (builder) =>
            {
                builder.RegisterType<PhantomJsDriverManagerAccessor>();
            };

            PhantomJsDriverManagerAccessor manager = Resolver.Resolve<PhantomJsDriverManagerAccessor>();

            Assert.Throws<ArgumentNullException>(() => manager.GetCurrentVersion(new Uri("http://uri"), null));
        }

        private class PhantomJsDriverManagerAccessor : PhantomJsDriverManager
        {
            public PhantomJsDriverManagerAccessor(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
                : base(config, shell, preferences, logger, fileStorage)
            {
            }

            public string GetCurrentVersion(Uri url, string driverName)
            {
                return base.GetCurrentVersion(url, driverName);
            }
        }

        [Test]
        public void GetVersionsMirror()
        {
            Uri driverUrl = new Uri("http://fake.npm.taobao.org/mirrors/phantomjs");
            configMock.Setup(x => x.GetPhantomjsDriverMirrorUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeJson = "<a href=\"/mirrors/phantomjs/phantomjs-1.9.2-windows.zip\">phantomjs-1.9.2-windows.zip</a>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            List<string> versions = WebDriverManager.PhantomJS().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void SetVersionLatest()
        {
            WebDriverManager.PhantomJS().Version("latest");

            configMock.Verify(x => x.SetPhantomjsDriverVersion("latest"), Times.Once);
        }

        [Test]
        public void SetDriverRepositoryUrl()
        {
            Uri uri = new Uri("http://www.uri.com");

            WebDriverManager.PhantomJS().DriverRepositoryUrl(uri);

            configMock.Verify(x => x.SetPhantomjsDriverUrl(uri), Times.Once);
        }

        [Test]
        public void TestSetUp()
        {
            Uri driverUrl = new Uri("https://fake.bitbucket.org/ariya/phantomjs/downloads/");
            configMock.Setup(x => x.GetPhantomjsDriverUrl()).Returns(driverUrl);

            string fakeHtml = "<a class=\"execute\" rel=\"nofollow\" href=\"/ariya/phantomjs/downloads/phantomjs-2.5.0-windows.zip\">phantomjs-2.5.0-windows.zip</a>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Storage.File("c:\\config_target\\driver.exe"));

            WebDriverManager.PhantomJS().Setup();

            configMock.Verify(x => x.GetPhantomjsDriverExport(), Times.Once);
        }

        [Test]
        public void TestSetUpMirror()
        {
            Uri driverUrl = new Uri("http://fake.npm.taobao.org/mirrors/phantomjs");
            configMock.Setup(x => x.GetPhantomjsDriverMirrorUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeJson = "<a href=\"/mirrors/phantomjs/phantomjs-1.9.2-windows.zip\">phantomjs-1.9.2-windows.zip</a>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Storage.File("c:\\config_target\\driver.exe"));

            WebDriverManager.PhantomJS().Setup();

            configMock.Verify(x => x.GetPhantomjsDriverExport(), Times.Once);
        }

        [Test]
        public void PreDownloadNullTarget()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.PhantomJS().PreDownload(null, ""));
        }

        [Test]
        public void PreDownloadNullVersion()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.PhantomJS().PreDownload("", null));
        }

        [TestCase("C:\\Users\\robertb\\.m2\\repository\\webdriver\\phantomjs\\2.1.1\\phantomjs-2.1.1-windows.zip", "2.1.1", "C:\\Users\\robertb\\.m2\\repository\\webdriver\\phantomjs\\windows\\2.1.1\\phantomjs-2.1.1-windows.zip")]
        [TestCase("C:\\Users\\robertb\\.m2\\repository\\webdriver\\phantomjs\\2.1.1\\phantomjs-2.1.1-beta-windows.zip", "2.1.1", "C:\\Users\\robertb\\.m2\\repository\\webdriver\\phantomjs\\windows\\2.1.1\\phantomjs-2.1.1-windows.zip")]
        [TestCase("C:\\Users\\robertb\\.m2\\repository\\webdriver\\phantomjs\\2.1.1\\phantomjs-2.1.1-linux-i686.tar.bz2", "2.1.1", "C:\\Users\\robertb\\.m2\\repository\\webdriver\\phantomjs\\linux-i686\\2.1.1\\phantomjs-2.1.1-linux-i686.tar.bz2")]
        public void PreDownload(string file, string version, string expected)
        {
            string target = WebDriverManager.PhantomJS().PreDownload(file, version);

            Assert.That(target, Is.EqualTo(expected));
        }

        [Test]
        public void PostDownloadNull()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.PhantomJS().PostDownload(null));
        }

        [Test]
        public void PostDownloadWithBin()
        {
            Resolver.OverriddenRegistrations += (builder) =>
            {
                builder.RegisterInstance(new Mock<IDownloader>().Object).As<IDownloader>();
            };

            Mock<IFile> driverFileMock = new Mock<IFile>();
            driverFileMock.Setup(x => x.Name).Returns("phantomjs.exe");

            Mock<IDirectory> binDirectoryMock = new Mock<IDirectory>();
            binDirectoryMock.Setup(x => x.Name).Returns("bin");
            binDirectoryMock.Setup(x => x.Files).Returns(new List<IFile> { driverFileMock.Object });

            Mock<IDirectory> archiveDirectoryMock = new Mock<IDirectory>();
            archiveDirectoryMock.Setup(x => x.Name).Returns("phantomjs");
            archiveDirectoryMock.Setup(x => x.ChildDirectories).Returns(new List<IDirectory> { binDirectoryMock.Object });

            Mock<IDirectory> directoryMock = new Mock<IDirectory>();
            directoryMock.Setup(x => x.FullName).Returns("c:\\tmp\\archive");
            directoryMock.Setup(x => x.ChildDirectories).Returns(new List<IDirectory> { archiveDirectoryMock.Object });

            Mock<IFile> archiveFileMock = new Mock<IFile>();
            archiveFileMock.Setup(x => x.ParentDirectory).Returns(directoryMock.Object);

            IFile file = WebDriverManager.PhantomJS().PostDownload(archiveFileMock.Object);

            Assert.That(file.FullName, Is.EqualTo("c:\\tmp\\archive\\phantomjs.exe"));
        }

        [Test]
        public void PostDownloadWithoutBin()
        {
            Resolver.OverriddenRegistrations += (builder) =>
            {
                builder.RegisterInstance(new Mock<IDownloader>().Object).As<IDownloader>();
            };

            Mock<IFile> driverFileMock = new Mock<IFile>();
            driverFileMock.Setup(x => x.Name).Returns("phantomjs.exe");

            Mock<IDirectory> examplesDirectoryMock = new Mock<IDirectory>();
            examplesDirectoryMock.Setup(x => x.Name).Returns("examples");

            Mock<IDirectory> archiveDirectoryMock = new Mock<IDirectory>();
            archiveDirectoryMock.Setup(x => x.Name).Returns("phantomjs");
            archiveDirectoryMock.Setup(x => x.ChildDirectories).Returns(new List<IDirectory> { examplesDirectoryMock.Object });
            archiveDirectoryMock.Setup(x => x.Files).Returns(new List<IFile> { null, null, driverFileMock.Object });

            Mock<IDirectory> directoryMock = new Mock<IDirectory>();
            directoryMock.Setup(x => x.FullName).Returns("c:\\tmp\\archive");
            directoryMock.Setup(x => x.ChildDirectories).Returns(new List<IDirectory> { archiveDirectoryMock.Object });

            Mock<IFile> archiveFileMock = new Mock<IFile>();
            archiveFileMock.Setup(x => x.ParentDirectory).Returns(directoryMock.Object);

            IFile file = WebDriverManager.PhantomJS().PostDownload(archiveFileMock.Object);

            Assert.That(file.FullName, Is.EqualTo("c:\\tmp\\archive\\phantomjs.exe"));
        }
    }
}
