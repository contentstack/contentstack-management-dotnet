using Newtonsoft.Json;
using System.Collections.Generic;

namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ExtensionModel
    {
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }
        [JsonProperty(propertyName: "data_type")]
        public string DataType { get; set; }
        [JsonProperty(propertyName: "tags")]
        public List<string> Tags { get; set; }
        [JsonProperty(propertyName: "src")]
        public string Src { get; set; }
        [JsonProperty(propertyName: "srcdoc")]
        public string Srcdoc { get; set; }
        [JsonProperty(propertyName: "type")]
        public string Type { get; set; }
        [JsonProperty(propertyName: "config")]
        public string Config { get; set; }
        [JsonProperty(propertyName: "multiple")]
        public bool Multiple { get; set; }
        [JsonProperty(propertyName: "scope")]
        public ExtensionScope Scope { get; set; }
    }

    public class ExtensionScope
    {
        [JsonProperty(propertyName: "content_types")]
        public List<string> ContentTypes { get; set; }
    }
}
