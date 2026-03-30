using Contentstack.Management.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.Helpers
{
    /// <summary>
    /// Loads embedded content-type JSON and assigns unique UIDs/titles for disposable integration tests.
    /// </summary>
    public static class ContentTypeFixtureLoader
    {
        public static ContentModelling LoadFromMock(JsonSerializer serializer, string embeddedFileName, string uidSuffix)
        {
            var text = Contentstack.GetResourceText(embeddedFileName);
            var jo = JObject.Parse(text);
            var baseUid = jo["uid"]?.Value<string>() ?? "ct";
            jo["uid"] = $"{baseUid}_{uidSuffix}";
            var title = jo["title"]?.Value<string>() ?? "CT";
            jo["title"] = $"{title} {uidSuffix}";
            return jo.ToObject<ContentModelling>(serializer);
        }
    }
}
