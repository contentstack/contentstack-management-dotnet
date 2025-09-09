using System;
using Newtonsoft.Json;
using Contentstack.Management.Core.Http;

namespace Contentstack.Management.Core.Services.OAuth
{
    /// <summary>
    /// Service for revoking OAuth app authorizations.
    /// </summary>
    internal class OAuthAppRevocationService : ContentstackService
    {
        private readonly string _appId;
        private readonly string _authorizationId;
        private readonly string _organizationUid;

        /// <summary>
        /// Initializes a new instance of the OAuthAppRevocationService class.
        /// </summary>
        /// <param name="serializer">The JSON serializer.</param>
        /// <param name="appId">The OAuth app ID.</param>
        /// <param name="authorizationId">The authorization ID to revoke.</param>
        /// <param name="organizationUid">The organization UID for OAuth operations.</param>
        internal OAuthAppRevocationService(JsonSerializer serializer, string appId, string authorizationId, string organizationUid = null)
            : base(serializer)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentException("App ID cannot be null or empty.", nameof(appId));
            if (string.IsNullOrEmpty(authorizationId))
                throw new ArgumentException("Authorization ID cannot be null or empty.", nameof(authorizationId));

            _appId = appId;
            _authorizationId = authorizationId;
            _organizationUid = organizationUid;
            InitializeService();
        }

        /// <summary>
        /// Initializes the service properties.
        /// </summary>
        private void InitializeService()
        {
            HttpMethod = "DELETE";
            ResourcePath = $"manifests/{_appId}/authorizations/{_authorizationId}";
        }

        /// <summary>
        /// Creates the HTTP request for OAuth app revocation operations.
        /// Overrides the base implementation to use the Developer Hub API URL.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use for the request.</param>
        /// <param name="config">The Contentstack client configuration.</param>
        /// <param name="addAcceptMediaHeader">Whether to add accept media headers.</param>
        /// <param name="apiVersion">The API version to use.</param>
        /// <returns>The HTTP request for OAuth app revocation operations.</returns>
        public override IHttpRequest CreateHttpRequest(System.Net.Http.HttpClient httpClient, ContentstackClientOptions config, bool addAcceptMediaHeader = false, string apiVersion = null)
        {
            // Create a custom config with Developer Hub hostname for OAuth app revocation operations
            // OAuth endpoints don't use API versioning, so we set Version to empty
            var devHubConfig = new ContentstackClientOptions
            {
                Host = GetDeveloperHubHostname(config.Host),
                Port = config.Port,
                Version = "", // OAuth endpoints don't use versioning
                Authtoken = config.Authtoken,
                IsOAuthToken = true // This service requires OAuth authentication
            };
            
            // Set the required headers BEFORE calling base.CreateHttpRequest
            // Add organization_uid header if available
            if (!string.IsNullOrEmpty(_organizationUid))
            {
                Headers["organization_uid"] = _organizationUid;
            }
            
            var request = base.CreateHttpRequest(httpClient, devHubConfig, addAcceptMediaHeader, apiVersion);
            
            return request;
        }

        /// <summary>
        /// Transforms the base hostname to the Developer Hub API hostname.
        /// </summary>
        /// <param name="baseHost">The base hostname (e.g., api.contentstack.io)</param>
        /// <returns>The transformed Developer Hub hostname (e.g., developerhub-api.contentstack.com)</returns>
        private static string GetDeveloperHubHostname(string baseHost)
        {
            if (string.IsNullOrEmpty(baseHost))
                return baseHost;

            // Transform api.contentstack.io -> developerhub-api.contentstack.com
            var devHubHost = baseHost;
            
            // Replace 'api' with 'developerhub-api'
            if (devHubHost.Contains("api."))
            {
                devHubHost = devHubHost.Replace("api.", "developerhub-api.");
            }
            
            // Replace .io with .com
            if (devHubHost.EndsWith(".io"))
            {
                devHubHost = devHubHost.Replace(".io", ".com");
            }
            
            // Ensure https:// protocol
            if (!devHubHost.StartsWith("http"))
            {
                devHubHost = "https://" + devHubHost;
            }
            
            return devHubHost;
        }
    }
}
