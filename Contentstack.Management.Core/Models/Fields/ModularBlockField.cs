using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class ModularBlockField : Field
    {
        [JsonProperty(propertyName: "blocks")]
        public List<Block> blocks { get; set; }
    }

    public class Block
    {
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        [JsonProperty(propertyName: "autoEdit")]
        public bool AutoEdit { get; set; }
        [JsonProperty(propertyName: "schema")]
        public List<Field> Schema { get; set; }
    }
}
