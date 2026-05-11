using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Tests.Model
{
    public partial class PageJSONRTE : IEntry
    {
        public const string ContentType = "page_json_rte";

        [JsonPropertyName("uid")]
        public string Uid { get; set; }
        [JsonPropertyName("_content_type_uid")]
        public string ContentTypeUid { get; set; }
        public string Title { get; set; }
        [JsonPropertyName("rte_data")]
        public Node RteData { get; set; }
    }
}
