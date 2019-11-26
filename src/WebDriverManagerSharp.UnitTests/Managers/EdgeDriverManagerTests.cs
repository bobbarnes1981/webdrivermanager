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
    using System.Text;

    [TestFixture]
    public class EdgeDriverManagerTests : BaseManagerTests
    {
        [Test]
        public void GetVersions()
        {
            Uri driverUrl = new Uri("https://fake.developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/");
            configMock.Setup(x => x.GetOs()).Returns("WIN");
            configMock.Setup(x => x.GetEdgeDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetLocalRepositoryUser()).Returns("fakeUser");
            configMock.Setup(x => x.GetLocalRepositoryPassword()).Returns("fakePass");

            string fakeHtml = "<ul class='driver-downloads'><li class='driver-download'><a aria-label='' href='http://www.microsoft.com'></a></li></ul><ul class='driver-downloads'><li class='driver-download'><p class='driver-download__meta'>version 1</p></li></ul>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));

            List<string> versions = WebDriverManager.EdgeDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void SetVersionLatest()
        {
            WebDriverManager.EdgeDriver().Version("latest");

            configMock.Verify(x => x.SetEdgeDriverVersion("latest"), Times.Once);
        }

        [Test]
        public void SetDriverRepositoryUrl()
        {
            Uri uri = new Uri("http://www.uri.com");

            WebDriverManager.EdgeDriver().DriverRepositoryUrl(uri);

            configMock.Verify(x => x.SetEdgeDriverUrl(uri), Times.Once);
        }

        [Test]
        public void TestSetUp()
        {
            Uri driverUrl = new Uri("https://fake.developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/");
            configMock.Setup(x => x.GetEdgeDriverUrl()).Returns(driverUrl);

            string fakeHtml = "<ul class='driver-downloads'><li class='driver-download'><a aria-label='' href='http://www.microsoft.com'></a></li></ul><ul class='driver-downloads'><li class='driver-download'><p class='driver-download__meta'>version 1</p></li></ul>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Storage.File("c:\\config_target\\driver.exe"));

            WebDriverManager.EdgeDriver().Setup();

            configMock.Verify(x => x.GetEdgeDriverExport(), Times.Once);
        }

        [Test]
        public void PreDownload()
        {
            string target = WebDriverManager.EdgeDriver().PreDownload("path/1.2", "1.2");

            Assert.That(target, Is.EqualTo("path/1.2"));
        }

        [Test]
        public void PreDownloadChromium()
        {
            string target = WebDriverManager.EdgeDriver().PreDownload("path/1.2.3", "1.2.3");

            Assert.That(target, Is.EqualTo("path/default\\1.2.3"));
        }

        [Test]
        public void PostDownloadNull()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.EdgeDriver().PostDownload(null));
        }

        [Test]
        public void PostDownload()
        {
            // TODO: 
        }
    }
}
