using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class ModularBlockField : Field
    {
        [JsonPropertyName("blocks")]
        public List<Block> blocks { get; set; }
    }

    public class Block
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
        [JsonPropertyName("autoEdit")]
        public bool AutoEdit { get; set; }
        [JsonPropertyName("blockType")]
        public bool BlockType { get; set; }
        [JsonPropertyName("schema")]
        public List<Field> Schema { get; set; }
    }
}
