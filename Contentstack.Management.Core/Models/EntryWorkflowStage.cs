using System.Collections.Generic;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntryWorkflowStage
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "comment")]
        public string Comment { get; set; }
        [JsonProperty(propertyName: "due_date")]
        public string DueDate { get; set; }
        [JsonProperty(propertyName: "notify")]
        public bool Notify { get; set; } = true;
        [JsonProperty(propertyName: "assigned_to")]
        public List<AssignToUser> AssignedTo;
        [JsonProperty(propertyName: "assigned_by_roles")]
        public List<AssignByRole> AssignedByRoles;
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AssignToUser
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "email")]
        public string Email { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AssignByRole
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EntryPublishAction
    {
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "action")]
        public string Action { get; set; }
        [JsonProperty(propertyName: "comment")]
        public string Comment { get; set; }
        [JsonProperty(propertyName: "notify")]
        public bool Notify { get; set; } = true;
        [JsonProperty(propertyName: "status")]
        public int Status { get; set; }

    }
}
