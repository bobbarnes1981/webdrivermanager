/*
 * (C) Copyright 2015 Boni Garcia (http://bonigarcia.github.io/)
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
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;

namespace WebDriverManagerSharp.Tests.Test
{

    /**
     * Parameterized test with several browsers.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.3.1
     */
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    //[TestFixture(typeof(PhantomJsDriver))]
    public class WebDriverTest
    {
        public Type driverClass;

        public WebDriverTest(Type driverClass)
        {
            this.driverClass = driverClass;
        }

        [Test]
        public void testWebDriver()
        {
            WebDriverManager.getInstance(driverClass).setup();
            string binaryPath = WebDriverManager.getInstance(driverClass).getBinaryPath();
            FileInfo binary = new FileInfo(binaryPath);
            Assert.True(binary.Exists);
        }
    }
}