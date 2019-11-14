/*
 * (C) Copyright 2016 Boni Garcia (http://bonigarcia.github.io/)
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
    using System;
    using System.IO;
    using NUnit.Framework;
    using WebDriverManagerSharp.Exceptions;

    /**
     * Test for taobao.org mirror.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.6.1
     */
    public class TaobaoTest
    {
        [Test]
        public void TestTaobao()
        {
            WebDriverManager.ChromeDriver().Config().SetAvoidAutoVersion(true)
                    .SetChromeDriverMirrorUrl(
                            new Uri("http://npm.taobao.org/mirrors/chromedriver/"));
            WebDriverManager.ChromeDriver().UseMirror().ForceDownload().Setup();

            FileInfo binary = new FileInfo(WebDriverManager.ChromeDriver().GetBinaryPath());
            Assert.True(binary.Exists);
        }

        [Ignore("Flaky test due to cnpmjs.org")]
        [Test]
        public void TestOtherMirrorUrl()
        {
            WebDriverManager.ChromeDriver().Config().SetAvoidAutoVersion(true)
                        .SetChromeDriverMirrorUrl(
                                new Uri("https://cnpmjs.org/mirrors/chromedriver/"));
            WebDriverManager.ChromeDriver().UseMirror().ForceDownload().Setup();

            FileInfo binary = new FileInfo(WebDriverManager.ChromeDriver().GetBinaryPath());
            Assert.True(binary.Exists);
        }

        [Test]
        public void TestTaobaoException()
        {
            Assert.Throws<WebDriverManagerException>(() => WebDriverManager.EdgeDriver().UseMirror());
        }
    }
}