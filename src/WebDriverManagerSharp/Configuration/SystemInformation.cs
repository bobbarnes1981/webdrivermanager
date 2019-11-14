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

namespace WebDriverManagerSharp.Configuration
{
    using System.Globalization;
    using WebDriverManagerSharp.Enums;

    public class SystemInformation : ISystemInformation
    {
        public Architecture Architecture
        {
            get
            {
                // RuntimeInformation.OSArchitecture ?
                System.Runtime.InteropServices.Architecture arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
                if (arch == System.Runtime.InteropServices.Architecture.X86)
                {
                    return Architecture.X32;
                }

                if (arch == System.Runtime.InteropServices.Architecture.X64)
                {
                    return Architecture.X64;
                }

                throw new System.Exception(string.Format(CultureInfo.InvariantCulture, "Unhandled architecture {0}", arch));
            }
        }

        public OperatingSystem OperatingSystem
        {
            get
            {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    return OperatingSystem.WIN;
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                {
                    return OperatingSystem.LINUX;
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                {
                    return OperatingSystem.MAC;
                }

                throw new System.Exception("Could not determine operating system");
            }
        }
    }
}
