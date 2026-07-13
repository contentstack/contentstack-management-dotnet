using Contentstack.Management.Core.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Contentstack.Management.Core.Tests.Helpers
{
    /// <summary>
    /// Loads embedded content-type JSON and assigns unique UIDs/titles for disposable integration tests.
    /// </summary>
    public static class ContentTypeFixtureLoader
    {
        public static ContentModelling LoadFromMock(JsonSerializerOptions options, string embeddedFileName, string uidSuffix)
        {
            var text = Contentstack.GetResourceText(embeddedFileName);
            var jo = JsonNode.Parse(text)!.AsObject();
            var baseUid = jo["uid"]?.GetValue<string>() ?? "ct";
            jo["uid"] = $"{baseUid}_{uidSuffix}";
            var title = jo["title"]?.GetValue<string>() ?? "CT";
            jo["title"] = $"{title} {uidSuffix}";
            return JsonSerializer.Deserialize<ContentModelling>(jo.ToJsonString(), options);
        }
    }
}
