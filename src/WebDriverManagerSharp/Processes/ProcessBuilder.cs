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
    using WebDriverManagerSharp.Storage;

    public class ProcessBuilder : IProcessBuilder
    {
        private readonly string[] command;

        private IDirectory directory;

        private bool redirectOutputStream;

        public ProcessBuilder(params string[] command)
        {
            this.command = command;
        }

        public IProcessBuilder Directory(IDirectory directory)
        {
            this.directory = directory;

            return this;
        }

        public IProcessBuilder RedirectOutputStream(bool redirectOutputStream)
        {
            this.redirectOutputStream = redirectOutputStream;

            return this;
        }

        public IProcess Start()
        {
            IProcess process = Resolver.Resolve<IProcess>();
            process.StartInfo.FileName = Path.Combine(this.directory != null ? this.directory.FullName : "", this.command[0]);
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
