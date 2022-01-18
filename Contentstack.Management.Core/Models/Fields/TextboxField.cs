using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class TextboxField : Field
    {

        [JsonProperty(propertyName: "format")]
        public string Format { get; set; }

        [JsonProperty(propertyName: "error_messages")]
        public Dictionary<string, string> ErrorMessages { get; set; }
    }
}
