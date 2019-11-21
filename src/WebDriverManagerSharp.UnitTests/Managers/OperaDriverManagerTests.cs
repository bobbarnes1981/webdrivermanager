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
    public class OperaDriverManagerTests : BaseManagerTests
    {
        [Test]
        public void GetVersions()
        {
            Uri driverUrl = new Uri("https://fake.api.github.com/repos/operasoftware/operachromiumdriver/releases");
            configMock.Setup(x => x.GetOperaDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");

            string fakeJson = "[  {    \"url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/21466859\",    \"assets_url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/21466859/assets\",    \"upload_url\": \"https://uploads.github.com/repos/operasoftware/operachromiumdriver/releases/21466859/assets{?name,label}\",    \"html_url\": \"https://github.com/operasoftware/operachromiumdriver/releases/tag/v.78.0.3904.87\",    \"id\": 21466859,    \"node_id\": \"MDc6UmVsZWFzZTIxNDY2ODU5\",    \"tag_name\": \"v.78.0.3904.87\",    \"target_commitish\": \"master\",    \"name\": \"78.0.3904.87\",    \"draft\": false,    \"author\": {      \"login\": \"rkrupski\",      \"id\": 741882,      \"node_id\": \"MDQ6VXNlcjc0MTg4Mg==\",      \"avatar_url\": \"https://avatars2.githubusercontent.com/u/741882?v=4\",      \"gravatar_id\": \"\",      \"url\": \"https://api.github.com/users/rkrupski\",      \"html_url\": \"https://github.com/rkrupski\",      \"followers_url\": \"https://api.github.com/users/rkrupski/followers\",      \"following_url\": \"https://api.github.com/users/rkrupski/following{/other_user}\",      \"gists_url\": \"https://api.github.com/users/rkrupski/gists{/gist_id}\",      \"starred_url\": \"https://api.github.com/users/rkrupski/starred{/owner}{/repo}\",      \"subscriptions_url\": \"https://api.github.com/users/rkrupski/subscriptions\",      \"organizations_url\": \"https://api.github.com/users/rkrupski/orgs\",      \"repos_url\": \"https://api.github.com/users/rkrupski/repos\",      \"events_url\": \"https://api.github.com/users/rkrupski/events{/privacy}\",      \"received_events_url\": \"https://api.github.com/users/rkrupski/received_events\",      \"type\": \"User\",      \"site_admin\": false    },    \"prerelease\": false,    \"created_at\": \"2017-04-05T14:23:06Z\",    \"published_at\": \"2019-11-14T10:34:07Z\",    \"assets\": [      {        \"url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/assets/16155574\",        \"id\": 16155574,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE2MTU1NTc0\",        \"name\": \"operadriver_linux64.zip\",        \"label\": null,        \"uploader\": {          \"login\": \"rkrupski\",          \"id\": 741882,          \"node_id\": \"MDQ6VXNlcjc0MTg4Mg==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/741882?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/rkrupski\",          \"html_url\": \"https://github.com/rkrupski\",          \"followers_url\": \"https://api.github.com/users/rkrupski/followers\",          \"following_url\": \"https://api.github.com/users/rkrupski/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/rkrupski/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/rkrupski/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/rkrupski/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/rkrupski/orgs\",          \"repos_url\": \"https://api.github.com/users/rkrupski/repos\",          \"events_url\": \"https://api.github.com/users/rkrupski/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/rkrupski/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-zip-compressed\",        \"state\": \"uploaded\",        \"size\": 5898614,        \"download_count\": 225,        \"created_at\": \"2019-11-14T09:10:04Z\",        \"updated_at\": \"2019-11-14T09:10:32Z\",        \"browser_download_url\": \"https://github.com/operasoftware/operachromiumdriver/releases/download/v.78.0.3904.87/operadriver_linux64.zip\"      },      {        \"url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/assets/16155575\",        \"id\": 16155575,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE2MTU1NTc1\",        \"name\": \"operadriver_mac64.zip\",        \"label\": null,        \"uploader\": {          \"login\": \"rkrupski\",          \"id\": 741882,          \"node_id\": \"MDQ6VXNlcjc0MTg4Mg==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/741882?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/rkrupski\",          \"html_url\": \"https://github.com/rkrupski\",          \"followers_url\": \"https://api.github.com/users/rkrupski/followers\",          \"following_url\": \"https://api.github.com/users/rkrupski/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/rkrupski/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/rkrupski/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/rkrupski/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/rkrupski/orgs\",          \"repos_url\": \"https://api.github.com/users/rkrupski/repos\",          \"events_url\": \"https://api.github.com/users/rkrupski/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/rkrupski/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-zip-compressed\",        \"state\": \"uploaded\",        \"size\": 7517740,        \"download_count\": 85,        \"created_at\": \"2019-11-14T09:10:05Z\",        \"updated_at\": \"2019-11-14T09:10:33Z\",        \"browser_download_url\": \"https://github.com/operasoftware/operachromiumdriver/releases/download/v.78.0.3904.87/operadriver_mac64.zip\"      }    ],    \"tarball_url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/tarball/v0.1.0\",    \"zipball_url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/zipball/v0.1.0\",    \"body\": \"OperaChromiumDriver early beta.\n\"  }]";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            List<string> versions = WebDriverManager.OperaDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetVersionsMirror()
        {
            Uri driverUrl = new Uri("http://fake.npm.taobao.org/mirrors/operadriver");
            Uri driverSubUrl = new Uri("http://fake.npm.taobao.org/mirrors/operadriver/.75.0.3770.100/");
            configMock.Setup(x => x.GetOperaDriverMirrorUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");
            configMock.Setup(x => x.IsUseMirror()).Returns(true);

            string fakeHtml = "<a href=\"/mirrors/operadriver/.75.0.3770.100/\">.75.0.3770.100</a>";
            string fakeSubHtml = "<a href=\"/mirrors/operadriver/.75.0.3770.100/operadriver_win64.zip\">operadriver_win64.zip</a>";

            httpClientMock.Setup(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeHtml)));
            httpClientMock.Setup(x => x.ExecuteHttpGet(driverSubUrl, It.IsAny<AuthenticationHeaderValue>())).Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeSubHtml)));

            List<string> versions = WebDriverManager.OperaDriver().GetVersions();

            Assert.That(versions, Is.Not.Null);
            Assert.That(versions.Count, Is.EqualTo(1));
        }

        [Test]
        public void SetVersionLatest()
        {
            WebDriverManager.OperaDriver().Version("latest");

            configMock.Verify(x => x.SetOperaDriverVersion("latest"), Times.Once);
        }

        [Test]
        public void SetDriverRepositoryUrl()
        {
            Uri uri = new Uri("http://www.uri.com");

            WebDriverManager.OperaDriver().DriverRepositoryUrl(uri);

            configMock.Verify(x => x.SetOperaDriverUrl(uri), Times.Once);
        }

        [Test]
        public void TestSetUp()
        {
            Uri driverUrl = new Uri("https://fake.api.github.com/repos/operasoftware/operachromiumdriver/releases");
            configMock.Setup(x => x.GetOperaDriverUrl()).Returns(driverUrl);
            configMock.Setup(x => x.GetGitHubTokenName()).Returns("fakeUser");
            configMock.Setup(x => x.GetGitHubTokenSecret()).Returns("fakePass");

            string fakeJson = "[  {    \"url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/21466859\",    \"assets_url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/21466859/assets\",    \"upload_url\": \"https://uploads.github.com/repos/operasoftware/operachromiumdriver/releases/21466859/assets{?name,label}\",    \"html_url\": \"https://github.com/operasoftware/operachromiumdriver/releases/tag/v.78.0.3904.87\",    \"id\": 21466859,    \"node_id\": \"MDc6UmVsZWFzZTIxNDY2ODU5\",    \"tag_name\": \"v.78.0.3904.87\",    \"target_commitish\": \"master\",    \"name\": \"78.0.3904.87\",    \"draft\": false,    \"author\": {      \"login\": \"rkrupski\",      \"id\": 741882,      \"node_id\": \"MDQ6VXNlcjc0MTg4Mg==\",      \"avatar_url\": \"https://avatars2.githubusercontent.com/u/741882?v=4\",      \"gravatar_id\": \"\",      \"url\": \"https://api.github.com/users/rkrupski\",      \"html_url\": \"https://github.com/rkrupski\",      \"followers_url\": \"https://api.github.com/users/rkrupski/followers\",      \"following_url\": \"https://api.github.com/users/rkrupski/following{/other_user}\",      \"gists_url\": \"https://api.github.com/users/rkrupski/gists{/gist_id}\",      \"starred_url\": \"https://api.github.com/users/rkrupski/starred{/owner}{/repo}\",      \"subscriptions_url\": \"https://api.github.com/users/rkrupski/subscriptions\",      \"organizations_url\": \"https://api.github.com/users/rkrupski/orgs\",      \"repos_url\": \"https://api.github.com/users/rkrupski/repos\",      \"events_url\": \"https://api.github.com/users/rkrupski/events{/privacy}\",      \"received_events_url\": \"https://api.github.com/users/rkrupski/received_events\",      \"type\": \"User\",      \"site_admin\": false    },    \"prerelease\": false,    \"created_at\": \"2017-04-05T14:23:06Z\",    \"published_at\": \"2019-11-14T10:34:07Z\",    \"assets\": [      {        \"url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/assets/16155574\",        \"id\": 16155574,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE2MTU1NTc0\",        \"name\": \"operadriver_linux64.zip\",        \"label\": null,        \"uploader\": {          \"login\": \"rkrupski\",          \"id\": 741882,          \"node_id\": \"MDQ6VXNlcjc0MTg4Mg==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/741882?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/rkrupski\",          \"html_url\": \"https://github.com/rkrupski\",          \"followers_url\": \"https://api.github.com/users/rkrupski/followers\",          \"following_url\": \"https://api.github.com/users/rkrupski/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/rkrupski/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/rkrupski/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/rkrupski/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/rkrupski/orgs\",          \"repos_url\": \"https://api.github.com/users/rkrupski/repos\",          \"events_url\": \"https://api.github.com/users/rkrupski/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/rkrupski/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-zip-compressed\",        \"state\": \"uploaded\",        \"size\": 5898614,        \"download_count\": 225,        \"created_at\": \"2019-11-14T09:10:04Z\",        \"updated_at\": \"2019-11-14T09:10:32Z\",        \"browser_download_url\": \"https://github.com/operasoftware/operachromiumdriver/releases/download/v.78.0.3904.87/operadriver_linux64.zip\"      },      {        \"url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/releases/assets/16155575\",        \"id\": 16155575,        \"node_id\": \"MDEyOlJlbGVhc2VBc3NldDE2MTU1NTc1\",        \"name\": \"operadriver_win64.zip\",        \"label\": null,        \"uploader\": {          \"login\": \"rkrupski\",          \"id\": 741882,          \"node_id\": \"MDQ6VXNlcjc0MTg4Mg==\",          \"avatar_url\": \"https://avatars2.githubusercontent.com/u/741882?v=4\",          \"gravatar_id\": \"\",          \"url\": \"https://api.github.com/users/rkrupski\",          \"html_url\": \"https://github.com/rkrupski\",          \"followers_url\": \"https://api.github.com/users/rkrupski/followers\",          \"following_url\": \"https://api.github.com/users/rkrupski/following{/other_user}\",          \"gists_url\": \"https://api.github.com/users/rkrupski/gists{/gist_id}\",          \"starred_url\": \"https://api.github.com/users/rkrupski/starred{/owner}{/repo}\",          \"subscriptions_url\": \"https://api.github.com/users/rkrupski/subscriptions\",          \"organizations_url\": \"https://api.github.com/users/rkrupski/orgs\",          \"repos_url\": \"https://api.github.com/users/rkrupski/repos\",          \"events_url\": \"https://api.github.com/users/rkrupski/events{/privacy}\",          \"received_events_url\": \"https://api.github.com/users/rkrupski/received_events\",          \"type\": \"User\",          \"site_admin\": false        },        \"content_type\": \"application/x-zip-compressed\",        \"state\": \"uploaded\",        \"size\": 7517740,        \"download_count\": 85,        \"created_at\": \"2019-11-14T09:10:05Z\",        \"updated_at\": \"2019-11-14T09:10:33Z\",        \"browser_download_url\": \"https://github.com/operasoftware/operachromiumdriver/releases/download/v.78.0.3904.87/operadriver_win64.zip\"      }    ],    \"tarball_url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/tarball/v0.1.0\",    \"zipball_url\": \"https://api.github.com/repos/operasoftware/operachromiumdriver/zipball/v0.1.0\",    \"body\": \"OperaChromiumDriver early beta.\n\"  }]";

            httpClientMock.SetupSequence(x => x.ExecuteHttpGet(driverUrl, It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(fakeJson)));

            fileStorageMock.Setup(x => x.GetFileInfos(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new FileInfo[0]);

            configMock.Setup(x => x.GetTargetPath()).Returns("c:\\config_target");
            configMock.Setup(x => x.GetOs()).Returns("WIN");

            downloaderMock.Setup(x => x.GetTargetPath()).Returns("c:\\download_target");
            downloaderMock.Setup(x => x.Download(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo("c:\\config_target\\driver.exe"));

            WebDriverManager.OperaDriver().Setup();

            configMock.Verify(x => x.GetOperaDriverExport(), Times.Once);
        }
    }
}
