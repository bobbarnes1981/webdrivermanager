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

namespace WebDriverManagerSharp.Processes
{
    using System.IO;
    using System.Linq;

    public class ProcessBuilder
    {
        private readonly string[] command;

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

        public IProcess Start()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Path.Combine(this.directory.FullName, this.command[0]);
            process.StartInfo.Arguments = string.Join(" ", this.command.Skip(1));

            process.StartInfo.UseShellExecute = false;

            if (redirectOutputStream)
            {
                process.StartInfo.RedirectStandardOutput = true;
            }

            process.Start();
            return new Process(process);
        }
    }
}
