using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    /// <summary>
    /// Numeric field in a content type schema.
    /// </summary>
    public class NumberField : Field
    {
        [JsonProperty(propertyName: "min")]
        public int? Min { get; set; }

        [JsonProperty(propertyName: "max")]
        public int? Max { get; set; }
    }
}
