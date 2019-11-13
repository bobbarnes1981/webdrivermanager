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

namespace WebDriverManagerSharp.Tests.Test
{
    using System.IO;
    using NUnit.Framework;

    /**
     * Test with Microsoft Edge forcing to extract MSI file.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.0
     */
    public class EdgeMsiTest
    {
        private readonly ILogger log = Logger.GetLogger();

        [Test]
        public void testMsiInWindows()
        {
            WebDriverManager.EdgeDriver().Version("2.10586").Setup();
            FileInfo binary = new FileInfo(WebDriverManager.EdgeDriver().GetBinaryPath());
            log.Debug("Edge driver {0}", binary);
            Assert.That(binary.Extension, Is.EqualTo(".exe"));
        }
    }
}