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

namespace WebDriverManagerSharp.UnitTests
{
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Text;

    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void GetFileTestNull()
        {
            Uri uri = null;

            Assert.Throws<ArgumentNullException>(() => uri.GetFile());
        }

        [Test]
        public void GetFileTest()
        {
            Uri uri = new Uri("http://www.website.com/path/subpath/file.html");

            string file = uri.GetFile();

            Assert.That(file, Is.EqualTo("/path/subpath/file.html"));
        }

        [Test]
        public void SubstringJavaTestNull()
        {
            string str = null;

            Assert.Throws<ArgumentNullException>(() => str.SubstringJava(2));
        }

        [Test]
        public void SubstringJavaTestIndex()
        {
            string str = "my test string";

            string result = str.SubstringJava(2);

            Assert.That(result, Is.EqualTo(" test string"));
        }

        [Test]
        public void SubstringJavaEndIndexNull()
        {
            string str = null;

            Assert.Throws<ArgumentNullException>(() => str.SubstringJava(2, 5));
        }

        [Test]
        public void SubstringJavaEndIndexEndIndexGreater()
        {
            string str = "my test string";

            Assert.Throws<ArgumentOutOfRangeException>(() => str.SubstringJava(2, 15));
        }

        [Test]
        public void SubstringJavaEndIndexStartIndexLessThan()
        {
            string str = "my test string";

            Assert.Throws<ArgumentOutOfRangeException>(() => str.SubstringJava(-1, 13));
        }

        [Test]
        public void SubstringJavaEndIndexStartIndexGreater()
        {
            string str = "my test string";

            Assert.Throws<ArgumentOutOfRangeException>(() => str.SubstringJava(9, 5));
        }

        [Test]
        public void SubstringJavaEndIndex()
        {
            string str = "my test string";

            string result = str.SubstringJava(5, 10);

            Assert.That(result, Is.EqualTo("st st"));
        }

        [Test]
        public void CopyOfTestNull()
        {
            string[] array = null;

            Assert.Throws<ArgumentNullException>(() => array.CopyOf(1));
        }

        [Test]
        public void CopyOfTest()
        {
            string[] array = new string[] { "a" };

            string[] result = array.CopyOf(2);

            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("a"));
            Assert.That(result[1], Is.Null);
        }

        [Test]
        public void UriAppendTestUriNull()
        {
            Uri uri = null;

            Assert.Throws<ArgumentNullException>(() => uri.Append(""));
        }

        [Test]
        public void UriAppendTestPartNull()
        {
            Uri uri = new Uri("http://www.website.com");

            Assert.Throws<ArgumentNullException>(() => uri.Append(null));
        }

        [TestCase("http://www.website.com", "path/test.html", "http://www.website.com/path/test.html")]
        [TestCase("http://www.website.com/", "path/test.html", "http://www.website.com/path/test.html")]
        [TestCase("http://www.website.com", "/path/test.html", "http://www.website.com/path/test.html")]
        [TestCase("http://www.website.com/", "/path/test.html", "http://www.website.com/path/test.html")]
        [TestCase("http://www.website.com/parent", "path/test.html", "http://www.website.com/parent/path/test.html")]
        [TestCase("http://www.website.com/parent/", "path/test.html", "http://www.website.com/parent/path/test.html")]
        [TestCase("http://www.website.com/parent", "/path/test.html", "http://www.website.com/parent/path/test.html")]
        [TestCase("http://www.website.com/parent/", "/path/test.html", "http://www.website.com/parent/path/test.html")]
        public void UriAppendTest(string uriString, string uriPartString, string expectedUri)
        {
            Uri uri = new Uri(uriString);

            Uri result = uri.Append(uriPartString);

            Assert.That(result.ToString(), Is.EqualTo(expectedUri));
        }
    }
}
