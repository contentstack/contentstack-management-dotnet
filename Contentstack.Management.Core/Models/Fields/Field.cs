using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Field
    {
        /// <summary>
        /// Determines the display name of a field. It is a mandatory field.
        /// </summary>
        [JsonProperty(propertyName: "display_name")]
        public string DisplayName { get; set; }
        /// <summary>
        /// Represents the unique ID of each field. It is a mandatory field.	
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
        /// <summary>
        /// Determines what value can be provided to the Title field.	
        /// </summary>
        [JsonProperty(propertyName: "data_type")]
        public string DataType { get; set; }
        /// <summary>
        /// Allows you to enter additional data about a field. Also, you can add additional values under 'field_metadata'.
        /// </summary>
        [JsonProperty(propertyName: "field_metadata")]
        public FieldMetadata FieldMetadata { get; set; }

        [JsonProperty(propertyName: "multiple")]
        public bool Multiple { get; set; }

        [JsonProperty(propertyName: "mandatory")]
        public bool Mandatory { get; set; }

        [JsonProperty(propertyName: "unique")]
        public bool Unique { get; set; }
    }
}
