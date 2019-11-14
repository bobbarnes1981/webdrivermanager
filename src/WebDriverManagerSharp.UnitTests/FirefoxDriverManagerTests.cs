﻿/*
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
    using OpenQA.Selenium.Firefox;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Managers;

    [TestFixture]
    public class FirefoxDriverManagerTests
    {
        [Test]
        public void GetDriverManager()
        {
            WebDriverManager manager = WebDriverManager.FirefoxDriver();

            Assert.That(manager, Is.InstanceOf<FirefoxDriverManager>());
        }

        [Test]
        public void GetInstanceType()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(typeof(FirefoxDriver));

            Assert.That(manager, Is.InstanceOf<FirefoxDriverManager>());
        }

        [Test]
        public void GetInstanceEnum()
        {
            WebDriverManager manager = WebDriverManager.GetInstance(DriverManagerType.FIREFOX);

            Assert.That(manager, Is.InstanceOf<FirefoxDriverManager>());
        }
    }
}
