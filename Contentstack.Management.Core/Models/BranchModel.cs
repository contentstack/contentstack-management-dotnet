using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class BranchAliasModel
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }

    /// <summary>
    /// Publish configuration for a branch. <see cref="Environments"/> lists the environment names (not UIDs) to clone.
    /// </summary>
    public class BranchPublishConfigModel
    {
        /// <summary>
        /// Environment names to clone into the branch. An empty list clones none; leaving the publish config unset clones all environments.
        /// The list is sent unchanged; the SDK does not validate the names or limit the list length.
        /// </summary>
        [JsonPropertyName("environments")]
        public List<string>? Environments { get; set; }
    }

    public class BranchModel
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("created_by")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("updated_by")]
        public string? UpdatedBy { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("alias")]
        public List<BranchAliasModel>? Alias { get; set; }

        [JsonPropertyName("publish_config")]
        public BranchPublishConfigModel? PublishConfig { get; set; }
    }
}

