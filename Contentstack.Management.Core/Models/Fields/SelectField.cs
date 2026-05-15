using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class SelectField : Field
    {
        [JsonProperty(propertyName: "enum")]
        public SelectEnum Enum { get; set; }

    }

    public class SelectEnum
    {
        [JsonProperty(propertyName: "advanced")]
        public bool Advanced { get; set; }

        [JsonProperty(propertyName: "choices")]
        public List<Dictionary<string, object>> Choices { get; set; }

    }
}
