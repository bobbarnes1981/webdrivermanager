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

namespace WebDriverManagerSharp.Processes
{
    using Autofac;
    using System;
    using System.IO;
    using WebDriverManagerSharp.Logging;

    /**
     * Command line executor.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public class Shell : IShell
    {
        private readonly ILogger logger;

        public Shell(ILogger logger)
        {
            this.logger = logger;
        }

        public string RunAndWait(params string[] command)
        {
            return RunAndWaitArray(null, command);
        }

        public string RunAndWait(DirectoryInfo folder, params string[] command)
        {
            return RunAndWaitArray(folder, command);
        }

        public string RunAndWaitArray(DirectoryInfo folder, string[] command)
        {
            string commandStr = command.ToStringJava();
            logger.Debug("Running command on the shell: {0}", commandStr);
            string result = RunAndWaitNoLog(folder, command);
            logger.Debug("Result: {0}", result);
            return result;
        }

        public string RunAndWaitNoLog(DirectoryInfo folder, params string[] command)
        {
            string output = string.Empty;
            try
            {
                using (IProcess process = Resolver.Resolve<IProcessBuilder>(new NamedParameter("command", command)).Directory(folder).RedirectOutputStream(true).Start())
                {
                    output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }
            }
            catch (IOException e)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("There was a problem executing command <{0}> on the shell: {1}", string.Join(" ", command), e.Message);
                }
            }

            return output.Trim();
        }

        public string GetVersionFromWmicOutput(string output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            int i = output.IndexOf('=');
            int j = output.IndexOf('.');
            return i != -1 && j != -1 ? output.SubstringJava(i + 1, j) : output;
        }

        public string GetVersionFromPosixOutput(string output, string driverType)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (driverType == null)
            {
                throw new ArgumentNullException(nameof(driverType));
            }

            // Special case: using Chromium as Chrome
            if (output.Contains("Chromium"))
            {
                driverType = "Chromium";
            }

            int i = output.IndexOf(driverType, StringComparison.OrdinalIgnoreCase);
            int j = output.IndexOf('.');
            return i != -1 && j != -1 ? output.SubstringJava(i + driverType.Length, j).Trim() : output;
        }

        public string GetVersionFromPowerShellOutput(string output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            int i = output.IndexOf("Version", StringComparison.OrdinalIgnoreCase);
            int j = output.IndexOf(':', i);
            int k = output.IndexOf('.', j);
            return i != -1 && j != -1 && k != -1 ? output.SubstringJava(j + 1, k).Trim() : output;
        }
    }
}