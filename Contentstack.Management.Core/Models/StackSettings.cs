using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class StackSettings
    {
        [JsonPropertyName("stack_variables")]
        public Dictionary<string, object> StackVariables { get; set; }
        [JsonPropertyName("discrete_variables")]
        public Dictionary<string, object> DiscreteVariables { get; set; }
        [JsonPropertyName("rte")]
        public Dictionary<string, object> Rte { get; set; }
    }
}
