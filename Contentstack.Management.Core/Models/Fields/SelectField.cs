using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class SelectField : Field
    {
        [JsonPropertyName("enum")]
        public SelectEnum? Enum { get; set; }

    }

    public class SelectEnum
    {
        [JsonPropertyName("advanced")]
        public bool Advanced { get; set; }

        [JsonPropertyName("choices")]
        public List<Dictionary<string, JsonElement>>? Choices { get; set; }

    }
}
