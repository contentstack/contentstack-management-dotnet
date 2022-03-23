using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Abstractions
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public interface ILocale
    {
        [JsonProperty(propertyName: "name")]
        string Name{ get; set; }
        [JsonProperty(propertyName: "code")]
        string Code { get; set; }
        [JsonProperty(propertyName: "fallback_locale")]
        string FallbackLocale { get; set; }
    }
}
