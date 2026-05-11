using System.Text.Json.Serialization;
﻿using System.Collections.Generic;
namespace Contentstack.Management.Core.Models
{
        public class ReleaseModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("locked")]
        public bool Locked { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

    }

        public class DeployModel
    {
        [JsonPropertyName("environments")]
        public List<string> Environments { get; set; }

        [JsonPropertyName("locales")]
        public List<string> Locales { get; set; }

        [JsonPropertyName("scheduledAt")]
        public string ScheduledAt { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }

        public class ReleaseItemModel
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        [JsonPropertyName("content_type_uid")]
        public string ContentTypeUID { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
