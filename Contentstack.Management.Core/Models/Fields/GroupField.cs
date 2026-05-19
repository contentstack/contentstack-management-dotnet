using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class GroupField : Field
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }
        [JsonPropertyName("schema")]
        public List<Field> Schema { get; set; }
        [JsonPropertyName("max_instance")]
        public int? MaxInstance { get; set; }
    }
}
