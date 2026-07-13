using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Model for Taxonomy create/update and API response.
    /// </summary>
    public class TaxonomyModel
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        [JsonPropertyName("terms_count")]
        public int? TermsCount { get; set; }

        [JsonPropertyName("referenced_terms_count")]
        public int? ReferencedTermsCount { get; set; }

        [JsonPropertyName("referenced_entries_count")]
        public int? ReferencedEntriesCount { get; set; }

        [JsonPropertyName("referenced_content_type_count")]
        public int? ReferencedContentTypeCount { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }
    }
}
