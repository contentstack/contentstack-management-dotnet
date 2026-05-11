using System.Text.Encodings.Web;
using System.Text.Json;
using Contentstack.Management.Core;

namespace Contentstack.Management.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests should use the same <see cref="JsonSerializerOptions"/> defaults as <see cref="ContentstackClient"/>
    /// (ignore nulls, case-insensitive property names, Field/Node/TextNode converters) unless testing an isolated option set.
    /// </summary>
    internal static class TestJsonSerializerOptions
    {
        public static JsonSerializerOptions CreateDefault() => new ContentstackClient().SerializerOptions;

        /// <summary>
        /// Same as <see cref="CreateDefault"/> but allows non-ASCII characters in serialized JSON without \uXXXX escapes
        /// (for tests that assert raw JSON substrings containing Unicode).
        /// </summary>
        public static JsonSerializerOptions CreateWithRelaxedUnicode()
        {
            var opts = new JsonSerializerOptions(new ContentstackClient().SerializerOptions)
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return opts;
        }
    }
}
