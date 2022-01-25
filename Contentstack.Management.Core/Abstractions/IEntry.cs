using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Abstractions
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public interface IEntry
    {
        [JsonProperty(propertyName: "title")]
        string Title { get; set; }
    }
}
