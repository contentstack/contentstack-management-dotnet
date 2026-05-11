using System.Collections.Generic;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models.Fields;
namespace Contentstack.Management.Core.Models
{
    public class ContentModelling
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("field_rules")]
        public List<FieldRules> FieldRules { get; set; }

        [JsonPropertyName("schema")]
        public List<Field> Schema { get; set; }

        [JsonPropertyName("global_field_refs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GlobalFieldRefs> GlobalFieldRefs { get; set; }

        [JsonPropertyName("options")]
        public Option Options { get; set; }
    }

    public class Option
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("sub_title")]
        public List<string> SubTitle { get; set; }

        [JsonPropertyName("singleton")]
        public bool Singleton { get; set; }

        [JsonPropertyName("is_page")]
        public bool IsPage { get; set; }

        [JsonPropertyName("url_pattern")]
        public string UrlPattern { get; set; }

        [JsonPropertyName("url_prefix")]
        public string UrlPrefix { get; set; }
    }
}
