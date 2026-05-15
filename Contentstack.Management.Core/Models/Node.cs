using System.Collections.Generic;
using Contentstack.Management.Core.Utils;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    [JsonConverter(typeof(NodeJsonConverter))]
    public class Node
    {
        public string type { get; set; }

        public IDictionary<string, object> attrs { get; set; }

        public List<Node> children { get; set; }
    }
}