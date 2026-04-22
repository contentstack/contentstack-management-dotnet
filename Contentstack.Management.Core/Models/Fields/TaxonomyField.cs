using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    /// <summary>
    /// Taxonomy field in a content type schema.
    /// </summary>
    public class TaxonomyField : Field
    {
        [JsonProperty(propertyName: "taxonomies")]
        public List<TaxonomyFieldBinding> Taxonomies { get; set; }
    }

    /// <summary>
    /// Binding between a taxonomy field and a taxonomy definition.
    /// </summary>
    public class TaxonomyFieldBinding
    {
        [JsonProperty(propertyName: "taxonomy_uid")]
        public string TaxonomyUid { get; set; }

        [JsonProperty(propertyName: "max_terms")]
        public int? MaxTerms { get; set; }

        [JsonProperty(propertyName: "mandatory")]
        public bool Mandatory { get; set; }

        [JsonProperty(propertyName: "multiple")]
        public bool Multiple { get; set; }

        [JsonProperty(propertyName: "non_localizable")]
        public bool NonLocalizable { get; set; }
    }
}
