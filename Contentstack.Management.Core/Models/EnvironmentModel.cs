using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EnvironmentModel
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "servers")]
        [JsonPropertyName("servers")]
        public List<Server> Servers { get; set; }
        [JsonProperty(propertyName: "urls")]
        [JsonPropertyName("urls")]
        public List<LocalesUrl> Urls { get; set; }
        [JsonProperty(propertyName: "deploy_content")]
        [JsonPropertyName("deploy_content")]
        public bool DeployContent { get; set; } = true;
        
    }

    public class Server
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class LocalesUrl
    {
        [JsonProperty(propertyName: "url")]
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonProperty(propertyName: "locale")]
        [JsonPropertyName("locale")]
        public string Locale { get; set; }
    }
}
