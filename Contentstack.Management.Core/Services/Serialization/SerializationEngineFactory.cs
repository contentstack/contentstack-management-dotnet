using System.Text.Json;
using Contentstack.Management.Core.Enums;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Serialization
{
    /// <summary>
    /// Factory for creating serialization engines based on the specified mode.
    /// </summary>
    public static class SerializationEngineFactory
    {
        /// <summary>
        /// Creates a serialization engine based on the specified mode and settings.
        /// </summary>
        /// <param name="mode">The serialization mode to use.</param>
        /// <param name="newtonsoftSettings">Newtonsoft.Json settings (optional).</param>
        /// <param name="stjOptions">System.Text.Json options (optional).</param>
        /// <returns>An instance of ISerializationEngine.</returns>
        public static ISerializationEngine Create(
            SerializationMode mode, 
            JsonSerializerSettings newtonsoftSettings = null, 
            JsonSerializerOptions stjOptions = null)
        {
            return mode switch
            {
                SerializationMode.SystemTextJson => new SystemTextJsonSerializationEngine(stjOptions ?? new JsonSerializerOptions()),
                SerializationMode.Auto => new NewtonsoftSerializationEngine(newtonsoftSettings ?? new JsonSerializerSettings()), // Default to Newtonsoft for now
                _ => new NewtonsoftSerializationEngine(newtonsoftSettings ?? new JsonSerializerSettings())
            };
        }
    }
}