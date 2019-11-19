/*
 * (C) Copyright 2019 Robert barnes
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

namespace WebDriverManagerSharp.UnitTests
{
    using NUnit.Framework;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Edge;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Opera;
    using System;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Managers;

    [TestFixture]
    public class WebDriverManagerTests
    {
        [Test]
        public void GetFirefoxDriverManager()
        {
            WebDriverManager manager = WebDriverManager.FirefoxDriver();

            Assert.That(manager, Is.InstanceOf<FirefoxDriverManager>());
        }

        [Test]
        public void GetFirefoxInstanceType()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(typeof(FirefoxDriver));

            Assert.That(manager, Is.InstanceOf<FirefoxDriverManager>());
        }

        [Test]
        public void GetFirefoxInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.FIREFOX);

            Assert.That(manager, Is.InstanceOf<FirefoxDriverManager>());
        }

        [Test]
        public void GetInternetExplorerDriverManager()
        {
            WebDriverManager manager = WebDriverManager.IEDriver();

            Assert.That(manager, Is.InstanceOf<InternetExplorerDriverManager>());
        }

        [Test]
        public void GetInternetExplorerInstanceType()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(typeof(InternetExplorerDriver));

            Assert.That(manager, Is.InstanceOf<InternetExplorerDriverManager>());
        }

        [Test]
        public void GetInternetExplorerInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.IEXPLORER);

            Assert.That(manager, Is.InstanceOf<InternetExplorerDriverManager>());
        }

        [Test]
        public void GetEdgeDriverManager()
        {
            WebDriverManager manager = WebDriverManager.EdgeDriver();

            Assert.That(manager, Is.InstanceOf<EdgeDriverManager>());
        }

        [Test]
        public void GetEdgeInstanceType()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(typeof(EdgeDriver));

            Assert.That(manager, Is.InstanceOf<EdgeDriverManager>());
        }

        [Test]
        public void GetEdgeInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.EDGE);

            Assert.That(manager, Is.InstanceOf<EdgeDriverManager>());
        }

        [Test]
        public void GetChromeDriverManager()
        {
            WebDriverManager manager = WebDriverManager.ChromeDriver();

            Assert.That(manager, Is.InstanceOf<ChromeDriverManager>());
        }

        [Test]
        public void GetChromeInstanceType()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(typeof(ChromeDriver));

            Assert.That(manager, Is.InstanceOf<ChromeDriverManager>());
        }

        [Test]
        public void GetChromeInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.CHROME);

            Assert.That(manager, Is.InstanceOf<ChromeDriverManager>());
        }

        [Test]
        public void GetOperaDriverManager()
        {
            WebDriverManager manager = WebDriverManager.OperaDriver();

            Assert.That(manager, Is.InstanceOf<OperaDriverManager>());
        }

        [Test]
        public void GetOperaInstanceType()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(typeof(OperaDriver));

            Assert.That(manager, Is.InstanceOf<OperaDriverManager>());
        }

        [Test]
        public void GetOperaInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.OPERA);

            Assert.That(manager, Is.InstanceOf<OperaDriverManager>());
        }

        [Test]
        public void GetPhantomJSDriverManager()
        {
            WebDriverManager manager = WebDriverManager.PhantomJS();

            Assert.That(manager, Is.InstanceOf<PhantomJsDriverManager>());
        }

        [Test]
        [Ignore("not supported")]
        public void GetPhantomJSInstanceType()
        {
            //WebDriverManager manager = WebDriverManager.GetInstance(typeof(PhantomJSDriver));

            //Assert.That(manager, Is.InstanceOf<PhantomJsDriverManager>());
        }

        [Test]
        public void GetPhantomJSInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.PHANTOMJS);

            Assert.That(manager, Is.InstanceOf<PhantomJsDriverManager>());
        }

        [Test]
        public void GetStandaloneDriverManager()
        {
            WebDriverManager manager = WebDriverManager.SeleniumServerStandalone();

            Assert.That(manager, Is.InstanceOf<SeleniumServerStandaloneManager>());
        }

        [Test]
        public void GetStandaloneInstanceType()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(typeof(object));

            Assert.That(manager, Is.InstanceOf<VoidDriverManager>());
        }

        [Test]
        public void GetStandaloneInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.SELENIUM_SERVER_STANDALONE);

            Assert.That(manager, Is.InstanceOf<SeleniumServerStandaloneManager>());
        }

        [Test]
        public void GetNullInstanceType()
        {
            Assert.Throws<ArgumentNullException>(() => WebDriverManager.GetInstance(null));
        }
    }
}
