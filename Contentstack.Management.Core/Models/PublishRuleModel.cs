using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    public class PublishRuleModel
    {
        [JsonPropertyName("workflow")]
        public string? WorkflowUid { get; set; }
        [JsonPropertyName("actions")]
        public List<string>? Actions { get; set; }
        [JsonPropertyName("branches")]
        public List<string>? Branches { get; set; }
        [JsonPropertyName("content_types")]
        public List<string>? ContentTypes { get; set; }
        [JsonPropertyName("locales")]
        public List<string>? Locales { get; set; }
        [JsonPropertyName("environment")]
        public string? Environment { get; set; }
        [JsonPropertyName("approvers")]
        public Approvals? Approvers { get; set; }
        [JsonPropertyName("workflow_stage")]
        public string? WorkflowStageUid { get; set; }
        [JsonPropertyName("disable_approver_publishing")]
        public bool DisableApproval { get; set; } = false;
    }

    public class Approvals
    {
        [JsonPropertyName("users")]
        public List<string>? Users { get; set; }
        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }
    }
}
