using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class FileField : Field
    {
        [JsonPropertyName("extensions")]
        public List<string> Extensions { get; set; }
        [JsonPropertyName("max")]
        public int? Maxsize { get; set; }
        [JsonPropertyName("min")]
        public int? MinSize { get; set; }
        
    }
    public class ImageField : FileField
    {
        [JsonPropertyName("dimension")]
        public Dimension Dimensions { get; set; }
        /// <summary>
        /// Allows you to enter additional data about a field. Also, you can add additional values under ‘field_metadata’.
        /// </summary>
        [JsonPropertyName("field_metadata")]
        public new FileFieldMetadata FieldMetadata { get; set; }
    }
    public class Dimension
    {
        [JsonPropertyName("height")]
        public Dictionary<string, int?> Height { get; set; }
        [JsonPropertyName("width")]
        public Dictionary<string, int?> Width { get; set; }
    }
}
