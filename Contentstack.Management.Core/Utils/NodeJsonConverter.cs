using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Utils
{
	public class NodeJsonConverter : JsonConverter<Node>
    {
        public override Node Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var raw = root.GetRawText();

            var missingOrNullType =
                !root.TryGetProperty("type", out var typeProp) ||
                typeProp.ValueKind == JsonValueKind.Null;

            if (missingOrNullType)
            {
                var innerOpts = options.WithoutConverter<TextNodeJsonConverter>();
                var textNode = JsonSerializer.Deserialize<TextNode>(raw, innerOpts) ?? new TextNode();
                if (string.IsNullOrEmpty(textNode.type))
                    textNode.type = "text";
                return textNode;
            }

            var nodeOpts = options.WithoutConverter<NodeJsonConverter>();
            return JsonSerializer.Deserialize<Node>(raw, nodeOpts) ?? new Node();
        }

        public override void Write(Utf8JsonWriter writer, Node value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            if (value is TextNode textNode)
            {
                new TextNodeJsonConverter().Write(writer, textNode, options);
                return;
            }

            writer.WriteStartObject();

            if (!string.IsNullOrEmpty(value.type))
            {
                writer.WriteString("type", value.type);
            }

            if (value.attrs != null)
            {
                writer.WritePropertyName("attrs");
                JsonSerializer.Serialize(writer, value.attrs, options);
            }

            if (value.children != null)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (var child in value.children)
                {
                    JsonSerializer.Serialize(writer, child, options);
                }
                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}