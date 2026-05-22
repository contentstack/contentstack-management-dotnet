using System;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class Field
    {
        /// <summary>
        /// Determines the display name of a field. It is a mandatory field.
        /// </summary>
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }
        /// <summary>
        /// Represents the unique ID of each field. It is a mandatory field.	
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
        /// <summary>
        /// Determines what value can be provided to the Title field.	
        /// </summary>
        [JsonPropertyName("data_type")]
        public string? DataType { get; set; }
        /// <summary>
        /// Allows you to enter additional data about a field. Also, you can add additional values under 'field_metadata'.
        /// </summary>
        [JsonPropertyName("field_metadata")]
        public FieldMetadata? FieldMetadata { get; set; }

        [JsonPropertyName("multiple")]
        public bool Multiple { get; set; }

        [JsonPropertyName("mandatory")]
        public bool Mandatory { get; set; }

        [JsonPropertyName("unique")]
        public bool Unique { get; set; }

        /// <summary>
        /// Presentation widget for text fields (e.g. dropdown, checkbox).
        /// </summary>
        [JsonPropertyName("display_type")]
        public string? DisplayType { get; set; }
    }
}
