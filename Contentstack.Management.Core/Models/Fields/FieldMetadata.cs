using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class FieldMetadata
    {
        /// <summary>
        /// Allows you to set default fields for content types.
        /// </summary>
        [JsonProperty(propertyName: "_default")]
        public string Default { get; set; }

        /// <summary>
        /// Allows you to set a default value for a field.
        /// </summary>
        [JsonProperty(propertyName: "default_value")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// Determines whether the editor will support rich text, and is set to ‘true’ by default for Rich Text Editors.
        /// </summary>
        [JsonProperty(propertyName: "allow_rich_text")]
        public bool AllowRichText { get; set; }

        /// <summary>
        /// Allows you to provide the content for the Rich text editor field.
        /// </summary>
        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        /// <summary>
        /// Provides multi-line capabilities to the Rich text editor.
        /// </summary>
        [JsonProperty(propertyName: "multiline")]
        public bool Multiline { get; set; }

        /// <summary>
        /// Lets you enable either the basic, custom, or advanced editor to enter your content.
        /// </summary>
        [JsonProperty(propertyName: "rich_text_type")]
        public string RichTextType { get; set; }

        /// <summary>
        /// If you choose the Custom editor, then the options key lets you specify the formatting options you prefer for your RTE toolbar,
        /// e.g., "options": ["h3", "blockquote", "sup"]
        /// </summary>
        [JsonProperty(propertyName: "options")]
        public List<string> Options { get; set; }

        /// <summary>
        /// This key determines whether you are using the older version of the Rich Text Editor or the latest version.
        /// The value of 1 denotes that it is an older version of the editor, while 3 denotes that it is the latest version of the editor.
        /// </summary>
        [JsonProperty(propertyName: "version")]
        public int Version { get; set; }

        /// <summary>
        /// Lets you assign a field to be a markdown by setting its value to ‘true’.
        /// </summary>
        [JsonProperty(propertyName: "markdown")]
        public bool Markdown { get; set; }

        /// <summary>
        /// Allows you to provide a hint text about the values that need to be entered in an input field, e.g., Single Line Textbox.
        /// This text can be seen inside the field until you enter a value.
        /// </summary>
        [JsonProperty(propertyName: "placeholder")]
        public string Placeholder { get; set; }

        /// <summary>
        /// Allows you to add instructions for the content managers while entering values for a field. The instructional text appears below the field.
        /// </summary>
        [JsonProperty(propertyName: "instruction")]
        public string Instruction { get; set; }

        /// <summary>
        /// Allows you to set single or multiple reference to Reference field.
        /// </summary>
        [JsonProperty(propertyName: "ref_multiple")]
        public bool RefMultiple { get; set; }

    }
}
