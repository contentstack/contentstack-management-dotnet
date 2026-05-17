using System;
using System.Text.Json;

namespace Contentstack.Management.Core.Services.Serialization
{
    /// <summary>
    /// System.Text.Json implementation of the serialization engine.
    /// </summary>
    public class SystemTextJsonSerializationEngine : ISerializationEngine
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the SystemTextJsonSerializationEngine class.
        /// </summary>
        /// <param name="options">The JsonSerializerOptions to use for serialization.</param>
        public SystemTextJsonSerializationEngine(JsonSerializerOptions options)
        {
            _options = options ?? new JsonSerializerOptions();
        }

        /// <summary>
        /// Serializes an object to JSON string using System.Text.Json.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>JSON string representation of the object.</returns>
        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }

        /// <summary>
        /// Deserializes JSON string to an object of type T using System.Text.Json.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>Deserialized object of type T.</returns>
        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        /// <summary>
        /// Deserializes JSON string to an object of the specified type using System.Text.Json.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="type">The type to deserialize to.</param>
        /// <returns>Deserialized object of the specified type.</returns>
        public object Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, _options);
        }
    }
}