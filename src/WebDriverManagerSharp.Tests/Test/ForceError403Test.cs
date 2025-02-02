﻿/*
 * (C) Copyright 2019 Boni Garcia (http://bonigarcia.github.io/)
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
    using NUnit.Framework;
    using WebDriverManagerSharp.Logging;

    /**
     * Force download test.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.3.0
     */
    public class ForceError403Test
    {
        private const int NUM = 40;

        private readonly ILogger log = Resolver.Resolve<ILogger>();

        [Ignore("")]
        [Test]
        public void Test403()
        {
            for (int i = 0; i < NUM; i++)
            {
                log.Debug("Forcing 403 error {0}/{1}", i + 1, NUM);
                WebDriverManager.FirefoxDriver().AvoidAutoVersion().AvoidPreferences().Setup();
                Assert.That(WebDriverManager.FirefoxDriver().GetBinaryPath(), Is.Not.Null);
            }
        }
    }
}