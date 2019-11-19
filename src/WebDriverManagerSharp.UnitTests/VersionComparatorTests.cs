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

    [TestFixture]
    public class VersionComparatorTests
    {
        [Test]
        public void TestVersion2Null()
        {
            string version1 = "1.2.3.4";

            string version2 = null;

            Assert.Throws<ArgumentNullException>(() => new VersionComparator().Compare(version1, version2));
        }

        [Test]
        public void TestVersion1Null()
        {
            string version1 = null;

            string version2 = "1.2.3.4";

            Assert.Throws<ArgumentNullException>(() => new VersionComparator().Compare(version1, version2));
        }

        [Test]
        public void TestVersion1Invalid()
        {
            string version1 = "1.2.3.A";

            string version2 = "1.2.3.4";

            int result = new VersionComparator().Compare(version1, version2);

            // TODO: is this right? Shouldn't an invalid version throw an exception?
            Assert.That(result, Is.EqualTo(0));
        }

        [TestCase("1.2.3.4", "1.2.3.5", -1)]
        [TestCase("1.2.3.4", "1.2.4.4", -1)]
        [TestCase("1.2.3.4", "1.3.3.4", -1)]
        [TestCase("1.2.3.4", "2.2.3.4", -1)]
        [TestCase("1.2.3.4", "1.2.3.4", 0)]
        [TestCase("1.2.3.5", "1.2.3.4", 1)]
        [TestCase("1.2.4.4", "1.2.3.4", 1)]
        [TestCase("1.3.3.4", "1.2.3.4", 1)]
        [TestCase("2.2.3.4", "1.2.3.4", 1)]
        public void TestVersionComparison(string version1, string version2, int outcome)
        {
            int result = new VersionComparator().Compare(version1, version2);

            Assert.That(result, Is.EqualTo(outcome));
        }
    }
}
