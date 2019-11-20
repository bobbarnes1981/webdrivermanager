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
    using WebDriverManagerSharp.Web;

    [TestFixture]
    public class FirefoxDriverManagerTests
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
            System.Uri driverUrl = new System.Uri("https://api.github.com/repos/mozilla/geckodriver/releases");
            configMock.Setup(x => x.GetFirefoxDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");

            string fakeJson = "[  {    \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/20654191\",    \"assets_url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/20654191/assets\",    \"upload_url\": \"https://uploads.github.com/repos/mozilla/geckodriver/releases/20654191/assets{?name,label}\",    \"html_url\": \"https://github.com/mozilla/geckodriver/releases/tag/v0.26.0\",    \"id\": 20654191,    \"node_id\": \"MDc6UmVsZWFzZTIwNjU0MTkx\",    \"tag_name\": \"v0.26.0\",    \"target_commitish\": \"master\",    \"name\": \"\",    \"draft\": false,    \"author\": {      \"login\": \"andreastt\",      \"id\": 399120,      \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",      \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",      \"gravatar_id\": \"\",      \"url\": \"https://api.github.com/users/andreastt\",      \"html_url\": \"https://github.com/andreastt\",      \"followers_url\": \"https://api.github.com/users/andreastt/followers\",      \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",      \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",      \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",      \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",      \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",      \"repos_url\": \"https://api.github.com/users/andreastt/repos\",      \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",      \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",      \"type\": \"User\",      \"site_admin\": false    },    \"prerelease\": false,    \"created_at\": \"2019-08-16T10:41:23Z\",    \"published_at\": \"2019-10-11T23:34:06Z\",    \"assets\": [      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445299\",        \"id\": 15445299,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1Mjk5\",        \"name\": \"geckodriver-v0.26.0-linux32.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2330668,        \"download_count\": 6279,        \"created_at\": \"2019-10-12T18:17:37Z\",        \"updated_at\": \"2019-10-12T18:17:43Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux32.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445301\",        \"id\": 15445301,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1MzAx\",        \"name\": \"geckodriver-v0.26.0-linux64.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2390549,        \"download_count\": 5162465,        \"created_at\": \"2019-10-12T18:17:49Z\",        \"updated_at\": \"2019-10-12T18:17:53Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux64.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15436813\",        \"id\": 15436813,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDM2ODEz\",        \"name\": \"geckodriver-v0.26.0-macos.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"andreastt\",          \"id\": 399120,          \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",          \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/andreastt\",          \"html_url\": \"https://github.com/andreastt\",          \"followers_url\": \"https://api.github.com/users/andreastt/followers\",          \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",          \"repos_url\": \"https://api.github.com/users/andreastt/repos\",          \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2006880,        \"download_count\": 314992,        \"created_at\": \"2019-10-11T23:33:42Z\",        \"updated_at\": \"2019-10-11T23:33:50Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-macos.tar.gz\"      }    ],    \"tarball_url\": \"https://api.github.com/repos/mozilla/geckodriver/tarball/v0.4.0\",    \"zipball_url\": \"https://api.github.com/repos/mozilla/geckodriver/zipball/v0.4.0\",    \"body\": \"Only Nightly and DevEdition are official supported. Other versions may work but are not explicitly supported.\n\"  }]";

            Mock<IHttpClient> httpClientMock = new Mock<IHttpClient>();
            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.Build(It.IsAny<IConfig>())).Returns(httpClientMock.Object);

            WebDriverManager.HttpClientFactory = httpClientFactoryMock.Object;

            List<string> versions = WebDriverManager.FirefoxDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetVersionsMirror()
        {
            System.Uri driverUrl = new System.Uri("https://api.github.com/repos/mozilla/geckodriver/releases");
            configMock.Setup(x => x.GetFirefoxDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeJson = "[  {    \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/20654191\",    \"assets_url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/20654191/assets\",    \"upload_url\": \"https://uploads.github.com/repos/mozilla/geckodriver/releases/20654191/assets{?name,label}\",    \"html_url\": \"https://github.com/mozilla/geckodriver/releases/tag/v0.26.0\",    \"id\": 20654191,    \"node_id\": \"MDc6UmVsZWFzZTIwNjU0MTkx\",    \"tag_name\": \"v0.26.0\",    \"target_commitish\": \"master\",    \"name\": \"\",    \"draft\": false,    \"author\": {      \"login\": \"andreastt\",      \"id\": 399120,      \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",      \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",      \"gravatar_id\": \"\",      \"url\": \"https://api.github.com/users/andreastt\",      \"html_url\": \"https://github.com/andreastt\",      \"followers_url\": \"https://api.github.com/users/andreastt/followers\",      \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",      \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",      \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",      \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",      \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",      \"repos_url\": \"https://api.github.com/users/andreastt/repos\",      \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",      \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",      \"type\": \"User\",      \"site_admin\": false    },    \"prerelease\": false,    \"created_at\": \"2019-08-16T10:41:23Z\",    \"published_at\": \"2019-10-11T23:34:06Z\",    \"assets\": [      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445299\",        \"id\": 15445299,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1Mjk5\",        \"name\": \"geckodriver-v0.26.0-linux32.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2330668,        \"download_count\": 6279,        \"created_at\": \"2019-10-12T18:17:37Z\",        \"updated_at\": \"2019-10-12T18:17:43Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux32.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15445301\",        \"id\": 15445301,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDQ1MzAx\",        \"name\": \"geckodriver-v0.26.0-linux64.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"whimboo\",          \"id\": 129603,          \"node_id\": \"MDQ6VXNlcjEyOTYwMw==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/129603?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/whimboo\",          \"html_url\": \"https://github.com/whimboo\",          \"followers_url\": \"https://api.github.com/users/whimboo/followers\",          \"following_url\": \"https://api.github.com/users/whimboo/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/whimboo/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/whimboo/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/whimboo/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/whimboo/orgs\",          \"repos_url\": \"https://api.github.com/users/whimboo/repos\",          \"events_url\": \"https://api.github.com/users/whimboo/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/whimboo/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2390549,        \"download_count\": 5162465,        \"created_at\": \"2019-10-12T18:17:49Z\",        \"updated_at\": \"2019-10-12T18:17:53Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-linux64.tar.gz\"      },      {        \"url\": \"https://api.github.com/repos/mozilla/geckodriver/releases/assets/15436813\",        \"id\": 15436813,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE1NDM2ODEz\",        \"name\": \"geckodriver-v0.26.0-macos.tar.gz\",        \"label\": null,        \"uploader\": {          \"login\": \"andreastt\",          \"id\": 399120,          \"node_id\": \"MDQ6VXNlcjM5OTEyMA==\",          \"avatar_url\": \"https://avatars3.githubusercontent.com/u/399120?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/andreastt\",          \"html_url\": \"https://github.com/andreastt\",          \"followers_url\": \"https://api.github.com/users/andreastt/followers\",          \"following_url\": \"https://api.github.com/users/andreastt/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/andreastt/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/andreastt/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/andreastt/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/andreastt/orgs\",          \"repos_url\": \"https://api.github.com/users/andreastt/repos\",          \"events_url\": \"https://api.github.com/users/andreastt/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/andreastt/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-gzip\",        \"state\": \"uploaded\",        \"size\": 2006880,        \"download_count\": 314992,        \"created_at\": \"2019-10-11T23:33:42Z\",        \"updated_at\": \"2019-10-11T23:33:50Z\",        \"browser_download_url\": \"https://github.com/mozilla/geckodriver/releases/download/v0.26.0/geckodriver-v0.26.0-macos.tar.gz\"      }    ],    \"tarball_url\": \"https://api.github.com/repos/mozilla/geckodriver/tarball/v0.4.0\",    \"zipball_url\": \"https://api.github.com/repos/mozilla/geckodriver/zipball/v0.4.0\",    \"body\": \"Only Nightly and DevEdition are official supported. Other versions may work but are not explicitly supported.\n\"  }]";

            Mock<IHttpClient> httpClientMock = new Mock<IHttpClient>();
            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.Build(It.IsAny<IConfig>())).Returns(httpClientMock.Object);

            WebDriverManager.HttpClientFactory = httpClientFactoryMock.Object;

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
    }
}
