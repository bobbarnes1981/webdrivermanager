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
using WebDriverManager.GitHubApi;

namespace WebDriverManager
{
    /**
     * Parent driver manager.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 2.1.0
     */
    public abstract class WebDriverManager
    {
        protected static ILogger log = Logger.GetLogger();

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

        protected static Dictionary<DriverManagerType, WebDriverManager> instanceMap = new Dictionary<DriverManagerType, WebDriverManager>();

        protected HttpClient httpClient;
        protected Downloader downloader;
        protected UrlFilter urlFilter;
        protected string versionToDownload;
        protected string downloadedVersion;
        protected string latestVersion;
        protected string binaryPath;
        protected bool mirrorLog;
        protected List<string> listVersions;
        protected bool forcedArch;
        protected bool forcedOs;
        protected bool isLatest;
        protected bool retry = true;
        protected Config config = new Config();
        //protected Preferences preferences = new Preferences(config);
        protected Preferences preferences;
        protected string preferenceKey;
        protected Properties versionsProperties;

        protected WebDriverManager()
        {
            preferences = new Preferences(config);
        }

        public static Config globalConfig()
        {
            Config global = new Config();
            global.setAvoidAutoReset(true);
            foreach (DriverManagerType type in Enum.GetValues(typeof(DriverManagerType)))
            {
                getInstance(type).setConfig(global);
            }
            return global;
        }

        public Config Config()
        {
            return config;
        }

        public static WebDriverManager chromedriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.CHROME))
            {
                instanceMap.Add(DriverManagerType.CHROME, new ChromeDriverManager());
            }
            return instanceMap[DriverManagerType.CHROME];
        }

        public static WebDriverManager firefoxdriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.FIREFOX))
            {
                instanceMap.Add(DriverManagerType.FIREFOX, new FirefoxDriverManager());
            }
            return instanceMap[DriverManagerType.FIREFOX];
        }

        public static WebDriverManager operadriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.OPERA))
            {
                instanceMap.Add(DriverManagerType.OPERA, new OperaDriverManager());
            }
            return instanceMap[DriverManagerType.OPERA];
        }

        public static WebDriverManager edgedriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.EDGE))
            {
                instanceMap.Add(DriverManagerType.EDGE, new EdgeDriverManager());
            }
            return instanceMap[DriverManagerType.EDGE];
        }

        public static WebDriverManager iedriver()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.IEXPLORER))
            {
                instanceMap.Add(DriverManagerType.IEXPLORER, new InternetExplorerDriverManager());
            }
            return instanceMap[DriverManagerType.IEXPLORER];
        }

        public static WebDriverManager phantomjs()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.PHANTOMJS))
            {
                instanceMap.Add(DriverManagerType.PHANTOMJS, new PhantomJsDriverManager());
            }
            return instanceMap[DriverManagerType.PHANTOMJS];
        }

        public static WebDriverManager seleniumServerStandalone()
        {
            if (!instanceMap.ContainsKey(DriverManagerType.SELENIUM_SERVER_STANDALONE))
            {
                instanceMap.Add(DriverManagerType.SELENIUM_SERVER_STANDALONE, new SeleniumServerStandaloneManager());
            }
            return instanceMap[DriverManagerType.SELENIUM_SERVER_STANDALONE];
        }

        protected static WebDriverManager voiddriver()
        {
            return new VoidDriverManager();
        }

        public static WebDriverManager getInstance(DriverManagerType driverManagerType)
        {
            switch (driverManagerType)
            {
                case DriverManagerType.CHROME:
                    return chromedriver();
                case DriverManagerType.FIREFOX:
                    return firefoxdriver();
                case DriverManagerType.OPERA:
                    return operadriver();
                case DriverManagerType.IEXPLORER:
                    return iedriver();
                case DriverManagerType.EDGE:
                    return edgedriver();
                case DriverManagerType.PHANTOMJS:
                    return phantomjs();
                case DriverManagerType.SELENIUM_SERVER_STANDALONE:
                    return seleniumServerStandalone();
                default:
                    return voiddriver();
            }
        }

        public static WebDriverManager getInstance<T>()
        {
            switch (typeof(T).Name)
            {
                case "org.openqa.selenium.chrome.ChromeDriver":
                    return chromedriver();
                case "org.openqa.selenium.firefox.FirefoxDriver":
                    return firefoxdriver();
                case "org.openqa.selenium.opera.OperaDriver":
                    return operadriver();
                case "org.openqa.selenium.ie.InternetExplorerDriver":
                    return iedriver();
                case "org.openqa.selenium.edge.EdgeDriver":
                    return edgedriver();
                case "org.openqa.selenium.phantomjs.PhantomJSDriver":
                    return phantomjs();
                default:
                    return voiddriver();
            }
        }

        public void setup()
        {
            if (GetDriverManagerType() != null)
            {
                try
                {
                    Architecture architecture = Config().getArchitecture();
                    string driverVersion = GetDriverVersion();
                    isLatest = isVersionLatest(driverVersion);
                    manage(architecture, driverVersion);
                }
                finally
                {
                    if (!Config().isAvoidAutoReset())
                    {
                        reset();
                    }
                }
            }
        }

        public WebDriverManager version(string version)
        {
            SetDriverVersion(version);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager architecture(Architecture architecture)
        {
            Config().setArchitecture(architecture);
            forcedArch = true;
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager arch32()
        {
            architecture(Architecture.X32);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager arch64()
        {
            architecture(Architecture.X64);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager operatingSystem(OperatingSystem os)
        {
            Config().setOs(os.ToString());
            forcedOs = true;
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager forceCache()
        {
            Config().setForceCache(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager forceDownload()
        {
            Config().setOverride(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager driverRepositoryUrl(System.Uri url)
        {
            SetDriverUrl(url);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager useMirror()
        {
            System.Uri mirrorUrl = GetMirrorUrl();
            if (mirrorUrl != null)
            {
                throw new WebDriverManagerException("Mirror System.Uri not available");
            }
            Config().setUseMirror(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager proxy(string proxy)
        {
            Config().setProxy(proxy);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager proxyUser(string proxyUser)
        {
            Config().setProxyUser(proxyUser);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager proxyPass(string proxyPass)
        {
            Config().setProxyPass(proxyPass);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager useBetaVersions()
        {
            Config().setUseBetaVersions(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager ignoreVersions(params string[] versions)
        {
            Config().setIgnoreVersions(versions);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager gitHubTokenSecret(string gitHubTokenSecret)
        {
            Config().setGitHubTokenSecret(gitHubTokenSecret);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager gitHubTokenName(string gitHubTokenName)
        {
            Config().setGitHubTokenName(gitHubTokenName);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager localRepositoryUser(string localRepositoryUser)
        {
            Config().setLocalRepositoryUser(localRepositoryUser);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager localRepositoryPassword(
                string localRepositoryPassword)
        {
            Config().setLocalRepositoryPassword(localRepositoryPassword);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager timeout(int timeout)
        {
            Config().setTimeout(timeout);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager properties(string properties)
        {
            Config().setProperties(properties);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager targetPath(string targetPath)
        {
            Config().setTargetPath(targetPath);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager avoidExport()
        {
            Config().setAvoidExport(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager avoidOutputTree()
        {
            Config().setAvoidOutputTree(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager avoidAutoVersion()
        {
            Config().setAvoidAutoVersion(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager avoidPreferences()
        {
            Config().setAvoidPreferences(true);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager ttl(int seconds)
        {
            Config().setTtl(seconds);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager browserPath(string browserPath)
        {
            Config().setBinaryPath(browserPath);
            return instanceMap[GetDriverManagerType().Value];
        }

        public WebDriverManager useLocalVersionsPropertiesFirst()
        {
            Config().setVersionsPropertiesOnlineFirst(false);
            return instanceMap[GetDriverManagerType().Value];
        }

        // ------------

        public string getBinaryPath()
        {
            return instanceMap[GetDriverManagerType().Value].binaryPath;
        }

        public string getDownloadedVersion()
        {
            return instanceMap[GetDriverManagerType().Value].downloadedVersion;
        }

        public virtual List<string> getVersions()
        {
            httpClient = new HttpClient(Config());
            try
            {
                List<System.Uri> drivers = GetDrivers();
                List<string> versions = new List<string>();
                foreach (System.Uri url in drivers)
                {
                    string version = getCurrentVersion(url, GetDriverName());
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
                log.Trace("Version list before sorting {0}", versions);
                versions.Sort(new VersionComparator());
                return versions;
            }
            catch (IOException e)
            {
                throw new WebDriverManagerException(e);
            }
        }

        public void clearPreferences()
        {
            instanceMap[GetDriverManagerType().Value].preferences.clear();
        }

        public void clearCache()
        {
            string targetPath = Config().getTargetPath();
            try
            {
                log.Debug("Clearing cache at {0}", targetPath);
                Directory.Delete(targetPath);
            }
            catch (System.Exception e)
            {
                log.Warn("Exception deleting cache at {0}", targetPath, e);
            }
        }

        // ------------

        public virtual string preDownload(string target, string version)
        {
            log.Trace("Pre-download. target={0}, version={1}", target, version);
            return target;
        }

        public virtual FileInfo postDownload(FileInfo archive)
        {
            DirectoryInfo parentFolder = archive.Directory;
            FileInfo[] ls = parentFolder.GetFiles();
            foreach (FileInfo f in ls)
            {
                if (GetDriverName().Contains(f.Name.Substring(0, f.Name.Length - f.Extension.Length)))
                {
                    log.Trace("Found binary in post-download: {0}", f);
                    return f;
                }
            }
            throw new WebDriverManagerException("Driver " + GetDriverName() + " not found (using temporal folder " + parentFolder + ")");
        }

        protected virtual string getCurrentVersion(System.Uri url, string driverName)
        {
            string currentVersion = "";
            try
            {
                currentVersion = url.GetFile().SubstringJava(
                        url.GetFile().IndexOf(SLASH) + 1,
                        url.GetFile().LastIndexOf(SLASH));
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                log.Trace("Exception getting version of System.Uri {0} ({1})", url, e.Message);
            }

            return currentVersion;
        }

        protected void manage(Architecture arch, string version)
        {
            httpClient = new HttpClient(Config());
            try
            {
                downloader = new Downloader(GetDriverManagerType().Value);
                urlFilter = new UrlFilter();

                bool getLatest = isVersionLatest(version);
                bool cache = Config().isForceCache();

                if (getLatest)
                {
                    version = detectDriverVersionFromBrowser();
                }
                getLatest = string.IsNullOrEmpty(version);

                // Check latest version
                if (getLatest && !Config().isUseBetaVersions())
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

                string os = Config().getOs();
                log.Trace("Managing {0} arch={1} version={2} getLatest={3} cache={4}", GetDriverName(), arch, version, getLatest, cache);

                if (getLatest && latestVersion != null)
                {
                    log.Debug("Latest version of {0} is {1} (recently resolved)", GetDriverName(), latestVersion);
                    version = latestVersion;
                    cache = true;
                }

                string driverInCache = handleCache(arch, version, os, getLatest, cache);

                string versionStr = getLatest ? "(latest version)" : version;
                if (driverInCache != null && !Config().isOverride())
                {
                    storeVersionToDownload(version);
                    downloadedVersion = version;
                    log.Debug("Driver {0} {1} found in cache", GetDriverName(), versionStr);
                    exportDriver(driverInCache);
                }
                else
                {
                    List<System.Uri> candidateUrls = filterCandidateUrls(arch, version, getLatest);
                    if (candidateUrls.Count == 0)
                    {
                        string errorMessage = string.Format("{0} {1} for {2}{3} not found in {4}", GetDriverName(), versionStr, os, arch.ToString(), GetDriverUrl());
                        log.Error(errorMessage);
                        throw new WebDriverManagerException(errorMessage);
                    }

                    downloadCandidateUrls(candidateUrls);
                }

            }
            catch (System.Exception e)
            {
                handleException(e, arch, version);
            }
        }

        private string detectDriverVersionFromBrowser()
        {
            string version = "";
            if (Config().isAvoidAutoVersion())
            {
                return version;
            }

            string driverManagerTypeLowerCase = GetDriverManagerType().ToString().ToLower();
            string optionalBrowserVersion;
            if (usePreferences() && preferences.checkKeyInPreferences(driverManagerTypeLowerCase))
            {
                optionalBrowserVersion = preferences.getValueFromPreferences(driverManagerTypeLowerCase);
            }
            else
            {
                optionalBrowserVersion = GetBrowserVersion();
            }

            if (optionalBrowserVersion != null)
            {
                string browserVersion = optionalBrowserVersion;
                log.Trace("Detected {0} version {1}", GetDriverManagerType(), browserVersion);

                preferenceKey = driverManagerTypeLowerCase + browserVersion;

                if (usePreferences() && preferences.checkKeyInPreferences(preferenceKey))
                {
                    // Get driver version from preferences
                    version = preferences.getValueFromPreferences(preferenceKey);
                }
                else
                {
                    // Get driver version from properties
                    version = getVersionForInstalledBrowser(browserVersion);
                }
                if (!string.IsNullOrEmpty(version))
                {
                    log.Info("Using {0} {1} (since {2} {3} is installed in your machine)", GetDriverName(), version, GetDriverManagerType(), browserVersion);
                    preferences.putValueInPreferencesIfEmpty(driverManagerTypeLowerCase, browserVersion);
                }
            }
            else
            {
                log.Debug("The proper {0} version for your {1} is unknown ... trying with the latest", GetDriverName(), GetDriverManagerType());
            }

            return version;
        }

        private bool usePreferences()
        {
            bool usePrefs = !Config().isAvoidPreferences() && !Config().isOverride() && !forcedArch && !forcedOs;
            log.Trace("Using preferences {0}", usePrefs);
            return usePrefs;
        }

        private bool checkPreInstalledVersion(string version)
        {
            if (version.Equals(PRE_INSTALLED))
            {
                string systemRoot = System.Environment.GetEnvironmentVariable("SystemRoot");
                FileInfo microsoftWebDriverFile = new FileInfo(Path.Combine(systemRoot, "System32" + Path.DirectorySeparatorChar + "MicrosoftWebDriver.exe"));
                if (microsoftWebDriverFile.Exists)
                {
                    downloadedVersion = PRE_INSTALLED;
                    exportDriver(microsoftWebDriverFile.ToString());
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
                log.Debug("The driver version for {0} {1} is unknown ... trying with latest", driverManagerType, browserVersion);
            }
            return driverVersion;
        }

        private string getDriverVersionForBrowserFromProperties(string key)
        {
            bool online = Config().getVersionsPropertiesOnlineFirst();
            string onlineMessage = online ? ONLINE : LOCAL;
            log.Debug("Getting driver version for {0} from {1} versions.properties", key, onlineMessage);
            string value = getVersionFromProperties(online).GetProperty(key);
            if (value == null)
            {
                string notOnlineMessage = online ? LOCAL : ONLINE;
                log.Debug("Driver for {0} not found in {1} properties (using {2} version)", key, onlineMessage, notOnlineMessage);
                versionsProperties = null;
                value = getVersionFromProperties(!online).GetProperty(key);
            }
            return value;
        }

        private Properties getVersionFromProperties(bool online)
        {
            if (versionsProperties != null)
            {
                log.Trace("Already created versions.properties");
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
            log.Trace("Reading {0} version.properties to find out driver version", onlineMessage);
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
                log.Warn("Error reading version.properties, using {0} instead", exceptionMessage);
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
            return httpClient.executeHttpGet(Config().getVersionsPropertiesUrl()).Content.ReadAsStreamAsync().Result;
        }

        protected void handleException(System.Exception e, Architecture arch, string version)
        {
            string versionStr = string.IsNullOrEmpty(version) ? "(latest version)" : version;
            string errorMessage = string.Format("There was an error managing {0} {1} ({2})", GetDriverName(), versionStr, e.Message);
            if (!Config().isForceCache() && retry)
            {
                Config().setForceCache(true);
                Config().setUseMirror(true);
                retry = false;
                log.Warn("{0} ... trying again using cache and mirror", errorMessage);
                manage(arch, version);
            }
            else
            {
                log.Error("{0}", errorMessage, e);
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
            System.Uri url = candidateUrls.First();
            string exportValue = downloader.download(url, versionToDownload, GetDriverName());
            exportDriver(exportValue);
            downloadedVersion = versionToDownload;
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
            log.Trace("All System.Uris: {0}", urls);

            bool continueSearchingVersion;
            do
            {
                // Get the latest or concrete version
                candidateUrls = getLatest ? checkLatest(urls, GetDriverName()) : getVersion(urls, GetDriverName(), version);
                log.Trace("Candidate System.Uris: {0}", candidateUrls);
                if (versionToDownload == null || this.GetType().Equals(typeof(EdgeDriverManager)))
                {
                    break;
                }

                // Filter by OS
                if (!GetDriverName().Equals("IEDriverServer", System.StringComparison.InvariantCultureIgnoreCase) && !GetDriverName().Equals("selenium-server-standalone", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    candidateUrls = urlFilter.filterByOs(candidateUrls, Config().getOs());
                }

                // Filter by architecture
                candidateUrls = urlFilter.filterByArch(candidateUrls, arch, forcedArch);

                // Filter by distro
                candidateUrls = filterByDistro(candidateUrls);

                // Filter by ignored versions
                candidateUrls = filterByIgnoredVersions(candidateUrls);

                // Find out if driver version has been found or not
                continueSearchingVersion = candidateUrls.Count == 0 && getLatest;
                if (continueSearchingVersion)
                {
                    log.Info("No binary found for {0} {1} ... seeking another version", GetDriverName(), versionToDownload);
                    urls = removeFromList(urls, versionToDownload);
                    versionToDownload = null;
                }
            } while (continueSearchingVersion);
            return candidateUrls;
        }

        protected List<System.Uri> filterByIgnoredVersions(List<System.Uri> candidateUrls)
        {
            if (Config().getIgnoreVersions() != null && candidateUrls.Count != 0)
            {
                candidateUrls = urlFilter.filterByIgnoredVersions(candidateUrls, Config().getIgnoreVersions());
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
            if (Config().getOs().Equals("linux", System.StringComparison.InvariantCultureIgnoreCase) && GetDriverName().Contains("phantomjs"))
            {
                candidateUrls = urlFilter.filterByDistro(candidateUrls, "2.5.0");
            }
            return candidateUrls;
        }

        protected string handleCache(Architecture arch, string version, string os, bool getLatest, bool cache)
        {
            string driverInCache = null;
            if (cache || !getLatest)
            {
                driverInCache = getDriverFromCache(version, arch, os);
            }
            storeVersionToDownload(version);
            return driverInCache;
        }

        protected string getDriverFromCache(string driverVersion, Architecture arch, string os)
        {
            log.Trace("Checking if {0} exists in cache", GetDriverName());
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
                    return filesInCache[0].ToString();
                }

                // Filter by arch
                filesInCache = filterCacheBy(filesInCache, arch.GetString());

                if (filesInCache != null && filesInCache.Count > 1)
                {
                    return filesInCache[filesInCache.Count() - 1].ToString();
                }
            }

            log.Trace("{0} not found in cache", GetDriverName());
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
            log.Trace("Filter cache by {0} -- input list {1} -- output list {2} ", key, input, output);
            return output;
        }

        protected List<FileInfo> getFilesInCache()
        {
            return new DirectoryInfo(downloader.getTargetPath()).GetFiles("*.*", SearchOption.AllDirectories).ToList();
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

            if (versionToDownload != null && !versionToDownload.Equals(version))
            {
                versionToDownload = version;
                log.Info("Using {0} {1}", driver, version);
            }

            return outList;
        }

        protected virtual List<System.Uri> checkLatest(List<System.Uri> list, string driver)
        {
            log.Trace("Checking the lastest version of {0} with System.Uri list {1}", driver, list);
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
                    log.Trace("There was a problem with System.Uri {0} : {1}", url, e.Message);
                    list.Remove(url);
                }
            }
            storeVersionToDownload(versionToDownload);
            latestVersion = versionToDownload;
            log.Info("Latest version of {0} is {1}", driver, versionToDownload);
            return outList;
        }

        protected void handleDriver(System.Uri url, string driver, List<System.Uri> outList)
        {
            if (!Config().isUseBetaVersions() && (url.GetFile().ToLower().Contains("beta")))
            {
                return;
            }

            if (url.GetFile().Contains(driver))
            {
                string currentVersion = getCurrentVersion(url, driver);

                if (currentVersion.Equals(driver, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }
                if (versionToDownload == null)
                {
                    versionToDownload = currentVersion;
                }
                if (versionCompare(currentVersion, versionToDownload) > 0)
                {
                    versionToDownload = currentVersion;
                    outList.Clear();
                }
                if (url.GetFile().Contains(versionToDownload))
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
                log.Info("Crawling driver list from mirror {0}", driverUrl);
                mirrorLog = true;
            }
            else
            {
                log.Trace("[Recursive call] Crawling driver list from mirror {0}", driverUrl);
            }

            string driverStr = driverUrl.ToString();

            using (StreamReader inStream = new StreamReader(httpClient.executeHttpGet(driverUrl).Content.ReadAsStreamAsync().Result))
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
            log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());
            List<System.Uri> urls = new List<System.Uri>();
            try
            {
                using (Stream reader = httpClient.executeHttpGet(driverUrl).Content.ReadAsStreamAsync().Result)
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

        protected void exportDriver(string variableValue)
        {
            binaryPath = variableValue;
            string exportParameter = GetExportParameter();
            if (!config.isAvoidExport() && exportParameter != null)
            {
                // TODO: maybe remove this
                string variableName = exportParameter;
                log.Info("Exporting {0} as {1}", variableName, variableValue);
                System.Environment.SetEnvironmentVariable(variableName, variableValue);

                // Add driver to PATH
                string pathVar = System.Environment.GetEnvironmentVariable("PATH");
                pathVar += ";" + new FileInfo(variableValue).DirectoryName;
                System.Environment.SetEnvironmentVariable("PATH", pathVar);
            }
            else
            {
                log.Info("Resulting binary {0}", variableValue);
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
            string gitHubTokenName = Config().getGitHubTokenName().Trim();
            string gitHubTokenSecret = Config().getGitHubTokenSecret().Trim();
            AuthenticationHeaderValue authHeader = null;
            if (!string.IsNullOrEmpty(gitHubTokenName) && !string.IsNullOrEmpty(gitHubTokenSecret))
            {
                authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(gitHubTokenName + ":" + gitHubTokenSecret)));
            }

            return httpClient.executeHttpGet(driverUrl, authHeader).Content.ReadAsStreamAsync().Result;
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
            log.Info("Reading {0} to seek {1}", driverUrl, GetDriverName());

            System.Uri mirrorUrl = GetMirrorUrl();
            if (mirrorUrl != null && config.isUseMirror())
            {
                urls = getDriversFromMirror(mirrorUrl);
            }
            else
            {
                string driverVersion = versionToDownload;

                using (JsonReader reader = new JsonTextReader(new StreamReader(openGitHubConnection(driverUrl))))
                {
                    Release[] releaseArray = new JsonSerializer().Deserialize<Release[]>(reader);

                    if (driverVersion != null)
                    {
                        releaseArray = new Release[]
                        {
                            getVersion(releaseArray, driverVersion)
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

        protected static Release getVersion(Release[] releaseArray, string version)
        {
            Release outRelease = null;
            foreach (Release release in releaseArray)
            {
                log.Trace("Get version {0} of {1}", version, release);
                if ((release.Name != null && release.Name.Contains(version)) || (release.TagName != null && release.TagName.Contains(version)))
                {
                    outRelease = release;
                    break;
                }
            }
            return outRelease;
        }

        public HttpClient getHttpClient()
        {
            return httpClient;
        }

        protected DirectoryInfo[] getFolderFilter(DirectoryInfo directory)
        {
            return directory.GetDirectories().Where(d => d.Name.ToLower().Contains(GetDriverName())).ToArray();
        }

        protected string getDefaultBrowserVersion(string[] programFilesEnvs, string winBrowserName, string linuxBrowserName, string macBrowserName, string versionFlag, string browserNameInOutput)
        {

            string browserBinaryPath = Config().getBinaryPath();
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
            Config().reset();
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
            if (Config().isUseMirror())
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
                    new Preferences(new Config()).clear();
                }
                else
                {
                    resolveLocal(validBrowsers, arg);
                }
            }
        }

        private static void resolveLocal(string validBrowsers, string arg)
        {
            log.Info("Using WebDriverManager to resolve {0}", arg);
            try
            {
                DriverManagerType driverManagerType = (DriverManagerType)System.Enum.Parse(typeof(DriverManagerType), arg.ToUpper());
                WebDriverManager wdm = WebDriverManager.getInstance(driverManagerType).avoidExport().targetPath(".").forceDownload();
                if (arg.Equals("edge", System.StringComparison.InvariantCultureIgnoreCase) || arg.Equals("iexplorer", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    wdm.operatingSystem(OperatingSystem.WIN);
                }
                wdm.avoidOutputTree().setup();
            }
            catch (Exception)
            {
                log.Error("Driver for {0} not found (valid browsers {1})", arg, validBrowsers);
            }
        }

        private static void startServer(string[] args)
        {
            if (args.Length < 2 || !int.TryParse(args[1], out int port))
            {
                port = new Config().getServerPort();
            }
            new Server(port);
        }

        private static void logCliError(string validBrowsers)
        {
            log.Error("There are 3 options to run WebDriverManager CLI");
            log.Error("1. WebDriverManager used to resolve binary drivers locally:");
            log.Error("\tWebDriverManager browserName");
            log.Error("\t(where browserName={0})", validBrowsers);

            log.Error("2. WebDriverManager as a server:");
            log.Error("\tWebDriverManager server <port>");
            log.Error("\t(where default port is 4041)");

            log.Error("3. To clear previously resolved driver versions (as Java preferences):");
            log.Error("\tWebDriverManager clear-preferences");
        }

        private void storeVersionToDownload(string version)
        {
            if (!string.IsNullOrEmpty(version))
            {
                if (version.StartsWith("."))
                {
                    version = version.SubstringJava(1);
                }
                versionToDownload = version;
                if (isLatest && usePreferences() && !string.IsNullOrEmpty(preferenceKey))
                {
                    preferences.putValueInPreferencesIfEmpty(preferenceKey, version);
                }
            }
        }

        private void setConfig(Config config)
        {
            this.config = config;
        }
    }
}