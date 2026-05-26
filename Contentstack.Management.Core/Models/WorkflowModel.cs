using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    public class WorkflowModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;
        [JsonPropertyName("branches")]
        public List<string> Branches { get; set; }
        [JsonPropertyName("content_types")]
        public List<string> ContentTypes { get; set; }
        [JsonPropertyName("admin_users")]
        public Dictionary<string, object> AdminUsers { get; set; }

        [JsonPropertyName("workflow_stages")]
        public List<WorkflowStage> WorkflowStages { get; set; }
    }

    public class WorkflowStage
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("SYS_ACL")]
        public Dictionary<string, object> SystemACL { get; set; }
        [JsonPropertyName("next_available_stages")]
        public List<string> NextAvailableStages { get; set; }
        [JsonPropertyName("allStages")]
        public bool AllStages { get; set; } = true;
        [JsonPropertyName("allUsers")]
        public bool AllUsers { get; set; } = true;
        [JsonPropertyName("specificStages")]
        public bool SpecificStages { get; set; } = false;
        [JsonPropertyName("specificUsers")]
        public bool SpecificUsers { get; set; } = false;
        [JsonPropertyName("entry_lock")]
        public string EntryLock { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
