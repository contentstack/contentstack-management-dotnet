using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class LocaleModel
    {
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }

        [JsonPropertyName("fallback_locale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FallbackLocale { get; set; }
    }
}
