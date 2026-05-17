using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Utils
{
    /// <summary>
    /// Extension methods for converting between Newtonsoft.Json and System.Text.Json types.
    /// </summary>
    public static class JsonConversionExtensions
    {
        /// <summary>
        /// Converts a Newtonsoft.Json JObject to System.Text.Json JsonObject.
        /// </summary>
        /// <param name="jObject">The JObject to convert.</param>
        /// <returns>A JsonObject representation of the input.</returns>
        public static JsonObject ToJsonObject(this JObject jObject)
        {
            if (jObject == null)
                return null;
                
            return JsonNode.Parse(jObject.ToString())?.AsObject();
        }

        /// <summary>
        /// Converts a System.Text.Json JsonObject to Newtonsoft.Json JObject.
        /// </summary>
        /// <param name="jsonObject">The JsonObject to convert.</param>
        /// <returns>A JObject representation of the input.</returns>
        public static JObject ToJObject(this JsonObject jsonObject)
        {
            if (jsonObject == null)
                return null;
                
            return JObject.Parse(jsonObject.ToJsonString());
        }

        /// <summary>
        /// Gets the JsonObject representation of a response using System.Text.Json.
        /// </summary>
        /// <param name="response">The response to convert.</param>
        /// <returns>A JsonObject representation of the response.</returns>
        public static JsonObject AsJsonObject(this IResponse response)
        {
            if (response == null)
                return null;
                
            return response.OpenJObjectResponse().ToJsonObject();
        }

        /// <summary>
        /// Gets the JObject representation of a response using Newtonsoft.Json.
        /// This method is provided for consistency when the response already supports JsonObject.
        /// </summary>
        /// <param name="response">The response to convert.</param>
        /// <returns>A JObject representation of the response.</returns>
        public static JObject AsJObject(this IResponse response)
        {
            if (response == null)
                return null;

            // Check if response supports JsonObject natively (for future implementation)
            // For now, use the existing JObject method
            return response.OpenJObjectResponse();
        }
    }
}