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
using System.Reflection;
using System.Runtime.InteropServices;

namespace WebDriverManagerSharp.Tests.Test
{

    /**
     * Test using wdm in interactive mode (from the shell).
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.2
     */
    [TestFixture("chrome", "chromedriver")]
    [TestFixture("firefox", "geckodriver")]
    [TestFixture("opera", "operadriver")]
    [TestFixture("phantomjs", "phantomjs")]
    [TestFixture("edge", "msedgedriver")]
    [TestFixture("iexplorer", "IEDriverServer")]
    public class InteractiveTest
    {
        public static ILogger log = Logger.GetLogger();

        public static string EXT = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";

        public string argument;

        public string driver;

        public InteractiveTest(string argument, string driver)
        {
            this.argument = argument;
            this.driver = driver + EXT;
        }

        [Test]
        public void testInteractive()
        {
            log.Debug("Running interactive wdm with arguments: {0}", argument);
            WebDriverManager.main(new string[] { argument });
            FileInfo binary = new FileInfo(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, driver));
            Assert.True(binary.Exists);
            binary.Delete();
            log.Debug("Interactive test with {0} OK", argument);
        }
    }
}