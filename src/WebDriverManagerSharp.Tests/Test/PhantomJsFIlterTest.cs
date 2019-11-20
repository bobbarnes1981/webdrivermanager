/*
 * (C) Copyright 2016 Boni Garcia (http://bonigarcia.github.io/)
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

namespace WebDriverManagerSharp.Tests.Test
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using NUnit.Framework;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Web;

    /**
     * Filter verifications for phantomjs.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.0
     */
    public class PhantomJsFilterTest
    {
        protected WebDriverManager phantomJsManager;
        protected List<Uri> driversUrls;

        private readonly ILogger log = Logger.GetLogger();
        private const string phantomJsBinaryName = "phantomjs";

        [SetUp]
        public void Setup()
        {
            phantomJsManager = WebDriverManager.PhantomJS();
            FieldInfo field = typeof(WebDriverManager).GetField("httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(phantomJsManager, new HttpClient(new Config(Logger.GetLogger(), new SystemInformation(), new FileStorage()), Logger.GetLogger()));

            MethodInfo method = typeof(WebDriverManager).GetMethod("GetDrivers", BindingFlags.Instance | BindingFlags.NonPublic);
            driversUrls = (List<Uri>)method.Invoke(phantomJsManager, new object[0]);
        }

        [Test]
        public void TestFilterPhantomJs()
        {
            MethodInfo method = typeof(WebDriverManager).GetMethod("checkLatest", BindingFlags.Instance | BindingFlags.NonPublic);
            List<Uri> latestUrls = (List<Uri>)method.Invoke(phantomJsManager, new object[] { driversUrls, phantomJsBinaryName });

            List<Uri> filteredLatestUrls = new UrlFilter(Logger.GetLogger(), new FileStorage()).FilterByArch(latestUrls, Architecture.X64, false);

            log.Info("Filtered UriS for LATEST version {0} : {1}", phantomJsBinaryName, filteredLatestUrls);

            Assert.That(filteredLatestUrls, Is.Not.Empty);
        }

        [Test]
        public void TestFilterVersionPhantomJs()
        {
            string specificVersion = "1.9.6";
            MethodInfo method = typeof(WebDriverManager).GetMethod("getVersion", BindingFlags.Instance | BindingFlags.NonPublic);
            List<Uri> specificVersionUrls = (List<Uri>)method.Invoke(phantomJsManager, new object[] { driversUrls, phantomJsBinaryName, specificVersion });

            List<Uri> filteredVersionUrls = new UrlFilter(Logger.GetLogger(), new FileStorage()).FilterByArch(specificVersionUrls, Architecture.X64, false);

            log.Info("Filtered UriS for {0} version {1}: {2}", phantomJsBinaryName, specificVersion, filteredVersionUrls);

            Assert.That(filteredVersionUrls, Is.Not.Empty);
        }
    }
}