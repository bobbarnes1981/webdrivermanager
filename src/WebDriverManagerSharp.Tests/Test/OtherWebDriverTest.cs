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
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Reflection;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Parameterized test with several browsers.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.3.1
     */
    [TestFixture(typeof(SafariDriver), typeof(DriverServiceNotFoundException))]
    //[TestFixture(typeof(EventFiringWebDriver), InstantiationException)]
    //[TestFixture(typeof(HtmlUnitDriver), null)]
    //[TestFixture(typeof(RemoteWebDriver), typeof(Exception))] // IllegalAccesException)]
    public class OtherWebDriverTest
    {
        public Type driverClass;

        public Type exception;

        protected IWebDriver driver;

        public OtherWebDriverTest(Type driverClass, Type exception)
        {
            this.driverClass = driverClass;
            this.exception = exception;
        }

        [SetUp]
        public void setupTest()
        {
            WebDriverManager.getInstance(driverClass).setup();
        }

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
            ConstructorInfo constructor = driverClass.GetConstructor(new Type[] { });

            if (exception != null)
            {
                Assert.Throws<TargetInvocationException>(() => constructor.Invoke(new object[] { }));
            }
        }
    }
}