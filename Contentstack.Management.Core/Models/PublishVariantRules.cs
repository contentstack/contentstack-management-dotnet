using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    public class PublishVariantRules
    {
        [JsonProperty("publish_latest_base")]
        public bool? PublishLatestBase { get; set; }

        [JsonProperty("publish_latest_base_conditionally")]
        public bool? PublishLatestBaseConditionally { get; set; }
    }
}
