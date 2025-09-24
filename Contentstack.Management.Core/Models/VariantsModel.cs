using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class VariantsModel
    {
        [JsonProperty(propertyName: "title")]
        string Title { get; set; }
    }
}
