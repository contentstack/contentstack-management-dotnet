using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models.Fields;

namespace Contentstack.Management.Core.Utils
{
    /// <summary>
    /// Deserializes <see cref="Field"/> polymorphically by <c>data_type</c> so nested groups, blocks, and references round-trip.
    /// </summary>
    public class FieldJsonConverter : JsonConverter<Field>
    {
        public override Field Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var targetType = ResolveConcreteType(root);
            var innerOpts = options.WithoutConverter<FieldJsonConverter>();
            return (Field)JsonSerializer.Deserialize(root.GetRawText(), targetType, innerOpts);
        }

        public override void Write(Utf8JsonWriter writer, Field value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var innerOpts = options.WithoutConverter<FieldJsonConverter>();
            JsonSerializer.Serialize(writer, value, value.GetType(), innerOpts);
        }

        private static Type ResolveConcreteType(JsonElement jo)
        {
            // Simplified implementation - only use base Field type for now
            // Specific field types (GroupField, ModularBlockField, etc.) are still excluded from compilation
            // Uncomment the full implementation below when those field types are migrated to STJ and re-enabled

            return typeof(Field);

            // TODO: Uncomment this when specific field types are re-enabled in compilation
            /*
            // Check for extension fields first
            var extensionUid = jo.TryGetProperty("extension_uid", out var ext) ? ext.GetString() : null;
            if (!string.IsNullOrEmpty(extensionUid))
                return typeof(ExtensionField);

            var dataType = jo.TryGetProperty("data_type", out var dt) ? dt.GetString() : null;

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
                    if (jo.TryGetProperty("field_metadata", out var fm))
                    {
                        if (jo.TryGetProperty("dimension", out _) ||
                            (fm.TryGetProperty("image", out var img) && img.ValueKind == JsonValueKind.True))
                            return typeof(ImageField);
                    }
                    else if (jo.TryGetProperty("dimension", out _))
                        return typeof(ImageField);

                    return typeof(FileField);
                case "json":
                    return typeof(JsonField);
                case "text":
                    if (jo.TryGetProperty("enum", out _))
                        return typeof(SelectField);
                    var displayType = jo.TryGetProperty("display_type", out var dtp) ? dtp.GetString() : null;
                    if (displayType == "dropdown" || displayType == "checkbox")
                        return typeof(SelectField);
                    return typeof(TextboxField);
                default:
                    return typeof(Field);
            }
            */
        }
    }
}
