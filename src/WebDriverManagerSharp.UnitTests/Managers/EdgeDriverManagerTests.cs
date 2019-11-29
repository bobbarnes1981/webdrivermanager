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
    using WebDriverManagerSharp.Storage;

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
        public void GetVersionsBadUrl()
        {
            Uri driverUrl = new Uri("https://fake.developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/");
            configMock.Setup(x => x.GetOs()).Returns("WIN");
            configMock.Setup(x => x.GetEdgeDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetLocalRepositoryUser()).Returns("fakeUser");
            configMock.Setup(x => x.GetLocalRepositoryPassword()).Returns("fakePass");

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, null)).Throws(new IOException());

            WebDriverManagerException exception = Assert.Throws<WebDriverManagerException>(() => WebDriverManager.EdgeDriver().GetVersions());

            Assert.That(exception.InnerException, Is.InstanceOf<IOException>());
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

        [TestCase(Enums.OperatingSystem.WIN, Enums.Architecture.X32, "80")]
        [TestCase(Enums.OperatingSystem.WIN, Enums.Architecture.X32, "42")]
        [TestCase(Enums.OperatingSystem.WIN, Enums.Architecture.X32, null)]
        [TestCase(Enums.OperatingSystem.WIN, Enums.Architecture.X64, "80")]
        [TestCase(Enums.OperatingSystem.MAC, Enums.Architecture.X32, "80")]
        [TestCase(Enums.OperatingSystem.MAC, Enums.Architecture.X64, "80")]
        public void TestSetUp(Enums.OperatingSystem os, Enums.Architecture arch, string edgeVersion)
        {
            shellMock.Setup(x => x.RunAndWait(It.Is<IDirectory>(d => d.FullName == @"C:\WINDOWS\System32\wbem"), "wmic.exe", "datafile", "where", "name='" + "C:\\\\Program Files (x86)\\\\Microsoft\\\\Edge Dev\\\\Application\\\\msedge.exe" + "'", "get", "Version", "/value")).Returns(edgeVersion == "80" ? "80.0.0.0" : null);
            shellMock.Setup(x => x.GetVersionFromWmicOutput("80.0.0.0")).Returns("80");
            shellMock.Setup(x => x.RunAndWait("powershell", "get-appxpackage Microsoft.MicrosoftEdge")).Returns(edgeVersion == "42" ? "42.0" : null);
            shellMock.Setup(x => x.GetVersionFromPowerShellOutput("42.0")).Returns("42");

            Uri driverUrl = new Uri("https://fake.developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/");
            configMock.Setup(x => x.GetEdgeDriverUrl()).Returns(driverUrl);

            string fakeHtml = "<div class=\"layout layout--equal\"><div class=\"module\"><ul class=\"bare subsection__body driver-downloads\"><li class=\"driver-download\"><p class=\"subtitle placement__title\" aria-label=\"WebDriver for Microsoft Edge (Chromium)\">Microsoft Edge (Chromium)</p><p class=\"driver-download__meta\">Microsoft Edge Driver is now downloadable separately from Windows.</p><p class=\"driver-download__meta\">Below you can find the latest versions of WebDriver. If your version is not listed please visit the <a href=\"https://msedgewebdriverstorage.z22.web.core.windows.net/\">full version directory</a>.</p></li> <li class=\"driver-download\"><p class=\"subtitle\" aria-label=\"WebDriver for release number 80\">Release 80</p><p class=\"driver-download__meta\">Version: 80.0.346.0 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/80.0.346.0/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 80 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/80.0.346.0/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 80 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/80.0.346.0/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 80 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 80.0.345.0 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/80.0.345.0/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 80 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/80.0.345.0/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 80 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/80.0.345.0/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 80 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 80.0.344.0 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/80.0.344.0/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 80 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/80.0.344.0/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 80 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/80.0.344.0/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 80 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p></li><li class=\"driver-download\"><p class=\"subtitle\" aria-label=\"WebDriver for release number 79\">Release 79</p><p class=\"driver-download__meta\">Version: 79.0.313.0 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/79.0.313.0/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 79 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/79.0.313.0/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 79 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/79.0.313.0/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 79 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 79.0.309.30 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/79.0.309.30/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 79 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/79.0.309.30/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 79 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/79.0.309.30/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 79 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 79.0.309.25 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/79.0.309.25/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 79 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/79.0.309.25/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 79 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/79.0.309.25/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 79 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p></li><li class=\"driver-download\"><p class=\"subtitle\" aria-label=\"WebDriver for release number 78\">Release 78</p><p class=\"driver-download__meta\">Version: 78.0.277.0 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/78.0.277.0/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 78 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/78.0.277.0/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 78 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/78.0.277.0/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 78 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 78.0.276.24 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/78.0.276.24/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 78 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/78.0.276.24/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 78 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/78.0.276.24/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 78 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 78.0.276.20 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/78.0.276.20/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 78 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/78.0.276.20/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 78 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/78.0.276.20/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 78 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p></li><li class=\"driver-download\"><p class=\"subtitle\" aria-label=\"WebDriver for release number 77\">Release 77</p><p class=\"driver-download__meta\">Version: 77.0.237.0 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/77.0.237.0/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 77 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/77.0.237.0/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 77 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/77.0.237.0/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 77 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 77.0.235.27 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/77.0.235.27/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 77 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/77.0.235.27/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 77 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/77.0.235.27/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 77 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 77.0.235.25 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/77.0.235.25/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 77 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/77.0.235.25/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 77 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/77.0.235.25/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 77 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p></li><li class=\"driver-download\"><p class=\"subtitle\" aria-label=\"WebDriver for release number 76\">Release 76</p><p class=\"driver-download__meta\">Version: 76.0.183.0 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/76.0.183.0/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 76 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/76.0.183.0/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 76 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/76.0.183.0/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 76 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 76.0.182.22 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/76.0.182.22/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 76 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/76.0.182.22/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 76 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/76.0.182.22/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 76 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p><p class=\"driver-download__meta\">Version: 76.0.182.6 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/76.0.182.6/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 76 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/76.0.182.6/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 76 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/76.0.182.6/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 76 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p></li><li class=\"driver-download\"><p class=\"subtitle\" aria-label=\"WebDriver for release number 75\">Release 75</p><p class=\"driver-download__meta\">Version: 75.0.139.20 | Choose your OS:  <a href=\"https://msedgedriver.azureedge.net/75.0.139.20/edgedriver_win32.zip\" aria-label=\"WebDriver for release number 75 x86\">x86</a>,  <a href=\"https://msedgedriver.azureedge.net/75.0.139.20/edgedriver_win64.zip\" aria-label=\"WebDriver for release number 75 x64\">x64</a>,  <a href=\"https://msedgedriver.azureedge.net/75.0.139.20/edgedriver_mac64.zip\" aria-label=\"WebDriver for release number 75 Mac\">Mac</a>  | <a href=\"https://az813057.vo.msecnd.net/webdriver/license.html\">License terms</a> | <a href=\"https://az813057.vo.msecnd.net/webdriver/notices.html\">Notices</a></p></li> </ul></div><div class=\"module\"><ul class=\"bare subsection__body driver-downloads\"><li class=\"driver-download\"><p class=\"subtitle placement__title\" aria-label=\"WebDriver for Microsoft Edge (EdgeHTML) Edge\">Microsoft Edge (EdgeHTML)</p><p class=\"driver-download__meta\">Microsoft WebDriver for Microsoft Edge version 18 is a Windows Feature on Demand.</p><p class=\"driver-download__meta\">To install run the following in an elevated command prompt:</p><p class=\"driver-download__meta\">DISM.exe /Online /Add-Capability /CapabilityName:Microsoft.WebDriver~~~~0.0.1.0</p><p class=\"driver-download__meta\">For builds prior to 18, download the approriate driver for your installed version of Microsoft Edge:</p></li><li class=\"driver-download\"><a class=\"subtitle\" href=\"https://download.microsoft.com/download/F/8/A/F8AF50AB-3C3A-4BC4-8773-DC27B32988DD/MicrosoftWebDriver.exe\" aria-label=\"WebDriver for release number 17134\">Release 17134</a><p class=\"driver-download__meta\">Version: 6.17134 | Microsoft Edge version supported: 17.17134 | <a href=\"https://az813057.vo.msecnd.net/eulas/webdriver-eula.pdf\">License terms</a></p></li><li class=\"driver-download\"><a class=\"subtitle\" href=\"https://download.microsoft.com/download/D/4/1/D417998A-58EE-4EFE-A7CC-39EF9E020768/MicrosoftWebDriver.exe\" aria-label=\"WebDriver for release number 16299\">Release 16299</a><p class=\"driver-download__meta\">Version: 5.16299 | Microsoft Edge version supported: 16.16299 | <a href=\"https://az813057.vo.msecnd.net/eulas/webdriver-eula.pdf\">License terms</a></p></li><li class=\"driver-download\"><a class=\"subtitle\" href=\"https://download.microsoft.com/download/3/4/2/342316D7-EBE0-4F10-ABA2-AE8E0CDF36DD/MicrosoftWebDriver.exe\" aria-label=\"WebDriver for release number 15063\">Release 15063</a><p class=\"driver-download__meta\">Version: 4.15063 | Microsoft Edge version supported: 15.15063 | <a href=\"https://az813057.vo.msecnd.net/eulas/webdriver-eula.pdf\">License terms</a></p></li><li class=\"driver-download\"><a class=\"subtitle\" href=\"https://download.microsoft.com/download/3/2/D/32D3E464-F2EF-490F-841B-05D53C848D15/MicrosoftWebDriver.exe\" aria-label=\"WebDriver for release number 14393\">Release 14393</a><p class=\"driver-download__meta\">Version: 3.14393 | Microsoft Edge version supported: 14.14393 | <a href=\"https://az813057.vo.msecnd.net/eulas/webdriver-eula.pdf\">License terms</a></p></li><li class=\"driver-download\"><a class=\"subtitle\" href=\"https://download.microsoft.com/download/C/0/7/C07EBF21-5305-4EC8-83B1-A6FCC8F93F45/MicrosoftWebDriver.exe\" aria-label=\"WebDriver for release number 10586\">Release 10586</a><p class=\"driver-download__meta\">Version: 2.10586 | Microsoft Edge version supported: 13.10586 | <a href=\"https://az813057.vo.msecnd.net/eulas/webdriver-eula.pdf\">License terms</a></p></li><li class=\"driver-download\"><a class=\"subtitle\" href=\"https://download.microsoft.com/download/8/D/0/8D0D08CF-790D-4586-B726-C6469A9ED49C/MicrosoftWebDriver.exe\" aria-label=\"WebDriver for release number 10240\">Release 10240</a><p class=\"driver-download__meta\">Version: 1.10240 | Microsoft Edge version supported: 12.10240 | <a href=\"https://az813057.vo.msecnd.net/eulas/webdriver-eula.pdf\">License terms</a></p></li></ul></div></div>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, null))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));

            string versionProperties = "edge80=80.0.346.0\r\nedge42=6.17134";
            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);
            fileStorageMock.Setup(x => x.GetCurrentDirectory()).Returns("directory");
            fileStorageMock.SetupSequence(x => x.OpenRead("directory\\Resources\\versions.properties"))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(versionProperties)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(versionProperties)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(versionProperties)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(versionProperties)));

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns(os.ToString());
            configMock.Setup(x => x.GetArchitecture()).Returns(arch);
            configMock.Setup(x => x.GetEdgeDriverExport()).Returns("wdm.edgeDriver");

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
        public void PreDownloadNullTarget()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.EdgeDriver().PreDownload(null, "1.2"));
        }

        [Test]
        public void PreDownloadNullVersion()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.EdgeDriver().PreDownload("path/1.2", null));
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
        public void PostDownloadEdgeDriver()
        {
            Mock<IFile> driverFileMock = new Mock<IFile>();
            driverFileMock.Setup(x => x.FullName).Returns("msedgedriver.exe");

            Mock<IDirectory> parentDirectoryMock = new Mock<IDirectory>();
            parentDirectoryMock.Setup(x => x.Files).Returns(new List<IFile> { driverFileMock.Object });

            Mock<IFile> archiveMock = new Mock<IFile>();
            archiveMock.Setup(x => x.ParentDirectory).Returns(parentDirectoryMock.Object);

            WebDriverManager.EdgeDriver().PostDownload(archiveMock.Object);

        }

        [Test]
        public void PostDownloadNoDriver()
        {
            Mock<IFile> driverFileMock = new Mock<IFile>();
            driverFileMock.Setup(x => x.FullName).Returns("file.exe");

            Mock<IDirectory> parentDirectoryMock = new Mock<IDirectory>();
            parentDirectoryMock.Setup(x => x.Files).Returns(new List<IFile> { driverFileMock.Object });

            Mock<IFile> archiveMock = new Mock<IFile>();
            archiveMock.Setup(x => x.ParentDirectory).Returns(parentDirectoryMock.Object);

            WebDriverManager.EdgeDriver().PostDownload(archiveMock.Object);

        }
    }
}
