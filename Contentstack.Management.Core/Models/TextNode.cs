using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    [JsonConverter(typeof(TextNodeJsonConverter))]
    public class TextNode: Node
    {
        public bool bold { get; set; }
        public bool italic { get; set; }
        public bool underline { get; set; }
        public bool strikethrough { get; set; }
        public bool inlineCode { get; set; }
        public bool subscript { get; set; }
        public bool superscript { get; set; }
        public bool break { get; set; }
        public string text { get; set; }
    }
}
