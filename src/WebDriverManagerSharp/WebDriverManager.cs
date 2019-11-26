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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Autofac;
    using HtmlAgilityPack;
    using Nancy.Hosting.Self;
    using Newtonsoft.Json;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Exceptions;
    using WebDriverManagerSharp.GitHubApi;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Managers;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Web;

    /**
     * Parent driver manager.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.0
     */
    public abstract class WebDriverManager
    {
        protected const string SLASH = "/";
        protected const string PRE_INSTALLED = "pre-installed";
        protected const string BETA = "beta";
        protected const string ONLINE = "online";
        protected const string LOCAL = "local";

        private static readonly ISystemInformation systemInformation = new SystemInformation();

        private static readonly Dictionary<DriverManagerType, WebDriverManager> instanceMap = new Dictionary<DriverManagerType, WebDriverManager>();

        private IHttpClient httpClient;
        private IDownloader downloader;
        private UrlFilter urlFilter;
        private string versionToDownload;
        private string downloadedVersion;
        private string latestVersion;
        private string binaryPath;
        private bool mirrorLog;
        private List<string> listVersions;
        private bool forcedArch;
        private bool forcedOs;
        private bool isLatest;
        private bool retry = true;

        private IConfig config;
        private readonly IShell shell;
        private readonly IPreferences preferences;
        private readonly ILogger logger;
        private readonly IFileStorage fileStorage;

        private string preferenceKey;
        private Properties versionsProperties;

        protected WebDriverManager(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
        {
            this.config = config;
            this.shell = shell;
            this.preferences = preferences;
            this.logger = logger;
            this.fileStorage = fileStorage;
        }

        protected ILogger Log { get { return logger; } }

        protected IShell Shell { get { return shell; } }

        protected IDownloader Downloader { get { return downloader; } }

        public IHttpClient HttpClient { get { return httpClient; } protected set { httpClient = value; } } // setter only for edge driver

        protected string VersionToDownload { get { return versionToDownload; } set { versionToDownload = value; } } // setter only for edge driver

        protected List<string> ListVersions { get { return listVersions; } set { listVersions = value; } } // setter only for edge driver

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

        protected virtual Uri GetMirrorUrl()
        {
            return null;
        }

        protected abstract string GetExportParameter();

        public static IConfig GlobalConfig()
        {
            IConfig global = Resolver.Resolve<IConfig>();
            global.SetAvoidAutoReset(true);
            foreach (DriverManagerType type in Enum.GetValues(typeof(DriverManagerType)))
            {
                GetInstance(type).setConfig(global);
            }

            return global;
        }

        public IConfig Config()
        {
            return config;
        }

        public static WebDriverManager ChromeDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.CHROME))
            {
                instanceMap.Add(DriverManagerType.CHROME, Resolver.Resolve<ChromeDriverManager>());
            }

            return instanceMap[DriverManagerType.CHROME];
        }

        public static WebDriverManager FirefoxDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.FIREFOX))
            {
                instanceMap.Add(DriverManagerType.FIREFOX, Resolver.Resolve<FirefoxDriverManager>());
            }

            return instanceMap[DriverManagerType.FIREFOX];
        }

        public static WebDriverManager OperaDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.OPERA))
            {
                instanceMap.Add(DriverManagerType.OPERA, Resolver.Resolve<OperaDriverManager>());
            }

            return instanceMap[DriverManagerType.OPERA];
        }

        public static WebDriverManager EdgeDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.EDGE))
            {
                instanceMap.Add(DriverManagerType.EDGE, Resolver.Resolve<EdgeDriverManager>());
            }

            return instanceMap[DriverManagerType.EDGE];
        }

        public static WebDriverManager IEDriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.IEXPLORER))
            {
                instanceMap.Add(DriverManagerType.IEXPLORER, Resolver.Resolve<InternetExplorerDriverManager>());
            }

            return instanceMap[DriverManagerType.IEXPLORER];
        }

        public static WebDriverManager PhantomJS()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.PHANTOMJS))
            {
                instanceMap.Add(DriverManagerType.PHANTOMJS, Resolver.Resolve<PhantomJsDriverManager>());
            }

            return instanceMap[DriverManagerType.PHANTOMJS];
        }

        public static WebDriverManager SeleniumServerStandalone()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.SELENIUM_SERVER_STANDALONE))
            {
                instanceMap.Add(DriverManagerType.SELENIUM_SERVER_STANDALONE, Resolver.Resolve<SeleniumServerStandaloneManager>());
            }

            return instanceMap[DriverManagerType.SELENIUM_SERVER_STANDALONE];
        }

        protected static WebDriverManager VoidDriver()
        {
            return Resolver.Resolve<VoidDriverManager>();
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
            if (driverType == null)
            {
                throw new ArgumentNullException(nameof(driverType));
            }

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
            Architecture(Enums.Architecture.X32);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager Arch64()
        {
            Architecture(Enums.Architecture.X64);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager OperatingSystem(Enums.OperatingSystem os)
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

        public WebDriverManager DriverRepositoryUrl(Uri url)
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

        public WebDriverManager LocalRepositoryPassword(string localRepositoryPassword)
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
            httpClient = Resolver.Resolve<IHttpClient>(new Autofac.NamedParameter("config", Config()));
            try
            {
                List<Uri> drivers = GetDrivers();
                List<string> versions = new List<string>();
                foreach (Uri url in drivers)
                {
                    string version = GetCurrentVersion(url, GetDriverName());
                    if (string.IsNullOrEmpty(version) || version.Equals("icons", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    if (version.StartsWith(".", StringComparison.OrdinalIgnoreCase))
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

        public void ClearPreferences()
        {
            instanceMap[GetDriverManagerType().Value].preferences.Clear();
        }

        public void ClearCache()
        {
            IDirectory targetPath = new Storage.Directory(Config().GetTargetPath());
            try
            {
                Log.Debug("Clearing cache at {0}", targetPath);
                targetPath.Delete(true);
            }
            catch (Exception e)
            {
                Log.Warn("Exception deleting cache at {0}", targetPath, e);
            }
        }

        public static void ClearDrivers()
        {
            instanceMap.Clear();
        }

        // ------------

        public virtual string PreDownload(string target, string version)
        {
            Log.Trace("Pre-download. target={0}, version={1}", target, version);
            return target;
        }

        public virtual IFile PostDownload(IFile archive)
        {
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            IDirectory parentFolder = archive.ParentDirectory;
            IReadOnlyList<IFile> ls = parentFolder.Files;
            foreach (IFile f in ls)
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
            string currentVersion = string.Empty;
            try
            {
                currentVersion = url.GetFile().SubstringJava(
                        url.GetFile().IndexOf(SLASH, StringComparison.OrdinalIgnoreCase) + 1,
                        url.GetFile().LastIndexOf(SLASH, StringComparison.OrdinalIgnoreCase));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.Trace("Exception getting version of System.Uri {0} ({1})", url, e.Message);
            }

            return currentVersion;
        }

        protected void Manage(Architecture arch, string version)
        {
            httpClient = Resolver.Resolve<IHttpClient>(new Autofac.NamedParameter("config", Config()));
            try
            {
                downloader = Resolver.Resolve<IDownloader>(new NamedParameter("driverManagerType", GetDriverManagerType().Value));
                urlFilter = new UrlFilter(logger, fileStorage);

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

                IFile driverInCache = handleCache(arch, version, os, getLatest, cache);

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
                    List<Uri> candidateUrls = filterCandidateUrls(arch, version, getLatest);
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
            string version = string.Empty;
            if (Config().IsAvoidAutoVersion())
            {
                return version;
            }

            string driverManagerTypeLowerCase = GetDriverManagerType().ToString().ToLower(CultureInfo.InvariantCulture);
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
            if (version.Equals(PRE_INSTALLED, StringComparison.OrdinalIgnoreCase))
            {
                string systemRoot = Environment.GetEnvironmentVariable("SystemRoot");
                IFile microsoftWebDriverFile = new Storage.File(Path.Combine(systemRoot, "System32", "MicrosoftWebDriver.exe"));
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
            return string.IsNullOrEmpty(version) || version.Equals("latest", StringComparison.InvariantCultureIgnoreCase);
        }

        private string getVersionForInstalledBrowser(string browserVersion)
        {
            string driverVersion = string.Empty;
            DriverManagerType driverManagerType = GetDriverManagerType().Value;
            string driverLowerCase = driverManagerType.ToString().ToLower(CultureInfo.InvariantCulture);

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
            bool online = Config().IsVersionsPropertiesOnlineFirst();
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

        private Stream getLocalVersionsInputStream()
        {
            Stream inputStream;
            inputStream = fileStorage.OpenRead(Path.Combine(fileStorage.GetCurrentDirectory(), "Resources", "versions.properties"));
            return inputStream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        private Stream getOnlineVersionsInputStream()
        {
            return HttpClient.ExecuteHttpGet(Config().GetVersionsPropertiesUrl());
        }

        protected void handleException(Exception e, Architecture arch, string version)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            string versionStr = string.IsNullOrEmpty(version) ? "(latest version)" : version;
            string errorMessage = string.Format(CultureInfo.InvariantCulture, "There was an error managing {0} {1} ({2})", GetDriverName(), versionStr, e.Message);
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

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"/>
        /// <param name="candidateUrls"></param>
        protected void downloadCandidateUrls(List<Uri> candidateUrls)
        {
            Uri url = candidateUrls.First();
            IFile exportValue = downloader.Download(url, VersionToDownload, GetDriverName());
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
        protected List<Uri> filterCandidateUrls(Architecture arch, string version, bool getLatest)
        {
            List<Uri> urls = GetDrivers();
            List<Uri> candidateUrls;
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
            }
            while (continueSearchingVersion);

            return candidateUrls;
        }

        protected List<Uri> filterByIgnoredVersions(List<Uri> candidateUrls)
        {
            if (candidateUrls == null)
            {
                throw new ArgumentNullException(nameof(candidateUrls));
            }

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

        protected IFile handleCache(Architecture arch, string version, string os, bool getLatest, bool cache)
        {
            IFile driverInCache = null;
            if (cache || !getLatest)
            {
                driverInCache = getDriverFromCache(version, arch, os);
            }

            storeVersionToDownload(version);
            return driverInCache;
        }

        protected IFile getDriverFromCache(string driverVersion, Architecture arch, string os)
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
                if (!GetDriverName().Equals("msedgedriver", StringComparison.OrdinalIgnoreCase))
                {
                    filesInCache = filterCacheBy(filesInCache, os);
                }

                if (filesInCache.Count == 1)
                {
                    return new Storage.File(filesInCache[0].FullName);
                }

                // Filter by arch
                filesInCache = filterCacheBy(filesInCache, arch.GetString());

                if (filesInCache != null && filesInCache.Count > 1)
                {
                    return new Storage.File(filesInCache[filesInCache.Count - 1].FullName);
                }
            }

            Log.Trace("{0} not found in cache", GetDriverName());
            return null;
        }

        protected List<FileInfo> filterCacheBy(List<FileInfo> input, string key)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            List<FileInfo> output = new List<FileInfo>(input);
            if (!string.IsNullOrEmpty(key))
            {
                foreach (FileInfo f in input)
                {
                    if (f.FullName.IndexOf(key, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        output.Remove(f);
                    }
                }
            }

            logger.Trace("Filter cache by {0} -- input list {1} -- output list {2} ", key, input, output);
            return output;
        }

        protected List<FileInfo> getFilesInCache()
        {
            return fileStorage.GetFileInfos(downloader.GetTargetPath(), "*.*", SearchOption.AllDirectories).ToList();
        }

        protected static List<Uri> removeFromList(List<Uri> list, string version)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

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
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            List<Uri> outList = new List<Uri>();
            if (GetDriverName().Contains("msedgedriver"))
            {
                int i = listVersions.IndexOf(version);
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

            if (VersionToDownload != null && !VersionToDownload.Equals(version, StringComparison.OrdinalIgnoreCase))
            {
                VersionToDownload = version;
                Log.Info("Using {0} {1}", driver, version);
            }

            return outList;
        }

        protected virtual List<Uri> checkLatest(List<Uri> list, string driver)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            Log.Trace("Checking the lastest version of {0} with System.Uri list {1}", driver, list);
            List<Uri> outList = new List<Uri>();
            List<Uri> copyOfList = new List<Uri>(list);

            foreach (Uri url in copyOfList)
            {
                try
                {
                    handleDriver(url, driver, outList);
                }
                catch (Exception e)
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

        protected void handleDriver(Uri url, string driver, List<Uri> outList)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (outList == null)
            {
                throw new ArgumentNullException(nameof(outList));
            }

            if (!Config().IsUseBetaVersions() && url.GetFile().IndexOf("beta", StringComparison.OrdinalIgnoreCase) != -1)
            {
                return;
            }

            if (url.GetFile().Contains(driver))
            {
                string currentVersion = GetCurrentVersion(url, driver);

                if (currentVersion.Equals(driver, StringComparison.InvariantCultureIgnoreCase))
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
            if (str1 == null)
            {
                throw new ArgumentNullException(nameof(str1));
            }

            if (str2 == null)
            {
                throw new ArgumentNullException(nameof(str2));
            }

            string[] vals1 = str1.Replace("v", string.Empty).Split(new string[] { "." }, StringSplitOptions.None);
            string[] vals2 = str2.Replace("v", string.Empty).Split(new string[] { "." }, StringSplitOptions.None);

            if (vals1[0].Length == 0)
            {
                vals1[0] = "0";
            }

            if (vals2[0].Length == 0)
            {
                vals2[0] = "0";
            }

            int i = 0;
            while (i < vals1.Length && i < vals2.Length && vals1[i].Equals(vals2[i], StringComparison.OrdinalIgnoreCase))
            {
                i++;
            }

            if (i < vals1.Length && i < vals2.Length)
            {
                return Math.Sign(int.Parse(vals1[i], CultureInfo.InvariantCulture).CompareTo(int.Parse(vals2[i], CultureInfo.InvariantCulture)));
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
        protected List<Uri> getDriversFromMirror(Uri driverUrl)
        {
            if (driverUrl == null)
            {
                throw new ArgumentNullException(nameof(driverUrl));
            }

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

            using (StreamReader inStream = new StreamReader(HttpClient.ExecuteHttpGet(driverUrl)))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(inStream.ReadToEnd());
                IEnumerator<HtmlNode> iterator = doc.DocumentNode.SelectNodes("//a").AsEnumerable().GetEnumerator();
                List<Uri> urlList = new List<Uri>();

                while (iterator.MoveNext())
                {
                    Uri link = new Uri(driverUrl, iterator.Current.Attributes["href"].Value);
                    if (link.AbsoluteUri.StartsWith(driverStr, StringComparison.OrdinalIgnoreCase) && link.AbsoluteUri.EndsWith(SLASH, StringComparison.OrdinalIgnoreCase))
                    {
                        urlList.AddRange(getDriversFromMirror(link));
                    }
                    else if (link.AbsoluteUri.StartsWith(driverStr, StringComparison.OrdinalIgnoreCase)
                        && !link.AbsoluteUri.Contains("icons")
                        && (link.AbsoluteUri.EndsWith(".bz2", StringComparison.OrdinalIgnoreCase)
                        || link.AbsoluteUri.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                        || link.AbsoluteUri.EndsWith(".msi", StringComparison.OrdinalIgnoreCase)
                        || link.AbsoluteUri.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)))
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
        protected List<Uri> getDriversFromXml(Uri driverUrl)
        {
            if (driverUrl == null)
            {
                throw new ArgumentNullException(nameof(driverUrl));
            }

            Log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());
            List<Uri> urls = new List<Uri>();
            try
            {
                using (Stream reader = HttpClient.ExecuteHttpGet(driverUrl))
                {
                    XmlDocument xml = loadXML(reader);

                    // skip xml declaration
                    XmlNodeList nodes = xml.FirstChild.NextSibling.SelectNodes("//*[local-name()='Contents']/*[local-name()='Key']");

                    for (int i = 0; i < nodes.Count; ++i)
                    {
                        XmlNode e = nodes[i];
                        urls.Add(new Uri(driverUrl.ToString()  + e.InnerText));
                    }
                }
            }
            catch (Exception e)
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

        protected void exportDriver(IFile variableValue)
        {
            if (variableValue == null)
            {
                throw new ArgumentNullException(nameof(variableValue));
            }

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
                pathVar += ";" + new Storage.File(variableValue.FullName).ParentDirectory.FullName;
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

            return HttpClient.ExecuteHttpGet(driverUrl, authHeader);
        }

        /// <summary>
        /// Get driver Uris from GitHub
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected List<Uri> getDriversFromGitHub()
        {
            List<Uri> urls;
            Uri driverUrl = GetDriverUrl();
            Log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());

            Uri mirrorUrl = GetMirrorUrl();
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

                    urls = new List<Uri>();
                    foreach (Release release in releaseArray)
                    {
                        if (release != null)
                        {
                            Asset[] assets = release.Assets;
                            foreach (Asset asset in assets)
                            {
                                urls.Add(new Uri(asset.BrowserDownloadUrl));
                            }
                        }
                    }
                }
            }

            return urls;
        }

        protected Release GetVersion(Release[] releaseArray, string version)
        {
            if (releaseArray == null)
            {
                throw new ArgumentNullException(nameof(releaseArray));
            }

            Release outRelease = null;
            foreach (Release release in releaseArray)
            {
                logger.Trace("Get version {0} of {1}", version, release);
                if ((release.Name != null && release.Name.Contains(version)) || (release.TagName != null && release.TagName.Contains(version)))
                {
                    outRelease = release;
                    break;
                }
            }

            return outRelease;
        }

        protected IDirectory[] GetFolderFilter(IDirectory directory)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            return directory.ChildDirectories.Where(d => d.Name.IndexOf(GetDriverName(), StringComparison.OrdinalIgnoreCase) != -1).ToArray();
        }

        protected string GetDefaultBrowserVersion(string[] programFilesEnvs, string winBrowserName, string linuxBrowserName, string macBrowserName, string versionFlag, string browserNameInOutput)
        {
            if (programFilesEnvs == null)
            {
                throw new ArgumentNullException(nameof(programFilesEnvs));
            }

            string browserBinaryPath = Config().GetBinaryPath();
            if (systemInformation.OperatingSystem == Enums.OperatingSystem.WIN)
            {
                foreach (string programFilesEnv in programFilesEnvs)
                {
                    string browserVersionOutput = getBrowserVersionInWindows(programFilesEnv, winBrowserName, browserBinaryPath);
                    if (!string.IsNullOrEmpty(browserVersionOutput))
                    {
                        return Shell.GetVersionFromWmicOutput(browserVersionOutput);
                    }
                }
            }
            else if (systemInformation.OperatingSystem == Enums.OperatingSystem.LINUX || systemInformation.OperatingSystem == Enums.OperatingSystem.MAC)
            {
                string browserPath = getPosixBrowserPath(linuxBrowserName, macBrowserName, browserBinaryPath);
                string browserVersionOutput = shell.RunAndWait(browserPath, versionFlag);
                if (!string.IsNullOrEmpty(browserVersionOutput))
                {
                    return shell.GetVersionFromPosixOutput(browserVersionOutput, browserNameInOutput);
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
                return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) ? linuxBrowserName : macBrowserName;
            }
        }

        private string getBrowserVersionInWindows(string programFilesEnv, string winBrowserName, string browserBinaryPath)
        {
            string programFiles = Environment.GetEnvironmentVariable(programFilesEnv).Replace("\\", "\\\\");
            string browserPath = string.IsNullOrEmpty(browserBinaryPath)
                    ? programFiles + winBrowserName
                    : browserBinaryPath;
            return shell.RunAndWait(getExecFile(), "wmic.exe", "datafile", "where", "name='" + browserPath + "'", "get", "Version", "/value");
        }

        protected DirectoryInfo getExecFile()
        {
            string systemRoot = Environment.GetEnvironmentVariable("SystemRoot");
            DirectoryInfo system32 = new DirectoryInfo(Path.Combine(systemRoot, "System32", "wbem"));
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) && system32.Exists)
            {
                return system32;
            }

            return new DirectoryInfo(fileStorage.GetCurrentDirectory());
        }

        protected virtual string getLatestVersion()
        {
            return null;
        }

        protected void reset()
        {
            Config().Reset();
            mirrorLog = false;
            listVersions = null;
            versionToDownload = null;
            forcedArch = false;
            forcedOs = false;
            retry = true;
            isLatest = true;
        }

        protected static string getProgramFilesEnv()
        {
            // TODO: use RuntimeInformation.OSArchitecture?
            return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().Contains("64")
                ? "PROGRAMFILES(X86)"
                : "PROGRAMFILES";
        }

        protected static string getOtherProgramFilesEnv()
        {
            // TODO: use RuntimeInformation.OSArchitecture?
            return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().Contains("64")
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

        private void storeVersionToDownload(string version)
        {
            if (!string.IsNullOrEmpty(version))
            {
                if (version.StartsWith(".", StringComparison.OrdinalIgnoreCase))
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

        private void setConfig(IConfig config)
        {
            this.config = config;
        }
    }
}