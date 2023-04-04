using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class FileField : Field
    {
        [JsonProperty(propertyName: "extensions")]
        public List<string> Extensions { get; set; }
        [JsonProperty(propertyName: "max")]
        public int Maxsize { get; set; }
        [JsonProperty(propertyName: "min")]
        public int MinSize { get; set; }
        
    }
    public class ImageField : FileField
    {
        [JsonProperty(propertyName: "dimension")]
        public Dimension Dimensions { get; set; }
        /// <summary>
        /// Allows you to enter additional data about a field. Also, you can add additional values under ‘field_metadata’.
        /// </summary>
        [JsonProperty(propertyName: "field_metadata")]
        public new FileFieldMetadata FieldMetadata { get; set; }
    }
    public class Dimension
    {
        [JsonProperty(propertyName: "height")]
        public Dictionary<string, int> Height { get; set; }
        [JsonProperty(propertyName: "width")]
        public Dictionary<string, int> Width { get; set; }
    }
}
