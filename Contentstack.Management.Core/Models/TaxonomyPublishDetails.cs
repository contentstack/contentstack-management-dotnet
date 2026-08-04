using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Details for publishing or unpublishing one or more taxonomies.
    /// </summary>
    public class TaxonomyPublishDetails
    {
        [JsonPropertyName("locales")]
        public List<string>? Locales { get; set; }

        [JsonPropertyName("environments")]
        public List<string>? Environments { get; set; }

        [JsonPropertyName("scheduled_at")]
        public string? ScheduledAt { get; set; }

        [JsonPropertyName("items")]
        public List<TaxonomyPublishItem>? Items { get; set; }
    }

    /// <summary>
    /// Identifies a taxonomy to publish or unpublish by UID.
    /// </summary>
    public class TaxonomyPublishItem
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; } = null!;
    }
}
