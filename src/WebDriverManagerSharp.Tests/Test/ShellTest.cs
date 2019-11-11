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

using NUnit.Framework;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Shell utilities test.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.6.1
     */
    [TestFixture("Chromium 73.0.3683.86 Built on Ubuntu , running on Ubuntu 16.04", "73")]
    [TestFixture("Chromium 74.0.3729.169 Built on Ubuntu , running on Ubuntu 18.04", "74")]
    [TestFixture("Google Chrome 75.0.3770.80", "75")]
    public class ShellTest
    {
        public string output;

        public string version;

        public ShellTest(string output, string version)
        {
            this.output = output;
            this.version = version;
        }

        [Test]
        public void versionFromPosixOutputTest()
        {
            string versionFromPosixOutput = Shell.getVersionFromPosixOutput(output, DriverManagerType.CHROME.ToString());
            Assert.That(versionFromPosixOutput, Is.EqualTo(version));
        }
    }
}