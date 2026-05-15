using System.Collections.Generic;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ReleaseModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "locked")]
        public bool Locked { get; set; }

        [JsonProperty(propertyName: "archived")]
        public bool Archived { get; set; }

    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeployModel
    {
        [JsonProperty(propertyName: "environments")]
        public List<string> Environments { get; set; }

        [JsonProperty(propertyName: "locales")]
        public List<string> Locales { get; set; }

        [JsonProperty(propertyName: "scheduledAt")]
        public string ScheduledAt { get; set; }

        [JsonProperty(propertyName: "action")]
        public string Action { get; set; }

    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ReleaseItemModel
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        [JsonProperty(propertyName: "version")]
        public int Version { get; set; }

        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }

        [JsonProperty(propertyName: "content_type_uid")]
        public string ContentTypeUID { get; set; }

        [JsonProperty(propertyName: "action")]
        public string Action { get; set; }
    }
}
