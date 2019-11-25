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
    using System.IO;
    using System.Text;
    using Autofac;
    using Moq;
    using NUnit.Framework;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Processes;

    [TestFixture]
    public class ShellTests
    {
        private Mock<ILogger> loggerMock;
        private Mock<IProcessBuilder> builderMock;
        private Mock<IProcess> processMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<ILogger>();
            builderMock = new Mock<IProcessBuilder>();
            processMock = new Mock<IProcess>();

            builderMock.Setup(x => x.Directory(It.IsAny<DirectoryInfo>())).Returns(builderMock.Object);
            builderMock.Setup(x => x.RedirectOutputStream(It.IsAny<bool>())).Returns(builderMock.Object);
            builderMock.Setup(x => x.Start()).Returns(processMock.Object);

            Resolver.OverriddenRegistrations = (builder) =>
            {
                builder.RegisterInstance(builderMock.Object).As<IProcessBuilder>();
            };
        }

        [Test]
        public void TestRunAndWaitException()
        {
            processMock.Setup(x => x.StandardOutput).Throws(new IOException("failed"));

            Shell shell = new Shell(loggerMock.Object);

            string result = shell.RunAndWait("mycommand");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void TestRunAndWaitExceptionDebug()
        {
            processMock.Setup(x => x.StandardOutput).Throws(new IOException("failed"));

            loggerMock.Setup(x => x.IsDebugEnabled()).Returns(true);

            Shell shell = new Shell(loggerMock.Object);

            string result = shell.RunAndWait("mycommand");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void TestRunAndWait()
        {
            processMock.Setup(x => x.StandardOutput).Returns(new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes("test response"))));

            Shell shell = new Shell(loggerMock.Object);

            string result = shell.RunAndWait("mycommand");

            Assert.That(result, Is.EqualTo("test response"));
        }

        [Test]
        public void TestRunAndWaitDirectory()
        {
            processMock.Setup(x => x.StandardOutput).Returns(new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes("test response"))));

            Shell shell = new Shell(loggerMock.Object);

            string result = shell.RunAndWait(new DirectoryInfo("c:\\fake"), "mycommand");

            Assert.That(result, Is.EqualTo("test response"));
        }
    }
}
