using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Contentstack.Management.Core.Models
{
        public class LabelModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("parent")]
        public List<string> Parent { get; set; }
        [JsonPropertyName("content_types")]
        public List<string> ContentTypes { get; set; }
    }
}
