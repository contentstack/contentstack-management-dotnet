using System;
using System.Net;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core
{
    /// <summary>
    /// Interface for a response.
    /// </summary>
    public interface IResponse
    {
        long ContentLength { get; }
        string? ContentType { get; }
        HttpStatusCode StatusCode { get; }
        bool IsSuccessStatusCode { get; }
        string[] GetHeaderNames();
        bool IsHeaderPresent(string headerName);
        string GetHeaderValue(string headerName);

        string OpenResponse();

        JsonObject OpenJsonObjectResponse();

        /// <summary>
        /// Backward compatibility method for non-migrated models. Will be removed in future versions.
        /// </summary>
        [Obsolete("Use OpenJsonObjectResponse() instead. This method will be removed in future versions.")]
        JObject OpenJObjectResponse();

        TResponse? OpenTResponse<TResponse>();
    }
}
