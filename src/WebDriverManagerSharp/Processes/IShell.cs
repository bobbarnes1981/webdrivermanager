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
    using WebDriverManagerSharp.Storage;

    public interface IShell
    {
        string RunAndWait(params string[] command);
        string RunAndWait(IDirectory folder, params string[] command);

        string GetVersionFromWmicOutput(string output);
        string GetVersionFromPosixOutput(string output, string driverType);
        string GetVersionFromPowerShellOutput(string output);
    }
}
