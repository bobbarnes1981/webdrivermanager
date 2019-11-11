/*
 * (C) Copyright 2017 Boni Garcia (http://bonigarcia.github.io/)
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

namespace WebDriverManager.Tests.Test
{
    /**
     * Test with incorrect version numbers.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.7.2
     */
    [TestFixture(typeof(ChromeDriver), "99")]
    [TestFixture(typeof(FirefoxDriver), "99")]
    public class WrongVersionTest
    {
        public Type driverClass;

        public string version;

        public WrongVersionTest(Type driverClass, string version)
        {
            this.driverClass = driverClass;
            this.version = version;
        }

        [Test]
        public void testWrongVersion()
        {
            WebDriverManagerException exception = Assert.Throws<WebDriverManagerException>(WebDriverManager.getInstance(driverClass).version(version).setup);
            //FileInfo binary = new FileInfo(WebDriverManager.getInstance(driverClass).getBinaryPath());
            //Assert.That(binary.Exists);
        }
    }
}