/*
 * (C) Copyright 2018 Boni Garcia (http://bonigarcia.github.io/)
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

namespace WebDriverManagerSharp.Tests.Test
{
    using System.IO;
    using NUnit.Framework;

    /**
     * Test for preferences.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public class PreferencesTest
    {
        [Test]
        public void testEmptyTtl()
        {
            WebDriverManager.main(new string[] { "clear-preferences" });
            WebDriverManager.ChromeDriver().Ttl(0).Setup();
            string binaryPath = WebDriverManager.ChromeDriver().GetBinaryPath();
            FileInfo binary = new FileInfo(binaryPath);
            Assert.True(binary.Exists);
        }
    }
}