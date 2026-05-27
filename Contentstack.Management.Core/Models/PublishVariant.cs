using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class PublishVariant
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyName("version")]
        public int? Version { get; set; }
    }
}
