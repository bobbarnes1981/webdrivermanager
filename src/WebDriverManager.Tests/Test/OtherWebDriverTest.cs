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
using System;

namespace WebDriverManager.Tests.Test
{
    /**
     * Parameterized test with several browsers.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.3.1
     */
    public class OtherWebDriverTest
    {
        public Type driverClass;

        public Exception exception;

        protected IWebDriver driver;

        //public static Collection<Object[]> data()
        //{
        //    return asList(new Object[][] {
        //            { SafariDriver.class, WebDriverException.class },
        //            { EventFiringWebDriver.class, InstantiationException.class },
        //            { HtmlUnitDriver.class, null },
        //            { RemoteWebDriver.class, IllegalAccessException.class } });
        //}

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
            if (exception != null)
            {
                //thrown.expect(exception);
            }
            //driver = driverClass.newInstance();
        }

    }
}