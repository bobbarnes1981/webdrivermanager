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

namespace WebDriverManagerSharp.Managers
{
    using Autofac;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Exceptions;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Processes;
    using WebDriverManagerSharp.Storage;
    using WebDriverManagerSharp.Web;

    /**
     * Manager for Opera.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class OperaDriverManager : WebDriverManager
    {
        public OperaDriverManager(IConfig config, IShell shell, IPreferences preferences, ILogger logger, IFileStorage fileStorage)
            : base(config, shell, preferences, logger, fileStorage)
        {
        }

        protected override DriverManagerType? GetDriverManagerType()
        {
            return DriverManagerType.OPERA;
        }

        protected override string GetDriverName()
        {
            return "operadriver";
        }

        protected override string GetDriverVersion()
        {
            return Config().GetOperaDriverVersion();
        }

        protected override Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().GetOperaDriverUrl());
        }

        protected override Uri GetMirrorUrl()
        {
            return Config().GetOperaDriverMirrorUrl();
        }

        protected override string GetExportParameter()
        {
            return Config().GetOperaDriverExport();
        }

        protected override void SetDriverVersion(string version)
        {
            Config().SetOperaDriverVersion(version);
        }

        protected override void SetDriverUrl(Uri url)
        {
            Config().SetOperaDriverUrl(url);
        }

        protected override string GetCurrentVersion(Uri url, string driverName)
        {
            if (Config().IsUseMirror())
            {
                int i = url.GetFile().LastIndexOf(SLASH, StringComparison.OrdinalIgnoreCase);
                int j = url.GetFile().SubstringJava(0, i).LastIndexOf(SLASH, StringComparison.OrdinalIgnoreCase) + 1;
                return url.GetFile().SubstringJava(j, i);
            }
            else
            {
                return url.GetFile().SubstringJava(
                        url.GetFile().IndexOf(SLASH + "v", StringComparison.OrdinalIgnoreCase) + 2,
                        url.GetFile().LastIndexOf(SLASH, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Get the driver Uris for Opera
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<Uri> GetDrivers()
        {
            return getDriversFromGitHub();
        }

        public override IFile PostDownload(IFile archive)
        {
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            Log.Trace("Post processing for Opera: {0}", archive);

            IDirectory[] folders = GetFolderFilter(archive.ParentDirectory);
            if (folders.Length > 0)
            {
                IDirectory extractFolder = GetFolderFilter(archive.ParentDirectory)[0];
                IFile target;
                IDownloader downloader = Resolver.Resolve<IDownloader>(new NamedParameter("driverManagerType", GetDriverManagerType().Value));
                try
                {
                    Log.Trace("Opera extract folder (to be deleted): {0}", extractFolder);
                    IReadOnlyList<IFile> listFiles = extractFolder.Files;
                    int i = 0;
                    IFile operadriver;
                    bool isOperaDriver;
                    do
                    {
                        if (i >= listFiles.Count)
                        {
                            throw new WebDriverManagerException("Driver binary for Opera not found in zip file");
                        }

                        operadriver = listFiles[i];
                        isOperaDriver = Config().IsExecutable(operadriver) && operadriver.FullName.Contains(GetDriverName());
                        i++;
                        Log.Trace("{0} is valid: {1}", operadriver, isOperaDriver);
                    }
                    while (!isOperaDriver);

                    Log.Info("Operadriver binary: {0}", operadriver);

                    target = new Storage.File(Path.Combine(archive.ParentDirectory.FullName, operadriver.Name));
                    Log.Trace("Operadriver target: {0}", target);

                    downloader.RenameFile(operadriver, target);
                }
                finally
                {
                    downloader.DeleteFolder(extractFolder);
                }

                return target;
            }
            else
            {
                return base.PostDownload(archive);
            }
        }

        protected override string GetBrowserVersion()
        {
            string[] programFilesEnvs = { "PROGRAMFILES" };
            return GetDefaultBrowserVersion(programFilesEnvs, "\\\\Opera\\\\launcher.exe", "opera", "/Applications/Opera.app/Contents/MacOS/Opera", "--version", string.Empty);
        }
    }
}