using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class GroupField : Field
    {
        [JsonProperty(propertyName: "format")]
        public string Format { get; set; }
        [JsonProperty(propertyName: "schema")]
        public List<Field> Schema { get; set; }
        [JsonProperty(propertyName: "max_instance")]
        public int MaxInstance { get; set; }
    }
}
