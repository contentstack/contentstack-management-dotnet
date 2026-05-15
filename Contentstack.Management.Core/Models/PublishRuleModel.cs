using System.Collections.Generic;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PublishRuleModel
    {
        [JsonProperty(propertyName: "workflow")]
        public string WorkflowUid { get; set; }
        [JsonProperty(propertyName: "actions")]
        public List<string> Actions { get; set; }
        [JsonProperty(propertyName: "branches")]
        public List<string> Branches { get; set; }
        [JsonProperty(propertyName: "content_types")]
        public List<string> ContentTypes { get; set; }
        [JsonProperty(propertyName: "locales")]
        public List<string> Locales { get; set; }
        [JsonProperty(propertyName: "environment")]
        public string Environment { get; set; }
        [JsonProperty(propertyName: "approvers")]
        public Approvals Approvers { get; set; }
        [JsonProperty(propertyName: "workflow_stage")]
        public string WorkflowStageUid { get; set; }
        [JsonProperty(propertyName: "disable_approver_publishing")]
        public bool DisableApproval { get; set; } = false;
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Approvals
    {
        [JsonProperty(propertyName: "users")]
        public List<string> Users { get; set; }
        [JsonProperty(propertyName: "roles")]
        public List<string> Roles { get; set; }
    }
}
