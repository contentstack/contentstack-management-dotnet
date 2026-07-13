using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    /// <summary>
    /// Numeric field in a content type schema.
    /// </summary>
    public class NumberField : Field
    {
        [JsonPropertyName("min")]
        public int? Min { get; set; }

        [JsonPropertyName("max")]
        public int? Max { get; set; }
    }
}
