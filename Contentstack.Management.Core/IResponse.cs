using System;
using System.Net;
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

        JsonObject OpenJsonObjectResponse();

        TResponse OpenTResponse<TResponse>();
    }
}
