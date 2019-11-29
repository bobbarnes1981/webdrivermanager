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

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace WebDriverManagerSharp.Processes
{
    class Process : IProcess
    {
        private readonly System.Diagnostics.Process process;

        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        private bool disposed = false;

        public Process()
        {
            this.process = new System.Diagnostics.Process();
        }

        public ProcessStartInfo StartInfo
        {
            get
            {
                return process.StartInfo;
            }
        }

        public StreamReader StandardOutput
        {
            get
            {
                return process.StandardOutput;
            }
        }

        public bool Start()
        {
            return process.Start();
        }

        public void WaitForExit()
        {
            process.WaitForExit();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                handle.Dispose();
                process.Dispose();
            }

            disposed = true;
        }
    }
}
