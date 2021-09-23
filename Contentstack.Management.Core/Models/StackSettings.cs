using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    public class StackSettings
    {
        [JsonProperty("stack_variables")]
        public Dictionary<string, object> StackVariables { get; set; }
        [JsonProperty("discrete_variables")]
        public Dictionary<string, object> DiscreteVariables { get; set; }
        [JsonProperty("rte")]
        public Dictionary<string, object> Rte { get; set; }
    }
}
