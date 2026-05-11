using System;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("reference_to")]
        public string ReferenceTo { get; set; }

        /// <summary>
        /// Determines if this field is non-localizable.
        /// </summary>
        [JsonPropertyName("non_localizable")]
        public bool NonLocalizable { get; set; }
    }
} 
