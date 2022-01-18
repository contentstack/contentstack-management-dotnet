using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class FileField : Field
    {
        [JsonProperty(propertyName: "extensions")]
        public List<string> Extensions { get; set; }
    }
}
