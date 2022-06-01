using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LabelModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "parent")]
        public List<string> Parent { get; set; }
        [JsonProperty(propertyName: "content_types")]
        public List<string> ContentTypes { get; set; }
    }
}
