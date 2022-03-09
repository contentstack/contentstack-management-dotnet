using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.Model
{
    public class StackModel
    {
        [JsonProperty("api_key")]
        public string APIKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [JsonProperty("master_locale")]
        public string MasterLocale { get; set; }

        [JsonProperty("org_uid")]
        public string OrgUid { get; set; }
    }

    public class StackResponse
    {
        public StackModel Stack { get; set; }

        public static StackResponse getStack(JsonSerializer serializer)
        {
            string response = File.ReadAllText("./stackApiKey.txt");
            JObject jObject = JObject.Parse(response);
            return jObject.ToObject<StackResponse>(serializer);
        }
    }
}
