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

namespace WebDriverManagerSharp.Tests.Base
{

    /**
     * Parent class for version based tests.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.4.1
     */
    [TestFixture(Architecture.DEFAULT)]
    [TestFixture(Architecture.X32)]
    [TestFixture(Architecture.X64)]
    public abstract class VersionTestParent
    {
        public Architecture architecture;

        protected WebDriverManager browserManager;
        protected string[] specificVersions;
        protected OperatingSystem? os;

        private readonly ILogger log = Logger.GetLogger();

        protected VersionTestParent(Architecture architecture)
        {
            this.architecture = architecture;
        }

        [Test]
        public void testLatestVersion()
        {
            if (os != null)
            {
                browserManager.OperatingSystem(os.Value);
            }
            switch (architecture)
            {
                case Architecture.X32:
                    browserManager.Arch32().Setup();
                    break;
                case Architecture.X64:
                    browserManager.Arch64().Setup();
                    break;
                default:
                    browserManager.Setup();
                    break;
            }

            Assert.That(browserManager.GetDownloadedVersion(), Is.Not.Null);
        }

        [Test]
        public void testSpecificVersions()
        {
            foreach (string specificVersion in specificVersions)
            {
                log.Info("Test specific version arch={0} version={1}", architecture, specificVersion);
                if (architecture != Architecture.DEFAULT)
                {
                    browserManager.Architecture(architecture);
                }
                if (os != null)
                {
                    browserManager.OperatingSystem(os.Value);
                }
                browserManager.Version(specificVersion).Setup();

                Assert.That(browserManager.GetDownloadedVersion(), Is.EqualTo(specificVersion));
            }
        }

    }
}