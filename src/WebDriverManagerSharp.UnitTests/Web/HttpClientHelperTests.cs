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

namespace WebDriverManager.UnitTests.Web
{
    using Moq;
    using NUnit.Framework;
    using System;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Web;

    [TestFixture]
    public class HttpClientHelperTests
    {
        private Mock<IConfig> configMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            configMock = new Mock<IConfig>();
            loggerMock = new Mock<ILogger>();
        }

        [Test]
        public void CreateProxyTest()
        {
            System.Net.IWebProxy proxy = HttpClientHelper.CreateProxy("http://proxy.co.uk:8080");

            Assert.That(proxy, Is.Not.Null);
            Assert.That(proxy.GetProxy(new Uri("http://www.google.co.uk")), Is.EqualTo(new Uri("http://proxy.co.uk:8080")));
        }

        [Test]
        public void ConfigNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HttpClientHelper(null, loggerMock.Object));
        }

        [Test]
        public void Test()
        {
            configMock.Setup(x => x.GetLocalRepositoryUser()).Returns("fakeUser");
            configMock.Setup(x => x.GetLocalRepositoryPassword()).Returns("fakePass");

            HttpClientHelper helper = new HttpClientHelper(configMock.Object, loggerMock.Object);
        }
    }
}
