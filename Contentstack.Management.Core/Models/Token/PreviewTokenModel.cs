using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Token
{
    /// <summary>
    /// Model for creating a Preview Token.
    /// Preview Tokens provide access to retrieve website details within the Live Preview panel
    /// and are compatible only with the rest-preview.contentstack.com endpoint.
    /// </summary>
    public class PreviewTokenModel
    {
        /// <summary>
        /// The name of the preview token.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The description of the preview token.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
