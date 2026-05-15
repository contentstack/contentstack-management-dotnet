using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Utils
{
	public class NodeJsonConverter : JsonConverter<Node>
    {
        public override Node ReadJson(JsonReader reader, Type objectType, Node existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Node node = null;
            JObject jObject = JObject.Load(reader);
            if (jObject["type"] == null)
            {
                node = new TextNode();
                node.type = "text";
            }
            else
            {
                node = new Node();
            }
            serializer.Populate(jObject.CreateReader(), node);
            return node;
        }

        public override void WriteJson(JsonWriter writer, Node value, JsonSerializer serializer)
        {
            writer.WriteStartObject(); 

            writer.WritePropertyName("type");
            writer.WriteValue(value.type);

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

            writer.WriteEndObject();
        }
    }
}