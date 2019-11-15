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
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using OpenQA.Selenium.Opera;
    using WebDriverManagerSharp.Tests.Base;

    /**
     * Test with Opera browser.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class OperaTest : BrowserTestParent
    {
        [OneTimeSetUp]
        public void SetupClass()
        {
            WebDriverManager.OperaDriver().Setup();
        }

        [SetUp]
        public void SetupTest()
        {
            string operaBinary = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? getWindowsOperaExecutable()
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? getOsxOperaExecutable()
                            : getLinuxOperaExecutable();

            OperaOptions options = new OperaOptions()
            {
                BinaryLocation = operaBinary
            };

            FileInfo opera = new FileInfo(operaBinary);
            Assume.That(opera.Exists);

            Driver = new OperaDriver(options);
        }

        private string getWindowsOperaExecutable()
        {
            DirectoryInfo installPath = new DirectoryInfo(Path.Combine(System.Environment.ExpandEnvironmentVariables("%LocalAppData%"), @"Programs\Opera\"));

            foreach (DirectoryInfo dir in installPath.GetDirectories("*.*.*.*"))
            {
                // return first directory
                return dir.GetFiles("opera.exe")[0].FullName;
            }

            throw new System.Exception("Opera not found");
        }

        private string getOsxOperaExecutable()
        {
            return "/Applications/Opera.app/Contents/MacOS/Opera";
        }

        private string getLinuxOperaExecutable()
        {
            return "/usr/bin/opera";
        }
    }
}