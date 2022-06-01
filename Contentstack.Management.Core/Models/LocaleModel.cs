using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LocaleModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "code")]
        public string Code { get; set; }
        [JsonProperty(propertyName: "fallback_locale")]
        public string FallbackLocale { get; set; }
    }
}
