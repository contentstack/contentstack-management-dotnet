using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    public class WebhookModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("destinations")]
        public List<WebhookTarget>? destinations { get; set; }
        [JsonPropertyName("channels")]
        public List<string>? Channels { get; set; }
        [JsonPropertyName("branches")]
        public List<string>? Branches { get; set; }
        [JsonPropertyName("retry_policy")]
        public string? RetryPolicy { get; set; }
        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; } = false;
        [JsonPropertyName("concise_payload")]
        public bool ConcisePayload { get; set; } = true;
    }

    public class WebhookTarget
    {
        [JsonPropertyName("target_url")]
        public string? TargetUrl { get; set; }
        [JsonPropertyName("http_basic_auth")]
        public string? HttpBasicAuth { get; set; }
        [JsonPropertyName("http_basic_password")]
        public string? HttpBasicPassword { get; set; }
        [JsonPropertyName("custom_header")]
        public List<Dictionary<string, object>>? CustomHeader { get; set; }
    }
}
