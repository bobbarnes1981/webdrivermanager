/*
 * (C) Copyright 2015 Boni Garcia (http://bonigarcia.github.io/)
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
    using System.Net;
    using System.Net.Http.Headers;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Win32.SafeHandles;

    /**
     * HTTP Client.
     *
     * @author Boni Garcia
     * @since 2.1.0
     */
    public class HttpClient : IDisposable
    {
        private readonly ILogger log = Logger.GetLogger();

        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        private readonly Config config;
        private readonly System.Net.Http.HttpClient closeableHttpClient;

        private bool disposed = false;

        public HttpClient(Config config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.config = config;

            System.Net.Http.HttpClientHandler handler = new System.Net.Http.HttpClientHandler();

            try
            {
                string proxy = config.GetProxy();

                IWebProxy proxyHost = CreateProxy(proxy);
                if (proxyHost != null)
                {
                    handler.Proxy = proxyHost;
                    handler.UseProxy = true;

                    ICredentials credentialsProvider = createBasicCredentialsProvider(proxy, config.GetProxyUser(), config.GetProxyPass());
                    if (credentialsProvider != null)
                    {
                        handler.Proxy.Credentials = credentialsProvider;
                    }
                }

                string localRepositoryUser = config.GetLocalRepositoryUser().Trim();
                string localRepositoryPassword = config.GetLocalRepositoryPassword().Trim();

                if (!string.IsNullOrEmpty(localRepositoryUser) && !string.IsNullOrEmpty(localRepositoryPassword))
                {
                    ICredentials provider = new NetworkCredential(localRepositoryUser, localRepositoryPassword);
                    handler.Credentials = provider;
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // TODO: this handler requires .NET 4.7.1
                ////handler.ClientCertificateOptions = System.Net.Http.ClientCertificateOption.Manual;
                ////handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) =>
                ////{
                ////    return true;
                ////};
            }
            catch (Exception e)
            {
                throw new WebDriverManagerException(e);
            }

            closeableHttpClient = new System.Net.Http.HttpClient(handler);
            closeableHttpClient.DefaultRequestHeaders.Add("User-Agent", "WebDriverManager-Sharp");
        }

        public static IWebProxy CreateProxy(string proxy)
        {
            Uri url = determineProxyUrl(proxy);
            if (url != null)
            {
                string proxyHost = url.Host;
                int proxyPort = url.Port;

                // TODO: can this be simplified to new System.Net.WebProxy(url);
                return new WebProxy(proxyHost, proxyPort);
            }

            return null;
        }

        public System.Net.Http.HttpResponseMessage ExecuteHttpGet(Uri url, AuthenticationHeaderValue authHeader = null)
        {
            if (authHeader != null)
            {
                closeableHttpClient.DefaultRequestHeaders.Authorization = authHeader;
            }

            Task<System.Net.Http.HttpResponseMessage> responseTask = closeableHttpClient.GetAsync(url);
            responseTask.Wait(System.TimeSpan.FromSeconds(config.GetTimeout()).Milliseconds);
            System.Net.Http.HttpResponseMessage response = responseTask.Result;
            if (!response.IsSuccessStatusCode)
            {
                string responseString = response.Content.ReadAsStringAsync().Result;

                string errorMessage = "Error HTTP " + response.StatusCode + " executing " + url + " [" + responseString + "]";
                log.Error(errorMessage);
                throw new WebDriverManagerException(errorMessage);
            }

            return response;
        }

        private static Uri determineProxyUrl(string proxy)
        {
            string proxyFromEnvCaps = Environment.GetEnvironmentVariable("HTTPS_PROXY");
            string proxyFromEnv = string.IsNullOrEmpty(proxyFromEnvCaps) ? System.Environment.GetEnvironmentVariable("https_proxy") : proxyFromEnvCaps;
            string proxyInput = string.IsNullOrEmpty(proxy) ? proxyFromEnv : proxy;
            if (!string.IsNullOrEmpty(proxyInput))
            {
                Regex rx = new Regex("^http[s]?://.*$");
                return new Uri(rx.IsMatch(proxyInput) ? proxyInput : "http://" + proxyInput);
            }

            return null;
        }

        private ICredentials createBasicCredentialsProvider(string proxy, string proxyUser, string proxyPass)
        {
            Uri proxyUrl = determineProxyUrl(proxy);
            if (proxyUrl == null)
            {
                return null;
            }

            string username = null;
            string password = null;

            // apply env value
            string userInfo = proxyUrl.UserInfo;
            if (userInfo != null)
            {
                string[] st = userInfo.Split(':');
                username = st[0];
                password = st[1];
            }

            string envProxyUser = Environment.GetEnvironmentVariable("HTTPS_PROXY_USER");
            string envProxyPass = Environment.GetEnvironmentVariable("HTTPS_PROXY_PASS");
            username = envProxyUser ?? username;
            password = envProxyPass ?? password;

            // apply option value
            username = proxyUser ?? username;
            password = proxyPass ?? password;

            if (username == null)
            {
                return null;
            }

            string ntlmUsername = username;
            string ntlmDomain = null;

            int index = username.IndexOf('\\');
            if (index > 0)
            {
                ntlmDomain = username.SubstringJava(0, index);
                ntlmUsername = username.SubstringJava(index + 1);
            }

            NetworkCredential credentialsProvider = new NetworkCredential(ntlmUsername, password, ntlmDomain);

            return credentialsProvider;
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
                closeableHttpClient.Dispose();
            }

            disposed = true;
        }
    }
}