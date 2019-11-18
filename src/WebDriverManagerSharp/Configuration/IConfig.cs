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
    using System;
    using System.IO;
    using WebDriverManagerSharp.Enums;
    public interface IConfig
    {
        void Reset();

        bool IsExecutable(FileInfo file);

        string GetProperties();

        IConfig SetProperties(string properties);

        string GetTargetPath();

        IConfig SetTargetPath(string value);

        bool IsForceCache();

        IConfig SetForceCache(bool value);

        bool IsOverride();

        IConfig SetOverride(bool value);

        bool IsUseMirror();

        IConfig SetUseMirror(bool value);

        bool IsUseBetaVersions();

        IConfig SetUseBetaVersions(bool value);

        bool IsAvoidExport();

        IConfig SetAvoidExport(bool value);

        bool IsAvoidOutputTree();

        IConfig SetAvoidOutputTree(bool value);

        bool IsAvoidAutoVersion();

        IConfig SetAvoidAutoVersion(bool value);

        bool IsAvoidAutoReset();

        IConfig SetAvoidAutoReset(bool value);

        bool IsAvoidPreferences();

        IConfig SetAvoidPreferences(bool value);

        int GetTimeout();

        IConfig SetTimeout(int value);

        bool IsVersionsPropertiesOnlineFirst();

        IConfig SetVersionsPropertiesOnlineFirst(bool value);

        Uri GetVersionsPropertiesUrl();

        IConfig SetVersionsPropertiesUrl(System.Uri value);

        Architecture GetArchitecture();

        IConfig SetArchitecture(Architecture value);

        string GetOs();

        IConfig SetOs(string value);

        string GetProxy();

        IConfig SetProxy(string value);

        string GetProxyUser();

        IConfig SetProxyUser(string value);

        string GetProxyPass();

        IConfig SetProxyPass(string value);

        string[] GetIgnoreVersions();

        IConfig SetIgnoreVersions(params string[] value);

        string GetGitHubTokenName();

        IConfig SetGitHubTokenName(string value);

        string GetGitHubTokenSecret();

        IConfig SetGitHubTokenSecret(string value);

        string GetLocalRepositoryUser();

        IConfig SetLocalRepositoryUser(string value);

        string GetLocalRepositoryPassword();

        IConfig SetLocalRepositoryPassword(string value);

        int GetServerPort();

        IConfig SetServerPort(int value);

        int GetTtl();

        IConfig SetTtl(int value);

        string GetBinaryPath();

        IConfig SetBinaryPath(string value);

        string GetChromeDriverVersion();

        IConfig SetChromeDriverVersion(string value);

        string GetChromeDriverExport();

        IConfig SetChromeDriverExport(string value);

        Uri GetChromeDriverUrl();

        IConfig SetChromeDriverUrl(Uri value);

        Uri GetChromeDriverMirrorUrl();

        IConfig SetChromeDriverMirrorUrl(Uri value);

        string GetEdgeDriverVersion();

        IConfig SetEdgeDriverVersion(string value);

        string GetEdgeDriverExport();

        IConfig SetEdgeDriverExport(string value);

        Uri GetEdgeDriverUrl();

        IConfig SetEdgeDriverUrl(Uri value);

        string GetFirefoxDriverVersion();

        IConfig SetFirefoxDriverVersion(string value);

        string GetFirefoxDriverExport();

        IConfig SetFirefoxDriverExport(string value);

        Uri GetFirefoxDriverUrl();

        IConfig SetFirefoxDriverUrl(Uri value);

        Uri GetFirefoxDriverMirrorUrl();

        IConfig SetFirefoxDriverMirrorUrl(Uri value);

        string GetInternetExplorerDriverVersion();

        IConfig SetInternetExplorerDriverVersion(string value);

        string GetInternetExplorerDriverExport();

        IConfig SetInternetExplorerDriverExport(string value);

        Uri GetInternetExplorerDriverUrl();

        IConfig SetInternetExplorerDriverUrl(Uri value);

        string GetOperaDriverVersion();

        IConfig SetOperaDriverVersion(string value);

        string GetOperaDriverExport();

        IConfig SetOperaDriverExport(string value);

        Uri GetOperaDriverUrl();

        IConfig SetOperaDriverUrl(Uri value);

        Uri GetOperaDriverMirrorUrl();

        IConfig SetOperaDriverMirrorUrl(Uri value);

        string GetPhantomjsDriverVersion();

        IConfig SetPhantomjsDriverVersion(string value);

        string GetPhantomjsDriverExport();

        IConfig SetPhantomjsDriverExport(string value);

        Uri GetPhantomjsDriverUrl();

        IConfig SetPhantomjsDriverUrl(Uri value);

        Uri GetPhantomjsDriverMirrorUrl();

        IConfig SetPhantomjsDriverMirrorUrl(Uri value);

        string GetSeleniumServerStandaloneVersion();

        IConfig SetSeleniumServerStandaloneVersion(string value);

        Uri GetSeleniumServerStandaloneUrl();

        IConfig SetSeleniumServerStandaloneUrl(Uri value);
    }
}
