using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    public class PublishRuleModel
    {
        [JsonPropertyName("workflow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? WorkflowUid { get; set; }
        [JsonPropertyName("actions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Actions { get; set; }
        [JsonPropertyName("branches")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Branches { get; set; }
        [JsonPropertyName("content_types")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? ContentTypes { get; set; }
        [JsonPropertyName("locales")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Locales { get; set; }
        [JsonPropertyName("environment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Environment { get; set; }
        [JsonPropertyName("approvers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Approvals? Approvers { get; set; }
        [JsonPropertyName("workflow_stage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? WorkflowStageUid { get; set; }
        [JsonPropertyName("disable_approver_publishing")]
        public bool DisableApproval { get; set; } = false;
    }

    public class Approvals
    {
        [JsonPropertyName("users")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Users { get; set; }
        [JsonPropertyName("roles")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Roles { get; set; }
    }
}
