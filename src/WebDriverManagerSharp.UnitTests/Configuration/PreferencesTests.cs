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

namespace WebDriverManager.UnitTests.Configuration
{
    using Moq;
    using NUnit.Framework;
    using System.Threading;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Logging;

    [TestFixture]
    public class PreferencesTests
    {
        private Mock<ILogger> loggerMock;
        private Mock<IConfig> configMock;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<ILogger>();
            configMock = new Mock<IConfig>();

            loggerMock.Setup(x => x.IsDebugEnabled()).Returns(true);
            configMock.Setup(x => x.GetTtl()).Returns(1);
        }

        [Test]
        public void TestAddValueIfEmpty()
        {
            Preferences preferences = new Preferences(loggerMock.Object, configMock.Object);

            preferences.PutValueInPreferencesIfEmpty("testKey", "firstValue");

            string val = preferences.GetValueFromPreferences("testKey");

            Assert.That(val, Is.EqualTo("firstValue"));

            preferences.PutValueInPreferencesIfEmpty("testKey", "secondValue");

            val = preferences.GetValueFromPreferences("testKey");

            Assert.That(val, Is.EqualTo("firstValue"));
        }

        [Test]
        public void TestClearValues()
        {
            Preferences preferences = new Preferences(loggerMock.Object, configMock.Object);

            preferences.PutValueInPreferencesIfEmpty("testKey", "firstValue");

            string val = preferences.GetValueFromPreferences("testKey");

            Assert.That(val, Is.EqualTo("firstValue"));

            preferences.Clear();

            val = preferences.GetValueFromPreferences("testKey");

            Assert.That(val, Is.Null);
        }

        [Test]
        public void TestClearValuesException()
        {
            loggerMock.Setup(x => x.Info(It.IsAny<string>())).Throws(new System.Exception("my exception"));

            Preferences preferences = new Preferences(loggerMock.Object, configMock.Object);

            preferences.PutValueInPreferencesIfEmpty("testKey", "firstValue");

            string val = preferences.GetValueFromPreferences("testKey");

            Assert.That(val, Is.EqualTo("firstValue"));

            preferences.Clear();

            loggerMock.Verify(x => x.Warn(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void TestCheckValueExists()
        {
            Preferences preferences = new Preferences(loggerMock.Object, configMock.Object);

            bool exists = preferences.CheckKeyInPreferences("testKey");

            Assert.That(exists, Is.False);

            preferences.PutValueInPreferencesIfEmpty("testKey", "firstValue");

            exists = preferences.CheckKeyInPreferences("testKey");

            Assert.That(exists, Is.True);

            Thread.Sleep(1000);

            exists = preferences.CheckKeyInPreferences("testKey");

            Assert.That(exists, Is.False);
        }
    }
}
