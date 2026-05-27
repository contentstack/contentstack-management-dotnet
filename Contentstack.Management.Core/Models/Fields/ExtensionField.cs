using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class ExtensionField : Field
    {
        [JsonPropertyName("extension_uid")]
        public string? extension_uid { get; set; }
        [JsonPropertyName("config")]
        public Dictionary<string, JsonElement>? config { get; set; }
    }
}
