using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class TextboxField : Field
    {
        [JsonPropertyName("format")]
        public string? Format { get; set; }

        [JsonPropertyName("error_messages")]
        public Dictionary<string, string>? ErrorMessages { get; set; }
    }
}
