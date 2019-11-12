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

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WebDriverManagerSharp
{
    /**
     * Configuration class.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.2.0
     */
    public class Config
    {
        private readonly ILogger log = Logger.GetLogger();

        private const string HOME = "~";

        private readonly ConfigKey<string> properties = new ConfigKey<string>("wdm.properties", "webdrivermanager.properties");

        private readonly ConfigKey<string> targetPath = new ConfigKey<string>("wdm.targetPath");
        private readonly ConfigKey<bool> forceCache = new ConfigKey<bool>("wdm.forceCache");
        private readonly ConfigKey<bool> over_ride = new ConfigKey<bool>("wdm.override");
        private readonly ConfigKey<bool> useMirror = new ConfigKey<bool>("wdm.useMirror");
        private readonly ConfigKey<bool> useBetaVersions = new ConfigKey<bool>("wdm.useBetaVersions");
        private readonly ConfigKey<bool> avoidExport = new ConfigKey<bool>("wdm.avoidExport");
        private readonly ConfigKey<bool> avoidOutputTree = new ConfigKey<bool>("wdm.avoidOutputTree");
        private readonly ConfigKey<bool> avoidAutoVersion = new ConfigKey<bool>("wdm.avoidAutoVersion");
        private readonly ConfigKey<bool> avoidAutoReset = new ConfigKey<bool>("wdm.avoidAutoReset");
        private readonly ConfigKey<bool> avoidPreferences = new ConfigKey<bool>("wdm.avoidPreferences");
        private readonly ConfigKey<int> timeout = new ConfigKey<int>("wdm.timeout");
        private readonly ConfigKey<bool> versionsPropertiesOnlineFirst = new ConfigKey<bool>("wdm.versionsPropertiesOnlineFirst");
        private readonly ConfigKey<Uri> versionsPropertiesUrl = new ConfigKey<Uri>("wdm.versionsPropertiesUrl");

        private readonly ConfigKey<string> architecture = new ConfigKey<string>("wdm.architecture", defaultArchitecture());
        private readonly ConfigKey<string> os = new ConfigKey<string>("wdm.os", defaultOsName());
        private readonly ConfigKey<string> proxy = new ConfigKey<string>("wdm.proxy");
        private readonly ConfigKey<string> proxyUser = new ConfigKey<string>("wdm.proxyUser");
        private readonly ConfigKey<string> proxyPass = new ConfigKey<string>("wdm.proxyPass");
        private readonly ConfigKey<string> ignoreVersions = new ConfigKey<string>("wdm.ignoreVersions");
        private readonly ConfigKey<string> gitHubTokenName = new ConfigKey<string>("wdm.gitHubTokenName");
        private readonly ConfigKey<string> gitHubTokenSecret = new ConfigKey<string>("wdm.gitHubTokenSecret");
        private readonly ConfigKey<string> localRepositoryUser = new ConfigKey<string>("wdm.localRepositoryUser");
        private readonly ConfigKey<string> localRepositoryPassword = new ConfigKey<string>("wdm.localRepositoryPassword");

        private readonly ConfigKey<string> chromeDriverVersion = new ConfigKey<string>("wdm.chromeDriverVersion");
        private readonly ConfigKey<string> chromeDriverExport = new ConfigKey<string>("wdm.chromeDriverExport");
        private readonly ConfigKey<Uri> chromeDriverUrl = new ConfigKey<Uri>("wdm.chromeDriverUrl");
        private readonly ConfigKey<Uri> chromeDriverMirrorUrl = new ConfigKey<Uri>("wdm.chromeDriverMirrorUrl");

        private readonly ConfigKey<string> edgeDriverVersion = new ConfigKey<string>("wdm.edgeDriverVersion");
        private readonly ConfigKey<string> edgeDriverExport = new ConfigKey<string>("wdm.edgeDriverExport");
        private readonly ConfigKey<Uri> edgeDriverUrl = new ConfigKey<Uri>("wdm.edgeDriverUrl");

        private readonly ConfigKey<string> firefoxDriverVersion = new ConfigKey<string>("wdm.geckoDriverVersion");
        private readonly ConfigKey<string> firefoxDriverExport = new ConfigKey<string>("wdm.geckoDriverExport");
        private readonly ConfigKey<Uri> firefoxDriverUrl = new ConfigKey<Uri>("wdm.geckoDriverUrl");
        private readonly ConfigKey<Uri> firefoxDriverMirrorUrl = new ConfigKey<Uri>("wdm.geckoDriverMirrorUrl");

        private readonly ConfigKey<string> internetExplorerDriverVersion = new ConfigKey<string>("wdm.internetExplorerDriverVersion");
        private readonly ConfigKey<string> internetExplorerDriverExport = new ConfigKey<string>("wdm.internetExplorerDriverExport");
        private readonly ConfigKey<Uri> internetExplorerDriverUrl = new ConfigKey<Uri>("wdm.internetExplorerDriverUrl");

        private readonly ConfigKey<string> operaDriverVersion = new ConfigKey<string>("wdm.operaDriverVersion");
        private readonly ConfigKey<string> operaDriverExport = new ConfigKey<string>("wdm.operaDriverExport");
        private readonly ConfigKey<Uri> operaDriverUrl = new ConfigKey<Uri>("wdm.operaDriverUrl");
        private readonly ConfigKey<Uri> operaDriverMirrorUrl = new ConfigKey<Uri>("wdm.operaDriverMirrorUrl");

        private readonly ConfigKey<string> phantomjsDriverVersion = new ConfigKey<string>("wdm.phantomjsDriverVersion");
        private readonly ConfigKey<string> phantomjsDriverExport = new ConfigKey<string>("wdm.phantomjsDriverExport");
        private readonly ConfigKey<Uri> phantomjsDriverUrl = new ConfigKey<Uri>("wdm.phantomjsDriverUrl");
        private readonly ConfigKey<Uri> phantomjsDriverMirrorUrl = new ConfigKey<Uri>("wdm.phantomjsDriverMirrorUrl");

        private readonly ConfigKey<string> seleniumServerStandaloneVersion = new ConfigKey<string>("wdm.seleniumServerStandaloneVersion");
        private readonly ConfigKey<Uri> seleniumServerStandaloneUrl = new ConfigKey<Uri>("wdm.seleniumServerStandaloneUrl");

        private readonly ConfigKey<int> serverPort = new ConfigKey<int>("wdm.serverPort");
        private readonly ConfigKey<string> binaryPath = new ConfigKey<string>("wdm.binaryPath");
        private readonly ConfigKey<int> ttl = new ConfigKey<int>("wdm.ttl");

        private T resolve<T>(ConfigKey<T> configKey)
        {
            string name = configKey.GetName();
            T value = configKey.GetValue();

            return resolver(name, value);
        }

        private T resolver<T>(string name, T value)
        {
            string strValue;
            strValue = Environment.GetEnvironmentVariable(name.ToUpper().Replace(".", "_"));
            if (strValue == null)
            {
                strValue = Environment.GetEnvironmentVariable(name);
            }
            if (strValue == null && value != null)
            {
                return value;
            }
            if (strValue == null)
            {
                strValue = getProperty(name);
            }
            return (T)parse<T>(strValue);
        }

        private object parse<T>(string strValue)
        {
            object output;
            if (typeof(T).Equals(typeof(string)))
            {
                output = strValue;
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                output = int.Parse(strValue);
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                output = bool.Parse(strValue);
            }
            else if (typeof(T).Equals(typeof(System.Uri)))
            {
                try
                {
                    output = new Uri(strValue);
                }
                catch (Exception e)
                {
                    throw new WebDriverManagerException(e);
                }
            }
            else
            {
                throw new WebDriverManagerException("Type " + typeof(T).FullName + " cannot be parsed");
            }
            return output;
        }

        private string getProperty(string key)
        {
            string value = null;
            string propertiesValue = GetProperties();
            string defaultProperties = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources", "webdrivermanager.properties");
            try
            {
                value = getPropertyFrom(propertiesValue, key);
                if (value == null)
                {
                    log.Trace("Property {0} not found in {1}, using default values (in {2})", key, propertiesValue, defaultProperties);
                    value = getPropertyFrom(defaultProperties, key);
                }
            }
            finally
            {
                if (value == null)
                {
                    log.Trace("Property {0} not found in {1}, using blank value", key, defaultProperties);
                    value = "";
                }
            }
            return value;
        }

        private string getPropertyFrom(string properties, string key)
        {
            Properties props = new Properties();
            try
            {
                if (File.Exists(properties))
                {
                    Stream inputStream = File.OpenRead(properties);
                    props.Load(inputStream);
                }
            }
            catch (IOException)
            {
                log.Trace("Property {0} not found in {1}", key, properties);
            }
            return props.GetProperty(key);
        }

        public void Reset()
        {
            foreach (FieldInfo field in typeof(Config).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetType().IsGenericType && field.GetType().GetGenericTypeDefinition() == typeof(ConfigKey<>))
                {
                    try
                    {
                        field.GetType().GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public).Invoke(this, new object[0]);
                    }
                    catch (Exception)
                    {
                        log.Warn("Exception resetting {0}", field.Name);
                    }
                }
            }
        }

        private static string defaultOsName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "WIN";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "LINUX";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "MAC";
            }

            throw new Exception("Could not determine operating system");
        }

        private static string defaultArchitecture()
        {
            //RuntimeInformation.OSArchitecture ?
            System.Runtime.InteropServices.Architecture arch = RuntimeInformation.ProcessArchitecture;
            if (arch == System.Runtime.InteropServices.Architecture.X86)
                return "X32";
            if (arch == System.Runtime.InteropServices.Architecture.X64)
                return "X64";
            throw new Exception(string.Format("Unhandled architecture {0}", arch));
        }

        public bool IsExecutable(FileInfo file)
        {
            if (resolve(os).ToLower().Equals("win"))
            {
                return file.Extension.Equals(".exe");
            }

            throw new NotImplementedException("Detection of executable files on non-windows is not implemented");
        }

        // Getters and setters

        public string GetProperties()
        {
            return resolve(properties);
        }

        public Config SetProperties(string properties)
        {
            this.properties.SetValue(properties);
            return this;
        }

        public string GetTargetPath()
        {
            string resolved = resolve(targetPath);
            string path = null;

            if (resolved != null)
            {
                path = resolved;
                if (path.Contains(HOME))
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        path = path.Replace(HOME, Environment.ExpandEnvironmentVariables("%userprofile%"));
                    }
                    else
                    {
                        //path = path.Replace(HOME, getSystemProperty("user.home"));
                        throw new NotImplementedException(string.Format("Replacement of home ({0}) on non-windows is not implemented", HOME));
                    }
                }
                if (path.Equals("."))
                {
                    path = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                }
            }
            return path;
        }

        public Config SetTargetPath(string value)
        {
            this.targetPath.SetValue(value);
            return this;
        }

        public bool IsForceCache()
        {
            return resolve(forceCache);
        }

        public Config SetForceCache(bool value)
        {
            this.forceCache.SetValue(value);
            return this;
        }

        public bool IsOverride()
        {
            return resolve(over_ride);
        }

        public Config SetOverride(bool value)
        {
            this.over_ride.SetValue(value);
            return this;
        }

        public bool IsUseMirror()
        {
            return resolve(useMirror);
        }

        public Config SetUseMirror(bool value)
        {
            this.useMirror.SetValue(value);
            return this;
        }

        public bool IsUseBetaVersions()
        {
            return resolve(useBetaVersions);
        }

        public Config SetUseBetaVersions(bool value)
        {
            this.useBetaVersions.SetValue(value);
            return this;
        }

        public bool IsAvoidExport()
        {
            return resolve(avoidExport);
        }

        public Config SetAvoidExport(bool value)
        {
            this.avoidExport.SetValue(value);
            return this;
        }

        public bool IsAvoidOutputTree()
        {
            return resolve(avoidOutputTree);
        }

        public Config SetAvoidOutputTree(bool value)
        {
            this.avoidOutputTree.SetValue(value);
            return this;
        }

        public bool IsAvoidAutoVersion()
        {
            return resolve(avoidAutoVersion);
        }

        public Config SetAvoidAutoVersion(bool value)
        {
            this.avoidAutoVersion.SetValue(value);
            return this;
        }

        public bool IsAvoidAutoReset()
        {
            return resolve(avoidAutoReset);
        }

        public Config SetAvoidAutoReset(bool value)
        {
            this.avoidAutoReset.SetValue(value);
            return this;
        }

        public bool IsAvoidPreferences()
        {
            return resolve(avoidPreferences);
        }

        public Config SetAvoidPreferences(bool value)
        {
            this.avoidPreferences.SetValue(value);
            return this;
        }

        public int GetTimeout()
        {
            return resolve(timeout);
        }

        public Config SetTimeout(int value)
        {
            this.timeout.SetValue(value);
            return this;
        }

        public bool GetVersionsPropertiesOnlineFirst()
        {
            return resolve(versionsPropertiesOnlineFirst);
        }

        public Config SetVersionsPropertiesOnlineFirst(bool value)
        {
            this.versionsPropertiesOnlineFirst.SetValue(value);
            return this;
        }

        public Uri GetVersionsPropertiesUrl()
        {
            return resolve(versionsPropertiesUrl);
        }

        public Config SetVersionsPropertiesUrl(System.Uri value)
        {
            this.versionsPropertiesUrl.SetValue(value);
            return this;
        }

        public Architecture GetArchitecture()
        {
            string architectureString = resolve(architecture);
            if ("32".Equals(architectureString))
            {
                return Architecture.X32;
            }
            if ("64".Equals(architectureString))
            {
                return Architecture.X64;
            }
            return (Architecture)Enum.Parse(typeof(Architecture), architectureString);
        }

        public Config SetArchitecture(Architecture value)
        {
            this.architecture.SetValue(value.ToString());
            return this;
        }

        public string GetOs()
        {
            return resolve(os);
        }

        public Config SetOs(string value)
        {
            this.os.SetValue(value);
            return this;
        }

        public string GetProxy()
        {
            return resolve(proxy);
        }

        public Config SetProxy(string value)
        {
            this.proxy.SetValue(value);
            return this;
        }

        public string GetProxyUser()
        {
            return resolve(proxyUser);
        }

        public Config SetProxyUser(string value)
        {
            this.proxyUser.SetValue(value);
            return this;
        }

        public string GetProxyPass()
        {
            return resolve(proxyPass);
        }

        public Config SetProxyPass(string value)
        {
            this.proxyPass.SetValue(value);
            return this;
        }

        public string[] GetIgnoreVersions()
        {
            string ignored = resolve(ignoreVersions);
            string[] output = new string[0];
            if (!string.IsNullOrEmpty(ignored))
            {
                output = ignored.Split(',');
            }
            return output;
        }

        public Config SetIgnoreVersions(params string[] value)
        {
            this.ignoreVersions.SetValue(string.Join(",", value));
            return this;
        }

        public string GetGitHubTokenName()
        {
            return resolve(gitHubTokenName);
        }

        public Config SetGitHubTokenName(string value)
        {
            this.gitHubTokenName.SetValue(value);
            return this;
        }

        public string GetGitHubTokenSecret()
        {
            return resolve(gitHubTokenSecret);
        }

        public Config SetGitHubTokenSecret(string value)
        {
            this.gitHubTokenSecret.SetValue(value);
            return this;
        }

        public string GetLocalRepositoryUser()
        {
            return resolve(localRepositoryUser);
        }

        public Config SetLocalRepositoryUser(string value)
        {
            this.localRepositoryUser.SetValue(value);
            return this;
        }

        public string GetLocalRepositoryPassword()
        {
            return resolve(localRepositoryPassword);
        }

        public Config SetLocalRepositoryPassword(string value)
        {
            this.localRepositoryPassword.SetValue(value);
            return this;
        }

        public int GetServerPort()
        {
            return resolve(serverPort);
        }

        public Config SetServerPort(int value)
        {
            this.serverPort.SetValue(value);
            return this;
        }

        public int GetTtl()
        {
            return resolve(ttl);
        }

        public Config SetTtl(int value)
        {
            this.ttl.SetValue(value);
            return this;
        }

        public string GetBinaryPath()
        {
            return resolve(binaryPath);
        }

        public Config SetBinaryPath(string value)
        {
            this.binaryPath.SetValue(value);
            return this;
        }

        public string GetChromeDriverVersion()
        {
            return resolve(chromeDriverVersion);
        }

        public Config SetChromeDriverVersion(string value)
        {
            this.chromeDriverVersion.SetValue(value);
            return this;
        }

        public string GetChromeDriverExport()
        {
            return resolve(chromeDriverExport);
        }

        public Config SetChromeDriverExport(string value)
        {
            this.chromeDriverExport.SetValue(value);
            return this;
        }

        public Uri GetChromeDriverUrl()
        {
            return resolve(chromeDriverUrl);
        }

        public Config SetChromeDriverUrl(Uri value)
        {
            this.chromeDriverUrl.SetValue(value);
            return this;
        }

        public Uri GetChromeDriverMirrorUrl()
        {
            return resolve(chromeDriverMirrorUrl);
        }

        public Config SetChromeDriverMirrorUrl(System.Uri value)
        {
            this.chromeDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string GetEdgeDriverVersion()
        {
            return resolve(edgeDriverVersion);
        }

        public Config SetEdgeDriverVersion(string value)
        {
            this.edgeDriverVersion.SetValue(value);
            return this;
        }

        public string GetEdgeDriverExport()
        {
            return resolve(edgeDriverExport);
        }

        public Config SetEdgeDriverExport(string value)
        {
            this.edgeDriverExport.SetValue(value);
            return this;
        }

        public Uri GetEdgeDriverUrl()
        {
            return resolve(edgeDriverUrl);
        }

        public Config SetEdgeDriverUrl(Uri value)
        {
            this.edgeDriverUrl.SetValue(value);
            return this;
        }

        public string GetFirefoxDriverVersion()
        {
            return resolve(firefoxDriverVersion);
        }

        public Config SetFirefoxDriverVersion(string value)
        {
            this.firefoxDriverVersion.SetValue(value);
            return this;
        }

        public string GetFirefoxDriverExport()
        {
            return resolve(firefoxDriverExport);
        }

        public Config SetFirefoxDriverExport(string value)
        {
            this.firefoxDriverExport.SetValue(value);
            return this;
        }

        public Uri GetFirefoxDriverUrl()
        {
            return resolve(firefoxDriverUrl);
        }

        public Config SetFirefoxDriverUrl(Uri value)
        {
            this.firefoxDriverUrl.SetValue(value);
            return this;
        }

        public Uri getFirefoxDriverMirrorUrl()
        {
            return resolve(firefoxDriverMirrorUrl);
        }

        public Config SetFirefoxDriverMirrorUrl(Uri value)
        {
            this.firefoxDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string GetInternetExplorerDriverVersion()
        {
            return resolve(internetExplorerDriverVersion);
        }

        public Config SetInternetExplorerDriverVersion(string value)
        {
            this.internetExplorerDriverVersion.SetValue(value);
            return this;
        }

        public string GetInternetExplorerDriverExport()
        {
            return resolve(internetExplorerDriverExport);
        }

        public Config SetInternetExplorerDriverExport(string value)
        {
            this.internetExplorerDriverExport.SetValue(value);
            return this;
        }

        public Uri GetInternetExplorerDriverUrl()
        {
            return resolve(internetExplorerDriverUrl);
        }

        public Config SetInternetExplorerDriverUrl(Uri value)
        {
            this.internetExplorerDriverUrl.SetValue(value);
            return this;
        }

        public string GetOperaDriverVersion()
        {
            return resolve(operaDriverVersion);
        }

        public Config SetOperaDriverVersion(string value)
        {
            this.operaDriverVersion.SetValue(value);
            return this;
        }

        public string GetOperaDriverExport()
        {
            return resolve(operaDriverExport);
        }

        public Config SetOperaDriverExport(string value)
        {
            this.operaDriverExport.SetValue(value);
            return this;
        }

        public Uri GetOperaDriverUrl()
        {
            return resolve(operaDriverUrl);
        }

        public Config SetOperaDriverUrl(Uri value)
        {
            this.operaDriverUrl.SetValue(value);
            return this;
        }

        public Uri GetOperaDriverMirrorUrl()
        {
            return resolve(operaDriverMirrorUrl);
        }

        public Config SetOperaDriverMirrorUrl(Uri value)
        {
            this.operaDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string GetPhantomjsDriverVersion()
        {
            return resolve(phantomjsDriverVersion);
        }

        public Config SetPhantomjsDriverVersion(string value)
        {
            this.phantomjsDriverVersion.SetValue(value);
            return this;
        }

        public string GetPhantomjsDriverExport()
        {
            return resolve(phantomjsDriverExport);
        }

        public Config SetPhantomjsDriverExport(string value)
        {
            this.phantomjsDriverExport.SetValue(value);
            return this;
        }

        public Uri GetPhantomjsDriverUrl()
        {
            return resolve(phantomjsDriverUrl);
        }

        public Config SetPhantomjsDriverUrl(Uri value)
        {
            this.phantomjsDriverUrl.SetValue(value);
            return this;
        }

        public Uri GetPhantomjsDriverMirrorUrl()
        {
            return resolve(phantomjsDriverMirrorUrl);
        }

        public Config SetPhantomjsDriverMirrorUrl(Uri value)
        {
            this.phantomjsDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string GetSeleniumServerStandaloneVersion()
        {
            return resolve(seleniumServerStandaloneVersion);
        }

        public Config SetSeleniumServerStandaloneVersion(string value)
        {
            this.seleniumServerStandaloneVersion.SetValue(value);
            return this;
        }

        public Uri GetSeleniumServerStandaloneUrl()
        {
            return resolve(seleniumServerStandaloneUrl);
        }

        public Config SetSeleniumServerStandaloneUrl(Uri value)
        {
            this.seleniumServerStandaloneUrl.SetValue(value);
            return this;
        }

    }
}