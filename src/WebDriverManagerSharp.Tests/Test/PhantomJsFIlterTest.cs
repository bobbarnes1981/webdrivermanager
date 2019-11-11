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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebDriverManager.Tests.Test
{

    /**
     * Filter verifications for phantomjs.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.0
     */
    public class PhantomJsFilterTest
    {

        private readonly ILogger log = Logger.GetLogger();

        protected WebDriverManager phantomJsManager;
        protected List<Uri> driversUrls;
        protected string phantomJsBinaryName = "phantomjs";

        [SetUp]
        public void setup()
        {
            phantomJsManager = WebDriverManager.phantomjs();
            FieldInfo field = typeof(WebDriverManager).GetField("httpClient");
            field.SetValue(phantomJsManager, new HttpClient(new Config()));

            MethodInfo method = typeof(WebDriverManager).GetMethod("getDrivers");
            driversUrls = (List<Uri>)method.Invoke(phantomJsManager, new object[0]);
        }

        [Test]
        public void testFilterPhantomJs()
        {
            MethodInfo method = typeof(WebDriverManager).GetMethod("checkLatest");
            List<Uri> latestUrls = (List<Uri>)method.Invoke(phantomJsManager, new object[] { driversUrls, phantomJsBinaryName });

            List<Uri> filteredLatestUrls = new UrlFilter().filterByArch(latestUrls, Architecture.X64, false);

            log.Info("Filtered UriS for LATEST version {} : {}", phantomJsBinaryName, filteredLatestUrls);

            Assert.That(filteredLatestUrls, Is.Not.Empty);
        }

        [Test]
        public void testFilterVersionPhantomJs()
        {
            String specificVersion = "1.9.6";
            MethodInfo method = typeof(WebDriverManager).GetMethod("getVersion");
            List<Uri> specificVersionUrls = (List<Uri>)method.Invoke(
                    phantomJsManager, new object[] { driversUrls, phantomJsBinaryName,
                    specificVersion });

            List<Uri> filteredVersionUrls = new UrlFilter()
                    .filterByArch(specificVersionUrls, Architecture.X64, false);

            log.Info("Filtered UriS for {} version {}: {}", phantomJsBinaryName,
                            specificVersion, filteredVersionUrls);

            Assert.That(filteredVersionUrls, Is.Not.Empty);
        }
    }
}