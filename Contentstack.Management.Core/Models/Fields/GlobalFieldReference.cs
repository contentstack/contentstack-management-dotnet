using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    /// <summary>
    /// Represents a global field reference in a schema.
    /// This field type allows referencing other global fields within a global field schema.
    /// </summary>
    public class GlobalFieldReference : Field
    {
        /// <summary>
        /// The UID of the global field being referenced.
        /// </summary>
        [JsonProperty(propertyName: "reference_to")]
        public string ReferenceTo { get; set; }

        /// <summary>
        /// Determines if this field can accept multiple values.
        /// </summary>
        [JsonProperty(propertyName: "multiple")]
        public new bool Multiple { get; set; }

        /// <summary>
        /// Determines if this field is mandatory.
        /// </summary>
        [JsonProperty(propertyName: "mandatory")]
        public new bool Mandatory { get; set; }

        /// <summary>
        /// Determines if this field value must be unique.
        /// </summary>
        [JsonProperty(propertyName: "unique")]
        public new bool Unique { get; set; }

        /// <summary>
        /// Determines if this field is non-localizable.
        /// </summary>
        [JsonProperty(propertyName: "non_localizable")]
        public bool NonLocalizable { get; set; }
    }
} 