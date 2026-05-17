using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LocaleModel
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonProperty(propertyName: "fallback_locale")]
        [JsonPropertyName("fallback_locale")]
        public string FallbackLocale { get; set; }
    }
}
