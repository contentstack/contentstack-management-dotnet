using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Token
{
        public class ManagementTokenModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("Scope")]
        public List<TokenScope> Scope { get; set; }
        [JsonPropertyName("expires_on")]
        public string ExpiresOn { get; set; }
        [JsonPropertyName("is_email_notification_enabled")]
        public bool IsEmailNotificationEnabled { get; set; } = false;
    }

        public class DeliveryTokenModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("scope")]
        public List<DeliveryTokenScope> Scope { get; set; }
    }

        public class DeliveryTokenScope: TokenScope
    {
        [JsonPropertyName("environments")]
        public List<string> Environments { get; set; }
    }

        public class TokenScope
    {
        [JsonPropertyName("module")]
        public string Module { get; set; }
        [JsonPropertyName("acl")]
        public Dictionary<string, string> ACL { get; set; }
        [JsonPropertyName("branches")]
        public List<string> Branches { get; set; }
    }
}
