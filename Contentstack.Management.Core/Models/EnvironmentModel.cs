using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class EnvironmentModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("servers")]
        public List<Server>? Servers { get; set; }

        [JsonPropertyName("urls")]
        public List<LocalesUrl>? Urls { get; set; }
        
        [JsonPropertyName("deploy_content")]
        public bool DeployContent { get; set; } = true;
    }

    public class Server
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class LocalesUrl
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
    }
}
