using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class ReferenceField : Field
    {
        [JsonProperty(propertyName: "reference_to")]
        public object ReferenceTo { get; set; }
    }
}
