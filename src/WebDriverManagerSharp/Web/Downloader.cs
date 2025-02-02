﻿/*
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
    using WebDriverManagerSharp.Storage;

    /**
     * Downloader class.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class Downloader : IDownloader
    {
        private readonly ILogger logger;
        private readonly DriverManagerType driverManagerType;

        private readonly IHttpClientHelper httpClient;
        private readonly IConfig config;

        public Downloader(ILogger logger, DriverManagerType driverManagerType)
        {
            this.logger = logger;
            this.driverManagerType = driverManagerType;

            // TODO: autofac this?
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
        public IFile Download(Uri url, string version, string driverName)
        {
            IFile targetFile = GetTarget(version, url);
            IFile binary = checkBinary(driverName, targetFile);
            if (binary == null)
            {
                binary = downloadAndExtract(url, targetFile);
            }

            return binary;
        }

        public IFile GetTarget(string version, Uri url)
        {
            logger.Trace("getTarget {0} {1}", version, url);
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

            logger.Trace("Target file for System.Uri {0} version {1} = {2}", url, version, target);

            return new Storage.File(target);
        }

        public string GetTargetPath()
        {
            string targetPath = config.GetTargetPath();
            logger.Trace("Target path {0}", targetPath);

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
        private IFile downloadAndExtract(Uri url, IFile targetFile)
        {
            logger.Info("Downloading {0}", url);
            IDirectory targetFolder = targetFile.ParentDirectory;
            IDirectory tempDir = new Storage.Directory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            IFile temporaryFile = new Storage.File(Path.Combine(tempDir.FullName, targetFile.Name));
            if (!temporaryFile.ParentDirectory.Exists)
            {
                temporaryFile.ParentDirectory.Create();
            }

            logger.Trace("Target folder {0} ... using temporal file {1}", targetFolder, temporaryFile);
            temporaryFile.CreateFromStream(httpClient.ExecuteHttpGet(url));

            IFile extractedFile = extract(temporaryFile);
            IFile resultingBinary = new Storage.File(Path.Combine(targetFolder.FullName, extractedFile.Name));
            bool binaryExists = resultingBinary.Exists;

            if (!binaryExists || config.IsOverride())
            {
                if (binaryExists)
                {
                    logger.Info("Overriding former binary {0}", resultingBinary);
                    resultingBinary.Delete();
                }

                if (!resultingBinary.ParentDirectory.Exists)
                {
                    resultingBinary.ParentDirectory.Create();
                }

                extractedFile.MoveTo(resultingBinary.FullName);
            }

            if (!config.IsExecutable(resultingBinary))
            {
                SetFileExecutable(resultingBinary);
            }

            tempDir.Delete(true);
            logger.Trace("Binary driver after extraction {0}", resultingBinary);

            return new Storage.File(resultingBinary.FullName);
        }

        private IFile checkBinary(string driverName, IFile targetFile)
        {
            IDirectory parentFolder = targetFile.ParentDirectory;
            if (parentFolder.Exists && !config.IsOverride())
            {
                // Check if binary exits in parent folder and it is valid

                IReadOnlyList<IFile> listFiles = parentFolder.Files;
                foreach (IFile file in listFiles)
                {
                    if (file.FullName.StartsWith(driverName, StringComparison.OrdinalIgnoreCase) && config.IsExecutable(file))
                    {
                        logger.Info("Using binary driver previously downloaded");
                        return new Storage.File(file.FullName);
                    }
                }

                logger.Trace("{0} does not exist in cache", driverName);
            }

            return null;
        }

        /// <summary>
        /// Extract the provided file and return the FileInfo of the extracted driver binary
        /// </summary>
        /// <param name="compressedFile"></param>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        private IFile extract(IFile compressedFile)
        {
            string fileName = compressedFile.FullName;

            bool extractFile = !fileName.EndsWith(Constants.EXE, StringComparison.OrdinalIgnoreCase) && !fileName.EndsWith(Constants.JAR, StringComparison.OrdinalIgnoreCase);
            if (extractFile)
            {
                logger.Info("Extracting binary from compressed file {0}", fileName);
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

            IFile result = WebDriverManager.GetInstance(driverManagerType).PostDownload(compressedFile);
            logger.Trace("Resulting binary file {0}", result);

            return result;
        }

        /// <summary>
        /// Un Zip the provided file
        /// </summary>
        /// <param name="compressedFile"></param>
        /// <exception cref="IOException"/>
        private void unZip(IFile compressedFile)
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
                    logger.Trace("Unzipping {0} (size: {1} KB, compressed size: {2} KB)", name, size, compressedSize);

                    // TODO: handle more than one file
                    if (name.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        DirectoryInfo dir = new DirectoryInfo(Path.Combine(compressedFile.ParentDirectory.FullName, name));
                        if (!dir.Exists || config.IsOverride())
                        {
                            dir.Create();
                        }
                        else
                        {
                            logger.Debug("{0} already exists", dir);
                        }
                    }
                    else
                    {
                        IFile file = new Storage.File(Path.Combine(compressedFile.ParentDirectory.FullName, name));
                        if (!file.Exists || config.IsOverride())
                        {
                            zipEntry.ExtractToFile(file.FullName);
                            SetFileExecutable(file);
                        }
                        else
                        {
                            logger.Debug("{0} already exists", file);
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
        private void unGzip(IFile archive)
        {
            logger.Trace("UnGzip {0}", archive);
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
            IFile target = new Storage.File(Path.Combine(archive.ParentDirectory.FullName, fileName));

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
        private void unTarGz(IFile archive)
        {
            logger.Trace("unTarGz {0}", archive);
            ////Archiver archiver = createArchiver(TAR, GZIP);
            ////archiver.extract(archive, archive.getParentFile());
            throw new NotImplementedException("extract tar.gz not implemented");
        }

        /// <summary>
        /// Un BZip2 the provided file
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>
        private void unBZip2(IFile archive)
        {
            logger.Trace("Unbzip2 {0}", archive);
            ////Archiver archiver = createArchiver(TAR, BZIP2);
            ////archiver.extract(archive, archive.getParentFile());
            throw new NotImplementedException("extract bzip2 not implemented");
        }

        /// <summary>
        /// Extract the provided MSI
        /// </summary>
        /// <param name="msi"></param>
        /// <exception cref="IOException"/>
        private void ExtractMsi(IFile msi)
        {
            logger.Trace("Extract MSI {0}", msi);
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            FileInfo tmpMsi = new FileInfo(Path.Combine(tempDir, msi.Name));
            msi.MoveTo(tmpMsi.FullName);
            logger.Trace("Temporal msi file: {0}", tmpMsi);

            using (IProcess process = new ProcessBuilder(new string[] { "msiexec", "/a", tmpMsi.FullName, "/qb", "TARGETDIR=" + msi.ParentDirectory }).Start())
            {
                process.WaitForExit();
            }

            tmpMsi.Directory.Delete(true);
        }

        protected void SetFileExecutable(IFile file)
        {
            logger.Trace("Setting file {0} as executable", file);
            ////if (!file.setExecutable(true))
            ////{
            ////    logger.warn("Error setting file {} as executable", file);
            ////}
        }

        public void RenameFile(IFile from, IFile to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            logger.Trace("Renaming file from {0} to {1}", from, to);
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
                logger.Warn("Error renaming file from {0} to {1}", from, to);
            }
        }

        protected void deleteFile(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            logger.Trace("Deleting file {0}", file);
            try
            {
                file.Delete();
            }
            catch (IOException e)
            {
                throw new WebDriverManagerException(e);
            }
        }

        public void DeleteFolder(IDirectory folder)
        {
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }

            logger.Trace("Deleting folder {0}", folder);
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