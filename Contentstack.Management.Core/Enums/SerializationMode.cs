namespace Contentstack.Management.Core.Enums
{
    /// <summary>
    /// Defines the serialization mode for JSON operations.
    /// </summary>
    public enum SerializationMode
    {
        /// <summary>
        /// Use Newtonsoft.Json for serialization (default for backward compatibility).
        /// </summary>
        Newtonsoft = 0,
        
        /// <summary>
        /// Use System.Text.Json for serialization.
        /// </summary>
        SystemTextJson = 1,
        
        /// <summary>
        /// Auto-detect best serialization mode based on context (future implementation).
        /// </summary>
        Auto = 2
    }
}