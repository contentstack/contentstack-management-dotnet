using System.Collections.Generic;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    public class Node
    {
        public string type { get; set; }

        public IDictionary<string, object> attrs { get; set; }

        public List<Node> children { get; set; }
    }
}