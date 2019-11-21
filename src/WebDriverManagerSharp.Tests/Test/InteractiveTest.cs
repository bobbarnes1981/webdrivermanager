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
    using System.Reflection;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using WebDriverManagerSharp.Logging;

    /**
     * Test using wdm in interactive mode (from the shell).
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.2
     */
    public class InteractiveTest
    {
        private static readonly ILogger log = Resolver.Resolve<ILogger>();

        private static readonly string EXT = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : string.Empty;

        [TestCase("chrome", "chromedriver")]
        [TestCase("firefox", "geckodriver")]
        [TestCase("opera", "operadriver")]
        [TestCase("phantomjs", "phantomjs")]
        [TestCase("edge", "msedgedriver")]
        [TestCase("iexplorer", "IEDriverServer")]
        public void TestInteractive(string argument, string driver)
        {
            driver += EXT;

            log.Debug("Running interactive wdm with arguments: {0}", argument);
            CommandLineInterface.Main(new string[] { argument });
            FileInfo binary = new FileInfo(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, driver));
            Assert.True(binary.Exists);
            binary.Delete();
            log.Debug("Interactive test with {0} OK", argument);
        }
    }
}