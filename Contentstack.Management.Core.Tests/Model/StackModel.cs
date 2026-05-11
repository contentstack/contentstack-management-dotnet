using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Tests.Model
{
    public class StackModel
    {
        [JsonPropertyName("api_key")]
        public string APIKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [JsonPropertyName("master_locale")]
        public string MasterLocale { get; set; }

        [JsonPropertyName("org_uid")]
        public string OrgUid { get; set; }
    }

    public class StackResponse
    {
        public StackModel Stack { get; set; }

        public static StackResponse getStack(JsonSerializerOptions options)
        {
            string response = File.ReadAllText("./stackApiKey.txt");
            return JsonSerializer.Deserialize<StackResponse>(response, options);
        }
    }
}
