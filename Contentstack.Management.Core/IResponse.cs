using System;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace Contentstack.Management.Core
{
    /// <summary>
    /// Interface for a response.
    /// </summary>
    public interface IResponse
    {
        long ContentLength { get; }
        string ContentType { get; }
        HttpStatusCode StatusCode { get; }
        bool IsSuccessStatusCode { get; }
        string[] GetHeaderNames();
        bool IsHeaderPresent(string headerName);
        string GetHeaderValue(string headerName);

        string OpenResponse();

        JObject OpenJObjectResponse();

        TResponse OpenTResponse<TResponse>();

        /// <summary>
        /// Opens the response as a System.Text.Json JsonObject.
        /// </summary>
        /// <returns>JsonObject representation of the response.</returns>
        JsonObject OpenJsonObjectResponse();

        /// <summary>
        /// Deserializes the response to the specified type using System.Text.Json.
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize to.</typeparam>
        /// <returns>Deserialized object of the specified type.</returns>
        TResponse OpenTResponseStj<TResponse>();
    }
}
