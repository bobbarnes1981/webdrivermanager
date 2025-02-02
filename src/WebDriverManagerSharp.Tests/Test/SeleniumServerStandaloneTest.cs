﻿/*
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
    using System.IO;
    using NUnit.Framework;
    using WebDriverManagerSharp.Logging;

    /**
     * Test with Selenium Server.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.1
     */
    public class SeleniumServerStandaloneTest
    {
        private readonly ILogger log = Resolver.Resolve<ILogger>();

        [Test]
        public void TestSeleniumServerLatest()
        {
            WebDriverManager.SeleniumServerStandalone().Setup();
            assertBinary();
        }

        [Test]
        public void TestSeleniumServerVersion()
        {
            WebDriverManager.SeleniumServerStandalone().Version("3.13").Setup();
            assertBinary();
        }

        private void assertBinary()
        {
            FileInfo binary = new FileInfo(WebDriverManager.SeleniumServerStandalone().GetBinaryPath());
            log.Debug("Binary path for selenium-server-standalone {0}", binary);
            Assert.True(binary.Exists);
        }
    }
}