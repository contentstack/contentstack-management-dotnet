using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class ExtensionField : Field
    {
        [JsonProperty(propertyName: "extension_uid")]
        public string extension_uid { get; set; }
        [JsonProperty(propertyName: "config")]
        public Dictionary<string, object> config { get; set; }
    }
}
