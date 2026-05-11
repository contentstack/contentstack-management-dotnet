using System.Text.Json;
using System.Text.Json.Nodes;
using Contentstack.Management.Core.Models;

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
            var root = JsonNode.Parse(text)!.AsObject();
            var baseUid = root["uid"]?.GetValue<string>() ?? "ct";
            root["uid"] = $"{baseUid}_{uidSuffix}";
            var title = root["title"]?.GetValue<string>() ?? "CT";
            root["title"] = $"{title} {uidSuffix}";
            return root.Deserialize<ContentModelling>(options);
        }
    }
}
