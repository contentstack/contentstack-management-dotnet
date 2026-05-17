using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Serialization
{
    /// <summary>
    /// Newtonsoft.Json implementation of the serialization engine.
    /// </summary>
    public class NewtonsoftSerializationEngine : ISerializationEngine
    {
        private readonly JsonSerializer _serializer;
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the NewtonsoftSerializationEngine class.
        /// </summary>
        /// <param name="settings">The JsonSerializerSettings to use for serialization.</param>
        public NewtonsoftSerializationEngine(JsonSerializerSettings settings)
        {
            _settings = settings ?? new JsonSerializerSettings();
            _serializer = JsonSerializer.Create(_settings);
        }

        /// <summary>
        /// Serializes an object to JSON string using Newtonsoft.Json.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>JSON string representation of the object.</returns>
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        /// <summary>
        /// Deserializes JSON string to an object of type T using Newtonsoft.Json.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>Deserialized object of type T.</returns>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        /// <summary>
        /// Deserializes JSON string to an object of the specified type using Newtonsoft.Json.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="type">The type to deserialize to.</param>
        /// <returns>Deserialized object of the specified type.</returns>
        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, _settings);
        }
    }
}