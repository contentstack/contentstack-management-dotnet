using Contentstack.Management.Core.Models;
using Newtonsoft.Json;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Tests.Model
{
    public class MultiPageEntry : IEntry
    {
        public const string ContentType = "multi_page";

        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        
        [JsonProperty(propertyName: "_content_type_uid")]
        public string ContentTypeUid { get; set; }
        
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }
        
        [JsonProperty(propertyName: "url")]
        public string Url { get; set; }
    }
}
