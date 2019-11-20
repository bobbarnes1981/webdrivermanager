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
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Processes;

    [TestFixture]
    public class ShellTests
    {
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<ILogger>();
        }

        [Test]
        public void GetVersionFromPowerShellOutputNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Shell(loggerMock.Object).GetVersionFromPowerShellOutput(null));
        }

        [Test]
        public void GetVersionFromPowerShellOutputVersion()
        {
            string version = new Shell(loggerMock.Object).GetVersionFromPowerShellOutput("Version: 1.2.3.4");

            Assert.That(version, Is.EqualTo("1"));
        }

        [Test]
        public void GetVersionFromPosixOutputNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Shell(loggerMock.Object).GetVersionFromPosixOutput(null, "chrome"));

            Assert.Throws<ArgumentNullException>(() => new Shell(loggerMock.Object).GetVersionFromPosixOutput("some output", null));
        }

        [Test]
        public void GetVersionFromPosixOutputVersion()
        {
            string version = new Shell(loggerMock.Object).GetVersionFromPosixOutput("Chromium78.0.3904.97", "chrome");

            Assert.That(version, Is.EqualTo("78"));
        }

        [Test]
        public void GetVersionFromWmicOutputNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Shell(loggerMock.Object).GetVersionFromWmicOutput(null));
        }

        [Test]
        public void GetVersionFromWmicOutputVersion()
        {
            string version = new Shell(loggerMock.Object).GetVersionFromWmicOutput("Version=78.0.3904.97");

            Assert.That(version, Is.EqualTo("78"));
        }
    }
}
