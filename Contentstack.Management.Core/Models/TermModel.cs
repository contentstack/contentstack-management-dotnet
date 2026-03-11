using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Model for Term create/update and API response.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TermModel
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "taxonomy_uid")]
        public string TaxonomyUid { get; set; }

        [JsonProperty(propertyName: "parent_uid")]
        public string ParentUid { get; set; }

        [JsonProperty(propertyName: "depth")]
        public int? Depth { get; set; }

        [JsonProperty(propertyName: "children_count")]
        public int? ChildrenCount { get; set; }

        [JsonProperty(propertyName: "referenced_entries_count")]
        public int? ReferencedEntriesCount { get; set; }

        [JsonProperty(propertyName: "ancestors")]
        public List<TermAncestorDescendant> Ancestors { get; set; }

        [JsonProperty(propertyName: "descendants")]
        public List<TermAncestorDescendant> Descendants { get; set; }

        [JsonProperty(propertyName: "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(propertyName: "updated_at")]
        public string UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents an ancestor or descendant term in hierarchy.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TermAncestorDescendant
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "parent_uid")]
        public string ParentUid { get; set; }

        [JsonProperty(propertyName: "depth")]
        public int? Depth { get; set; }

        [JsonProperty(propertyName: "children_count")]
        public int? ChildrenCount { get; set; }

        [JsonProperty(propertyName: "referenced_entries_count")]
        public int? ReferencedEntriesCount { get; set; }
    }

    /// <summary>
    /// Model for Term move operation (parent_uid, order).
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TermMoveModel
    {
        [JsonProperty(propertyName: "parent_uid")]
        public string ParentUid { get; set; }

        [JsonProperty(propertyName: "order")]
        public int? Order { get; set; }
    }
}
