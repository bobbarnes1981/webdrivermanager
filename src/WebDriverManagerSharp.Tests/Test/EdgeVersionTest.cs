/*
 * (C) Copyright 2015 Boni Garcia (http://bonigarcia.github.io/)
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

using NUnit.Framework;
using OpenQA.Selenium.Edge;
using WebDriverManagerSharp.Tests.Base;

namespace WebDriverManagerSharp.Tests.Test
{
    /**
     * Test asserting Edge driver versions.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.3.0
     */
    public class EdgeVersionTest : VersionTestParent
    {
        public EdgeVersionTest(Architecture architecture)
            : base(architecture)
        {
        }

        [SetUp]
        public void setup()
        {
            browserManager = WebDriverManager.getInstance(typeof(EdgeDriver));
            os = OperatingSystem.WIN;
            specificVersions = new string[] { "1.10240", "2.10586", "3.14393", "4.15063", "5.16299", "6.17134", "75.0.139.20", "76.0.183.0", "77.0.237.0" };
        }
    }
}