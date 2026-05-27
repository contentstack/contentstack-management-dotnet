using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Contentstack.Management.Core.Models
{
    public class EntryWorkflowStage
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
        [JsonPropertyName("due_date")]
        public string? DueDate { get; set; }
        [JsonPropertyName("notify")]
        public bool Notify { get; set; } = true;
        [JsonPropertyName("assigned_to")]
        public List<AssignToUser>? AssignedTo { get; set; }
        [JsonPropertyName("assigned_by_roles")]
        public List<AssignByRole>? AssignedByRoles { get; set; }
    }

    public class AssignToUser
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class AssignByRole
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class EntryPublishAction
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
        [JsonPropertyName("action")]
        public string? Action { get; set; }
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
        [JsonPropertyName("notify")]
        public bool Notify { get; set; } = true;
        [JsonPropertyName("status")]
        public int Status { get; set; }

    }
}
