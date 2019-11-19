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

namespace WebDriverManagerSharp
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using WebDriverManagerSharp.Logging;

    /**
     * Command line executor.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.0.0
     */
    public static class Shell
    {
        private static readonly ILogger log = Logger.GetLogger();

        public static string runAndWait(params string[] command)
        {
            return runAndWaitArray(new DirectoryInfo(Directory.GetCurrentDirectory()), command);
        }

        public static string runAndWait(DirectoryInfo folder, params string[] command)
        {
            return runAndWaitArray(folder, command);
        }

        public static string runAndWaitArray(DirectoryInfo folder, string[] command)
        {
            string commandStr = command.ToStringJava();
            log.Debug("Running command on the shell: {0}", commandStr);
            string result = runAndWaitNoLog(folder, command);
            log.Debug("Result: {0}", result);
            return result;
        }

        public static string runAndWaitNoLog(DirectoryInfo folder, params string[] command)
        {
            string output = string.Empty;
            try
            {
                using (Process process = new ProcessBuilder(command).Directory(folder).RedirectOutputStream(true).Start())
                {
                    output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }
            }
            catch (IOException e)
            {
                if (log.IsDebugEnabled())
                {
                    log.Debug("There was a problem executing command <{0}> on the shell: {1}", string.Join(" ", command), e.Message);
                }
            }

            return output.Trim();
        }

        public static string GetVersionFromWmicOutput(string output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            int i = output.IndexOf('=');
            int j = output.IndexOf('.');
            return i != -1 && j != -1 ? output.SubstringJava(i + 1, j) : output;
        }

        public static string GetVersionFromPosixOutput(string output, string driverType)
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

        public static string GetVersionFromPowerShellOutput(string output)
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