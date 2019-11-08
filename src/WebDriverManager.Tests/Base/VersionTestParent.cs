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

namespace WebDriverManager.Tests.Base
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
        protected OperatingSystem os;

        ILogger log = Logger.GetLogger();

        public VersionTestParent(Architecture architecture)
        {
            this.architecture = architecture;
        }

        [Test]
        public void testLatestVersion()
        {
            if (os != null)
            {
                browserManager.operatingSystem(os);
            }
            switch (architecture)
            {
                case Architecture.X32:
                    browserManager.arch32().setup();
                    break;
                case Architecture.X64:
                    browserManager.arch64().setup();
                    break;
                default:
                    browserManager.setup();
                    break;
            }

            Assert.That(browserManager.getDownloadedVersion(), Is.Not.Null);
        }

        [Test]
        public void testSpecificVersions()
        {
            foreach (string specificVersion in specificVersions)
            {
                log.Info("Test specific version arch={0} version={1}", architecture, specificVersion);
                if (architecture != Architecture.DEFAULT)
                {
                    browserManager.architecture(architecture);
                }
                if (os != null)
                {
                    browserManager.operatingSystem(os);
                }
                browserManager.version(specificVersion).setup();

                Assert.That(browserManager.getDownloadedVersion(), Is.EqualTo(specificVersion));
            }
        }

    }
}