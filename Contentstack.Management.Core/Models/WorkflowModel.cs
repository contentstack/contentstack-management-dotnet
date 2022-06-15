using System.Collections.Generic;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WorkflowModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "enabled")]
        public bool Enabled { get; set; } = true;
        [JsonProperty(propertyName: "branches")]
        public List<string> Branches { get; set; }
        [JsonProperty(propertyName: "content_types")]
        public List<string> ContentTypes { get; set; }
        [JsonProperty(propertyName: "admin_users")]
        public Dictionary<string, object> AdminUsers { get; set; }

        [JsonProperty(propertyName: "workflow_stages")]
        public List<WorkflowStage> WorkflowStages { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WorkflowStage
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "color")]
        public string Color { get; set; }
        [JsonProperty(propertyName: "SYS_ACL")]
        public Dictionary<string, object> SystemACL { get; set; }
        [JsonProperty(propertyName: "next_available_stages")]
        public List<string> NextAvailableStages { get; set; }
        [JsonProperty(propertyName: "allStages")]
        public bool AllStages { get; set; } = true;
        [JsonProperty(propertyName: "allUsers")]
        public bool AllUsers { get; set; } = true;
        [JsonProperty(propertyName: "specificStages")]
        public bool SpecificStages { get; set; } = false;
        [JsonProperty(propertyName: "enabspecificUsersled")]
        public bool SpecificUsers { get; set; } = false;
        [JsonProperty(propertyName: "entry_lock")]
        public string EntryLock { get; set; }
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
    }
}
