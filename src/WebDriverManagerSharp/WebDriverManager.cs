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

using HtmlAgilityPack;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using WebDriverManagerSharp.GitHubApi;

namespace WebDriverManagerSharp
{
    /**
     * Parent driver manager.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.0
     */
    public abstract class WebDriverManager
    {
        protected static ILogger Log = Logger.GetLogger();

        protected static string SLASH = "/";
        protected static string PRE_INSTALLED = "pre-installed";
        protected static string BETA = "beta";
        protected static string ONLINE = "online";
        protected static string LOCAL = "local";

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        protected abstract List<Uri> GetDrivers();

        protected abstract string GetBrowserVersion();

        protected abstract DriverManagerType? GetDriverManagerType();

        protected abstract string GetDriverName();

        protected abstract void SetDriverVersion(string version);

        protected abstract string GetDriverVersion();

        protected abstract void SetDriverUrl(Uri url);

        protected abstract Uri GetDriverUrl();

        protected abstract Uri GetMirrorUrl();

        protected abstract string GetExportParameter();

        private static Dictionary<DriverManagerType, WebDriverManager> instanceMap = new Dictionary<DriverManagerType, WebDriverManager>();

        protected HttpClient HttpClient;
        protected Downloader Downloader;
        private UrlFilter urlFilter;
        protected string VersionToDownload;
        private string downloadedVersion;
        private string latestVersion;
        private string binaryPath;
        private bool mirrorLog;
        protected List<string> ListVersions;
        private bool forcedArch;
        private bool forcedOs;
        private bool isLatest;
        private bool retry = true;
        private Config config = new Config();
        private Preferences preferences;
        private string preferenceKey;
        private Properties versionsProperties;

        protected WebDriverManager()
        {
            preferences = new Preferences(config);
        }

        public static Config GlobalConfig()
        {
            Config global = new Config();
            global.SetAvoidAutoReset(true);
            foreach (DriverManagerType type in Enum.GetValues(typeof(DriverManagerType)))
            {
                GetInstance(type).setConfig(global);
            }
            return global;
        }

        public Config Config()
        {
            return config;
        }

        public static WebDriverManager ChromeDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.CHROME))
            {
                instanceMap.Add(DriverManagerType.CHROME, new ChromeDriverManager());
            }
            return instanceMap[DriverManagerType.CHROME];
        }

        public static WebDriverManager FirefoxDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.FIREFOX))
            {
                instanceMap.Add(DriverManagerType.FIREFOX, new FirefoxDriverManager());
            }
            return instanceMap[DriverManagerType.FIREFOX];
        }

        public static WebDriverManager OperaDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.OPERA))
            {
                instanceMap.Add(DriverManagerType.OPERA, new OperaDriverManager());
            }
            return instanceMap[DriverManagerType.OPERA];
        }

        public static WebDriverManager EdgeDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.EDGE))
            {
                instanceMap.Add(DriverManagerType.EDGE, new EdgeDriverManager());
            }
            return instanceMap[DriverManagerType.EDGE];
        }

        public static WebDriverManager IEDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.IEXPLORER))
            {
                instanceMap.Add(DriverManagerType.IEXPLORER, new InternetExplorerDriverManager());
            }
            return instanceMap[DriverManagerType.IEXPLORER];
        }

        public static WebDriverManager PhantomJS()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.PHANTOMJS))
            {
                instanceMap.Add(DriverManagerType.PHANTOMJS, new PhantomJsDriverManager());
            }
            return instanceMap[DriverManagerType.PHANTOMJS];
        }

        public static WebDriverManager SeleniumServerStandalone()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.SELENIUM_SERVER_STANDALONE))
            {
                instanceMap.Add(DriverManagerType.SELENIUM_SERVER_STANDALONE, new SeleniumServerStandaloneManager());
            }
            return instanceMap[DriverManagerType.SELENIUM_SERVER_STANDALONE];
        }

        protected static WebDriverManager VoidDriver()
        {
            return new VoidDriverManager();
        }

        public static WebDriverManager GetInstance(DriverManagerType driverManagerType)
        {
            switch (driverManagerType)
            {
                case DriverManagerType.CHROME:
                    return ChromeDriver();
                case DriverManagerType.FIREFOX:
                    return FirefoxDriver();
                case DriverManagerType.OPERA:
                    return OperaDriver();
                case DriverManagerType.IEXPLORER:
                    return IEDriver();
                case DriverManagerType.EDGE:
                    return EdgeDriver();
                case DriverManagerType.PHANTOMJS:
                    return PhantomJS();
                case DriverManagerType.SELENIUM_SERVER_STANDALONE:
                    return SeleniumServerStandalone();
                default:
                    return VoidDriver();
            }
        }

        public static WebDriverManager GetInstance(Type driverType)
        {
            switch (driverType.FullName)
            {
                case "OpenQA.Selenium.Chrome.ChromeDriver":
                    return ChromeDriver();
                case "OpenQA.Selenium.Firefox.FirefoxDriver":
                    return FirefoxDriver();
                case "OpenQA.Selenium.Opera.OperaDriver":
                    return OperaDriver();
                case "OpenQA.Selenium.IE.InternetExplorerDriver":
                    return IEDriver();
                case "OpenQA.Selenium.Edge.EdgeDriver":
                    return EdgeDriver();
                case "OpenQA.Selenium.Phantomjs.PhantomJSDriver":
                    return PhantomJS();
                default:
                    return VoidDriver();
            }
        }

        public void Setup()
        {
            if (GetDriverManagerType() != null)
            {
                try
                {
                    Architecture architecture = Config().GetArchitecture();
                    string driverVersion = GetDriverVersion();
                    isLatest = isVersionLatest(driverVersion);
                    Manage(architecture, driverVersion);
                }
                finally
                {
                    if (!Config().IsAvoidAutoReset())
                    {
                        reset();
                    }
                }
            }
        }

        public WebDriverManager Version(string version)
        {
            SetDriverVersion(version);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Architecture(Architecture architecture)
        {
            Config().SetArchitecture(architecture);
            forcedArch = true;
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Arch32()
        {
            Architecture(WebDriverManagerSharp.Architecture.X32);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Arch64()
        {
            Architecture(WebDriverManagerSharp.Architecture.X64);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager OperatingSystem(OperatingSystem os)
        {
            Config().SetOs(os.ToString());
            forcedOs = true;
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager ForceCache()
        {
            Config().SetForceCache(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager ForceDownload()
        {
            Config().SetOverride(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager DriverRepositoryUrl(System.Uri url)
        {
            SetDriverUrl(url);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager UseMirror()
        {
            Uri mirrorUrl = GetMirrorUrl();
            if (mirrorUrl == null)
            {
                throw new WebDriverManagerException("Mirror URL not available");
            }
            Config().SetUseMirror(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Proxy(string proxy)
        {
            Config().SetProxy(proxy);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager ProxyUser(string proxyUser)
        {
            Config().SetProxyUser(proxyUser);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager ProxyPass(string proxyPass)
        {
            Config().SetProxyPass(proxyPass);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager UseBetaVersions()
        {
            Config().SetUseBetaVersions(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager IgnoreVersions(params string[] versions)
        {
            Config().SetIgnoreVersions(versions);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager GitHubTokenSecret(string gitHubTokenSecret)
        {
            Config().SetGitHubTokenSecret(gitHubTokenSecret);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager GitHubTokenName(string gitHubTokenName)
        {
            Config().SetGitHubTokenName(gitHubTokenName);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager LocalRepositoryUser(string localRepositoryUser)
        {
            Config().SetLocalRepositoryUser(localRepositoryUser);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager LocalRepositoryPassword(
                string localRepositoryPassword)
        {
            Config().SetLocalRepositoryPassword(localRepositoryPassword);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Timeout(int timeout)
        {
            Config().SetTimeout(timeout);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Properties(string properties)
        {
            Config().SetProperties(properties);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager TargetPath(string targetPath)
        {
            Config().SetTargetPath(targetPath);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager AvoidExport()
        {
            Config().SetAvoidExport(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager AvoidOutputTree()
        {
            Config().SetAvoidOutputTree(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager AvoidAutoVersion()
        {
            Config().SetAvoidAutoVersion(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager AvoidPreferences()
        {
            Config().SetAvoidPreferences(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Ttl(int seconds)
        {
            Config().SetTtl(seconds);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager BrowserPath(string browserPath)
        {
            Config().SetBinaryPath(browserPath);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager UseLocalVersionsPropertiesFirst()
        {
            Config().SetVersionsPropertiesOnlineFirst(false);
            return instanceMap[GetDriverManagerType().Value];
        }

        // ------------

        public string GetBinaryPath()
        {
            return instanceMap[GetDriverManagerType().Value].binaryPath;
        }

        public string GetDownloadedVersion()
        {
            return instanceMap[GetDriverManagerType().Value].downloadedVersion;
        }

        public virtual List<string> GetVersions()
        {
            HttpClient = new HttpClient(Config());
            try
            {
                List<Uri> drivers = GetDrivers();
                List<string> versions = new List<string>();
                foreach (Uri url in drivers)
                {
                    string version = GetCurrentVersion(url, GetDriverName());
                    if (string.IsNullOrEmpty(version) || version.Equals("icons", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    if (version.StartsWith("."))
                    {
                        version = version.SubstringJava(1);
                    }
                    if (!versions.Contains(version))
                    {
                        versions.Add(version);
                    }
                }
                Log.Trace("Version list before sorting {0}", versions);
                versions.Sort(new VersionComparator());
                return versions;
            }
            catch (IOException e)
            {
                throw new WebDriverManagerException(e);
            }
        }

        public HttpClient GetHttpClient()
        {
            return HttpClient;
        }

        public void ClearPreferences()
        {
            instanceMap[GetDriverManagerType().Value].preferences.Clear();
        }

        public void ClearCache()
        {
            string targetPath = Config().GetTargetPath();
            try
            {
                Log.Debug("Clearing cache at {0}", targetPath);
                Directory.Delete(targetPath);
            }
            catch (Exception e)
            {
                Log.Warn("Exception deleting cache at {0}", targetPath, e);
            }
        }

        // ------------

        public virtual string PreDownload(string target, string version)
        {
            Log.Trace("Pre-download. target={0}, version={1}", target, version);
            return target;
        }

        public virtual FileInfo PostDownload(FileInfo archive)
        {
            DirectoryInfo parentFolder = archive.Directory;
            FileInfo[] ls = parentFolder.GetFiles();
            foreach (FileInfo f in ls)
            {
                if (GetDriverName().Contains(f.Name.Substring(0, f.Name.Length - f.Extension.Length)))
                {
                    Log.Trace("Found binary in post-download: {0}", f);
                    return f;
                }
            }
            throw new WebDriverManagerException("Driver " + GetDriverName() + " not found (using temporal folder " + parentFolder + ")");
        }

        protected virtual string GetCurrentVersion(Uri url, string driverName)
        {
            string currentVersion = "";
            try
            {
                currentVersion = url.GetFile().SubstringJava(
                        url.GetFile().IndexOf(SLASH) + 1,
                        url.GetFile().LastIndexOf(SLASH));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.Trace("Exception getting version of System.Uri {0} ({1})", url, e.Message);
            }

            return currentVersion;
        }

        protected void Manage(Architecture arch, string version)
        {
            HttpClient = new HttpClient(Config());
            try
            {
                Downloader = new Downloader(GetDriverManagerType().Value);
                urlFilter = new UrlFilter();

                bool getLatest = isVersionLatest(version);
                bool cache = Config().IsForceCache();

                if (getLatest)
                {
                    version = detectDriverVersionFromBrowser();
                }
                getLatest = string.IsNullOrEmpty(version);

                // Check latest version
                if (getLatest && !Config().IsUseBetaVersions())
                {
                    string lastVersion = getLatestVersion();
                    getLatest = lastVersion == null;
                    if (!getLatest)
                    {
                        version = lastVersion;
                    }
                }

                // For Edge
                if (checkPreInstalledVersion(version))
                {
                    return;
                }

                string os = Config().GetOs();
                Log.Trace("Managing {0} arch={1} version={2} getLatest={3} cache={4}", GetDriverName(), arch, version, getLatest, cache);

                if (getLatest && latestVersion != null)
                {
                    Log.Debug("Latest version of {0} is {1} (recently resolved)", GetDriverName(), latestVersion);
                    version = latestVersion;
                    cache = true;
                }

                FileInfo driverInCache = handleCache(arch, version, os, getLatest, cache);

                string versionStr = getLatest ? "(latest version)" : version;
                if (driverInCache != null && !Config().IsOverride())
                {
                    storeVersionToDownload(version);
                    downloadedVersion = version;
                    Log.Debug("Driver {0} {1} found in cache", GetDriverName(), versionStr);
                    exportDriver(driverInCache);
                }
                else
                {
                    List<System.Uri> candidateUrls = filterCandidateUrls(arch, version, getLatest);
                    if (candidateUrls.Count == 0)
                    {
                        string errorMessage = string.Format("{0} {1} for {2}{3} not found in {4}", GetDriverName(), versionStr, os, arch.ToString(), GetDriverUrl());
                        Log.Error(errorMessage);
                        throw new WebDriverManagerException(errorMessage);
                    }

                    downloadCandidateUrls(candidateUrls);
                }
            }
            catch (Exception e)
            {
                handleException(e, arch, version);
            }
        }

        private string detectDriverVersionFromBrowser()
        {
            string version = "";
            if (Config().IsAvoidAutoVersion())
            {
                return version;
            }

            string driverManagerTypeLowerCase = GetDriverManagerType().ToString().ToLower();
            string optionalBrowserVersion;
            if (usePreferences() && preferences.CheckKeyInPreferences(driverManagerTypeLowerCase))
            {
                optionalBrowserVersion = preferences.GetValueFromPreferences(driverManagerTypeLowerCase);
            }
            else
            {
                optionalBrowserVersion = GetBrowserVersion();
            }

            if (optionalBrowserVersion != null)
            {
                string browserVersion = optionalBrowserVersion;
                Log.Trace("Detected {0} version {1}", GetDriverManagerType(), browserVersion);

                preferenceKey = driverManagerTypeLowerCase + browserVersion;

                if (usePreferences() && preferences.CheckKeyInPreferences(preferenceKey))
                {
                    // Get driver version from preferences
                    version = preferences.GetValueFromPreferences(preferenceKey);
                }
                else
                {
                    // Get driver version from properties
                    version = getVersionForInstalledBrowser(browserVersion);
                }
                if (!string.IsNullOrEmpty(version))
                {
                    Log.Info("Using {0} {1} (since {2} {3} is installed in your machine)", GetDriverName(), version, GetDriverManagerType(), browserVersion);
                    preferences.PutValueInPreferencesIfEmpty(driverManagerTypeLowerCase, browserVersion);
                }
            }
            else
            {
                Log.Debug("The proper {0} version for your {1} is unknown ... trying with the latest", GetDriverName(), GetDriverManagerType());
            }

            return version;
        }

        private bool usePreferences()
        {
            bool usePrefs = !Config().IsAvoidPreferences() && !Config().IsOverride() && !forcedArch && !forcedOs;
            Log.Trace("Using preferences {0}", usePrefs);
            return usePrefs;
        }

        private bool checkPreInstalledVersion(string version)
        {
            if (version.Equals(PRE_INSTALLED))
            {
                string systemRoot = System.Environment.GetEnvironmentVariable("SystemRoot");
                FileInfo microsoftWebDriverFile = new FileInfo(Path.Combine(systemRoot, "System32", "MicrosoftWebDriver.exe"));
                if (microsoftWebDriverFile.Exists)
                {
                    downloadedVersion = PRE_INSTALLED;
                    exportDriver(microsoftWebDriverFile);
                    return true;
                }
                else
                {
                    retry = false;
                    throw new WebDriverManagerException(
                            "MicrosoftWebDriver.exe should be pre-installed in an elevated command prompt executing: "
                                    + "dism /Online /Add-Capability /CapabilityName:Microsoft.WebDriver~~~~0.0.1.0");
                }
            }
            return false;
        }

        private static bool isVersionLatest(string version)
        {
            return string.IsNullOrEmpty(version) || version.Equals("latest", System.StringComparison.InvariantCultureIgnoreCase);
        }

        private string getVersionForInstalledBrowser(string browserVersion)
        {
            string driverVersion = "";
            DriverManagerType driverManagerType = GetDriverManagerType().Value;
            string driverLowerCase = driverManagerType.ToString().ToLower();

            string driverVersionForBrowser = getDriverVersionForBrowserFromProperties(driverLowerCase + browserVersion);
            if (driverVersionForBrowser != null)
            {
                driverVersion = driverVersionForBrowser;
            }
            else
            {
                Log.Debug("The driver version for {0} {1} is unknown ... trying with latest", driverManagerType, browserVersion);
            }
            return driverVersion;
        }

        private string getDriverVersionForBrowserFromProperties(string key)
        {
            bool online = Config().GetVersionsPropertiesOnlineFirst();
            string onlineMessage = online ? ONLINE : LOCAL;
            Log.Debug("Getting driver version for {0} from {1} versions.properties", key, onlineMessage);
            string value = getVersionFromProperties(online).GetProperty(key);
            if (value == null)
            {
                string notOnlineMessage = online ? LOCAL : ONLINE;
                Log.Debug("Driver for {0} not found in {1} properties (using {2} version)", key, onlineMessage, notOnlineMessage);
                versionsProperties = null;
                value = getVersionFromProperties(!online).GetProperty(key);
            }
            return value;
        }

        private Properties getVersionFromProperties(bool online)
        {
            if (versionsProperties != null)
            {
                Log.Trace("Already created versions.properties");
                return versionsProperties;
            }
            else
            {
                try
                {
                    Stream inputStream = getVersionsInputStream(online);
                    versionsProperties = new Properties();
                    versionsProperties.Load(inputStream);
                    inputStream.Close();
                }
                catch (Exception e)
                {
                    versionsProperties = null;
                    throw new IllegalStateException("Cannot read versions.properties", e);
                }
                return versionsProperties;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="online"></param>
        /// <exception cref="IOException" />
        /// <returns></returns>
        private Stream getVersionsInputStream(bool online)
        {
            string onlineMessage = online ? ONLINE : LOCAL;
            Log.Trace("Reading {0} version.properties to find out driver version", onlineMessage);
            Stream inputStream;
            try
            {
                if (online)
                {
                    inputStream = getOnlineVersionsInputStream();
                }
                else
                {
                    inputStream = getLocalVersionsInputStream();
                }
            }
            catch (Exception)
            {
                string exceptionMessage = online ? LOCAL : ONLINE;
                Log.Warn("Error reading version.properties, using {0} instead", exceptionMessage);
                if (online)
                {
                    inputStream = getLocalVersionsInputStream();
                }
                else
                {
                    inputStream = getOnlineVersionsInputStream();
                }
            }
            return inputStream;
        }

        private static Stream getLocalVersionsInputStream()
        {
            Stream inputStream;
            inputStream = File.OpenRead(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources", "versions.properties"));
            return inputStream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        private Stream getOnlineVersionsInputStream()
        {
            return HttpClient.ExecuteHttpGet(Config().GetVersionsPropertiesUrl()).Content.ReadAsStreamAsync().Result;
        }

        protected void handleException(System.Exception e, Architecture arch, string version)
        {
            string versionStr = string.IsNullOrEmpty(version) ? "(latest version)" : version;
            string errorMessage = string.Format("There was an error managing {0} {1} ({2})", GetDriverName(), versionStr, e.Message);
            if (!Config().IsForceCache() && retry)
            {
                Config().SetForceCache(true);
                Config().SetUseMirror(true);
                retry = false;
                Log.Warn("{0} ... trying again using cache and mirror", errorMessage);
                Manage(arch, version);
            }
            else
            {
                Log.Error("{0}", errorMessage, e);
                throw new WebDriverManagerException(e);
            }
        }

        //throws InterruptedException
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"/>
        /// <param name="candidateUrls"></param>
        protected void downloadCandidateUrls(List<System.Uri> candidateUrls)
        {
            Uri url = candidateUrls.First();
            FileInfo exportValue = Downloader.Download(url, VersionToDownload, GetDriverName());
            exportDriver(exportValue);
            downloadedVersion = VersionToDownload;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arch"></param>
        /// <param name="version"></param>
        /// <param name="getLatest"></param>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        protected List<System.Uri> filterCandidateUrls(Architecture arch, string version, bool getLatest)
        {
            List<System.Uri> urls = GetDrivers();
            List<System.Uri> candidateUrls;
            Log.Trace("All System.Uris: {0}", urls);

            bool continueSearchingVersion;
            do
            {
                // Get the latest or concrete version
                candidateUrls = getLatest ? checkLatest(urls, GetDriverName()) : getVersion(urls, GetDriverName(), version);
                Log.Trace("Candidate System.Uris: {0}", candidateUrls);
                if (VersionToDownload == null || this.GetType().Equals(typeof(EdgeDriverManager)))
                {
                    break;
                }

                // Filter by OS
                if (!GetDriverName().Equals("IEDriverServer", System.StringComparison.InvariantCultureIgnoreCase) && !GetDriverName().Equals("selenium-server-standalone", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    candidateUrls = urlFilter.FilterByOs(candidateUrls, Config().GetOs());
                }

                // Filter by architecture
                candidateUrls = urlFilter.FilterByArch(candidateUrls, arch, forcedArch);

                // Filter by distro
                candidateUrls = filterByDistro(candidateUrls);

                // Filter by ignored versions
                candidateUrls = filterByIgnoredVersions(candidateUrls);

                // Find out if driver version has been found or not
                continueSearchingVersion = candidateUrls.Count == 0 && getLatest;
                if (continueSearchingVersion)
                {
                    Log.Info("No binary found for {0} {1} ... seeking another version", GetDriverName(), VersionToDownload);
                    urls = removeFromList(urls, VersionToDownload);
                    VersionToDownload = null;
                }
            } while (continueSearchingVersion);
            return candidateUrls;
        }

        protected List<System.Uri> filterByIgnoredVersions(List<System.Uri> candidateUrls)
        {
            if (Config().GetIgnoreVersions() != null && candidateUrls.Count != 0)
            {
                candidateUrls = urlFilter.FilterByIgnoredVersions(candidateUrls, Config().GetIgnoreVersions());
            }
            return candidateUrls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidateUrls"></param>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected List<System.Uri> filterByDistro(List<System.Uri> candidateUrls)
        {
            // Filter phantomjs 2.5.0 in Linux
            if (Config().GetOs().Equals("linux", System.StringComparison.InvariantCultureIgnoreCase) && GetDriverName().Contains("phantomjs"))
            {
                candidateUrls = urlFilter.FilterByDistro(candidateUrls, "2.5.0");
            }
            return candidateUrls;
        }

        protected FileInfo handleCache(Architecture arch, string version, string os, bool getLatest, bool cache)
        {
            FileInfo driverInCache = null;
            if (cache || !getLatest)
            {
                driverInCache = getDriverFromCache(version, arch, os);
            }
            storeVersionToDownload(version);
            return driverInCache;
        }

        protected FileInfo getDriverFromCache(string driverVersion, Architecture arch, string os)
        {
            Log.Trace("Checking if {0} exists in cache", GetDriverName());
            List<FileInfo> filesInCache = getFilesInCache();
            if (filesInCache.Count > 0)
            {
                // Filter by name
                filesInCache = filterCacheBy(filesInCache, GetDriverName());

                // Filter by version
                filesInCache = filterCacheBy(filesInCache, driverVersion);

                // Filter by OS
                if (!GetDriverName().Equals("msedgedriver"))
                {
                    filesInCache = filterCacheBy(filesInCache, os);
                }

                if (filesInCache.Count == 1)
                {
                    return filesInCache[0];
                }

                // Filter by arch
                filesInCache = filterCacheBy(filesInCache, arch.GetString());

                if (filesInCache != null && filesInCache.Count > 1)
                {
                    return filesInCache[filesInCache.Count() - 1];
                }
            }

            Log.Trace("{0} not found in cache", GetDriverName());
            return null;
        }

        protected static List<FileInfo> filterCacheBy(List<FileInfo> input, string key)
        {
            List<FileInfo> output = new List<FileInfo>(input);
            if (!string.IsNullOrEmpty(key))
            {
                string keyInLowerCase = key.ToLower();
                foreach (FileInfo f in input)
                {
                    if (!f.FullName.ToLower().Contains(keyInLowerCase))
                    {
                        output.Remove(f);
                    }
                }
            }
            Log.Trace("Filter cache by {0} -- input list {1} -- output list {2} ", key, input, output);
            return output;
        }

        protected List<FileInfo> getFilesInCache()
        {
            return new DirectoryInfo(Downloader.GetTargetPath()).GetFiles("*.*", SearchOption.AllDirectories).ToList();
        }

        protected static List<Uri> removeFromList(List<Uri> list, string version)
        {
            List<Uri> outList = new List<Uri>(list);
            foreach (Uri url in list)
            {
                if (url.GetFile().Contains(version))
                {
                    outList.Remove(url);
                }
            }
            return outList;
        }

        protected List<Uri> getVersion(List<Uri> list, string driver, string version)
        {
            List<Uri> outList = new List<Uri>();
            if (GetDriverName().Contains("msedgedriver"))
            {
                int i = ListVersions.IndexOf(version);
                if (i != -1)
                {
                    outList.Add(list[i]);
                }
            }

            foreach (Uri url in list)
            {
                if (url.GetFile().Contains(driver)
                        && url.GetFile().Contains(version)
                        && !url.GetFile().Contains("-symbols"))
                {
                    outList.Add(url);
                }
            }

            if (VersionToDownload != null && !VersionToDownload.Equals(version))
            {
                VersionToDownload = version;
                Log.Info("Using {0} {1}", driver, version);
            }

            return outList;
        }

        protected virtual List<System.Uri> checkLatest(List<System.Uri> list, string driver)
        {
            Log.Trace("Checking the lastest version of {0} with System.Uri list {1}", driver, list);
            List<System.Uri> outList = new List<System.Uri>();
            List<System.Uri> copyOfList = new List<System.Uri>(list);

            foreach (System.Uri url in copyOfList)
            {
                try
                {
                    handleDriver(url, driver, outList);
                }
                catch (System.Exception e)
                {
                    Log.Trace("There was a problem with System.Uri {0} : {1}", url, e.Message);
                    list.Remove(url);
                }
            }
            storeVersionToDownload(VersionToDownload);
            latestVersion = VersionToDownload;
            Log.Info("Latest version of {0} is {1}", driver, VersionToDownload);
            return outList;
        }

        protected void handleDriver(System.Uri url, string driver, List<System.Uri> outList)
        {
            if (!Config().IsUseBetaVersions() && (url.GetFile().ToLower().Contains("beta")))
            {
                return;
            }

            if (url.GetFile().Contains(driver))
            {
                string currentVersion = GetCurrentVersion(url, driver);

                if (currentVersion.Equals(driver, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }
                if (VersionToDownload == null)
                {
                    VersionToDownload = currentVersion;
                }
                if (versionCompare(currentVersion, VersionToDownload) > 0)
                {
                    VersionToDownload = currentVersion;
                    outList.Clear();
                }
                if (url.GetFile().Contains(VersionToDownload))
                {
                    outList.Add(url);
                }
            }
        }

        protected static int versionCompare(string str1, string str2)
        {
            string[] vals1 = str1.Replace("v", "").Split(new string[] { "." }, StringSplitOptions.None);
            string[] vals2 = str2.Replace("v", "").Split(new string[] { "." }, StringSplitOptions.None);

            if (vals1[0].Length == 0)
            {
                vals1[0] = "0";
            }
            if (vals2[0].Length == 0)
            {
                vals2[0] = "0";
            }

            int i = 0;
            while (i < vals1.Length && i < vals2.Length && vals1[i].Equals(vals2[i]))
            {
                i++;
            }

            if (i < vals1.Length && i < vals2.Length)
            {
                return Math.Sign(int.Parse(vals1[i]).CompareTo(int.Parse(vals2[i])));
            }
            else
            {
                return Math.Sign(vals1.Length - vals2.Length);
            }
        }

        /**
         * This method works also for http://npm.taobao.org/ and
         * https://bitbucket.org/ mirrors.
         */
        // throws IOException
        protected List<System.Uri> getDriversFromMirror(System.Uri driverUrl)
        {
            if (mirrorLog)
            {
                Log.Info("Crawling driver list from mirror {0}", driverUrl);
                mirrorLog = true;
            }
            else
            {
                Log.Trace("[Recursive call] Crawling driver list from mirror {0}", driverUrl);
            }

            string driverStr = driverUrl.ToString();

            using (StreamReader inStream = new StreamReader(HttpClient.ExecuteHttpGet(driverUrl).Content.ReadAsStreamAsync().Result))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(inStream.ReadToEnd());
                IEnumerator<HtmlNode> iterator = doc.DocumentNode.SelectNodes("//a").AsEnumerable().GetEnumerator();
                List<System.Uri> urlList = new List<System.Uri>();

                while (iterator.MoveNext())
                {
                    System.Uri link = new System.Uri(driverUrl, iterator.Current.Attributes["href"].Value);
                    if (link.AbsoluteUri.StartsWith(driverStr) && link.AbsoluteUri.EndsWith(SLASH))
                    {
                        urlList.AddRange(getDriversFromMirror(link));
                    }
                    else if (link.AbsoluteUri.StartsWith(driverStr) && !link.AbsoluteUri.Contains("icons") &&
                        (link.AbsoluteUri.ToLower().EndsWith(".bz2")
                        || link.AbsoluteUri.ToLower().EndsWith(".zip")
                        || link.AbsoluteUri.ToLower().EndsWith(".msi")
                        || link.AbsoluteUri.ToLower().EndsWith(".gz")))
                    {
                        urlList.Add(link);
                    }
                }
                return urlList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driverUrl"></param>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        protected List<System.Uri> getDriversFromXml(System.Uri driverUrl)
        {
            Log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());
            List<System.Uri> urls = new List<System.Uri>();
            try
            {
                using (Stream reader = HttpClient.ExecuteHttpGet(driverUrl).Content.ReadAsStreamAsync().Result)
                {
                    XmlDocument xml = loadXML(reader);
                    
                    // skip xml declaration
                    XmlNodeList nodes = xml.FirstChild.NextSibling.SelectNodes("//*[local-name()='Contents']/*[local-name()='Key']");

                    for (int i = 0; i < nodes.Count; ++i)
                    {
                        XmlNode e = nodes[i];
                        urls.Add(new System.Uri(driverUrl.ToString()  + e.InnerText));
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new WebDriverManagerException(e);
            }
            return urls;
        }

        protected static XmlDocument loadXML(Stream reader)
        {
            using (StreamReader stream = new StreamReader(reader))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(stream.ReadToEnd());
                return doc;
            }
        }

        protected void exportDriver(FileInfo variableValue)
        {
            binaryPath = variableValue.FullName;
            string exportParameter = GetExportParameter();
            if (!config.IsAvoidExport() && exportParameter != null)
            {
                // TODO: maybe remove this
                string variableName = exportParameter;
                Log.Info("Exporting {0} as {1}", variableName, variableValue);
                Environment.SetEnvironmentVariable(variableName, variableValue.FullName);

                // Add driver to PATH
                string pathVar = Environment.GetEnvironmentVariable("PATH");
                pathVar += ";" + new FileInfo(variableValue.FullName).DirectoryName;
                Environment.SetEnvironmentVariable("PATH", pathVar);
            }
            else
            {
                Log.Info("Resulting binary {0}", variableValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driverUrl"></param>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        protected Stream openGitHubConnection(Uri driverUrl)
        {
            string gitHubTokenName = Config().GetGitHubTokenName().Trim();
            string gitHubTokenSecret = Config().GetGitHubTokenSecret().Trim();
            AuthenticationHeaderValue authHeader = null;
            if (!string.IsNullOrEmpty(gitHubTokenName) && !string.IsNullOrEmpty(gitHubTokenSecret))
            {
                authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(gitHubTokenName + ":" + gitHubTokenSecret)));
            }

            return HttpClient.ExecuteHttpGet(driverUrl, authHeader).Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected List<Uri> getDriversFromGitHub()
        {
            List<Uri> urls;
            Uri driverUrl = GetDriverUrl();
            Log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());

            System.Uri mirrorUrl = GetMirrorUrl();
            if (mirrorUrl != null && config.IsUseMirror())
            {
                urls = getDriversFromMirror(mirrorUrl);
            }
            else
            {
                string driverVersion = VersionToDownload;

                using (JsonReader reader = new JsonTextReader(new StreamReader(openGitHubConnection(driverUrl))))
                {
                    Release[] releaseArray = new JsonSerializer().Deserialize<Release[]>(reader);

                    if (driverVersion != null)
                    {
                        releaseArray = new Release[]
                        {
                            GetVersion(releaseArray, driverVersion)
                        };
                    }

                    urls = new List<System.Uri>();
                    foreach (Release release in releaseArray)
                    {
                        if (release != null)
                        {
                            List<Asset> assets = release.Assets;
                            foreach (Asset asset in assets)
                            {
                                urls.Add(new System.Uri(asset.BrowserDownloadUrl));
                            }
                        }
                    }
                }
            }
            return urls;
        }

        protected static Release GetVersion(Release[] releaseArray, string version)
        {
            Release outRelease = null;
            foreach (Release release in releaseArray)
            {
                Log.Trace("Get version {0} of {1}", version, release);
                if ((release.Name != null && release.Name.Contains(version)) || (release.TagName != null && release.TagName.Contains(version)))
                {
                    outRelease = release;
                    break;
                }
            }
            return outRelease;
        }

        protected DirectoryInfo[] GetFolderFilter(DirectoryInfo directory)
        {
            return directory.GetDirectories().Where(d => d.Name.ToLower().Contains(GetDriverName())).ToArray();
        }

        protected string GetDefaultBrowserVersion(string[] programFilesEnvs, string winBrowserName, string linuxBrowserName, string macBrowserName, string versionFlag, string browserNameInOutput)
        {

            string browserBinaryPath = Config().GetBinaryPath();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (string programFilesEnv in programFilesEnvs)
                {
                    string browserVersionOutput = getBrowserVersionInWindows(programFilesEnv, winBrowserName, browserBinaryPath);
                    if (!string.IsNullOrEmpty(browserVersionOutput))
                    {
                        return Shell.getVersionFromWmicOutput(browserVersionOutput);
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string browserPath = getPosixBrowserPath(linuxBrowserName, macBrowserName, browserBinaryPath);
                string browserVersionOutput = Shell.runAndWait(browserPath, versionFlag);
                if (!string.IsNullOrEmpty(browserVersionOutput))
                {
                    return Shell.getVersionFromPosixOutput(browserVersionOutput, browserNameInOutput);
                }
            }
            return null;
        }

        private static string getPosixBrowserPath(string linuxBrowserName, string macBrowserName, string browserBinaryPath)
        {
            if (!string.IsNullOrEmpty(browserBinaryPath))
            {
                return browserBinaryPath;
            }
            else
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? linuxBrowserName : macBrowserName;
            }
        }

        private static string getBrowserVersionInWindows(string programFilesEnv, string winBrowserName, string browserBinaryPath)
        {
            string programFiles = System.Environment.GetEnvironmentVariable(programFilesEnv).Replace("\\", "\\\\");
            string browserPath = string.IsNullOrEmpty(browserBinaryPath)
                    ? programFiles + winBrowserName
                    : browserBinaryPath;
            return Shell.runAndWait(getExecFile(), "wmic.exe", "datafile", "where", "name='" + browserPath + "'", "get", "Version", "/value");
        }

        protected static DirectoryInfo getExecFile()
        {
            string systemRoot = System.Environment.GetEnvironmentVariable("SystemRoot");
            DirectoryInfo system32 = new DirectoryInfo(Path.Combine(systemRoot, "System32", "wbem"));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && system32.Exists)
            {
                return system32;
            }
            return new DirectoryInfo(Directory.GetCurrentDirectory());
        }

        protected virtual string getLatestVersion()
        {
            return null;
        }

        protected void reset()
        {
            Config().Reset();
            mirrorLog = false;
            ListVersions = null;
            VersionToDownload = null;
            forcedArch = false;
            forcedOs = false;
            retry = true;
            isLatest = true;
        }

        protected static string getProgramFilesEnv()
        {
            // TODO: use RuntimeInformation.OSArchitecture?
            return RuntimeInformation.ProcessArchitecture.ToString().Contains("64")
                ? "PROGRAMFILES(X86)"
                : "PROGRAMFILES";
        }

        protected static string getOtherProgramFilesEnv()
        {
            // TODO: use RuntimeInformation.OSArchitecture?
            return RuntimeInformation.ProcessArchitecture.ToString().Contains("64")
                ? "PROGRAMFILES"
                : "PROGRAMFILES(X86)";
        }

        protected Uri getDriverUrlCheckingMirror(Uri url)
        {
            if (Config().IsUseMirror())
            {
                Uri mirrorUrl = GetMirrorUrl();
                if (mirrorUrl != null)
                {
                    return mirrorUrl;
                }
            }

            return url;
        }

        public static void main(string[] args)
        {
            string validBrowsers = "chrome|firefox|opera|edge|phantomjs|iexplorer|selenium_server_standalone";
            if (args.Length <= 0)
            {
                logCliError(validBrowsers);
            }
            else
            {
                string arg = args[0];
                if (arg.Equals("server", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    startServer(args);
                }
                else if (arg.Equals("clear-preferences", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    new Preferences(new Config()).Clear();
                }
                else
                {
                    resolveLocal(validBrowsers, arg);
                }
            }
        }

        private static void resolveLocal(string validBrowsers, string arg)
        {
            Log.Info("using WebDriverManagerSharp to resolve {0}", arg);
            try
            {
                DriverManagerType driverManagerType = (DriverManagerType)System.Enum.Parse(typeof(DriverManagerType), arg.ToUpper());
                WebDriverManager wdm = WebDriverManager.GetInstance(driverManagerType).AvoidExport().TargetPath(".").ForceDownload();
                if (arg.Equals("edge", System.StringComparison.InvariantCultureIgnoreCase) || arg.Equals("iexplorer", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    wdm.OperatingSystem(WebDriverManagerSharp.OperatingSystem.WIN);
                }
                wdm.AvoidOutputTree().Setup();
            }
            catch (Exception)
            {
                Log.Error("Driver for {0} not found (valid browsers {1})", arg, validBrowsers);
            }
        }

        private static void startServer(string[] args)
        {
            int port;
            if (args.Length < 2 || !int.TryParse(args[1], out port))
            {
                port = new Config().GetServerPort();
            }

            new NancyHost(
                new HostConfiguration()
                {
                    UrlReservations = new UrlReservations()
                    {
                        CreateAutomatically = true
                    }
                },
                new Uri("http://localhost:" + port)).Start();

            Log.Info("WebDriverManager server listening on port {0}", port);
        }

        private static void logCliError(string validBrowsers)
        {
            Log.Error("There are 3 options to run WebDriverManager CLI");
            Log.Error("1. WebDriverManager used to resolve binary drivers locally:");
            Log.Error("\tWebDriverManager browserName");
            Log.Error("\t(where browserName={0})", validBrowsers);

            Log.Error("2. WebDriverManager as a server:");
            Log.Error("\tWebDriverManager server <port>");
            Log.Error("\t(where default port is 4041)");

            Log.Error("3. To clear previously resolved driver versions (as Java preferences):");
            Log.Error("\tWebDriverManager clear-preferences");
        }

        private void storeVersionToDownload(string version)
        {
            if (!string.IsNullOrEmpty(version))
            {
                if (version.StartsWith("."))
                {
                    version = version.SubstringJava(1);
                }
                VersionToDownload = version;
                if (isLatest && usePreferences() && !string.IsNullOrEmpty(preferenceKey))
                {
                    preferences.PutValueInPreferencesIfEmpty(preferenceKey, version);
                }
            }
        }

        private void setConfig(Config config)
        {
            this.config = config;
        }
    }
}