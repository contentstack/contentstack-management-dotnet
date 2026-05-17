using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RoleModel
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "rules")]
        [JsonPropertyName("rules")]
        public List<Rule> Rules { get; set; }

        [JsonProperty(propertyName: "deploy_content")]
        [JsonPropertyName("deploy_content")]
        public bool DeployContent { get; set; } = true;
    }

    public class Rule
    {
        [JsonProperty(propertyName: "acl")]
        [JsonPropertyName("acl")]
        public Dictionary<string, object> ACL { get; }

        [JsonProperty(propertyName: "restrict")]
        [JsonPropertyName("restrict")]
        public bool Restrict { get; }
    }

    public class ContentTypeRules: Rule
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; } = "content_type";

        [JsonProperty(propertyName: "content_types")]
        [JsonPropertyName("content_types")]
        public List<string> ContentTypes { get; set; }
    }

    public class BranchRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; } = "branch";

        [JsonProperty(propertyName: "branches")]
        [JsonPropertyName("branches")]
        public List<string> Branches { get; set; }
    }

    public class BranchAliasRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; } = "branch_alias";

        [JsonProperty(propertyName: "branch_aliases")]
        [JsonPropertyName("branch_aliases")]
        public List<string> BranchAliases { get; set; }
    }

    public class AssetRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; } = "asset";

        [JsonProperty(propertyName: "assets")]
        [JsonPropertyName("assets")]
        public List<string> Assets { get; set; }
    }

    public class FolderRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; } = "folder";

        [JsonProperty(propertyName: "folders")]
        [JsonPropertyName("folders")]
        public List<string> Folders { get; set; }
    }

    public class EnvironmentRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; } = "environment";

        [JsonProperty(propertyName: "environments")]
        [JsonPropertyName("environments")]
        public List<string> Environments { get; set; }
    }

    public class TaxonomyContentType
    {
        [JsonProperty(propertyName: "uid")]
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonProperty(propertyName: "acl")]
        [JsonPropertyName("acl")]
        public Dictionary<string, object> ACL { get; }
    }

    public class TaxonomyRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; } = "taxonomy";

        [JsonProperty(propertyName: "taxonomies")]
        [JsonPropertyName("taxonomies")]
        public List<string> Taxonomies { get; set; }

        [JsonProperty(propertyName: "terms")]
        [JsonPropertyName("terms")]
        public List<string> Terms { get; set; }

        [JsonProperty(propertyName: "content_types")]
        [JsonPropertyName("content_types")]
        public List<TaxonomyContentType> ContentTypes { get; set; }
    }
}
