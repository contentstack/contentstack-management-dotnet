using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core.Models;
using Microsoft.Extensions.Options;

namespace Contentstack.Management.Core.Utils
{
	public class TextNodeJsonConverter : JsonConverter<TextNode>
    {
        public override TextNode ReadJson(JsonReader reader, Type objectType, TextNode existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, TextNode value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value.attrs != null)
            {
                writer.WritePropertyName("attrs");
                serializer.Serialize(writer, value.attrs);
            }

            if (value.children != null)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (var child in value.children)
                {
                    serializer.Serialize(writer, child);
                }
                writer.WriteEndArray();
            }
            // Write additional properties specific to TextNode
            if (value.bold)
            {
                writer.WritePropertyName("bold");
                writer.WriteValue(value.bold);
            }
            if (value.italic)
            {
                writer.WritePropertyName("italic");
                writer.WriteValue(value.italic);
            }
            if (value.underline)
            {
                writer.WritePropertyName("underline");
                writer.WriteValue(value.underline);
            }
            if (value.strikethrough)
            {
                writer.WritePropertyName("strikethrough");
                writer.WriteValue(value.strikethrough);
            }
            if (value.inlineCode)
            {
                writer.WritePropertyName("inlineCode");
                writer.WriteValue(value.inlineCode);
            }
            if (value.subscript)
            {
                writer.WritePropertyName("subscript");
                writer.WriteValue(value.subscript);
            }
            if (value.superscript)
            {
                writer.WritePropertyName("superscript");
                writer.WriteValue(value.superscript);
            }
            if (value.break)
            {
                writer.WritePropertyName("break");
                writer.WriteValue(value.break);
            }
            if (!string.IsNullOrEmpty(value.text))
            {
                writer.WritePropertyName("text");
                writer.WriteValue(value.text);
            }

            writer.WriteEndObject();
        }
    }
}