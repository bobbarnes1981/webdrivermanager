/*
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

    /**
     * Test latest version of chromedriver.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.6.0
     */
    public class ChromeLatestTest
    {
        private readonly ILogger log = Logger.GetLogger();

        [Test]
        public void TestLatestAndBetaChromedriver()
        {
            WebDriverManager.ChromeDriver().AvoidPreferences().AvoidAutoVersion().Setup();
            string chromedriverStable = WebDriverManager.ChromeDriver().GetDownloadedVersion();
            log.Debug("Chromedriver LATEST version: {0}", chromedriverStable);

            WebDriverManager.ChromeDriver().AvoidPreferences().AvoidAutoVersion().UseBetaVersions().Setup();
            string chromedriverBeta = WebDriverManager.ChromeDriver().GetDownloadedVersion();
            log.Debug("Chromedriver BETA version: {0}", chromedriverBeta);

            Assert.That(chromedriverStable, Is.Not.EqualTo(chromedriverBeta));
        }
    }
}