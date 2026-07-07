using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Tests.Model
{
    public class ManagementTokenInfo
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ManagementTokenResponse
    {
        public ManagementTokenInfo Token { get; set; }

        public static ManagementTokenResponse getManagementToken(JsonSerializerOptions options)
        {
            string response = File.ReadAllText("./managementTokenInfo.txt");
            return JsonSerializer.Deserialize<ManagementTokenResponse>(response, options);
        }
    }
}
