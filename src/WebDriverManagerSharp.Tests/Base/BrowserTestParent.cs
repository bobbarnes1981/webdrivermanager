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

namespace WebDriverManagerSharp.Tests.Base
{
    using System;
    using NUnit.Framework;
    using OpenQA.Selenium;

    /**
     * Parent class for browser based tests.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.1
     */
    public abstract class BrowserTestParent
    {
        protected IWebDriver driver;

        [TearDown]
        public void teardown()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }

        [Test]
        public void test()
        {
            driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 30);
            driver.Url = "https://bonigarcia.github.io/selenium-jupiter/";
            driver.Navigate();
            Assert.That(driver.Title, Contains.Substring("JUnit 5 extension for Selenium"));
        }
    }
}