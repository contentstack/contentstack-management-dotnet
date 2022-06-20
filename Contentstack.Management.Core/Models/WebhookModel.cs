using System.Collections.Generic;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WebhookModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "destinations")]
        public List<WebhookTarget> destinations { get; set; }
        [JsonProperty(propertyName: "channels")]
        public List<string> Channels { get; set; }
        [JsonProperty(propertyName: "branches")]
        public List<string> Branches { get; set; }
        [JsonProperty(propertyName: "retry_policy")]
        public string RetryPolicy { get; set; }
        [JsonProperty(propertyName: "disabled")]
        public bool Disabled { get; set; } = false;
        [JsonProperty(propertyName: "concise_payload")]
        public bool ConcisePayload { get; set; } = true;
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WebhookTarget
    {
        [JsonProperty(propertyName: "target_url")]
        public string TargetUrl { get; set; }
        [JsonProperty(propertyName: "http_basic_auth")]
        public string HttpBasicAuth { get; set; }
        [JsonProperty(propertyName: "http_basic_password")]
        public string HttpBasicPassword { get; set; }
        [JsonProperty(propertyName: "custom_header")]
        public List<Dictionary<string, object>> CustomHeader { get; set; }
    }
}
