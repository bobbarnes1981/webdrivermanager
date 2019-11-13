/*
 * (C) Copyright 2019 Boni Garcia (http://bonigarcia.github.io/)
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
    using System.Collections.Generic;

    /**
     * Void manager.
     *
     * @author Boni Garcia (boni.gg@gmail.com)
     * @since 3.2.0
     */
    public class VoidDriverManager : WebDriverManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"/>
        /// <returns></returns>
        protected override List<System.Uri> GetDrivers()
        {
            return new List<System.Uri>();
        }

        protected override string GetBrowserVersion()
        {
            return null;
        }

        protected override string GetDriverVersion()
        {
            return string.Empty;
        }

        protected override System.Uri GetDriverUrl()
        {
            return null;
        }

        protected override System.Uri GetMirrorUrl()
        {
            return null;
        }

        protected override string GetExportParameter()
        {
            return null;
        }

        protected override DriverManagerType? GetDriverManagerType()
        {
            return null;
        }

        protected override string GetDriverName()
        {
            return string.Empty;
        }

        protected override void SetDriverVersion(string version)
        {
            // Nothing required
        }

        protected override void SetDriverUrl(System.Uri url)
        {
            // Nothing required
        }
    }
}