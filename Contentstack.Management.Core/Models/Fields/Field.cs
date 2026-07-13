using System;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(DateField))]
    [JsonDerivedType(typeof(ExtensionField))]
    [JsonDerivedType(typeof(FileField))]
    [JsonDerivedType(typeof(GlobalFieldReference))]
    [JsonDerivedType(typeof(GroupField))]
    [JsonDerivedType(typeof(ModularBlockField))]
    [JsonDerivedType(typeof(NumberField))]
    [JsonDerivedType(typeof(ReferenceField))]
    [JsonDerivedType(typeof(SelectField))]
    [JsonDerivedType(typeof(TaxonomyField))]
    [JsonDerivedType(typeof(TextboxField))]
    [JsonDerivedType(typeof(ImageField))]
    [JsonDerivedType(typeof(JsonField))]
    public class Field
    {
        /// <summary>
        /// Determines the display name of a field. It is a mandatory field.
        /// </summary>
        [JsonPropertyName("display_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DisplayName { get; set; }
        /// <summary>
        /// Represents the unique ID of each field. It is a mandatory field.
        /// </summary>
        [JsonPropertyName("uid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Uid { get; set; }
        /// <summary>
        /// Determines what value can be provided to the Title field.
        /// </summary>
        [JsonPropertyName("data_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DataType { get; set; }
        /// <summary>
        /// Allows you to enter additional data about a field. Also, you can add additional values under 'field_metadata'.
        /// </summary>
        [JsonPropertyName("field_metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FieldMetadata? FieldMetadata { get; set; }

        [JsonPropertyName("multiple")]
        public bool Multiple { get; set; }

        [JsonPropertyName("mandatory")]
        public bool Mandatory { get; set; }

        [JsonPropertyName("unique")]
        public bool Unique { get; set; }

        /// <summary>
        /// Presentation widget for text fields (e.g. dropdown, checkbox).
        /// </summary>
        [JsonPropertyName("display_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DisplayType { get; set; }
    }
}
