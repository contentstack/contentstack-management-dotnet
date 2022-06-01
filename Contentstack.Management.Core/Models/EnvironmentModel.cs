using System.Collections.Generic;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EnvironmentModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "servers")]
        public List<Server> Servers { get; set; }
        [JsonProperty(propertyName: "urls")]
        public List<LocalesUrl> Urls { get; set; }
        [JsonProperty(propertyName: "deploy_content")]
        public bool DeployContent { get; set; } = true;
        
    }

    public class Server
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
    }

    public class LocalesUrl
    {
        [JsonProperty(propertyName: "url")]
        public string Url { get; set; }
        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }
    }
}
