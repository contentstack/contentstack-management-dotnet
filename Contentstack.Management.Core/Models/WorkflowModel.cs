using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WorkflowModel
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "enabled")]
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;
        [JsonProperty(propertyName: "branches")]
        [JsonPropertyName("branches")]
        public List<string> Branches { get; set; }
        [JsonProperty(propertyName: "content_types")]
        [JsonPropertyName("content_types")]
        public List<string> ContentTypes { get; set; }
        [JsonProperty(propertyName: "admin_users")]
        [JsonPropertyName("admin_users")]
        public Dictionary<string, object> AdminUsers { get; set; }

        [JsonProperty(propertyName: "workflow_stages")]
        [JsonPropertyName("workflow_stages")]
        public List<WorkflowStage> WorkflowStages { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WorkflowStage
    {
        [JsonProperty(propertyName: "uid")]
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "color")]
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonProperty(propertyName: "SYS_ACL")]
        [JsonPropertyName("SYS_ACL")]
        public Dictionary<string, object> SystemACL { get; set; }
        [JsonProperty(propertyName: "next_available_stages")]
        [JsonPropertyName("next_available_stages")]
        public List<string> NextAvailableStages { get; set; }
        [JsonProperty(propertyName: "allStages")]
        [JsonPropertyName("allStages")]
        public bool AllStages { get; set; } = true;
        [JsonProperty(propertyName: "allUsers")]
        [JsonPropertyName("allUsers")]
        public bool AllUsers { get; set; } = true;
        [JsonProperty(propertyName: "specificStages")]
        [JsonPropertyName("specificStages")]
        public bool SpecificStages { get; set; } = false;
        [JsonProperty(propertyName: "specificUsers")]
        [JsonPropertyName("specificUsers")]
        public bool SpecificUsers { get; set; } = false;
        [JsonProperty(propertyName: "entry_lock")]
        [JsonPropertyName("entry_lock")]
        public string EntryLock { get; set; }
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
