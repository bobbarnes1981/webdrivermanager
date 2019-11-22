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
    public class FirefoxDriverManagerTests : BaseManagerTests
    {
        [Test]
        public void GetVersions()
        {
            Uri driverUrl = new Uri("https://fake.api.github.com/repos/mozilla/geckodriver/releases");
            configMock.Setup(x => x.GetFirefoxDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");

            string fakeJson = "[  {    \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/20654191\",    \"assets_url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/20654191/assets\",    \"upload_url\": \"https://uploads.github.com/repos/mozilla/geckodriver/releases/20654191/assets{?name,label}\",    \"html_url\": \"https://github.com/mozilla/geckodriver/releases/tag/v0.26.0\",    \"id\": 20654191,    \"node_id\": \"MDc6UmVsZWFzZTIwNjU0MTkx\",    \"tag_name\": \"v0.26.0\",    \"target_commitish\": \"master\",    \"name\": \"\",    \"draft\": false,    \"author\": {      \"login\": \"andreastt\",      \"id\": 399120,      \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",      \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",      \"gravatar_id\": \"\",      \"url\": \"https://api.github.com/users/andreastt\",      \"html_url\": \"https://github.com/andreastt\",      \"followers_url\": \"https://api.github.com/users/andreastt/followers\",      \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",      \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",      \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",      \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",      \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",      \"repos_url\": \"https://api.github.com/users/andreastt/repos\",      \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",      \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",      \"type\": \"User\",      \"site_admin\": false    },    \"prerelease\": false,    \"created_at\": \"2019-08-16T10:41:23Z\",    \"published_at\": \"2019-10-11T23:34:06Z\",    \"assets\": [      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445299\",        \"id\": 15445299,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1Mjk5\",        \"name\": \"geckodriver-v0.26.0-linux32.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2330668,        \"download_count\": 6279,        \"created_at\": \"2019-10-12T18:17:37Z\",        \"updated_at\": \"2019-10-12T18:17:43Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux32.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445301\",        \"id\": 15445301,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1MzAx\",        \"name\": \"geckodriver-v0.26.0-linux64.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2390549,        \"download_count\": 5162465,        \"created_at\": \"2019-10-12T18:17:49Z\",        \"updated_at\": \"2019-10-12T18:17:53Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux64.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15436813\",        \"id\": 15436813,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDM2ODEz\",        \"name\": \"geckodriver-v0.26.0-macos.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"andreastt\",          \"id\": 399120,          \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",          \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/andreastt\",          \"html_url\": \"https://github.com/andreastt\",          \"followers_url\": \"https://api.github.com/users/andreastt/followers\",          \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",          \"repos_url\": \"https://api.github.com/users/andreastt/repos\",          \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2006880,        \"download_count\": 314992,        \"created_at\": \"2019-10-11T23:33:42Z\",        \"updated_at\": \"2019-10-11T23:33:50Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-macos.tar.gz\"      }    ],    \"tarball_url\": \"https://api.github.com/repos/mozilla/geckodriver/tarball/v0.4.0\",    \"zipball_url\": \"https://api.github.com/repos/mozilla/geckodriver/zipball/v0.4.0\",    \"body\": \"Only Nightly and DevEdition are official supported. Other versions may work but are not explicitly supported.\n\"  }]";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            List<string> versions = WebDriverManager.FirefoxDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetVersionsMirror()
        {
            Uri driverUrl = new Uri("http://fake.npm.taobao.org/mirrors/geckodriver");
            Uri driverSubUrl = new Uri("http://fake.npm.taobao.org/mirrors/geckodriver/v0.20.0/");
            configMock.Setup(x => x.GetFirefoxDriverMirrorUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeHtml = "<a href=\"/mirrors/geckodriver/v0.20.0/\">v0.20.0/</a>";
            string fakeSubHtml = "<a href=\"/mirrors/geckodriver/v0.20.0/geckodriver-v0.20.0-win32.zip\">geckodriver-v0.20.0-win32.zip</a>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));
            httpClientMock.Setup(x => x.ExecuteHttpGet(driverSubUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeSubHtml)));

            List<string> versions = WebDriverManager.FirefoxDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void SetVersionLatest()
        {
            WebDriverManager.FirefoxDriver().Version("latest");

            configMock.Verify(x => x.SetFirefoxDriverVersion("latest"), Times.Once);
        }

        [Test]
        public void SetDriverRepositoryUrl()
        {
            Uri uri = new Uri("http://www.uri.com");

            WebDriverManager.FirefoxDriver().DriverRepositoryUrl(uri);

            configMock.Verify(x => x.SetFirefoxDriverUrl(uri), Times.Once);
        }

        [Test]
        public void TestSetUp()
        {
            Uri driverUrl = new Uri("https://fake.api.github.com/repos/mozilla/geckodriver/releases");
            configMock.Setup(x => x.GetFirefoxDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");

            string fakeJson = "[  {    \"url\": \"https://fake.api.github.com/repos/mozilla/geckodriver/releases/20654191\",    \"assets_url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/20654191/assets\",    \"upload_url\": \"https://uploads.github.com/repos/mozilla/geckodriver/releases/20654191/assets{?name,label}\",    \"html_url\": \"https://github.com/mozilla/geckodriver/releases/tag/v0.26.0\",    \"id\": 20654191,    \"node_id\": \"MDc6UmVsZWFzZTIwNjU0MTkx\",    \"tag_name\": \"v0.26.0\",    \"target_commitish\": \"master\",    \"name\": \"\",    \"draft\": false,    \"author\": {      \"login\": \"andreastt\",      \"id\": 399120,      \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",      \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",      \"gravatar_id\": \"\",      \"url\": \"https://api.github.com/users/andreastt\",      \"html_url\": \"https://github.com/andreastt\",      \"followers_url\": \"https://api.github.com/users/andreastt/followers\",      \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",      \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",      \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",      \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",      \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",      \"repos_url\": \"https://api.github.com/users/andreastt/repos\",      \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",      \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",      \"type\": \"User\",      \"site_admin\": false    },    \"prerelease\": false,    \"created_at\": \"2019-08-16T10:41:23Z\",    \"published_at\": \"2019-10-11T23:34:06Z\",    \"assets\": [      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445299\",        \"id\": 15445299,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1Mjk5\",        \"name\": \"geckodriver-v0.26.0-linux32.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2330668,        \"download_count\": 6279,        \"created_at\": \"2019-10-12T18:17:37Z\",        \"updated_at\": \"2019-10-12T18:17:43Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux32.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445301\",        \"id\": 15445301,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1MzAx\",        \"name\": \"geckodriver-v0.26.0-linux64.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2390549,        \"download_count\": 5162465,        \"created_at\": \"2019-10-12T18:17:49Z\",        \"updated_at\": \"2019-10-12T18:17:53Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux64.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15436813\",        \"id\": 15436813,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDM2ODEz\",        \"name\": \"geckodriver-v0.26.0-win.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"andreastt\",          \"id\": 399120,          \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",          \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/andreastt\",          \"html_url\": \"https://github.com/andreastt\",          \"followers_url\": \"https://api.github.com/users/andreastt/followers\",          \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",          \"repos_url\": \"https://api.github.com/users/andreastt/repos\",          \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2006880,        \"download_count\": 314992,        \"created_at\": \"2019-10-11T23:33:42Z\",        \"updated_at\": \"2019-10-11T23:33:50Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-win.tar.gz\"      }    ],    \"tarball_url\": \"https://api.github.com/repos/mozilla/geckodriver/tarball/v0.4.0\",    \"zipball_url\": \"https://api.github.com/repos/mozilla/geckodriver/zipball/v0.4.0\",    \"body\": \"Only Nightly and DevEdition are official supported. Other versions may work but are not explicitly supported.\n\"  }]";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo("c:\\config_target\\driver.exe"));

            WebDriverManager.FirefoxDriver().Setup();

            configMock.Verify(x => x.GetFirefoxDriverExport(), Times.Once);
        }

        [Test]
        public void TestSetUpMirror()
        {
            Uri driverUrl = new Uri("http://fake.npm.taobao.org/mirrors/geckodriver");
            Uri driverSubUrl = new Uri("http://fake.npm.taobao.org/mirrors/geckodriver/v0.20.0/");
            configMock.Setup(x => x.GetFirefoxDriverMirrorUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeHtml = "<a href=\"/mirrors/geckodriver/v0.20.0/\">v0.20.0/</a>";
            string fakeSubHtml = "<a href=\"/mirrors/geckodriver/v0.20.0/geckodriver-v0.20.0-win32.zip\">geckodriver-v0.20.0-win32.zip</a>";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));
            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverSubUrl, It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeSubHtml)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeSubHtml)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo("c:\\config_target\\driver.exe"));

            WebDriverManager.FirefoxDriver().Setup();

            configMock.Verify(x => x.GetFirefoxDriverExport(), Times.Once);
        }

        [Test]
        public void PreDownloadNullTarget()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.FirefoxDriver().PreDownload(null, ""));
        }

        [Test]
        public void PreDownloadNullVersion()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.FirefoxDriver().PreDownload("", null));
        }

        [TestCase("C:\\Users\\robertb\\.m2\\repository\\webdriver\\geckodriver\\0.26.0\\geckodriver-v0.26.0-win32.zip", "0.26.0", "C:\\Users\\robertb\\.m2\\repository\\webdriver\\geckodriver\\win32\\0.26.0\\geckodriver-v0.26.0-win32.zip")]
        [TestCase("C:\\Users\\robertb\\.m2\\repository\\webdriver\\geckodriver\\0.26.0\\geckodriver-v0.26.0-linux32.tar.gz", "0.26.0", "C:\\Users\\robertb\\.m2\\repository\\webdriver\\geckodriver\\linux32\\0.26.0\\geckodriver-v0.26.0-linux32.tar.gz")]
        [TestCase("C:\\Users\\robertb\\.m2\\repository\\webdriver\\geckodriver\\0.26.0\\geckodriver-v0.26.0-linux64.gz", "0.26.0", "C:\\Users\\robertb\\.m2\\repository\\webdriver\\geckodriver\\linux64\\0.26.0\\geckodriver-v0.26.0-linux64.gz")]
        public void PreDownload(string file, string version, string expected)
        {
            string target = WebDriverManager.FirefoxDriver().PreDownload(file, version);

            Assert.That(target, Is.EqualTo(expected));
        }

        [Test]
        public void PostDownloadNull()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.FirefoxDriver().PostDownload(null));
        }

        [Test]
        public void PostDownload()
        {
            // TODO: 
        }
    }
}
