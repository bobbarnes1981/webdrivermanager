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
    using NUnit.Framework;
    using WebDriverManagerSharp.Configuration;

    [TestFixture]
    public class ConfigKeyTests
    {
        [Test]
        public void TestName()
        {
            ConfigKey<string> key = new ConfigKey<string>("keyName");

            Assert.That(key.GetName(), Is.EqualTo("keyName"));
        }

        [Test]
        public void TestSetValue()
        {
            ConfigKey<string> key = new ConfigKey<string>("keyName");

            Assert.That(key.GetValue(), Is.Null);

            key.SetValue("testValue");

            Assert.That(key.GetValue(), Is.EqualTo("testValue"));
        }

        [Test]
        public void TestResetValue()
        {
            ConfigKey<string> key = new ConfigKey<string>("keyName", "defaultString");

            Assert.That(key.GetValue(), Is.EqualTo("defaultString"));

            key.SetValue("testValue");

            Assert.That(key.GetValue(), Is.EqualTo("testValue"));

            key.Reset();

            Assert.That(key.GetValue(), Is.EqualTo("defaultString"));
        }

        [Test]
        public void TestDefaultStringValue()
        {
            ConfigKey<string> key = new ConfigKey<string>("keyName");

            Assert.That(key.GetValue(), Is.Null);
        }

        [Test]
        public void TestDefaultIntValue()
        {
            ConfigKey<int> key = new ConfigKey<int>("keyName");

            Assert.That(key.GetValue(), Is.EqualTo(0));
        }
    }
}
