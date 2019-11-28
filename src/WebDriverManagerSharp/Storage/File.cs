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

namespace WebDriverManagerSharp.Storage
{
    using System;
    using System.IO;

    public class File : IFile
    {
        private FileInfo fileInfo;

        public File(string fullPath)
        {
            fileInfo = new FileInfo(fullPath);
        }

        public string Name
        {
            get
            {
                return fileInfo.Name;
            }
        }

        public string FullName
        {
            get
            {
                return fileInfo.FullName;
            }
        }

        public string Extension
        {
            get
            {
                return fileInfo.Extension;
            }
        }

        public IDirectory ParentDirectory
        {
            get
            {
                return new Directory(fileInfo.DirectoryName);
            }
        }

        public void Delete()
        {
            fileInfo.Delete();
        }

        public bool Exists
        {
            get
            {
                return fileInfo.Exists;
            }
        }

        public void CreateFromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (Stream stream = fileInfo.OpenWrite())
            {
                source.CopyTo(stream);
            }
        }

        public void MoveTo(string fullPath)
        {
            fileInfo.MoveTo(fullPath);
        }
    }
}
