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

using NUnit.Framework;
using System.IO;

namespace WebDriverManager.Tests.Test
{
    /**
     * Using different properties.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.1
     */
    public class PropertiesTest
    {
        [Test]
        public void testCustomProperties()
        {
            WebDriverManager chromedriver = WebDriverManager.chromedriver();
            chromedriver.Config().setProperties("wdm-test.properties");
            chromedriver.setup();
            string binaryPath = chromedriver.getBinaryPath();
            FileInfo binary = new FileInfo(binaryPath);
            Assert.True(binary.Exists);
        }

        [Test]
        public void testEmptyProperties()
        {
            WebDriverManager.chromedriver().properties("").setup();
        }
    }
}