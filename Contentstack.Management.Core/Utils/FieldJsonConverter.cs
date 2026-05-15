using System;
using Contentstack.Management.Core.Models.Fields;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Utils
{
    /// <summary>
    /// Deserializes <see cref="Field"/> polymorphically by <c>data_type</c> so nested groups, blocks, and references round-trip.
    /// </summary>
    public class FieldJsonConverter : JsonConverter<Field>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, Field value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override Field ReadJson(JsonReader reader, Type objectType, Field existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jo = JObject.Load(reader);
            var dataType = jo["data_type"]?.Value<string>();
            var targetType = ResolveConcreteType(jo, dataType);
            var field = (Field)Activator.CreateInstance(targetType);

            using (var subReader = jo.CreateReader())
            {
                serializer.Populate(subReader, field);
            }

            return field;
        }

        private static Type ResolveConcreteType(JObject jo, string dataType)
        {
            // API returns extension-backed fields with data_type = extension's data type (e.g. "text"), not "extension".
            var extensionUid = jo["extension_uid"]?.Value<string>();
            if (!string.IsNullOrEmpty(extensionUid))
                return typeof(ExtensionField);

            if (string.IsNullOrEmpty(dataType))
                return typeof(Field);

            switch (dataType)
            {
                case "group":
                    return typeof(GroupField);
                case "blocks":
                    return typeof(ModularBlockField);
                case "reference":
                    return typeof(ReferenceField);
                case "global_field":
                    return typeof(GlobalFieldReference);
                case "extension":
                    return typeof(ExtensionField);
                case "taxonomy":
                    return typeof(TaxonomyField);
                case "number":
                    return typeof(NumberField);
                case "isodate":
                    return typeof(DateField);
                case "file":
                    var fm = jo["field_metadata"];
                    if (jo["dimension"] != null || fm?["image"]?.Value<bool>() == true)
                        return typeof(ImageField);
                    return typeof(FileField);
                case "json":
                    return typeof(JsonField);
                case "text":
                    if (jo["enum"] != null)
                        return typeof(SelectField);
                    var displayType = jo["display_type"]?.Value<string>();
                    if (displayType == "dropdown" || displayType == "checkbox")
                        return typeof(SelectField);
                    return typeof(TextboxField);
                default:
                    return typeof(Field);
            }
        }
    }
}
