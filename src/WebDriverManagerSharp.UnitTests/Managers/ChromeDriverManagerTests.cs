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
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Web;

    [TestFixture]
    public class ChromeDriverManagerTests
    {
        private Mock<IConfig> configMock;

        [SetUp]
        public void SetUp()
        {
            WebDriverManager.ClearDrivers();

            configMock = new Mock<IConfig>();

            Mock<IConfigFactory> configFactoryMock = new Mock<IConfigFactory>();

            configFactoryMock.Setup(x => x.Build()).Returns(configMock.Object);

            WebDriverManager.ConfigFactory = configFactoryMock.Object;
        }

        [Test]
        public void GetVersions()
        {
            System.Uri driverUrl = new System.Uri("https://chromedriver.storage.googleapis.com/");
            configMock.Setup(x => x.GetChromeDriverUrl()).Returns(driverUrl);

            string fakeXml = "<?xml version='1.0' encoding='UTF-8'?> <ListBucketResult xmlns='http://doc.s3.amazonaws.com/2006-03-01'> <Name>chromedriver</Name> <Prefix/> <Marker/> <IsTruncated>false</IsTruncated> <Contents> <Key>2.0/chromedriver_linux32.zip</Key> <Generation>1380149859530000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:39.349Z</LastModified> <ETag>\"c0d96102715c4916b872f91f5bf9b12c\"</ETag> <Size>7262134</Size> </Contents> <Contents> <Key>2.0/chromedriver_linux64.zip</Key> <Generation>1380149860664000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:40.449Z</LastModified> <ETag>\"858ebaf47e13dce7600191ed59974c09\"</ETag> <Size>7433593</Size> </Contents> <Contents> <Key>2.0/chromedriver_mac32.zip</Key> <Generation>1380149857425000</Generation> <MetaGeneration>4</MetaGeneration> <LastModified>2013-09-25T22:57:37.204Z</LastModified> <ETag>\"efc13db5afc518000d886c2bdcb3a4bc\"</ETag> <Size>7614601</Size> </Contents> </ListBucketResult>";

            Mock<IHttpClient> httpClientMock = new Mock<IHttpClient>();
            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, null)).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeXml)));

            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.Build(It.IsAny<IConfig>())).Returns(httpClientMock.Object);

            WebDriverManager.HttpClientFactory = httpClientFactoryMock.Object;

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
    }
}
