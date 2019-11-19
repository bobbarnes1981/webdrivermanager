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

namespace WebDriverManager.UnitTests.Web
{
    using Moq;
    using NUnit.Framework;
    using System;
    using WebDriverManagerSharp;

    [TestFixture]
    public class ShellTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void GetVersionFromPowerShellOutputNull()
        {
            Assert.Throws<ArgumentNullException>(() => Shell.GetVersionFromPowerShellOutput(null));
        }

        [Test]
        public void GetVersionFromPowerShellOutputVersion()
        {
            string version = Shell.GetVersionFromPowerShellOutput("Version: 1.2.3.4");

            Assert.That(version, Is.EqualTo("1"));
        }

        [Test]
        public void GetVersionFromPosixOutputNull()
        {
            Assert.Throws<ArgumentNullException>(() => Shell.GetVersionFromPosixOutput(null, "chrome"));

            Assert.Throws<ArgumentNullException>(() => Shell.GetVersionFromPosixOutput("some output", null));
        }

        [Test]
        public void GetVersionFromPosixOutputVersion()
        {
            string version = Shell.GetVersionFromPosixOutput("Chromium78.0.3904.97", "chrome");

            Assert.That(version, Is.EqualTo("78"));
        }

        [Test]
        public void GetVersionFromWmicOutputNull()
        {
            Assert.Throws<ArgumentNullException>(() => Shell.GetVersionFromWmicOutput(null));
        }

        [Test]
        public void GetVersionFromWmicOutputVersion()
        {
            string version = Shell.GetVersionFromWmicOutput("Version=78.0.3904.97");

            Assert.That(version, Is.EqualTo("78"));
        }
    }
}
