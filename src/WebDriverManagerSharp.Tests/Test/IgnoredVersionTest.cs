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

namespace WebDriverManagerSharp.Tests.Test
{
    using System.IO;
    using NUnit.Framework;

    /**
     * Test for ignore versions.
     * 
     * @since 1.7.2
     */
    public class IgnoredVersionTest
    {
        private readonly ILogger log = Logger.GetLogger();

        [SetUp]
        [TearDown]
        public void CleanCache()
        {
            new DirectoryInfo(new Downloader(DriverManagerType.CHROME).GetTargetPath()).Delete(true);
        }

        [Test]
        public void TestIgnoreVersions()
        {
            string[] ignoredVersions = { "2.33", "2.32" };
            WebDriverManager.ChromeDriver().IgnoreVersions(ignoredVersions).Setup();
            FileInfo binary = new FileInfo(WebDriverManager.ChromeDriver().GetBinaryPath());
            log.Debug("Using binary {0} (ignoring {1})", binary, ignoredVersions.ToStringJava());

            foreach (string version in ignoredVersions)
            {
                StringAssert.DoesNotContain(binary.FullName, version);
            }
        }
    }
}