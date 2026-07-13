using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Utils
{
	public class TextNodeJsonConverter : JsonConverter<TextNode>
    {
        public override TextNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var innerOpts = options.WithoutConverter<TextNodeJsonConverter>();
            return JsonSerializer.Deserialize<TextNode>(doc.RootElement.GetRawText(), innerOpts) ?? new TextNode();
        }

        public override void Write(Utf8JsonWriter writer, TextNode value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

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

            if (value.bold)
            {
                writer.WritePropertyName("bold");
                writer.WriteBooleanValue(value.bold);
            }
            if (value.italic)
            {
                writer.WritePropertyName("italic");
                writer.WriteBooleanValue(value.italic);
            }
            if (value.underline)
            {
                writer.WritePropertyName("underline");
                writer.WriteBooleanValue(value.underline);
            }
            if (value.strikethrough)
            {
                writer.WritePropertyName("strikethrough");
                writer.WriteBooleanValue(value.strikethrough);
            }
            if (value.inlineCode)
            {
                writer.WritePropertyName("inlineCode");
                writer.WriteBooleanValue(value.inlineCode);
            }
            if (value.subscript)
            {
                writer.WritePropertyName("subscript");
                writer.WriteBooleanValue(value.subscript);
            }
            if (value.superscript)
            {
                writer.WritePropertyName("superscript");
                writer.WriteBooleanValue(value.superscript);
            }
            if (value.@break)
            {
                writer.WritePropertyName("break");
                writer.WriteBooleanValue(value.@break);
            }
            if (!string.IsNullOrEmpty(value.text))
            {
                writer.WritePropertyName("text");
                writer.WriteStringValue(value.text);
            }

            writer.WriteEndObject();
        }
    }
}