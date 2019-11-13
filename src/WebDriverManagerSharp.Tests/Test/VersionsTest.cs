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

namespace WebDriverManagerSharp.Tests.Test
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Edge;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Opera;

    /**
     * Test getting all versions.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.0
     */
    public class VersionsTest
    {
        private readonly ILogger log = Logger.GetLogger();

        [TestCase(typeof(ChromeDriver))]
        [TestCase(typeof(FirefoxDriver))]
        [TestCase(typeof(OperaDriver))]
        [TestCase(typeof(EdgeDriver))]
        [TestCase(typeof(InternetExplorerDriver))]
        //[TestCase(typeof(PhantomJsDriver))]
        public void testDriverVersions(Type driverManagerClass)
        {
            WebDriverManager driverManager = WebDriverManager.GetInstance(driverManagerClass);

            List<string> versions = driverManager.GetVersions();
            log.Debug("Versions of {0} {1}", driverManager.GetType().Name, versions);
            Assert.That(versions, Is.Not.Null);
            Assert.That(versions, Is.Not.Empty);
        }
    }
}