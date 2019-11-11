/*
 * (C) Copyright 2018 Boni Garcia (http://bonigarcia.github.io/)
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
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
using System;
using System.Collections.Generic;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Test getting all versions.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.0
     */
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    [TestFixture(typeof(OperaDriver))]
    [TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(InternetExplorerDriver))]
    //[TestFixture(typeof(PhantomJsDriver))]
    public class VersionsTest
    {
        private readonly ILogger log = Logger.GetLogger();

        public WebDriverManager driverManager;

        public VersionsTest(Type driverManager)
        {
            this.driverManager = WebDriverManager.getInstance(driverManager);
        }

        [Test]
        public void testChromeDriverVersions()
        {
            List<string> versions = driverManager.getVersions();
            log.Debug("Versions of {0} {1}", driverManager.GetType().Name, versions);
            Assert.That(versions, Is.Not.Null);
            Assert.That(versions, Is.Not.Empty);
        }
    }
}