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

namespace WebDriverManagerSharp.UnitTests.Processes
{
    using Autofac;
    using Moq;
    using NUnit.Framework;
    using System.Diagnostics;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;

    [TestFixture]
    public class ProcessBuilderTests
    {
        private ProcessStartInfo startInfo;
        private Mock<IProcess> processMock;

        [SetUp]
        public void SetUp()
        {
            startInfo = new ProcessStartInfo();

            processMock = new Mock<IProcess>();
            processMock.SetupGet(x => x.StartInfo).Returns(startInfo);

            Resolver.OverriddenRegistrations += (builder) =>
            {
                builder.RegisterInstance(processMock.Object).As<IProcess>();
            };
        }

        [Test]
        public void TestDirectory()
        {
            Mock<IDirectory> directoryMock = new Mock<IDirectory>();
            directoryMock.Setup(x => x.FullName).Returns("fake_dir");

            ProcessBuilder builder = new ProcessBuilder("fake_command");

            builder.Directory(directoryMock.Object).Start();

            directoryMock.Verify(x => x.FullName, Times.Once);

            Assert.That(processMock.Object.StartInfo.FileName, Is.EqualTo("fake_dir\\fake_command"));
        }

        [Test]
        public void TestRedirectOutputStream()
        {
            ProcessBuilder builder = new ProcessBuilder("fake_command");

            builder.RedirectOutputStream(true).Start();

            Assert.That(processMock.Object.StartInfo.RedirectStandardOutput, Is.True);
        }
    }
}
