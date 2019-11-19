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

namespace WebDriverManagerSharp.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using WebDriverManagerSharp.Configuration;
    using WebDriverManagerSharp.Enums;
    using WebDriverManagerSharp.Exceptions;
    using WebDriverManagerSharp.Logging;
    using WebDriverManagerSharp.Processes;

    /**
     * Downloader class.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class Downloader : IDownloader
    {
        private static readonly ILogger log = Logger.GetLogger();

        private readonly DriverManagerType driverManagerType;
        private readonly IHttpClient httpClient;
        private readonly IConfig config;

        public Downloader(DriverManagerType driverManagerType)
        {
            this.driverManagerType = driverManagerType;

            WebDriverManager webDriverManager = WebDriverManager.GetInstance(driverManagerType);
            config = webDriverManager.Config();
            httpClient = webDriverManager.HttpClient;
        }

        /// <summary>
        /// Download the driver from the provided Uri
        /// </summary>
        /// <param name="url">Url of driver to download</param>
        /// <param name="version">Required driver version</param>
        /// <param name="driverName">Required driver name</param>
        /// <exception cref="IOException" />
        /// <returns></returns>
        public FileInfo Download(Uri url, string version, string driverName)
        {
            FileInfo targetFile = GetTarget(version, url);
            FileInfo binary = checkBinary(driverName, targetFile);
            if (binary == null)
            {
                binary = downloadAndExtract(url, targetFile);
            }

            return binary;
        }

        public FileInfo GetTarget(string version, Uri url)
        {
            log.Trace("getTarget {0} {1}", version, url);
            string zip = url.GetFile().SubstringJava(url.GetFile().LastIndexOf('/'));

            int iFirst = zip.IndexOf('_');
            int iSecond = zip.IndexOf('-');
            int iLast = zip.Length;
            if (iFirst != zip.LastIndexOf('_'))
            {
                iLast = zip.LastIndexOf('_');
            }
            else if (iSecond != -1)
            {
                iLast = iSecond;
            }

            string folder = zip.SubstringJava(0, iLast).Replace(".zip", string.Empty)
                    .Replace(".tar.bz2", string.Empty).Replace(".tar.gz", string.Empty)
                    .Replace(".msi", string.Empty).Replace(".exe", string.Empty)
                    .Replace('_', Path.DirectorySeparatorChar);
            string path = config.IsAvoidOutputTree() ? GetTargetPath() + zip
                    : GetTargetPath() + folder + Path.DirectorySeparatorChar + version + zip;
            string target = WebDriverManager.GetInstance(driverManagerType).PreDownload(path, version);

            log.Trace("Target file for System.Uri {0} version {1} = {2}", url, version, target);

            return new FileInfo(target);
        }

        public string GetTargetPath()
        {
            string targetPath = config.GetTargetPath();
            log.Trace("Target path {0}", targetPath);

            // Create repository folder if not exits
            DirectoryInfo repository = new DirectoryInfo(targetPath);
            if (!repository.Exists)
            {
                repository.Create();
            }

            return targetPath;
        }

        /// <summary>
        /// Download driver from the provided Uri and extract it to the provided target file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="targetFile"></param>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        private FileInfo downloadAndExtract(Uri url, FileInfo targetFile)
        {
            log.Info("Downloading {0}", url);
            DirectoryInfo targetFolder = targetFile.Directory;
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            FileInfo temporaryFile = new FileInfo(Path.Combine(tempDir, targetFile.Name));
            if (!temporaryFile.Directory.Exists)
            {
                temporaryFile.Directory.Create();
            }

            log.Trace("Target folder {0} ... using temporal file {1}", targetFolder, temporaryFile);
            temporaryFile.CreateFromStream(httpClient.ExecuteHttpGet(url));

            FileInfo extractedFile = extract(temporaryFile);
            FileInfo resultingBinary = new FileInfo(Path.Combine(targetFolder.FullName, extractedFile.Name));
            bool binaryExists = resultingBinary.Exists;

            if (!binaryExists || config.IsOverride())
            {
                if (binaryExists)
                {
                    log.Info("Overriding former binary {0}", resultingBinary);
                    File.Delete(resultingBinary.FullName);
                }

                if (!resultingBinary.Directory.Exists)
                {
                    resultingBinary.Directory.Create();
                }

                extractedFile.MoveTo(Path.Combine(resultingBinary.FullName));
            }

            if (!config.IsExecutable(resultingBinary))
            {
                SetFileExecutable(resultingBinary);
            }

            Directory.Delete(tempDir, true);
            log.Trace("Binary driver after extraction {0}", resultingBinary);

            return new FileInfo(resultingBinary.FullName);
        }

        private FileInfo checkBinary(string driverName, FileInfo targetFile)
        {
            DirectoryInfo parentFolder = targetFile.Directory;
            if (parentFolder.Exists && !config.IsOverride())
            {
                // Check if binary exits in parent folder and it is valid

                FileInfo[] listFiles = parentFolder.GetFiles();
                foreach (FileInfo file in listFiles)
                {
                    if (file.FullName.StartsWith(driverName, StringComparison.OrdinalIgnoreCase) && config.IsExecutable(file))
                    {
                        log.Info("Using binary driver previously downloaded");
                        return new FileInfo(file.FullName);
                    }
                }

                log.Trace("{0} does not exist in cache", driverName);
            }

            return null;
        }

        /// <summary>
        /// Extract the provided file and return the FileInfo of the extracted driver binary
        /// </summary>
        /// <param name="compressedFile"></param>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        private FileInfo extract(FileInfo compressedFile)
        {
            string fileName = compressedFile.FullName;

            bool extractFile = !fileName.EndsWith(Constants.EXE, StringComparison.OrdinalIgnoreCase) && !fileName.EndsWith(Constants.JAR, StringComparison.OrdinalIgnoreCase);
            if (extractFile)
            {
                log.Info("Extracting binary from compressed file {0}", fileName);
            }

            if (fileName.EndsWith("tar.bz2", StringComparison.OrdinalIgnoreCase))
            {
                unBZip2(compressedFile);
            }
            else if (fileName.EndsWith("tar.gz", StringComparison.OrdinalIgnoreCase))
            {
                unTarGz(compressedFile);
            }
            else if (fileName.EndsWith("gz", StringComparison.OrdinalIgnoreCase))
            {
                unGzip(compressedFile);
            }
            else if (fileName.EndsWith("msi", StringComparison.OrdinalIgnoreCase))
            {
                ExtractMsi(compressedFile);
            }
            else if (fileName.EndsWith("zip", StringComparison.OrdinalIgnoreCase))
            {
                unZip(compressedFile);
            }

            if (extractFile)
            {
                compressedFile.Delete();
            }

            FileInfo result = WebDriverManager.GetInstance(driverManagerType).PostDownload(compressedFile);
            log.Trace("Resulting binary file {0}", result);

            return result;
        }

        /// <summary>
        /// Un Zip the provided file
        /// </summary>
        /// <param name="compressedFile"></param>
        /// <exception cref="IOException"/>
        private void unZip(FileInfo compressedFile)
        {
            using (ZipArchive zipFolder = ZipFile.OpenRead(compressedFile.FullName))
            {
                IEnumerator<ZipArchiveEntry> enu = zipFolder.Entries.GetEnumerator();

                while (enu.MoveNext())
                {
                    ZipArchiveEntry zipEntry = enu.Current;

                    string name = zipEntry.FullName;
                    long size = zipEntry.Length;
                    long compressedSize = zipEntry.CompressedLength;
                    log.Trace("Unzipping {0} (size: {1} KB, compressed size: {2} KB)", name, size, compressedSize);

                    // TODO: handle more than one file
                    if (name.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        DirectoryInfo dir = new DirectoryInfo(Path.Combine(compressedFile.Directory.FullName, name));
                        if (!dir.Exists || config.IsOverride())
                        {
                            dir.Create();
                        }
                        else
                        {
                            log.Debug("{0} already exists", dir);
                        }
                    }
                    else
                    {
                        FileInfo file = new FileInfo(Path.Combine(compressedFile.Directory.FullName, name));
                        if (!file.Exists || config.IsOverride())
                        {
                            zipEntry.ExtractToFile(file.FullName);
                            SetFileExecutable(file);
                        }
                        else
                        {
                            log.Debug("{0} already exists", file);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Un GZip the provided file
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>
        private void unGzip(FileInfo archive)
        {
            log.Trace("UnGzip {0}", archive);
            string fileName = archive.FullName;
            int iDash = fileName.IndexOf('-');
            if (iDash != -1)
            {
                fileName = fileName.SubstringJava(0, iDash);
            }
            int iDot = fileName.IndexOf('.');
            if (iDot != -1)
            {
                fileName = fileName.SubstringJava(0, iDot);
            }
            FileInfo target = new FileInfo(Path.Combine(archive.DirectoryName, fileName));

            using (GZipStream inStream = new GZipStream(new FileStream(archive.FullName, FileMode.Open), CompressionLevel.Optimal))
            {
                using (FileStream outStream = new FileStream(target.FullName, FileMode.Create))
                {
                    throw new NotImplementedException();
                    //for (int c = inStream.read(); c != -1; c = inStream.read())
                    //{
                    //        outStream.write(c);
                    //}
                }
            }

            if (!target.FullName.ToLower().Contains(Constants.EXE) && target.Exists)
            {
                SetFileExecutable(target);
            }
        }

        /// <summary>
        /// Un TarGz the provided file
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>
        private void unTarGz(FileInfo archive)
        {
            log.Trace("unTarGz {0}", archive);
            ////Archiver archiver = createArchiver(TAR, GZIP);
            ////archiver.extract(archive, archive.getParentFile());
            throw new NotImplementedException("extract tar.gz not implemented");
        }

        /// <summary>
        /// Un BZip2 the provided file
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>
        private void unBZip2(FileInfo archive)
        {
            log.Trace("Unbzip2 {0}", archive);
            ////Archiver archiver = createArchiver(TAR, BZIP2);
            ////archiver.extract(archive, archive.getParentFile());
            throw new NotImplementedException("extract bzip2 not implemented");
        }

        /// <summary>
        /// Extract the provided MSI
        /// </summary>
        /// <param name="msi"></param>
        /// <exception cref="IOException"/>
        private void ExtractMsi(FileInfo msi)
        {
            log.Trace("Extract MSI {0}", msi);
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            FileInfo tmpMsi = new FileInfo(Path.Combine(tempDir, msi.Name));
            msi.MoveTo(tmpMsi.FullName);
            log.Trace("Temporal msi file: {0}", tmpMsi);

            using (IProcess process = new ProcessBuilder(new string[] { "msiexec", "/a", tmpMsi.FullName, "/qb", "TARGETDIR=" + msi.DirectoryName }).Start())
            {
                process.WaitForExit();
            }

            tmpMsi.Directory.Delete(true);
        }

        protected void SetFileExecutable(FileInfo file)
        {
            log.Trace("Setting file {0} as executable", file);
            ////if (!file.setExecutable(true))
            ////{
            ////    log.warn("Error setting file {} as executable", file);
            ////}
        }

        public static void RenameFile(FileInfo from, FileInfo to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            log.Trace("Renaming file from {0} to {1}", from, to);
            if (to.Exists)
            {
                deleteFile(to);
            }

            try
            {
                from.MoveTo(to.FullName);
            }
            catch (Exception)
            {
                log.Warn("Error renaming file from {0} to {1}", from, to);
            }
        }

        protected static void deleteFile(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            log.Trace("Deleting file {0}", file);
            try
            {
                file.Delete();
            }
            catch (IOException e)
            {
                throw new WebDriverManagerException(e);
            }
        }

        public static void DeleteFolder(DirectoryInfo folder)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }

            log.Trace("Deleting folder {0}", folder);
            try
            {
                folder.Delete(true);
            }
            catch (IOException e)
            {
                throw new WebDriverManagerException(e);
            }
        }
    }
}