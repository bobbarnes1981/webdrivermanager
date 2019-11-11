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

using NUnit.Framework;
using System;
using System.IO;

namespace WebDriverManager.Tests.Test
{
    /**
     * Test for taobao.org mirror.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.6.1
     */
    public class TaobaoTest
    {

        [Test]
        public void testTaobao()
        {
            WebDriverManager.chromedriver().Config().setAvoidAutoVersion(true)
                    .setChromeDriverMirrorUrl(
                            new Uri("http://npm.taobao.org/mirrors/chromedriver/"));
            WebDriverManager.chromedriver().useMirror().forceDownload().setup();

            FileInfo binary = new FileInfo(WebDriverManager.chromedriver().getBinaryPath());
            Assert.True(binary.Exists);
        }

        [Ignore("Flaky test due to cnpmjs.org")]
        [Test]
        public void testOtherMirrorUrl()
        {
            WebDriverManager.chromedriver().Config().setAvoidAutoVersion(true)
                        .setChromeDriverMirrorUrl(
                                new Uri("https://cnpmjs.org/mirrors/chromedriver/"));
            WebDriverManager.chromedriver().useMirror().forceDownload().setup();

            FileInfo binary = new FileInfo(WebDriverManager.chromedriver().getBinaryPath());
            Assert.True(binary.Exists);
        }

        [Test]
        public void testTaobaoException()
        {
            Assert.Throws<WebDriverManagerException>(WebDriverManager.edgedriver().useMirror().setup);
            FileInfo binary = new FileInfo(WebDriverManager.edgedriver().getBinaryPath());
            Assert.True(binary.Exists);
        }
    }
}