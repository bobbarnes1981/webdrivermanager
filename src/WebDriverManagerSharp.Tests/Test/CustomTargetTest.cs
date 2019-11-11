/*
 * (C) Copyright 2017 Boni Garcia (http://bonigarcia.github.io/)
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

namespace WebDriverManagerSharp.Tests.Test
{ 
    /**
     * Test for custom target.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.7.2
     */
    public class CustomTargetTest
    {
        private readonly ILogger log = Logger.GetLogger();

        DirectoryInfo tmpFolder;

        [SetUp]
        public void setup()
        {
            tmpFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            WebDriverManager.globalConfig().setTargetPath(tmpFolder.ToString());
            log.Info("Using temporal folder {0} as cache", tmpFolder);
        }

        [Test]
        public void testTargetPath()
        {
            WebDriverManager.chromedriver().setup();
            string binaryPath = WebDriverManager.chromedriver().getBinaryPath();
            log.Info("Binary path {0}", binaryPath);
            StringAssert.StartsWith(tmpFolder.ToString(), binaryPath);
        }

        [TearDown]
        public void teardown()
        {
            log.Info("Deleting temporal folder {0}", tmpFolder);
            WebDriverManager.chromedriver().clearCache();
            WebDriverManager.globalConfig().reset();
        }
    }
}