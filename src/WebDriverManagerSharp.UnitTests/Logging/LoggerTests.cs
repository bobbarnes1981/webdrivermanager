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

namespace WebDriverManagerSharp.UnitTests.Logging
{
    using Moq;
    using NUnit.Framework;
    using WebDriverManagerSharp.Logging;

    [TestFixture]
    public class LoggerTests
    {
        private Mock<Serilog.ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<Serilog.ILogger>();
        }

        [Test]
        public void TestTrace()
        {
            Logger logger = new Logger(loggerMock.Object);

            logger.Trace("test");

            loggerMock.Verify(x => x.Verbose("test", It.Is<object[]>(o => o.Length == 0)), Times.Once);
        }

        [Test]
        public void TestWarn()
        {
            Logger logger = new Logger(loggerMock.Object);

            logger.Warn("test");

            loggerMock.Verify(x => x.Warning("test", It.Is<object[]>(o => o.Length == 0)), Times.Once);
        }

        [Test]
        public void TestDebug()
        {
            Logger logger = new Logger(loggerMock.Object);

            logger.Debug("test");

            loggerMock.Verify(x => x.Debug("test", It.Is<object[]>(o => o.Length == 0)), Times.Once);
        }

        [Test]
        public void TestInfo()
        {
            Logger logger = new Logger(loggerMock.Object);

            logger.Info("test");

            loggerMock.Verify(x => x.Information("test", It.Is<object[]>(o => o.Length == 0)), Times.Once);
        }

        [Test]
        public void TestError()
        {
            Logger logger = new Logger(loggerMock.Object);

            logger.Error("test");

            loggerMock.Verify(x => x.Error("test", It.Is<object[]>(o => o.Length == 0)), Times.Once);
        }

        [Test]
        public void TestIsDebug()
        {
            Logger logger = new Logger(loggerMock.Object);

            bool result = logger.IsDebugEnabled();

            loggerMock.Verify(x => x.IsEnabled(Serilog.Events.LogEventLevel.Debug), Times.Once);

            Assert.That(result, Is.False);

            loggerMock.Setup(x => x.IsEnabled(Serilog.Events.LogEventLevel.Debug)).Returns(true);

            result = logger.IsDebugEnabled();

            Assert.That(result, Is.True);
        }

        [Test]
        public void TestIsTrace()
        {
            Logger logger = new Logger(loggerMock.Object);

            bool result = logger.IsTraceEnabled();

            loggerMock.Verify(x => x.IsEnabled(Serilog.Events.LogEventLevel.Verbose), Times.Once);

            loggerMock.Setup(x => x.IsEnabled(Serilog.Events.LogEventLevel.Verbose)).Returns(true);

            result = logger.IsTraceEnabled();

            Assert.That(result, Is.True);
        }
    }
}
