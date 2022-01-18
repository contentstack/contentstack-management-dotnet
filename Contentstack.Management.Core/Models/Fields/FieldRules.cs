using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class FieldRules
    {
        [JsonProperty(propertyName: "match_type")]
        public string MatchType { get; set; }
        [JsonProperty(propertyName: "actions")]
        public List<Action> Actions { get; set; }
        [JsonProperty(propertyName: "conditions")]
        public List<Condition> conditions { get; set; }
    }

    public class Action
    {
        [JsonProperty(propertyName: "action")]
        public string state { get; set; }
        [JsonProperty(propertyName: "target_field")]
        public string TargetField { get; set; }
    }

    public class Condition
    {
        [JsonProperty(propertyName: "dataType")]
        public string DataType { get; set; }
        [JsonProperty(propertyName: "operand_field")]
        public string OperandField { get; set; }
        [JsonProperty(propertyName: "operator")]
        public string Operator { get; set; }
        [JsonProperty(propertyName: "value")]
        public string Value { get; set; }
    }
}
