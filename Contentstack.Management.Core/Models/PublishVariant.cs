using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    public class PublishVariant
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("version")]
        public int? Version { get; set; }
    }
}
