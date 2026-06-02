using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    public class RoleModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("rules")]
        public List<Rule>? Rules { get; set; }

        [JsonPropertyName("deploy_content")]
        public bool DeployContent { get; set; } = true;
    }

    public class Rule
    {
        [JsonPropertyName("acl")]
        public Dictionary<string, object>? ACL { get; }

        [JsonPropertyName("restrict")]
        public bool Restrict { get; }
    }

    public class ContentTypeRules: Rule
    {
        [JsonPropertyName("module")]
        public string Module { get; } = "content_type";

        [JsonPropertyName("content_types")]
        public List<string>? ContentTypes { get; set; }
    }

    public class BranchRules : Rule
    {
        [JsonPropertyName("module")]
        public string Module { get; } = "branch";

        [JsonPropertyName("branches")]
        public List<string>? Branches { get; set; }
    }

    public class BranchAliasRules : Rule
    {
        [JsonPropertyName("module")]
        public string Module { get; } = "branch_alias";

        [JsonPropertyName("branch_aliases")]
        public List<string>? BranchAliases { get; set; }
    }

    public class AssetRules : Rule
    {
        [JsonPropertyName("module")]
        public string Module { get; } = "asset";

        [JsonPropertyName("assets")]
        public List<string>? Assets { get; set; }
    }

    public class FolderRules : Rule
    {
        [JsonPropertyName("module")]
        public string Module { get; } = "folder";

        [JsonPropertyName("folders")]
        public List<string>? Folders { get; set; }
    }

    public class EnvironmentRules : Rule
    {
        [JsonPropertyName("module")]
        public string Module { get; } = "environment";

        [JsonPropertyName("environments")]
        public List<string>? Environments { get; set; }
    }

    public class TaxonomyContentType
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyName("acl")]
        public Dictionary<string, object>? ACL { get; }
    }

    public class TaxonomyRules : Rule
    {
        [JsonPropertyName("module")]
        public string Module { get; } = "taxonomy";

        [JsonPropertyName("taxonomies")]
        public List<string>? Taxonomies { get; set; }

        [JsonPropertyName("terms")]
        public List<string>? Terms { get; set; }

        [JsonPropertyName("content_types")]
        public List<TaxonomyContentType>? ContentTypes { get; set; }
    }
}
