using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    /// <summary>
    /// JSON field (e.g. JSON RTE) in a content type schema.
    /// </summary>
    public class JsonField : TextboxField
    {
        [JsonPropertyName("reference_to")]
        public object ReferenceTo { get; set; }
    }
}
