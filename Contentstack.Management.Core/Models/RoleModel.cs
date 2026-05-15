using System.Collections.Generic;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RoleModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "rules")]
        public List<Rule> Rules { get; set; }

        [JsonProperty(propertyName: "deploy_content")]
        public bool DeployContent { get; set; } = true;
    }

    public class Rule
    {
        [JsonProperty(propertyName: "acl")]
        public Dictionary<string, object> ACL { get; }

        [JsonProperty(propertyName: "restrict")]
        public bool Restrict { get; }
    }

    public class ContentTypeRules: Rule
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; } = "content_type";

        [JsonProperty(propertyName: "content_types")]
        public List<string> ContentTypes { get; set; }
    }

    public class BranchRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; } = "branch";

        [JsonProperty(propertyName: "branches")]
        public List<string> Branches { get; set; }
    }

    public class BranchAliasRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; } = "branch_alias";

        [JsonProperty(propertyName: "branch_aliases")]
        public List<string> BranchAliases { get; set; }
    }

    public class AssetRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; } = "asset";

        [JsonProperty(propertyName: "assets")]
        public List<string> Assets { get; set; }
    }

    public class FolderRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; } = "folder";

        [JsonProperty(propertyName: "folders")]
        public List<string> Folders { get; set; }
    }

    public class EnvironmentRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; } = "environment";

        [JsonProperty(propertyName: "environments")]
        public List<string> Environments { get; set; }
    }

    public class TaxonomyContentType
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        [JsonProperty(propertyName: "acl")]
        public Dictionary<string, object> ACL { get; }
    }

    public class TaxonomyRules : Rule
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; } = "taxonomy";

        [JsonProperty(propertyName: "taxonomies")]
        public List<string> Taxonomies { get; set; }

        [JsonProperty(propertyName: "terms")]
        public List<string> Terms { get; set; }

        [JsonProperty(propertyName: "content_types")]
        public List<TaxonomyContentType> ContentTypes { get; set; }
    }
}
