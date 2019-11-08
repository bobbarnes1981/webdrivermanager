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

using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebDriverManager
{

    /**
     * HTTP Client.
     *
     * @author Boni Garcia
     * @since 2.1.0
     */
    public class HttpClient : System.IDisposable
    {
        ILogger log = Logger.GetLogger();

        Config config;
        System.Net.Http.HttpClient closeableHttpClient;

        public HttpClient(Config config)
        {
            this.config = config;

            System.Net.Http.HttpClientHandler handler = new System.Net.Http.HttpClientHandler();

            try
            {
                string proxy = config.getProxy();

                IWebProxy proxyHost = createProxy(proxy);
                if (proxyHost != null)
                {
                    handler.Proxy = proxyHost;
                    handler.UseProxy = true;

                    ICredentials credentialsProvider = createBasicCredentialsProvider(proxy, config.getProxyUser(), config.getProxyPass(), proxyHost);
                    if (credentialsProvider != null)
                    {
                        handler.Proxy.Credentials = credentialsProvider;
                    }
                }

                string localRepositoryUser = config.getLocalRepositoryUser().Trim();
                string localRepositoryPassword = config.getLocalRepositoryPassword().Trim();

                if (!string.IsNullOrEmpty(localRepositoryUser) && !string.IsNullOrEmpty(localRepositoryPassword))
                {
                    ICredentials provider = new NetworkCredential(localRepositoryUser, localRepositoryPassword);
                    handler.Credentials = provider;
                }

                // TODO: this handler requires .NET 4.7.1
                handler.ClientCertificateOptions = System.Net.Http.ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) =>
                {
                    return true;
                };
            }
            catch (System.Exception e)
            {
                throw new WebDriverManagerException(e);
            }

            closeableHttpClient = new System.Net.Http.HttpClient(handler);
        }

        public IWebProxy createProxy(string proxy)
        {
            System.Uri url = determineProxyUrl(proxy);
            if (url != null)
            {
                string proxyHost = url.Host;
                int proxyPort = url.Port;

                // TODO: can this be simplified to new System.Net.WebProxy(url);
                return new WebProxy(proxyHost, proxyPort);
            }
            return null;
        }

        public System.Net.Http.HttpResponseMessage executeHttpGet(System.Uri url)
        {
            Task<System.Net.Http.HttpResponseMessage> responseTask = closeableHttpClient.GetAsync(url);
            responseTask.Wait(System.TimeSpan.FromSeconds(config.getTimeout()).Milliseconds);
            System.Net.Http.HttpResponseMessage response = responseTask.Result;
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = "Error HTTP "
                    + response.StatusCode + " executing "
                    + url;
                log.Error(errorMessage);
                throw new WebDriverManagerException(errorMessage);
            }
            return response;
        }

        private System.Uri determineProxyUrl(string proxy)
        {
            string proxyFromEnvCaps = System.Environment.GetEnvironmentVariable("HTTPS_PROXY");
            string proxyFromEnv = string.IsNullOrEmpty(proxyFromEnvCaps) ? System.Environment.GetEnvironmentVariable("https_proxy") : proxyFromEnvCaps;
            string proxyInput = string.IsNullOrEmpty(proxy) ? proxyFromEnv : proxy;
            if (!string.IsNullOrEmpty(proxyInput))
            {
                Regex rx = new Regex("^http[s]?://.*$");
                new System.Uri(rx.IsMatch(proxyInput) ? proxyInput : "http://" + proxyInput);
            }
            return null;
        }

        private ICredentials createBasicCredentialsProvider(string proxy, string proxyUser, string proxyPass, IWebProxy proxyHost)
        {
            System.Uri proxyUrl = determineProxyUrl(proxy);
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
            string envProxyUser = System.Environment.GetEnvironmentVariable("HTTPS_PROXY_USER");
            string envProxyPass = System.Environment.GetEnvironmentVariable("HTTPS_PROXY_PASS");
            username = (envProxyUser != null) ? envProxyUser : username;
            password = (envProxyPass != null) ? envProxyPass : password;

            // apply option value
            username = (proxyUser != null) ? proxyUser : username;
            password = (proxyPass != null) ? proxyPass : password;

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
            closeableHttpClient.Dispose();
        }
    }
}