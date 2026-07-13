using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    /// <summary>
    /// Taxonomy field in a content type schema.
    /// </summary>
    public class TaxonomyField : Field
    {
        [JsonPropertyName("taxonomies")]
        public List<TaxonomyFieldBinding>? Taxonomies { get; set; }
    }

    /// <summary>
    /// Binding between a taxonomy field and a taxonomy definition.
    /// </summary>
    public class TaxonomyFieldBinding
    {
        [JsonPropertyName("taxonomy_uid")]
        public string? TaxonomyUid { get; set; }

        [JsonPropertyName("max_terms")]
        public int? MaxTerms { get; set; }

        [JsonPropertyName("mandatory")]
        public bool Mandatory { get; set; }

        [JsonPropertyName("multiple")]
        public bool Multiple { get; set; }

        [JsonPropertyName("non_localizable")]
        public bool NonLocalizable { get; set; }
    }
}
