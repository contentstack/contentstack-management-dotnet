using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PublishRuleModel
    {
        [JsonProperty(propertyName: "workflow")]
        [JsonPropertyName("workflow")]
        public string WorkflowUid { get; set; }
        [JsonProperty(propertyName: "actions")]
        [JsonPropertyName("actions")]
        public List<string> Actions { get; set; }
        [JsonProperty(propertyName: "branches")]
        [JsonPropertyName("branches")]
        public List<string> Branches { get; set; }
        [JsonProperty(propertyName: "content_types")]
        [JsonPropertyName("content_types")]
        public List<string> ContentTypes { get; set; }
        [JsonProperty(propertyName: "locales")]
        [JsonPropertyName("locales")]
        public List<string> Locales { get; set; }
        [JsonProperty(propertyName: "environment")]
        [JsonPropertyName("environment")]
        public string Environment { get; set; }
        [JsonProperty(propertyName: "approvers")]
        [JsonPropertyName("approvers")]
        public Approvals Approvers { get; set; }
        [JsonProperty(propertyName: "workflow_stage")]
        [JsonPropertyName("workflow_stage")]
        public string WorkflowStageUid { get; set; }
        [JsonProperty(propertyName: "disable_approver_publishing")]
        [JsonPropertyName("disable_approver_publishing")]
        public bool DisableApproval { get; set; } = false;
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Approvals
    {
        [JsonProperty(propertyName: "users")]
        [JsonPropertyName("users")]
        public List<string> Users { get; set; }
        [JsonProperty(propertyName: "roles")]
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }
    }
}
