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

namespace WebDriverManager
{
    /**
     * Configuration class.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.2.0
     */
    public class Config
    {
        ILogger log = Logger.GetLogger();

        static string HOME = "~";

        ConfigKey<string> properties = new ConfigKey<string>("wdm.properties", "webdrivermanager.properties");

        ConfigKey<string> targetPath = new ConfigKey<string>("wdm.targetPath");
        ConfigKey<bool> forceCache = new ConfigKey<bool>("wdm.forceCache");
        ConfigKey<bool> over_ride = new ConfigKey<bool>("wdm.override");
        ConfigKey<bool> useMirror = new ConfigKey<bool>("wdm.useMirror");
        ConfigKey<bool> useBetaVersions = new ConfigKey<bool>("wdm.useBetaVersions");
        ConfigKey<bool> avoidExport = new ConfigKey<bool>("wdm.avoidExport");
        ConfigKey<bool> avoidOutputTree = new ConfigKey<bool>("wdm.avoidOutputTree");
        ConfigKey<bool> avoidAutoVersion = new ConfigKey<bool>("wdm.avoidAutoVersion");
        ConfigKey<bool> avoidAutoReset = new ConfigKey<bool>("wdm.avoidAutoReset");
        ConfigKey<bool> avoidPreferences = new ConfigKey<bool>("wdm.avoidPreferences");
        ConfigKey<int> timeout = new ConfigKey<int>("wdm.timeout");
        ConfigKey<bool> versionsPropertiesOnlineFirst = new ConfigKey<bool>("wdm.versionsPropertiesOnlineFirst");
        ConfigKey<Uri> versionsPropertiesUrl = new ConfigKey<Uri>("wdm.versionsPropertiesUrl");

        ConfigKey<string> architecture = new ConfigKey<string>("wdm.architecture", defaultArchitecture());
        ConfigKey<string> os = new ConfigKey<string>("wdm.os", defaultOsName());
        ConfigKey<string> proxy = new ConfigKey<string>("wdm.proxy");
        ConfigKey<string> proxyUser = new ConfigKey<string>("wdm.proxyUser");
        ConfigKey<string> proxyPass = new ConfigKey<string>("wdm.proxyPass");
        ConfigKey<string> ignoreVersions = new ConfigKey<string>("wdm.ignoreVersions");
        ConfigKey<string> gitHubTokenName = new ConfigKey<string>("wdm.gitHubTokenName");
        ConfigKey<string> gitHubTokenSecret = new ConfigKey<string>("wdm.gitHubTokenSecret");
        ConfigKey<string> localRepositoryUser = new ConfigKey<string>("wdm.localRepositoryUser");
        ConfigKey<string> localRepositoryPassword = new ConfigKey<string>("wdm.localRepositoryPassword");

        ConfigKey<string> chromeDriverVersion = new ConfigKey<string>("wdm.chromeDriverVersion");
        ConfigKey<string> chromeDriverExport = new ConfigKey<string>("wdm.chromeDriverExport");
        ConfigKey<Uri> chromeDriverUrl = new ConfigKey<Uri>("wdm.chromeDriverUrl");
        ConfigKey<Uri> chromeDriverMirrorUrl = new ConfigKey<Uri>("wdm.chromeDriverMirrorUrl");

        ConfigKey<string> edgeDriverVersion = new ConfigKey<string>("wdm.edgeDriverVersion");
        ConfigKey<string> edgeDriverExport = new ConfigKey<string>("wdm.edgeDriverExport");
        ConfigKey<Uri> edgeDriverUrl = new ConfigKey<Uri>("wdm.edgeDriverUrl");

        ConfigKey<string> firefoxDriverVersion = new ConfigKey<string>("wdm.geckoDriverVersion");
        ConfigKey<string> firefoxDriverExport = new ConfigKey<string>("wdm.geckoDriverExport");
        ConfigKey<Uri> firefoxDriverUrl = new ConfigKey<Uri>("wdm.geckoDriverUrl");
        ConfigKey<Uri> firefoxDriverMirrorUrl = new ConfigKey<Uri>("wdm.geckoDriverMirrorUrl");

        ConfigKey<string> internetExplorerDriverVersion = new ConfigKey<string>("wdm.internetExplorerDriverVersion");
        ConfigKey<string> internetExplorerDriverExport = new ConfigKey<string>("wdm.internetExplorerDriverExport");
        ConfigKey<Uri> internetExplorerDriverUrl = new ConfigKey<Uri>("wdm.internetExplorerDriverUrl");

        ConfigKey<string> operaDriverVersion = new ConfigKey<string>("wdm.operaDriverVersion");
        ConfigKey<string> operaDriverExport = new ConfigKey<string>("wdm.operaDriverExport");
        ConfigKey<Uri> operaDriverUrl = new ConfigKey<Uri>("wdm.operaDriverUrl");
        ConfigKey<Uri> operaDriverMirrorUrl = new ConfigKey<Uri>("wdm.operaDriverMirrorUrl");

        ConfigKey<string> phantomjsDriverVersion = new ConfigKey<string>("wdm.phantomjsDriverVersion");
        ConfigKey<string> phantomjsDriverExport = new ConfigKey<string>("wdm.phantomjsDriverExport");
        ConfigKey<Uri> phantomjsDriverUrl = new ConfigKey<Uri>("wdm.phantomjsDriverUrl");
        ConfigKey<Uri> phantomjsDriverMirrorUrl = new ConfigKey<Uri>("wdm.phantomjsDriverMirrorUrl");

        ConfigKey<string> seleniumServerStandaloneVersion = new ConfigKey<string>("wdm.seleniumServerStandaloneVersion");
        ConfigKey<Uri> seleniumServerStandaloneUrl = new ConfigKey<Uri>("wdm.seleniumServerStandaloneUrl");

        ConfigKey<int> serverPort = new ConfigKey<int>("wdm.serverPort");
        ConfigKey<string> binaryPath = new ConfigKey<string>("wdm.binaryPath");
        ConfigKey<int> ttl = new ConfigKey<int>("wdm.ttl");

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
                // TODO: implement this?
                //strValue = getSystemProperty(name);
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
            object output = null;
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
                    output = new System.Uri(strValue);
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
            string propertiesValue = getProperties();
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
                Stream inputStream = File.OpenRead(properties);
                props.Load(inputStream);
            }
            catch (IOException e)
            {
                log.Trace("Property {0} not found in {1}", key, properties);
            }
            return props.GetProperty(key);
        }

        public void reset()
        {
            foreach (FieldInfo field in typeof(Config).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.GetType().IsGenericType && field.GetType().GetGenericTypeDefinition() == typeof(ConfigKey<>))
                {
                    try
                    {
                        field.GetType().GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public).Invoke(this, new object[] { });
                    }
                    catch (Exception e)
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

        public bool isExecutable(FileInfo file)
        {
            return resolve(os).ToLower().Equals("win")
                    ? file.Extension.Equals(".exe")
                    : throw new NotImplementedException("Detection of executable files on non-windows is not implemented");
        }

        // Getters and setters

        public string getProperties()
        {
            return resolve(properties);
        }

        public Config setProperties(string properties)
        {
            this.properties.SetValue(properties);
            return this;
        }

        public string getTargetPath()
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

        public Config setTargetPath(string value)
        {
            this.targetPath.SetValue(value);
            return this;
        }

        public bool isForceCache()
        {
            return resolve(forceCache);
        }

        public Config setForceCache(bool value)
        {
            this.forceCache.SetValue(value);
            return this;
        }

        public bool isOverride()
        {
            return resolve(over_ride);
        }

        public Config setOverride(bool value)
        {
            this.over_ride.SetValue(value);
            return this;
        }

        public bool isUseMirror()
        {
            return resolve(useMirror);
        }

        public Config setUseMirror(bool value)
        {
            this.useMirror.SetValue(value);
            return this;
        }

        public bool isUseBetaVersions()
        {
            return resolve(useBetaVersions);
        }

        public Config setUseBetaVersions(bool value)
        {
            this.useBetaVersions.SetValue(value);
            return this;
        }

        public bool isAvoidExport()
        {
            return resolve(avoidExport);
        }

        public Config setAvoidExport(bool value)
        {
            this.avoidExport.SetValue(value);
            return this;
        }

        public bool isAvoidOutputTree()
        {
            return resolve(avoidOutputTree);
        }

        public Config setAvoidOutputTree(bool value)
        {
            this.avoidOutputTree.SetValue(value);
            return this;
        }

        public bool isAvoidAutoVersion()
        {
            return resolve(avoidAutoVersion);
        }

        public Config setAvoidAutoVersion(bool value)
        {
            this.avoidAutoVersion.SetValue(value);
            return this;
        }

        public bool isAvoidAutoReset()
        {
            return resolve(avoidAutoReset);
        }

        public Config setAvoidAutoReset(bool value)
        {
            this.avoidAutoReset.SetValue(value);
            return this;
        }

        public bool isAvoidPreferences()
        {
            return resolve(avoidPreferences);
        }

        public Config setAvoidPreferences(bool value)
        {
            this.avoidPreferences.SetValue(value);
            return this;
        }

        public int getTimeout()
        {
            return resolve(timeout);
        }

        public Config setTimeout(int value)
        {
            this.timeout.SetValue(value);
            return this;
        }

        public bool getVersionsPropertiesOnlineFirst()
        {
            return resolve(versionsPropertiesOnlineFirst);
        }

        public Config setVersionsPropertiesOnlineFirst(bool value)
        {
            this.versionsPropertiesOnlineFirst.SetValue(value);
            return this;
        }

        public System.Uri getVersionsPropertiesUrl()
        {
            return resolve(versionsPropertiesUrl);
        }

        public Config setVersionsPropertiesUrl(System.Uri value)
        {
            this.versionsPropertiesUrl.SetValue(value);
            return this;
        }

        public Architecture getArchitecture()
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

        public Config setArchitecture(Architecture value)
        {
            this.architecture.SetValue(value.ToString());
            return this;
        }

        public string getOs()
        {
            return resolve(os);
        }

        public Config setOs(string value)
        {
            this.os.SetValue(value);
            return this;
        }

        public string getProxy()
        {
            return resolve(proxy);
        }

        public Config setProxy(string value)
        {
            this.proxy.SetValue(value);
            return this;
        }

        public string getProxyUser()
        {
            return resolve(proxyUser);
        }

        public Config setProxyUser(string value)
        {
            this.proxyUser.SetValue(value);
            return this;
        }

        public string getProxyPass()
        {
            return resolve(proxyPass);
        }

        public Config setProxyPass(string value)
        {
            this.proxyPass.SetValue(value);
            return this;
        }

        public string[] getIgnoreVersions()
        {
            string ignored = resolve(ignoreVersions);
            string[] output = { };
            if (!string.IsNullOrEmpty(ignored))
            {
                output = ignored.Split(',');
            }
            return output;
        }

        public Config setIgnoreVersions(params string[] value)
        {
            this.ignoreVersions.SetValue(string.Join(",", value));
            return this;
        }

        public string getGitHubTokenName()
        {
            return resolve(gitHubTokenName);
        }

        public Config setGitHubTokenName(string value)
        {
            this.gitHubTokenName.SetValue(value);
            return this;
        }

        public string getGitHubTokenSecret()
        {
            return resolve(gitHubTokenSecret);
        }

        public Config setGitHubTokenSecret(string value)
        {
            this.gitHubTokenSecret.SetValue(value);
            return this;
        }

        public string getLocalRepositoryUser()
        {
            return resolve(localRepositoryUser);
        }

        public Config setLocalRepositoryUser(string value)
        {
            this.localRepositoryUser.SetValue(value);
            return this;
        }

        public string getLocalRepositoryPassword()
        {
            return resolve(localRepositoryPassword);
        }

        public Config setLocalRepositoryPassword(string value)
        {
            this.localRepositoryPassword.SetValue(value);
            return this;
        }

        public int getServerPort()
        {
            return resolve(serverPort);
        }

        public Config setServerPort(int value)
        {
            this.serverPort.SetValue(value);
            return this;
        }

        public int getTtl()
        {
            return resolve(ttl);
        }

        public Config setTtl(int value)
        {
            this.ttl.SetValue(value);
            return this;
        }

        public string getBinaryPath()
        {
            return resolve(binaryPath);
        }

        public Config setBinaryPath(string value)
        {
            this.binaryPath.SetValue(value);
            return this;
        }

        public string getChromeDriverVersion()
        {
            return resolve(chromeDriverVersion);
        }

        public Config setChromeDriverVersion(string value)
        {
            this.chromeDriverVersion.SetValue(value);
            return this;
        }

        public string getChromeDriverExport()
        {
            return resolve(chromeDriverExport);
        }

        public Config setChromeDriverExport(string value)
        {
            this.chromeDriverExport.SetValue(value);
            return this;
        }

        public System.Uri getChromeDriverUrl()
        {
            return resolve(chromeDriverUrl);
        }

        public Config setChromeDriverUrl(System.Uri value)
        {
            this.chromeDriverUrl.SetValue(value);
            return this;
        }

        public System.Uri getChromeDriverMirrorUrl()
        {
            return resolve(chromeDriverMirrorUrl);
        }

        public Config setChromeDriverMirrorUrl(System.Uri value)
        {
            this.chromeDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string getEdgeDriverVersion()
        {
            return resolve(edgeDriverVersion);
        }

        public Config setEdgeDriverVersion(string value)
        {
            this.edgeDriverVersion.SetValue(value);
            return this;
        }

        public string getEdgeDriverExport()
        {
            return resolve(edgeDriverExport);
        }

        public Config setEdgeDriverExport(string value)
        {
            this.edgeDriverExport.SetValue(value);
            return this;
        }

        public Uri getEdgeDriverUrl()
        {
            return resolve(edgeDriverUrl);
        }

        public Config setEdgeDriverUrl(System.Uri value)
        {
            this.edgeDriverUrl.SetValue(value);
            return this;
        }

        public string getFirefoxDriverVersion()
        {
            return resolve(firefoxDriverVersion);
        }

        public Config setFirefoxDriverVersion(string value)
        {
            this.firefoxDriverVersion.SetValue(value);
            return this;
        }

        public string getFirefoxDriverExport()
        {
            return resolve(firefoxDriverExport);
        }

        public Config setFirefoxDriverExport(string value)
        {
            this.firefoxDriverExport.SetValue(value);
            return this;
        }

        public System.Uri getFirefoxDriverUrl()
        {
            return resolve(firefoxDriverUrl);
        }

        public Config setFirefoxDriverUrl(System.Uri value)
        {
            this.firefoxDriverUrl.SetValue(value);
            return this;
        }

        public System.Uri getFirefoxDriverMirrorUrl()
        {
            return resolve(firefoxDriverMirrorUrl);
        }

        public Config setFirefoxDriverMirrorUrl(System.Uri value)
        {
            this.firefoxDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string getInternetExplorerDriverVersion()
        {
            return resolve(internetExplorerDriverVersion);
        }

        public Config setInternetExplorerDriverVersion(string value)
        {
            this.internetExplorerDriverVersion.SetValue(value);
            return this;
        }

        public string getInternetExplorerDriverExport()
        {
            return resolve(internetExplorerDriverExport);
        }

        public Config setInternetExplorerDriverExport(string value)
        {
            this.internetExplorerDriverExport.SetValue(value);
            return this;
        }

        public System.Uri getInternetExplorerDriverUrl()
        {
            return resolve(internetExplorerDriverUrl);
        }

        public Config setInternetExplorerDriverUrl(System.Uri value)
        {
            this.internetExplorerDriverUrl.SetValue(value);
            return this;
        }

        public string getOperaDriverVersion()
        {
            return resolve(operaDriverVersion);
        }

        public Config setOperaDriverVersion(string value)
        {
            this.operaDriverVersion.SetValue(value);
            return this;
        }

        public string getOperaDriverExport()
        {
            return resolve(operaDriverExport);
        }

        public Config setOperaDriverExport(string value)
        {
            this.operaDriverExport.SetValue(value);
            return this;
        }

        public System.Uri getOperaDriverUrl()
        {
            return resolve(operaDriverUrl);
        }

        public Config setOperaDriverUrl(System.Uri value)
        {
            this.operaDriverUrl.SetValue(value);
            return this;
        }

        public System.Uri getOperaDriverMirrorUrl()
        {
            return resolve(operaDriverMirrorUrl);
        }

        public Config setOperaDriverMirrorUrl(System.Uri value)
        {
            this.operaDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string getPhantomjsDriverVersion()
        {
            return resolve(phantomjsDriverVersion);
        }

        public Config setPhantomjsDriverVersion(string value)
        {
            this.phantomjsDriverVersion.SetValue(value);
            return this;
        }

        public string getPhantomjsDriverExport()
        {
            return resolve(phantomjsDriverExport);
        }

        public Config setPhantomjsDriverExport(string value)
        {
            this.phantomjsDriverExport.SetValue(value);
            return this;
        }

        public System.Uri getPhantomjsDriverUrl()
        {
            return resolve(phantomjsDriverUrl);
        }

        public Config setPhantomjsDriverUrl(System.Uri value)
        {
            this.phantomjsDriverUrl.SetValue(value);
            return this;
        }

        public System.Uri getPhantomjsDriverMirrorUrl()
        {
            return resolve(phantomjsDriverMirrorUrl);
        }

        public Config setPhantomjsDriverMirrorUrl(System.Uri value)
        {
            this.phantomjsDriverMirrorUrl.SetValue(value);
            return this;
        }

        public string getSeleniumServerStandaloneVersion()
        {
            return resolve(seleniumServerStandaloneVersion);
        }

        public Config setSeleniumServerStandaloneVersion(string value)
        {
            this.seleniumServerStandaloneVersion.SetValue(value);
            return this;
        }

        public System.Uri getSeleniumServerStandaloneUrl()
        {
            return resolve(seleniumServerStandaloneUrl);
        }

        public Config setSeleniumServerStandaloneUrl(System.Uri value)
        {
            this.seleniumServerStandaloneUrl.SetValue(value);
            return this;
        }

    }
}