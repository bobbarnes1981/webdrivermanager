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
    using WebDriverManagerSharp.Exceptions;

    [TestFixture]
    public class WebDriverManagerTests : BaseManagerTests
    {
        [Test]
        public void TestX32()
        {
            WebDriverManager.ChromeDriver().Arch32();
            configMock.Verify(x => x.SetArchitecture(Enums.Architecture.X32), Times.Once);
        }

        [Test]
        public void TestX64()
        {
            WebDriverManager.ChromeDriver().Arch64();
            configMock.Verify(x => x.SetArchitecture(Enums.Architecture.X64), Times.Once);
        }

        [Test]
        public void TestArchX32()
        {
            WebDriverManager.ChromeDriver().Architecture(Enums.Architecture.X32);
            configMock.Verify(x => x.SetArchitecture(Enums.Architecture.X32), Times.Once);
        }

        [Test]
        public void TestArchX64()
        {
            WebDriverManager.ChromeDriver().Architecture(Enums.Architecture.X64);
            configMock.Verify(x => x.SetArchitecture(Enums.Architecture.X64), Times.Once);
        }

        [Test]
        public void TestAvoidAutoVersion()
        {
            WebDriverManager.ChromeDriver().AvoidAutoVersion();
            configMock.Verify(x => x.SetAvoidAutoVersion(true), Times.Once);
        }

        [Test]
        public void TestAvoidExport()
        {
            WebDriverManager.ChromeDriver().AvoidExport();
            configMock.Verify(x => x.SetAvoidExport(true), Times.Once);
        }

        [Test]
        public void TestAvoudTree()
        {
            WebDriverManager.ChromeDriver().AvoidOutputTree();
            configMock.Verify(x => x.SetAvoidOutputTree(true), Times.Once);
        }

        [Test]
        public void TestAvoidPrefs()
        {
            WebDriverManager.ChromeDriver().AvoidPreferences();
            configMock.Verify(x => x.SetAvoidPreferences(true), Times.Once);
        }

        [Test]
        public void TestBrowserPath()
        {
            WebDriverManager.ChromeDriver().BrowserPath("browserPath");
            configMock.Verify(x => x.SetBinaryPath("browserPath"), Times.Once);
        }

        [Test]
        public void TestClearCache()
        {
            WebDriverManager.ChromeDriver().ClearCache();
            configMock.Verify(x => x.GetTargetPath(), Times.Once);
        }

        [Test]
        public void TestRepository()
        {
            WebDriverManager.ChromeDriver().DriverRepositoryUrl(new System.Uri("http://repoUrl"));
            configMock.Verify(x => x.SetChromeDriverUrl(new System.Uri("http://repoUrl")), Times.Once);
        }

        [Test]
        public void TestCache()
        {
            WebDriverManager.ChromeDriver().ForceCache();
            configMock.Verify(x => x.SetForceCache(true), Times.Once);
        }

        [Test]
        public void TestDownload()
        {
            WebDriverManager.ChromeDriver().ForceDownload();
            configMock.Verify(x => x.SetOverride(true), Times.Once);
        }

        [Test]
        public void TestGitHubName()
        {
            WebDriverManager.ChromeDriver().GitHubTokenName("githubName");
            configMock.Verify(x => x.SetGitHubTokenName("githubName"), Times.Once);
        }

        [Test]
        public void TestGitHubSecret()
        {
            WebDriverManager.ChromeDriver().GitHubTokenSecret("githubSecret");
            configMock.Verify(x => x.SetGitHubTokenSecret("githubSecret"), Times.Once);
        }

        [Test]
        public void TestIgnoreVersions()
        {
            WebDriverManager.ChromeDriver().IgnoreVersions("1.2.3.4", "1.2.3.5");
            configMock.Verify(x => x.SetIgnoreVersions("1.2.3.4", "1.2.3.5"), Times.Once);
        }

        [Test]
        public void TestLocalRepoPass()
        {
            WebDriverManager.ChromeDriver().LocalRepositoryPassword("localRepoPassword");
            configMock.Verify(x => x.SetLocalRepositoryPassword("localRepoPassword"), Times.Once);
        }

        [Test]
        public void TestLocalRepoUser()
        {
            WebDriverManager.ChromeDriver().LocalRepositoryUser("localRepoUser");
            configMock.Verify(x => x.SetLocalRepositoryUser("localRepoUser"), Times.Once);
        }

        [Test]
        public void TestProxy()
        {
            WebDriverManager.ChromeDriver().Proxy("proxy");
            configMock.Verify(x => x.SetProxy("proxy"), Times.Once);
        }

        [Test]
        public void TestProxyPass()
        {
            WebDriverManager.ChromeDriver().ProxyPass("proxyPass");
            configMock.Verify(x => x.SetProxyPass("proxyPass"), Times.Once);
        }

        [Test]
        public void TestProxyUser()
        {
            WebDriverManager.ChromeDriver().ProxyUser("proxyUser");
            configMock.Verify(x => x.SetProxyUser("proxyUser"), Times.Once);
        }

        [Test]
        public void TestTarget()
        {
            WebDriverManager.ChromeDriver().TargetPath("c:\\target");
            configMock.Verify(x => x.SetTargetPath("c:\\target"), Times.Once);
        }

        [Test]
        public void TestTimeout()
        {
            WebDriverManager.ChromeDriver().Timeout(100);
            configMock.Verify(x => x.SetTimeout(100), Times.Once);
        }

        [Test]
        public void TestTtl()
        {
            WebDriverManager.ChromeDriver().Ttl(100);
            configMock.Verify(x => x.SetTtl(100), Times.Once);
        }

        [Test]
        public void TestBeta()
        {
            WebDriverManager.ChromeDriver().UseBetaVersions();
            configMock.Verify(x => x.SetUseBetaVersions(true), Times.Once);
        }

        [Test]
        public void TestLocalFirst()
        {
            WebDriverManager.ChromeDriver().UseLocalVersionsPropertiesFirst();
            configMock.Verify(x => x.SetVersionsPropertiesOnlineFirst(false), Times.Once);
        }

        [Test]
        public void TestMirrorNull()
        {
            Assert.Throws<WebDriverManagerException>(() => WebDriverManager.ChromeDriver().UseMirror());
        }

        [Test]
        public void TestMirror()
        {
            configMock.Setup(x => x.GetChromeDriverMirrorUrl()).Returns(new System.Uri("http://url"));
            WebDriverManager.ChromeDriver().UseMirror();
            configMock.Verify(x => x.SetUseMirror(true), Times.Once);
        }

        [Test]
        public void TestVersion()
        {
            WebDriverManager.ChromeDriver().Version("1.2.3.4");
            configMock.Verify(x => x.SetChromeDriverVersion("1.2.3.4"), Times.Once);
        }
    }
}
