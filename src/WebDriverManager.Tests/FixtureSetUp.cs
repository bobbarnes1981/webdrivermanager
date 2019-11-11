using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WebDriverManager.Tests
{
    [SetUpFixture]
    class FixtureSetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // kill all driver processes

            string[] driversToKill = new string[]
            {
                "chromedriver",
                "operadriver"
            };

            IEnumerable<Process> processes = Process.GetProcesses().Where(p => driversToKill.Contains(p.ProcessName));

            foreach (Process p in processes)
            {
                p.Kill();
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
