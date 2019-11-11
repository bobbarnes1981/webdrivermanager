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
using System.IO.Compression;

namespace WebDriverManager
{

    /**
     * Downloader class.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 1.0.0
     */
    public class Downloader
    {
        private readonly ILogger log = Logger.GetLogger();

        private readonly DriverManagerType driverManagerType;
        private readonly HttpClient httpClient;
        private readonly Config config;

        public Downloader(DriverManagerType driverManagerType)
        {
            this.driverManagerType = driverManagerType;

            WebDriverManager webDriverManager = WebDriverManager.getInstance(driverManagerType);
            config = webDriverManager.Config();
            httpClient = webDriverManager.getHttpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="version"></param>
        /// <param name="driverName"></param>
        /// <exception cref="IOException" />
        /// <returns></returns>
        public FileInfo download(System.Uri url, string version, string driverName)
        {
            FileInfo targetFile = getTarget(version, url);
            FileInfo binary = checkBinary(driverName, targetFile);
            if (binary == null)
            {
                binary = downloadAndExtract(url, targetFile);
            }
            return binary;
        }

        public FileInfo getTarget(string version, System.Uri url)
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

            string folder = zip.SubstringJava(0, iLast).Replace(".zip", "")
                    .Replace(".tar.bz2", "").Replace(".tar.gz", "")
                    .Replace(".msi", "").Replace(".exe", "")
                    .Replace('_', Path.DirectorySeparatorChar);
            string path = config.isAvoidOutputTree() ? getTargetPath() + zip
                    : getTargetPath() + folder + Path.DirectorySeparatorChar + version + zip;
            string target = WebDriverManager.getInstance(driverManagerType).preDownload(path, version);

            log.Trace("Target file for System.Uri {0} version {1} = {2}", url, version, target);

            return new FileInfo(target);
        }

        public string getTargetPath()
        {
            string targetPath = config.getTargetPath();
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
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="targetFile"></param>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        /// //InterruptedException
        private FileInfo downloadAndExtract(System.Uri url, FileInfo targetFile)
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
            temporaryFile.CreateFromStream(httpClient.executeHttpGet(url).Content.ReadAsStreamAsync().Result);

            FileInfo extractedFile = extract(temporaryFile);
            FileInfo resultingBinary = new FileInfo(Path.Combine(targetFolder.FullName, extractedFile.Name));
            bool binaryExists = resultingBinary.Exists;

            if (!binaryExists || config.isOverride())
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
            if (!config.isExecutable(resultingBinary))
            {
                setFileExecutable(resultingBinary);
            }
            Directory.Delete(tempDir, true);
            log.Trace("Binary driver after extraction {0}", resultingBinary);

            return new FileInfo(resultingBinary.FullName);
        }

        private FileInfo checkBinary(string driverName, FileInfo targetFile)
        {
            DirectoryInfo parentFolder = targetFile.Directory;
            if (parentFolder.Exists && !config.isOverride())
            {
                // Check if binary exits in parent folder and it is valid

                FileInfo[] listFiles = parentFolder.GetFiles();
                foreach (FileInfo file in listFiles)
                {
                    if (file.FullName.StartsWith(driverName) && config.isExecutable(file))
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
        /// 
        /// </summary>
        /// <param name="compressedFile"></param>
        /// <exception cref="IOException"/>
        /// //InterruptedException
        /// <returns></returns>
        private FileInfo extract(FileInfo compressedFile)
        {
            string fileName = compressedFile.FullName.ToLower();

            bool extractFile = !fileName.EndsWith("exe") && !fileName.EndsWith("jar");
            if (extractFile)
            {
                log.Info("Extracting binary from compressed file {0}", fileName);
            }
            if (fileName.EndsWith("tar.bz2"))
            {
                unBZip2(compressedFile);
            }
            else if (fileName.EndsWith("tar.gz"))
            {
                unTarGz(compressedFile);
            }
            else if (fileName.EndsWith("gz"))
            {
                unGzip(compressedFile);
            }
            else if (fileName.EndsWith("msi"))
            {
                extractMsi(compressedFile);
            }
            else if (fileName.EndsWith("zip"))
            {
                unZip(compressedFile);
            }

            if (extractFile)
            {
                compressedFile.Delete();
            }

            FileInfo result = WebDriverManager.getInstance(driverManagerType).postDownload(compressedFile);
            log.Trace("Resulting binary file {0}", result);

            return result;
        }

        /// <summary>
        /// 
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
                    ZipArchiveEntry zipEntry = (ZipArchiveEntry)enu.Current;

                    string name = zipEntry.FullName;
                    long size = zipEntry.Length;
                    long compressedSize = zipEntry.CompressedLength;
                    log.Trace("Unzipping {0} (size: {1} KB, compressed size: {2} KB)", name, size, compressedSize);

                    // TODO: handle more than one file
                    if (name.EndsWith("/"))
                    {
                        DirectoryInfo dir = new DirectoryInfo(Path.Combine(compressedFile.Directory.FullName, name));
                        if (!dir.Exists || config.isOverride())
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
                        if (!file.Exists || config.isOverride())
                        {
                            zipEntry.ExtractToFile(file.FullName);
                            setFileExecutable(file);
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
        /// 
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>
        private void unGzip(FileInfo archive)
        {
            //log.Trace("UnGzip {0}", archive);
            //string fileName = archive.FullName;
            //    int iDash = fileName.IndexOf('-');
            //    if (iDash != -1)
            //    {
            //        fileName = fileName.SubstringJava(0, iDash);
            //    }
            //    int iDot = fileName.IndexOf('.');
            //    if (iDot != -1) {
            //        fileName = fileName.SubstringJava(0, iDot);
            //    }
            //    FileInfo target = new File(archive.getParentFile(), fileName);

            //    try (GZIPInputStream in = new GZIPInputStream(
            //            new FileInputStream(archive))) {
            //        try (FileOutputStream out = new FileOutputStream(target)) {
            //            for (int c = in.read(); c != -1; c = in.read()) {
            //                out.write(c);
            //            }
            //        }
            //    }

            //    if (!target.getName().ToLower().contains(".exe")
            //            && target.exists()) {
            //        setFileExecutable(target);
            //    }
            throw new System.NotImplementedException("extract gzip not implemented");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>

        private void unTarGz(FileInfo archive)
        {
            //Archiver archiver = createArchiver(TAR, GZIP);
            //archiver.extract(archive, archive.getParentFile());
            //log.trace("unTarGz {}", archive);
            throw new System.NotImplementedException("extract tar.gz not implemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>
        private void unBZip2(FileInfo archive)
        {
            //Archiver archiver = createArchiver(TAR, BZIP2);
            //archiver.extract(archive, archive.getParentFile());
            //log.trace("Unbzip2 {}", archive);
            throw new System.NotImplementedException("extract bzip2 not implemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archive"></param>
        /// <exception cref="IOException"/>
        /// //InterruptedException
        private void extractMsi(FileInfo msi)
        {
            //        File tmpMsi = new File(
            //                createTempDirectory("").toFile().getAbsoluteFile() + separator
            //                        + msi.getName());
            //move(msi.toPath(), tmpMsi.toPath());
            //log.trace("Temporal msi file: {}", tmpMsi);

            //        Process process = getRuntime().exec(new String[] { "msiexec", "/a",
            //                tmpMsi.toString(), "/qb", "TARGETDIR=" + msi.getParent() });
            //        try {
            //            process.waitFor();
            //        } finally {
            //            process.destroy();
            //        }

            //        deleteFolder(tmpMsi.getParentFile());
            throw new System.NotImplementedException("extract msi not implemented");
        }

        protected void setFileExecutable(FileInfo file)
        {
            log.Trace("Setting file {0} as executable", file);
            //if (!file.setExecutable(true))
            //{
            //    log.warn("Error setting file {} as executable", file);
            //}
        }

        public void RenameFile(FileInfo from, FileInfo to)
        {
            log.Trace("Renaming file from {0} to {1}", from, to);
            if (to.Exists)
            {
                deleteFile(to);
            }
            try
            {
                from.MoveTo(to.FullName);
            }
            catch (System.Exception)
            {
                log.Warn("Error renaming file from {0} to {1}", from, to);
            }
        }

        protected void deleteFile(FileInfo file)
        {
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

        public void deleteFolder(DirectoryInfo folder)
        {
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