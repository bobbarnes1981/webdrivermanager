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
    using Autofac;
    using Moq;
    using NUnit.Framework;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Web;

    public abstract class BaseManagerTests
    {
        protected Mock<IConfig> configMock { get; set; }
        protected Mock<IHttpClientHelper> httpClientMock { get; set; }
        protected Mock<IShell> shellMock { get; set; }
        protected Mock<IFileStorage> fileStorageMock { get; set; }
        protected Mock<IDownloader> downloaderMock { get; set; }

        [SetUp]
        public void SetUp()
        {
            WebDriverManager.ClearDrivers();

            configMock = new Mock<IConfig>();
            httpClientMock = new Mock<IHttpClientHelper>();
            shellMock = new Mock<IShell>();
            fileStorageMock = new Mock<IFileStorage>();
            downloaderMock = new Mock<IDownloader>();

            Resolver.OverriddenRegistrations = builder =>
            {
                builder.RegisterInstance(configMock.Object).As<IConfig>();
                builder.RegisterInstance(shellMock.Object).As<IShell>();
                builder.RegisterInstance(httpClientMock.Object).As<IHttpClientHelper>();
                builder.RegisterInstance(fileStorageMock.Object).As<IFileStorage>();
                builder.RegisterInstance(downloaderMock.Object).As<IDownloader>();
            };
        }
    }
}
