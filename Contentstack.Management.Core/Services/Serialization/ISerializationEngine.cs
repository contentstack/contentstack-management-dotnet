using System;

namespace Contentstack.Management.Core.Services.Serialization
{
    /// <summary>
    /// Abstraction for JSON serialization engines to support both Newtonsoft.Json and System.Text.Json.
    /// </summary>
    public interface ISerializationEngine
    {
        /// <summary>
        /// Serializes an object to JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>JSON string representation of the object.</returns>
        string Serialize<T>(T obj);

        /// <summary>
        /// Deserializes JSON string to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>Deserialized object of type T.</returns>
        T Deserialize<T>(string json);

        /// <summary>
        /// Deserializes JSON string to an object of the specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="type">The type to deserialize to.</param>
        /// <returns>Deserialized object of the specified type.</returns>
        object Deserialize(string json, Type type);
    }
}