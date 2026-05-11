using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents a global field reference tracking information.
    /// This is used to track references to other global fields within a global field schema.
    /// </summary>
    public class GlobalFieldRefs
    {
        /// <summary>
        /// The UID of the referenced global field.
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// The number of times this global field is referenced in the schema.
        /// </summary>
        [JsonPropertyName("occurrence_count")]
        public int OccurrenceCount { get; set; }

        /// <summary>
        /// Indicates whether this is a child reference.
        /// </summary>
        [JsonPropertyName("isChild")]
        public bool IsChild { get; set; }

        /// <summary>
        /// Array of paths where this global field reference occurs in the schema.
        /// </summary>
        [JsonPropertyName("paths")]
        public List<string> Paths { get; set; }
    }
} 