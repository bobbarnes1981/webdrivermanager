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

namespace WebDriverManagerSharp.Web
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public class HttpClient : IHttpClient
    {
        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        private readonly System.Net.Http.HttpClient httpClient;

        private bool disposed = false;

        public HttpClient()
        {
            httpClient = new System.Net.Http.HttpClient();
        }

        public System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders
        {
            get
            {
                return httpClient.DefaultRequestHeaders;
            }
        }

        public Task<System.Net.Http.HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return httpClient.GetAsync(requestUri);
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
                httpClient.Dispose();
            }

            disposed = true;
        }
    }
}
