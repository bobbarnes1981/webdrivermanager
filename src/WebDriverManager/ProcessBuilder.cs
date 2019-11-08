using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebDriverManager
{
    class ProcessBuilder
    {
        private string[] command;

        private DirectoryInfo directory;

        private bool redirectOutputStream;

        public ProcessBuilder(params string[] command)
        {
            this.command = command;
        }

        public ProcessBuilder Directory(DirectoryInfo directory)
        {
            this.directory = directory;

            return this;
        }

        public ProcessBuilder RedirectOutputStream(bool redirectOutputStream)
        {
            this.redirectOutputStream = redirectOutputStream;

            return this;
        }

        public Process Start()
        {
            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(this.directory.FullName, this.command[0]);
            process.StartInfo.Arguments = string.Join(" ", this.command.Skip(1));

            process.StartInfo.UseShellExecute = false;

            if (redirectOutputStream)
            {
                process.StartInfo.RedirectStandardOutput = true;
            }

            process.Start();
            return process;
        }
    }
}
