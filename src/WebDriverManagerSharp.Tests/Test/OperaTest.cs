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
using WebDriverManagerSharp.Tests.Base;

namespace WebDriverManagerSharp.Tests.Test
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
            WebDriverManager.OperaDriver().Setup();
        }

        [SetUp]
        public void setupTest()
        {
            string operaBinary = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? Path.Combine(System.Environment.ExpandEnvironmentVariables("%LocalAppData%"), @"Programs\Opera\64.0.3417.92\opera.exe")
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "/Applications/Opera.app/Contents/MacOS/Opera"
                            : "/usr/bin/opera";


            OperaOptions options = new OperaOptions()
            {
                BinaryLocation = operaBinary
            };

            FileInfo opera = new FileInfo(operaBinary);
            Assume.That(opera.Exists);

            driver = new OperaDriver(options);
        }
    }
}