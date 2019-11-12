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

using System.Collections.Generic;
using System.IO;

namespace WebDriverManagerSharp
{
    /**
     * Manager for Opera.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class OperaDriverManager : WebDriverManager
    {
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

        protected override System.Uri GetDriverUrl()
        {
            return getDriverUrlCheckingMirror(Config().GetOperaDriverUrl());
        }

        protected override System.Uri GetMirrorUrl()
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

        protected override void SetDriverUrl(System.Uri url)
        {
            Config().SetOperaDriverUrl(url);
        }

        protected override string GetCurrentVersion(System.Uri url, string driverName)
        {
            if (Config().isUseMirror())
            {
                int i = url.GetFile().LastIndexOf(SLASH);
                int j = url.GetFile().SubstringJava(0, i).LastIndexOf(SLASH) + 1;
                return url.GetFile().SubstringJava(j, i);
            }
            else
            {
                return url.GetFile().SubstringJava(
                        url.GetFile().IndexOf(SLASH + "v") + 2,
                        url.GetFile().LastIndexOf(SLASH));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException" />
        /// <returns></returns>
        protected override List<System.Uri> GetDrivers()
        {
            return getDriversFromGitHub();
        }

        public override FileInfo PostDownload(FileInfo archive)
        {
            Log.Trace("Post processing for Opera: {0}", archive);

            DirectoryInfo[] folders = GetFolderFilter(archive.Directory);
            if (folders.Length > 0)
            {
                DirectoryInfo extractFolder = GetFolderFilter(archive.Directory)[0];
                FileInfo target;
                try
                {
                    Log.Trace("Opera extract folder (to be deleted): {0}", extractFolder);
                    FileInfo[] listFiles = extractFolder.GetFiles();
                    int i = 0;
                    FileInfo operadriver;
                    bool isOperaDriver;
                    do
                    {
                        if (i >= listFiles.Length)
                        {
                            throw new WebDriverManagerException(
                                    "Driver binary for Opera not found in zip file");
                        }
                        operadriver = listFiles[i];
                        isOperaDriver = Config().isExecutable(operadriver) && operadriver.FullName.Contains(GetDriverName());
                        i++;
                        Log.Trace("{0} is valid: {1}", operadriver, isOperaDriver);
                    } while (!isOperaDriver);
                    Log.Info("Operadriver binary: {0}", operadriver);

                    target = new FileInfo(Path.Combine(archive.Directory.FullName, operadriver.Name));
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
            return GetDefaultBrowserVersion(programFilesEnvs, "\\\\Opera\\\\launcher.exe", "opera", "/Applications/Opera.app/Contents/MacOS/Opera", "--version", "");
        }
    }
}