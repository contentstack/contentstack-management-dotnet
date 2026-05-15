using Contentstack.Management.Core.Models;
using Newtonsoft.Json;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Tests.Model
{
    public partial class PageJSONRTE : IEntry
    {
        public const string ContentType = "page_json_rte";

        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "_content_type_uid")]
        public string ContentTypeUid { get; set; }
        public string Title { get; set; }
        [JsonProperty(propertyName: "rte_data")]
        public Node RteData { get; set; }
    }
}