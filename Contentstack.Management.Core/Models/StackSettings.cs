using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    public class StackSettings
    {
        [JsonPropertyName("stack_variables")]
        [JsonConverter(typeof(NativeDictionaryConverter))]
        public Dictionary<string, object>? StackVariables { get; set; }
        [JsonPropertyName("discrete_variables")]
        [JsonConverter(typeof(NativeDictionaryConverter))]
        public Dictionary<string, object>? DiscreteVariables { get; set; }
        [JsonPropertyName("rte")]
        [JsonConverter(typeof(NativeDictionaryConverter))]
        public Dictionary<string, object>? Rte { get; set; }
    }

    /// <summary>
    /// Deserializes Dictionary&lt;string, object&gt; with native .NET types instead of JsonElement.
    /// Needed because STJ boxes JSON primitives as JsonElement when target is object.
    /// </summary>
    internal sealed class NativeDictionaryConverter : JsonConverter<Dictionary<string, object>>
    {
        public override Dictionary<string, object>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var dict = new Dictionary<string, object>();
            reader.Read(); // StartObject
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                string? key = reader.GetString();
                reader.Read();
                if (key != null)
                    dict[key] = ReadValue(ref reader)!;
                reader.Read();
            }
            return dict;
        }

        private static object? ReadValue(ref Utf8JsonReader reader) => reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Null => null,
            JsonTokenType.Number => reader.TryGetInt64(out long l) ? (object)l : reader.GetDouble(),
            JsonTokenType.String => reader.GetString(),
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
        };

        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
