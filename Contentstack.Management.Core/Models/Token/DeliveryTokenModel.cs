using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Token
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ManagementTokenModel
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonProperty(propertyName: "Scope")]
        [JsonPropertyName("Scope")]
        public List<TokenScope> Scope { get; set; }
        [JsonProperty(propertyName: "expires_on")]
        [JsonPropertyName("expires_on")]
        public string ExpiresOn { get; set; }
        [JsonProperty(propertyName: "is_email_notification_enabled")]
        [JsonPropertyName("is_email_notification_enabled")]
        public bool IsEmailNotificationEnabled { get; set; } = false;
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeliveryTokenModel
    {
        [JsonProperty(propertyName: "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonProperty(propertyName: "scope")]
        [JsonPropertyName("scope")]
        public List<DeliveryTokenScope> Scope { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeliveryTokenScope: TokenScope
    {
        [JsonProperty(propertyName: "environments")]
        [JsonPropertyName("environments")]
        public List<string> Environments { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TokenScope
    {
        [JsonProperty(propertyName: "module")]
        [JsonPropertyName("module")]
        public string Module { get; set; }
        [JsonProperty(propertyName: "acl")]
        [JsonPropertyName("acl")]
        public Dictionary<string, string> ACL { get; set; }
        [JsonProperty(propertyName: "branches")]
        [JsonPropertyName("branches")]
        public List<string> Branches { get; set; }
    }
}
