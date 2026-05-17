using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class StackSettings
    {
        [JsonProperty("stack_variables")]
        [JsonPropertyName("stack_variables")]
        public Dictionary<string, object> StackVariables { get; set; }
        [JsonProperty("discrete_variables")]
        [JsonPropertyName("discrete_variables")]
        public Dictionary<string, object> DiscreteVariables { get; set; }
        [JsonProperty("rte")]
        [JsonPropertyName("rte")]
        public Dictionary<string, object> Rte { get; set; }
    }
}
