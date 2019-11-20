/*
 * (C) Copyright 2019 Robert barnes
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

namespace WebDriverManagerSharp.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;

    [SetUpFixture]
    public class FixtureSetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // kill all driver processes

            string[] driversToKill = new string[]
            {
                "chromedriver",
                "operadriver",
                "geckodriver",
                "iedriverserver",
            };

            IEnumerable<Process> processes = Process.GetProcesses().Where(p => driversToKill.Contains(p.ProcessName.ToLower()));

            foreach (Process p in processes)
            {
                p.Kill();
                p.WaitForExit();
            }

            // remove drivers in current directory

            string filter = string.Join(".exe|", driversToKill) + ".exe";

            DirectoryInfo currentDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;

            foreach (string driver in driversToKill)
            {
                foreach (FileInfo file in currentDirectory.GetFiles(driver + ".exe"))
                {
                    file.Delete();
                }
            }
        }
    }
}
