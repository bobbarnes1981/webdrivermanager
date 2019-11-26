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
    using WebDriverManagerSharp.Storage;

    [TestFixture]
    public class SeleniumServerStandaloneDriverManagerTests : BaseManagerTests
    {
        [Test]
        public void GetVersions()
        {
            Uri driverUrl = new Uri("https://selenium-release.storage.googleapis.com/");
            configMock.Setup(x => x.GetSeleniumServerStandaloneUrl()).Returns(driverUrl);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?><ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'><Name>selenium-release</Name><Prefix/><Marker/><IsTruncated>false</IsTruncated><Contents><Key>2.39/IEDriverServer_Win32_2.39.0.zip</Key><Generation>1389651460351000</Generation><MetaGeneration>4</MetaGeneration><LastModified>2014-01-13T22:17:40.327Z</LastModified><ETag>\"bd4bc2b77a04999148e7fab974336e99\"</ETag><Size>836478</Size></Contents><Contents><Key>2.39/IEDriverServer_x64_2.39.0.zip</Key><Generation>1389651273362000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:33.323Z</LastModified><ETag>\"7d19f3d7ffb9cb40fc26cc38885b9160\"</ETag><Size>946479</Size></Contents><Contents><Key>2.39/selenium-dotnet-2.39.0.zip</Key><Generation>1389651287806000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:47.774Z</LastModified><ETag>\"e5d82bd497eff0bf3a3990cb746a2680\"</ETag><Size>10263239</Size></Contents></ListBucketResult>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            List<string> versions = WebDriverManager.SeleniumServerStandalone().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetVersionsMirror()
        {
            Uri driverUrl = new Uri("https://fake.selenium-release.storage.googleapis.com/");
            configMock.Setup(x => x.GetSeleniumServerStandaloneUrl()).Returns(driverUrl);
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?><ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'><Name>selenium-release</Name><Prefix/><Marker/><IsTruncated>false</IsTruncated><Contents><Key>2.39/IEDriverServer_Win32_2.39.0.zip</Key><Generation>1389651460351000</Generation><MetaGeneration>4</MetaGeneration><LastModified>2014-01-13T22:17:40.327Z</LastModified><ETag>\"bd4bc2b77a04999148e7fab974336e99\"</ETag><Size>836478</Size></Contents><Contents><Key>2.39/IEDriverServer_x64_2.39.0.zip</Key><Generation>1389651273362000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:33.323Z</LastModified><ETag>\"7d19f3d7ffb9cb40fc26cc38885b9160\"</ETag><Size>946479</Size></Contents><Contents><Key>2.39/selenium-dotnet-2.39.0.zip</Key><Generation>1389651287806000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:47.774Z</LastModified><ETag>\"e5d82bd497eff0bf3a3990cb746a2680\"</ETag><Size>10263239</Size></Contents></ListBucketResult>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            List<string> versions = WebDriverManager.SeleniumServerStandalone().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void SetVersionLatest()
        {
            WebDriverManager.SeleniumServerStandalone().Version("latest");

            configMock.Verify(x => x.SetSeleniumServerStandaloneVersion("latest"), Times.Once);
        }

        [Test]
        public void SetDriverRepositoryUrl()
        {
            Uri uri = new Uri("http://www.uri.com");

            WebDriverManager.SeleniumServerStandalone().DriverRepositoryUrl(uri);

            configMock.Verify(x => x.SetSeleniumServerStandaloneUrl(uri), Times.Once);
        }

        [Test]
        public void TestSetUp()
        {
            Uri driverUrl = new Uri("https://fake.selenium-release.storage.googleapis.com/");
            configMock.Setup(x => x.GetSeleniumServerStandaloneUrl()).Returns(driverUrl);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?><ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'><Name>selenium-release</Name><Prefix/><Marker/><IsTruncated>false</IsTruncated><Contents><Key>2.39/IEDriverServer_Win32_2.39.0.zip</Key><Generation>1389651460351000</Generation><MetaGeneration>4</MetaGeneration><LastModified>2014-01-13T22:17:40.327Z</LastModified><ETag>\"bd4bc2b77a04999148e7fab974336e99\"</ETag><Size>836478</Size></Contents><Contents><Key>2.39/IEDriverServer_x64_2.39.0.zip</Key><Generation>1389651273362000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:33.323Z</LastModified><ETag>\"7d19f3d7ffb9cb40fc26cc38885b9160\"</ETag><Size>946479</Size></Contents><Contents><Key>2.39/selenium-server-standalone-2.39.0.zip</Key><Generation>1389651287806000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:47.774Z</LastModified><ETag>\"e5d82bd497eff0bf3a3990cb746a2680\"</ETag><Size>10263239</Size></Contents></ListBucketResult>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Storage.File("c:\\config_target\\driver.exe"));

            WebDriverManager.SeleniumServerStandalone().Setup();

            configMock.Verify(x => x.IsAvoidExport(), Times.Once);
        }

        [Test]
        public void TestSetUpMirror()
        {
            Uri driverUrl = new Uri("https://fake.selenium-release.storage.googleapis.com/");
            configMock.Setup(x => x.GetSeleniumServerStandaloneUrl()).Returns(driverUrl);
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?><ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'><Name>selenium-release</Name><Prefix/><Marker/><IsTruncated>false</IsTruncated><Contents><Key>2.39/IEDriverServer_Win32_2.39.0.zip</Key><Generation>1389651460351000</Generation><MetaGeneration>4</MetaGeneration><LastModified>2014-01-13T22:17:40.327Z</LastModified><ETag>\"bd4bc2b77a04999148e7fab974336e99\"</ETag><Size>836478</Size></Contents><Contents><Key>2.39/IEDriverServer_x64_2.39.0.zip</Key><Generation>1389651273362000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:33.323Z</LastModified><ETag>\"7d19f3d7ffb9cb40fc26cc38885b9160\"</ETag><Size>946479</Size></Contents><Contents><Key>2.39/selenium-server-standalone-2.39.0.zip</Key><Generation>1389651287806000</Generation><MetaGeneration>2</MetaGeneration><LastModified>2014-01-13T22:14:47.774Z</LastModified><ETag>\"e5d82bd497eff0bf3a3990cb746a2680\"</ETag><Size>10263239</Size></Contents></ListBucketResult>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Storage.File("c:\\config_target\\driver.exe"));

            WebDriverManager.SeleniumServerStandalone().Setup();

            configMock.Verify(x => x.IsAvoidExport(), Times.Once);
        }

        [Test]
        public void PreDownload()
        {
            string target = WebDriverManager.SeleniumServerStandalone().PreDownload("my target", "my version");

            Assert.That(target, Is.EqualTo("my target"));
        }

        [Test]
        public void PostDownloadNull()
        {
            IFile fileInfo =  WebDriverManager.SeleniumServerStandalone().PostDownload(null);

            Assert.That(fileInfo, Is.Null);
        }
    }
}
