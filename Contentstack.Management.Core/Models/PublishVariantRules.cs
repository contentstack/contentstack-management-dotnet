using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class PublishVariantRules
    {
        [JsonPropertyName("publish_latest_base")]
        public bool? PublishLatestBase { get; set; }

        [JsonPropertyName("publish_latest_base_conditionally")]
        public bool? PublishLatestBaseConditionally { get; set; }
    }
}
