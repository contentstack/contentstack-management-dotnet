using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class ReferenceField : Field
    {
        [JsonPropertyName("reference_to")]
        public object ReferenceTo { get; set; }
        [JsonPropertyName("plugins")]
        public List<string> Plugins { get; set; }
    }
}
