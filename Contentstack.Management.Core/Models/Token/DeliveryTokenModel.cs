using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Token
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ManagementTokenModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }
        [JsonProperty(propertyName: "Scope")]
        public List<TokenScope> Scope { get; set; }
        [JsonProperty(propertyName: "expires_on")]
        public string ExpiresOn { get; set; }
        [JsonProperty(propertyName: "is_email_notification_enabled")]
        public bool IsEmailNotificationEnabled { get; set; } = false;
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeliveryTokenModel
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }
        [JsonProperty(propertyName: "scope")]
        public List<DeliveryTokenScope> Scope { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeliveryTokenScope: TokenScope
    {
        [JsonProperty(propertyName: "environments")]
        public List<string> Environments { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TokenScope
    {
        [JsonProperty(propertyName: "module")]
        public string Module { get; set; }
        [JsonProperty(propertyName: "acl")]
        public Dictionary<string, string> ACL { get; set; }
        [JsonProperty(propertyName: "branches")]
        public List<string> Branches { get; set; }
    }
}
