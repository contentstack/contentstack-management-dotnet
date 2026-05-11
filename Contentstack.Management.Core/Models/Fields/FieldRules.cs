using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class FieldRules
    {
        [JsonPropertyName("match_type")]
        public string MatchType { get; set; }
        [JsonPropertyName("actions")]
        public List<Action> Actions { get; set; }
        [JsonPropertyName("conditions")]
        public List<Condition> conditions { get; set; }
    }

    public class Action
    {
        [JsonPropertyName("action")]
        public string state { get; set; }
        [JsonPropertyName("target_field")]
        public string TargetField { get; set; }
    }

    public class Condition
    {
        [JsonPropertyName("dataType")]
        public string DataType { get; set; }
        [JsonPropertyName("operand_field")]
        public string OperandField { get; set; }
        [JsonPropertyName("operator")]
        public string Operator { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
