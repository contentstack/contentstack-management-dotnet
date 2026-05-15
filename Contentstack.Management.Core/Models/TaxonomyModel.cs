using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Model for Taxonomy create/update and API response.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TaxonomyModel
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }

        [JsonProperty(propertyName: "terms_count")]
        public int? TermsCount { get; set; }

        [JsonProperty(propertyName: "referenced_terms_count")]
        public int? ReferencedTermsCount { get; set; }

        [JsonProperty(propertyName: "referenced_entries_count")]
        public int? ReferencedEntriesCount { get; set; }

        [JsonProperty(propertyName: "referenced_content_type_count")]
        public int? ReferencedContentTypeCount { get; set; }

        [JsonProperty(propertyName: "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(propertyName: "updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty(propertyName: "uuid")]
        public string Uuid { get; set; }
    }
}
