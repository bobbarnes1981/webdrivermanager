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

using Newtonsoft.Json;

namespace WebDriverManager.GitHubApi
{
    public class Asset
    {
        // url
        // id
        // node_id
        // name
        // label
        // uploader (user)
        // content_type
        // state
        // size
        // download_count
        // created_at
        // updated_at
        [JsonProperty("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }
}
