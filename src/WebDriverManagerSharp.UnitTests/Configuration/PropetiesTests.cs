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
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using WebDriverManagerSharp.Configuration;

    [TestFixture]
    public class PropertiesTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void TestLoadComment()
        {
            string data = "#testKey=1234";

            Properties properties = new Properties();

            using (Stream s = getStream(data))
            {
                properties.Load(s);
            }

            string val = properties.GetProperty("testKey");

            Assert.That(val, Is.Null);
        }

        [Test]
        public void TestLoadEmpty()
        {
            string data = "";

            Properties properties = new Properties();

            using (Stream s = getStream(data))
            {
                properties.Load(s);
            }

            string val = properties.GetProperty("testKey");

            Assert.That(val, Is.Null);
        }

        [Test]
        public void TestLoadBlank()
        {
            string data = "\r\n\r\n";

            Properties properties = new Properties();

            using (Stream s = getStream(data))
            {
                properties.Load(s);
            }

            string val = properties.GetProperty("testKey");

            Assert.That(val, Is.Null);
        }

        [Test]
        public void TestLoadKey()
        {
            string data = "testKey=1234";

            Properties properties = new Properties();

            using (Stream s = getStream(data))
            {
                properties.Load(s);
            }

            string val = properties.GetProperty("testKey");

            Assert.That(val, Is.EqualTo("1234"));
        }

        [Test]
        public void TestLoadKeys()
        {
            string data = "testKey=1234\r\ntestKey2=5678";

            Properties properties = new Properties();

            using (Stream s = getStream(data))
            {
                properties.Load(s);
            }

            string val = properties.GetProperty("testKey");

            Assert.That(val, Is.EqualTo("1234"));

            val = properties.GetProperty("testKey2");

            Assert.That(val, Is.EqualTo("5678"));
        }

        [Test]
        public void TestLoadData()
        {
            string data = "# comment in file\r\n\r\ntestKey=1234\r\n\r\ntestKey2=5678\r\n";

            Properties properties = new Properties();

            using (Stream s = getStream(data))
            {
                properties.Load(s);
            }

            string val = properties.GetProperty("testKey");

            Assert.That(val, Is.EqualTo("1234"));

            val = properties.GetProperty("testKey2");

            Assert.That(val, Is.EqualTo("5678"));
        }

        private Stream getStream(string data)
        {
            return new MemoryStream(Encoding.ASCII.GetBytes(data));
        }
    }
}
