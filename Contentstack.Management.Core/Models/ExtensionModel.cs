using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
        public class ExtensionModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("data_type")]
        public string DataType { get; set; }
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
        [JsonPropertyName("src")]
        public string Src { get; set; }
        [JsonPropertyName("srcdoc")]
        public string Srcdoc { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("config")]
        public string Config { get; set; }
        [JsonPropertyName("multiple")]
        public bool Multiple { get; set; }
        [JsonPropertyName("scope")]
        public ExtensionScope Scope { get; set; }
    }

    public class ExtensionScope
    {
        [JsonPropertyName("content_types")]
        public List<string> ContentTypes { get; set; }
    }
}
