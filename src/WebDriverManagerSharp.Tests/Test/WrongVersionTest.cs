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

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Test with incorrect version numbers.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.7.2
     */
    public class WrongVersionTest
    {
        [TestCase(typeof(ChromeDriver), "99")]
        [TestCase(typeof(FirefoxDriver), "99")]
        public void testWrongVersion(Type driverClass, string version)
        {
            WebDriverManagerException exception = Assert.Throws<WebDriverManagerException>(WebDriverManager.GetInstance(driverClass).Version(version).Setup);
            //FileInfo binary = new FileInfo(WebDriverManager.getInstance(driverClass).getBinaryPath());
            //Assert.That(binary.Exists);
        }
    }
}