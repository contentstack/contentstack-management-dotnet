using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Request body for bulk taxonomy publish and unpublish operations.
    /// </summary>
    public class TaxonomyPublishModel
    {
        /// <summary>Target locale codes, e.g. ["en-us", "fr-fr"]. Required.</summary>
        [JsonPropertyName("locales")]
        public List<string>? Locales { get; set; }

        /// <summary>Target environment UIDs. Required.</summary>
        [JsonPropertyName("environments")]
        public List<string>? Environments { get; set; }

        /// <summary>Taxonomy UIDs to publish/unpublish.</summary>
        [JsonPropertyName("items")]
        public List<TaxonomyPublishItem>? Items { get; set; }

        /// <summary>Optional ISO-8601 timestamp for scheduled publishing (Phase 2).</summary>
        [JsonPropertyName("scheduled_at")]
        public string? ScheduledAt { get; set; }
    }

    /// <summary>A single taxonomy item reference in a publish/unpublish request.</summary>
    public class TaxonomyPublishItem
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }
}
