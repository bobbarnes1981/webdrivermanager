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

namespace WebDriverManagerSharp.Web
{
    using System;
    using System.IO;

    public interface IDownloader
    {
        /// <summary>
        /// Download the driver from the provided Uri
        /// </summary>
        /// <param name="url">Url of driver to download</param>
        /// <param name="version">Required driver version</param>
        /// <param name="driverName">Required driver name</param>
        /// <exception cref="IOException" />
        /// <returns></returns>
        FileInfo Download(Uri url, string version, string driverName);

        FileInfo GetTarget(string version, Uri url);

        string GetTargetPath();
    }
}