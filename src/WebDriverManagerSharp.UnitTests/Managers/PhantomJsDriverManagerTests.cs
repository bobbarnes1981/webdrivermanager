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
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo("c:\\config_target\\driver.exe"));

            WebDriverManager.PhantomJS().Setup();

            configMock.Verify(x => x.GetPhantomjsDriverExport(), Times.Once);
        }
    }
}
