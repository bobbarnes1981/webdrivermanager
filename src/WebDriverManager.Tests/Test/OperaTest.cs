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
using OpenQA.Selenium.Opera;
using System.IO;
using System.Runtime.InteropServices;
using WebDriverManager.Tests.Base;

namespace WebDriverManager.Tests.Test
{
    /**
     * Test with Opera browser.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class OperaTest : BrowserTestParent
    {

        [OneTimeSetUp]
        public static void setupClass()
        {
            WebDriverManager.operadriver().setup();
        }

        [SetUp]
        public void setupTest()
        {
            string operaBinary = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "C:\\Program Files\\Opera\\launcher.exe"
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "/Applications/Opera.app/Contents/MacOS/Opera"
                            : "/usr/bin/opera";
            FileInfo opera = new FileInfo(operaBinary);
            Assume.That(opera.Exists);

            driver = new OperaDriver();
        }
    }
}