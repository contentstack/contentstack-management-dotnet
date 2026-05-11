using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Tests.Model
{
    public class MultiPageEntry : IEntry
    {
        public const string ContentType = "multi_page";

        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("_content_type_uid")]
        public string ContentTypeUid { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
