using System.Collections.Generic;
using Contentstack.Management.Core.Models.Fields;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ContentModelling
    {
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }

        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        [JsonProperty(propertyName: "field_rules")]
        public List<FieldRules> FieldRules { get; set; }

        [JsonProperty(propertyName: "schema")]
        public List<Field> Schema { get; set; }

        [JsonProperty(propertyName: "options")]
        public Option Options { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Option
    {
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }

        [JsonProperty(propertyName: "sub_title")]
        public List<string> SubTitle { get; set; }

        [JsonProperty(propertyName: "singleton")]
        public bool Singleton { get; set; }

        [JsonProperty(propertyName: "is_page")]
        public bool IsPage { get; set; }

        [JsonProperty(propertyName: "url_pattern")]
        public string UrlPattern { get; set; }

        [JsonProperty(propertyName: "url_prefix")]
        public string UrlPrefix { get; set; }

    }    
}
