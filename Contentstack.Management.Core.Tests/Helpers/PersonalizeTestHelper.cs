using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Contentstack.Management.Core.Endpoints;

namespace Contentstack.Management.Core.Tests.Helpers
{
    /// <summary>
    /// Raw-HTTP client for the Contentstack Personalize Management API, used only by
    /// integration tests. The core SDK has no Project/Audience/Experience wrapper classes
    /// (Personalize is a separate product surface from Content Management), so this helper
    /// drives Personalize directly to auto-provision real Variant Group/Variant data on a
    /// stack instead of relying on a hardcoded variant UID.
    ///
    /// NOTE(verify at implementation time): exact endpoint paths, payload shapes, and
    /// required headers below are best-effort based on Contentstack's published Personalize
    /// API conventions and have not been exercised against a live org in this change.
    /// Confirm against https://www.contentstack.com/docs/developers/apis/personalize-management-api
    /// before relying on this in CI, and adjust the marked TODO(verify) spots as needed.
    /// </summary>
    internal class PersonalizeTestHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _organizationUid;

        internal PersonalizeTestHelper(string authtoken, string organizationUid, string region = "na")
        {
            if (string.IsNullOrEmpty(authtoken))
            {
                throw new ArgumentException("authtoken is required", nameof(authtoken));
            }
            if (string.IsNullOrEmpty(organizationUid))
            {
                throw new ArgumentException("organizationUid is required", nameof(organizationUid));
            }

            _organizationUid = organizationUid;
            _baseUrl = Endpoint.GetContentstackEndpoint(region, "personalizeManagement").TrimEnd('/');

            _httpClient = new HttpClient(new LoggingHttpHandler())
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Add("authtoken", authtoken);
            _httpClient.DefaultRequestHeaders.Add("organization_uid", organizationUid);
        }

        /// <summary>
        /// Creates a Personalize project connected to the given stack, so that Audiences/
        /// Experiences created within it can auto-provision Variant Groups on that stack.
        /// TODO(verify): confirm path is "/projects" and payload shape against Personalize docs.
        /// </summary>
        internal async Task<string> CreateProjectAsync(string stackApiKey, string projectName)
        {
            var payload = new JsonObject
            {
                ["name"] = projectName,
                ["connectedStackApiKey"] = stackApiKey
            };

            var project = await PostAsync("/projects", payload, null);
            return project?["uid"]?.ToString();
        }

        /// <summary>
        /// Returns the uid of an existing audience on the project if one is found, otherwise
        /// creates a new default audience and returns its uid.
        /// TODO(verify): confirm paths "/audiences" (GET list / POST create) and payload shape.
        /// </summary>
        internal async Task<string> GetOrCreateDefaultAudienceAsync(string projectUid, string audienceName)
        {
            var existing = await GetAsync("/audiences", projectUid);
            var audiences = existing?["audiences"]?.AsArray();
            if (audiences != null && audiences.Count > 0)
            {
                return audiences[0]?["uid"]?.ToString();
            }

            var payload = new JsonObject
            {
                ["name"] = audienceName,
                ["definition"] = new JsonObject
                {
                    ["rules"] = new JsonArray(),
                    ["operator"] = "and"
                }
            };

            var audience = await PostAsync("/audiences", payload, projectUid);
            return audience?["uid"]?.ToString();
        }

        /// <summary>
        /// Creates an Experience linked to the given audience. In the real product, creating
        /// an Experience on a stack-connected project auto-provisions a Variant Group (and its
        /// Variants) on the Content Management side of that stack.
        /// TODO(verify): confirm path "/experiences" and payload shape (variant count/short_uids).
        /// </summary>
        internal async Task<string> CreateExperienceAsync(string projectUid, string audienceUid, string experienceName)
        {
            var payload = new JsonObject
            {
                ["name"] = experienceName,
                ["audiences"] = new JsonArray(audienceUid),
                ["variants"] = new JsonArray(new JsonObject { ["name"] = "Variant A" })
            };

            var experience = await PostAsync("/experiences", payload, projectUid);
            return experience?["uid"]?.ToString();
        }

        /// <summary>Best-effort teardown of an experience created for a test run. Failures are non-fatal.</summary>
        internal async Task DeleteExperienceAsync(string projectUid, string experienceUid)
        {
            await DeleteAsync($"/experiences/{experienceUid}", projectUid);
        }

        /// <summary>Best-effort teardown of a project created for a test run. Failures are non-fatal.</summary>
        internal async Task DeleteProjectAsync(string projectUid)
        {
            await DeleteAsync($"/projects/{projectUid}", null);
        }

        private async Task<JsonObject> PostAsync(string path, JsonObject payload, string projectUid)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + path);
            ApplyProjectHeader(request, projectUid);
            request.Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return string.IsNullOrEmpty(body) ? null : JsonNode.Parse(body)?.AsObject();
        }

        private async Task<JsonObject> GetAsync(string path, string projectUid)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + path);
            ApplyProjectHeader(request, projectUid);

            using var response = await _httpClient.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return string.IsNullOrEmpty(body) ? null : JsonNode.Parse(body)?.AsObject();
        }

        private async Task DeleteAsync(string path, string projectUid)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, _baseUrl + path);
            ApplyProjectHeader(request, projectUid);
            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private static void ApplyProjectHeader(HttpRequestMessage request, string projectUid)
        {
            if (!string.IsNullOrEmpty(projectUid))
            {
                request.Headers.TryAddWithoutValidation("x-project-uid", projectUid);
            }
        }
    }
}
