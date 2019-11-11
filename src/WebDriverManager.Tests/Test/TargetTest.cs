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
     * Target folder test.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.1
     */
    [TestFixture("2.21",
                        "http://chromedriver.storage.googleapis.com/2.21/chromedriver_linux64.zip",
                        "/chromedriver/linux64/2.21/chromedriver_linux64.zip",
                        DriverManagerType.CHROME)]
    [TestFixture("0.2.2",
                        "https://github.com/operasoftware/operachromiumdriver/releases/download/v0.2.2/operadriver_linux64.zip",
                        "/operadriver/linux64/0.2.2/operadriver_linux64.zip",
                        DriverManagerType.OPERA)]
    [TestFixture("2.1.1",
                        "https://bitbucket.org/ariya/phantomjs/downloads/phantomjs-2.1.1-linux-x86_64.tar.bz2",
                        "/phantomjs/linux-x86_64/2.1.1/phantomjs-2.1.1-linux-x86_64.tar.bz2",
                        DriverManagerType.PHANTOMJS)]
    [TestFixture("2.1.1",
                        "https://bitbucket.org/ariya/phantomjs/downloads/phantomjs-2.1.1-windows.zip",
                        "/phantomjs/windows/2.1.1/phantomjs-2.1.1-windows.zip",
                        DriverManagerType.PHANTOMJS)]
    [TestFixture("2.1.1",
                        "https://bitbucket.org/ariya/phantomjs/downloads/phantomjs-2.1.1-macosx.zip",
                        "/phantomjs/macosx/2.1.1/phantomjs-2.1.1-macosx.zip",
                        DriverManagerType.PHANTOMJS)]
    [TestFixture("8D0D08CF-790D-4586-B726-C6469A9ED49C",
                        "https://download.microsoft.com/download/1/4/1/14156DA0-D40F-460A-B14D-1B264CA081A5/MicrosoftWebDriver.exe",
                        "/MicrosoftWebDriver/8D0D08CF-790D-4586-B726-C6469A9ED49C/MicrosoftWebDriver.exe",
                        DriverManagerType.EDGE)]
    [TestFixture("3.14361",
                        "https://download.microsoft.com/download/1/4/1/14156DA0-D40F-460A-B14D-1B264CA081A5/MicrosoftWebDriver.exe",
                        "/MicrosoftWebDriver/3.14361/MicrosoftWebDriver.exe",
                        DriverManagerType.EDGE)]
    [TestFixture("75.0.137.0",
                        "https://az813057.vo.msecnd.net/webdriver/msedgedriver_x86/msedgedriver.exe",
                        "/msedgedriver/x32/75.0.137.0/msedgedriver.exe", DriverManagerType.EDGE)]
    [TestFixture("0.6.2",
                        "https://github.com/jgraham/wires/releases/download/v0.6.2/wires-0.6.2-OSX.gz",
                        "/wires/osx/0.6.2/wires-0.6.2-OSX.gz", DriverManagerType.FIREFOX)]
    [TestFixture("0.3.0",
                        "https://github.com/jgraham/wires/releases/download/0.3.0/wires-0.3.0-osx.tar.gz",
                        "/wires/osx/0.3.0/wires-0.3.0-osx.tar.gz", DriverManagerType.FIREFOX)]
    [TestFixture("0.6.2",
                        "https://github.com/jgraham/wires/releases/download/v0.6.2/wires-0.6.2-linux64.gz",
                        "/wires/linux64/0.6.2/wires-0.6.2-linux64.gz",
                        DriverManagerType.FIREFOX)]
    [TestFixture("0.8.0",
                        "https://github.com/mozilla/geckodriver/releases/download/v0.8.0/geckodriver-0.8.0-linux64.gz",
                        "/geckodriver/linux64/0.8.0/geckodriver-0.8.0-linux64.gz",
                        DriverManagerType.FIREFOX)]
    public class TargetTest
    {

        private readonly ILogger log = Logger.GetLogger();

        public string version;

        public string url;

        public string target;

        public DriverManagerType driverManagerType;

        public TargetTest(string version, string url, string target, DriverManagerType driverManagerType)
        {
            this.version = version;
            this.url = url;
            this.target = target;
            this.driverManagerType = driverManagerType;
        }

        [Test]
        public void testTarget()
        {
            Downloader downloader = new Downloader(driverManagerType);
            string targetPath = downloader.getTargetPath();

            FileInfo result = downloader.getTarget(version, new Uri(url));
            log.Info("{0}", result);
            log.Info(targetPath + target);

            FileInfo fileReal = new FileInfo(targetPath + target);

            Assert.That(result.FullName, Is.EqualTo(fileReal.FullName));
        }
    }
}