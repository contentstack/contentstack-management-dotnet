using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models.Fields;

namespace Contentstack.Management.Core.Models
{
    public class ContentModelling
    {
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        [JsonPropertyName("uid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Uid { get; set; }

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        [JsonPropertyName("field_rules")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<FieldRules>? FieldRules { get; set; }

        [JsonPropertyName("schema")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Field>? Schema { get; set; }

        [JsonPropertyName("global_field_refs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GlobalFieldRefs>? GlobalFieldRefs { get; set; }

        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Option? Options { get; set; }
    }

    public class Option
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("sub_title")]
        public List<string>? SubTitle { get; set; }

        [JsonPropertyName("singleton")]
        public bool Singleton { get; set; }

        [JsonPropertyName("is_page")]
        public bool IsPage { get; set; }

        [JsonPropertyName("url_pattern")]
        public string? UrlPattern { get; set; }

        [JsonPropertyName("url_prefix")]
        public string? UrlPrefix { get; set; }

    }    
}
