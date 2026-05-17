using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LabelModel
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "parent")]
        [JsonPropertyName("parent")]
        public List<string> Parent { get; set; }
        [JsonProperty(propertyName: "content_types")]
        [JsonPropertyName("content_types")]
        public List<string> ContentTypes { get; set; }
    }
}
