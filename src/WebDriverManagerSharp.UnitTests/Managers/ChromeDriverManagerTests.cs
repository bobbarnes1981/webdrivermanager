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
    using WebDriverManagerSharp.Exceptions;

    [TestFixture]
    public class ChromeDriverManagerTests : BaseManagerTests
    {
        [Test]
        public void GetVersions()
        {
            Uri driverUrl = new Uri("https://fake.chromedriver.storage.googleapis.com/");
            configMock.Setup(x => x.GetChromeDriverUrl()).Returns(driverUrl);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?> <ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'> <Name>chromedriver</Name> <Prefix/> <Marker/> <IsTruncated>false</IsTruncated> <Contents> <Key>2.0/chromedriver_linux32.zip</Key> <Generation>1380149859530000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:39.349Z</LastModified> <ETag>\"c0d96102715c4916b872f91f5bf9b12c\"</ETag> <Size>7262134</Size> </Contents> <Contents> <Key>2.0/chromedriver_linux64.zip</Key> <Generation>1380149860664000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:40.449Z</LastModified> <ETag>\"858ebaf47e13dce7600191ed59974c09\"</ETag> <Size>7433593</Size> </Contents> <Contents> <Key>2.0/chromedriver_mac32.zip</Key> <Generation>1380149857425000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:37.204Z</LastModified> <ETag>\"efc13db5afc518000d886c2bdcb3a4bc\"</ETag> <Size>7614601</Size> </Contents> </ListBucketResult>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            List<string> versions = WebDriverManager.ChromeDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetVersionsMirror()
        {
            Uri driverUrl = new Uri("http://fake.npm.taobao.org/mirrors/chromedriver");
            Uri driverSubUrl = new Uri("http://fake.npm.taobao.org/mirrors/chromedriver/2.0/");
            configMock.Setup(x => x.GetChromeDriverMirrorUrl()).Returns(driverUrl);
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeHtml = "<a href=\"/mirrors/chromedriver/2.0/\">2.0</a>";
            string fakeSubHtml = "<a href=\"http://fake.npm.taobao.org/mirrors/chromedriver/2.0/chromedriver_win32.zip\">chromedriver_win32.zip</a>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));
            httpClientMock.Setup(x => x.ExecuteHttpGet(driverSubUrl, null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeSubHtml)));

            List<string> versions = WebDriverManager.ChromeDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void SetVersionLatest()
        {
            WebDriverManager.ChromeDriver().Version("latest");

            configMock.Verify(x => x.SetChromeDriverVersion("latest"), Times.Once);
        }

        [Test]
        public void SetDriverRepositoryUrl()
        {
            Uri uri = new Uri("http://www.uri.com");

            WebDriverManager.ChromeDriver().DriverRepositoryUrl(uri);

            configMock.Verify(x => x.SetChromeDriverUrl(uri), Times.Once);
        }

        [Test]
        public void TestSetUp()
        {
            Uri driverUrl = new Uri("https://fake.chromedriver.storage.googleapis.com/");
            configMock.Setup(x => x.GetChromeDriverUrl()).Returns(driverUrl);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?> <ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'> <Name>chromedriver</Name> <Prefix/> <Marker/> <IsTruncated>false</IsTruncated> <Contents> <Key>2.0/chromedriver_linux32.zip</Key> <Generation>1380149859530000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:39.349Z</LastModified> <ETag>\"c0d96102715c4916b872f91f5bf9b12c\"</ETag> <Size>7262134</Size> </Contents> <Contents> <Key>2.0/chromedriver_linux64.zip</Key> <Generation>1380149860664000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:40.449Z</LastModified> <ETag>\"858ebaf47e13dce7600191ed59974c09\"</ETag> <Size>7433593</Size> </Contents> <Contents> <Key>2.0/chromedriver_win32.zip</Key> <Generation>1380149857425000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:37.204Z</LastModified> <ETag>\"efc13db5afc518000d886c2bdcb3a4bc\"</ETag> <Size>7614601</Size> </Contents> </ListBucketResult>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(new Uri(driverUrl, "LATEST_RELEASE"), null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes("2.0")));
            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Storage.File("c:\\config_target\\driver.exe"));

            WebDriverManager.ChromeDriver().Setup();

            configMock.Verify(x => x.GetChromeDriverExport(), Times.Once);
        }

        [Test]
        public void TestSetUpNullDownload()
        {
            Uri driverUrl = new Uri("https://fake.chromedriver.storage.googleapis.com/");
            configMock.Setup(x => x.GetChromeDriverUrl()).Returns(driverUrl);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?> <ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'> <Name>chromedriver</Name> <Prefix/> <Marker/> <IsTruncated>false</IsTruncated> <Contents> <Key>2.0/chromedriver_linux32.zip</Key> <Generation>1380149859530000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:39.349Z</LastModified> <ETag>\"c0d96102715c4916b872f91f5bf9b12c\"</ETag> <Size>7262134</Size> </Contents> <Contents> <Key>2.0/chromedriver_linux64.zip</Key> <Generation>1380149860664000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:40.449Z</LastModified> <ETag>\"858ebaf47e13dce7600191ed59974c09\"</ETag> <Size>7433593</Size> </Contents> <Contents> <Key>2.0/chromedriver_win32.zip</Key> <Generation>1380149857425000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:37.204Z</LastModified> <ETag>\"efc13db5afc518000d886c2bdcb3a4bc\"</ETag> <Size>7614601</Size> </Contents> </ListBucketResult>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(new Uri(driverUrl, "LATEST_RELEASE"), null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes("2.0")));
            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns<Storage.IFile>(null);

            WebDriverManagerException exception = Assert.Throws<WebDriverManagerException>(() => WebDriverManager.ChromeDriver().Setup());

            Assert.That(exception.InnerException, Is.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void TestSetUpMirror()
        {
            Uri driverUrl = new Uri("http://fake.npm.taobao.org/mirrors/chromedriver");
            Uri driverSubUrl = new Uri("http://fake.npm.taobao.org/mirrors/chromedriver/2.0/");
            configMock.Setup(x => x.GetChromeDriverMirrorUrl()).Returns(driverUrl);
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeHtml = "<a href=\"/mirrors/chromedriver/2.0/\">2.0</a>";
            string fakeSubHtml = "<a href=\"http://fake.npm.taobao.org/mirrors/chromedriver/2.0/chromedriver_win32.zip\">chromedriver_win32.zip</a>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));
            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverSubUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeSubHtml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeSubHtml)));


            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Storage.File("c:\\config_target\\driver.exe"));

            WebDriverManager.ChromeDriver().Setup();

            configMock.Verify(x => x.GetChromeDriverExport(), Times.Once);
        }

        [Test]
        public void PreDownload()
        {
            string target = WebDriverManager.ChromeDriver().PreDownload("my target", "my version");

            Assert.That(target, Is.EqualTo("my target"));
        }

        [Test]
        public void PostDownloadNull()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.ChromeDriver().PostDownload(null));
        }

        [Test]
        public void PostDownload()
        {
            // TODO: 
        }
    }
}
