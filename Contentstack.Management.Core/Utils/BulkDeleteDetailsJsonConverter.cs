using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Utils
{
    /// <summary>
    /// Serializes <see cref="BulkDeleteDetails"/> like Newtonsoft with conditional properties:
    /// omit <c>entries</c> / <c>assets</c> when null or empty (legacy <c>ShouldSerialize*</c> behavior).
    /// </summary>
    public sealed class BulkDeleteDetailsJsonConverter : JsonConverter<BulkDeleteDetails>
    {
        public override BulkDeleteDetails Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = new BulkDeleteDetails();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string prop = reader.GetString();
                reader.Read();
                if (string.Equals(prop, "entries", StringComparison.OrdinalIgnoreCase))
                {
                    result.Entries = JsonSerializer.Deserialize<List<BulkDeleteEntry>>(ref reader, options);
                }
                else if (string.Equals(prop, "assets", StringComparison.OrdinalIgnoreCase))
                {
                    result.Assets = JsonSerializer.Deserialize<List<BulkDeleteAsset>>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, BulkDeleteDetails value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.Entries != null && value.Entries.Count > 0)
            {
                writer.WritePropertyName("entries");
                JsonSerializer.Serialize(writer, value.Entries, options);
            }

            if (value.Assets != null && value.Assets.Count > 0)
            {
                writer.WritePropertyName("assets");
                JsonSerializer.Serialize(writer, value.Assets, options);
            }

            writer.WriteEndObject();
        }
    }
}
