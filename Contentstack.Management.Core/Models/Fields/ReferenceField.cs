using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class ReferenceField : Field
    {
        [JsonProperty(propertyName: "reference_to")]
        public object ReferenceTo { get; set; }
        [JsonProperty(propertyName: "plugins")]
        public List<string> Plugins { get; set; }
    }
}
