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
