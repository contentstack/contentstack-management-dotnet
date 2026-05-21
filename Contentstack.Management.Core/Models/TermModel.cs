using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Model for Term create/update and API response.
    /// </summary>
    public class TermModel
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("taxonomy_uid")]
        public string TaxonomyUid { get; set; }

        [JsonPropertyName("parent_uid")]
        public string ParentUid { get; set; }

        [JsonPropertyName("depth")]
        public int? Depth { get; set; }

        [JsonPropertyName("children_count")]
        public int? ChildrenCount { get; set; }

        [JsonPropertyName("referenced_entries_count")]
        public int? ReferencedEntriesCount { get; set; }

        [JsonPropertyName("ancestors")]
        public List<TermAncestorDescendant> Ancestors { get; set; }

        [JsonPropertyName("descendants")]
        public List<TermAncestorDescendant> Descendants { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents an ancestor or descendant term in hierarchy.
    /// </summary>
    public class TermAncestorDescendant
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parent_uid")]
        public string ParentUid { get; set; }

        [JsonPropertyName("depth")]
        public int? Depth { get; set; }

        [JsonPropertyName("children_count")]
        public int? ChildrenCount { get; set; }

        [JsonPropertyName("referenced_entries_count")]
        public int? ReferencedEntriesCount { get; set; }
    }

    /// <summary>
    /// Model for Term move operation (parent_uid, order).
    /// </summary>
    public class TermMoveModel
    {
        [JsonPropertyName("parent_uid")]
        public string ParentUid { get; set; }

        [JsonPropertyName("order")]
        public int? Order { get; set; }
    }
}
