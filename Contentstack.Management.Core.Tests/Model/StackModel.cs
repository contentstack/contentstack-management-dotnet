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

        /// <summary>
        /// Returns the active test stack. Prefers the in-memory key set by
        /// [AssemblyInitialize] (TestRunContext) so tests do not depend on a
        /// file that may not exist yet. Falls back to ./stackApiKey.txt for
        /// backward compatibility with runs that don't use the assembly setup.
        /// </summary>
        public static StackResponse getStack(JsonSerializer serializer)
        {
            if (!string.IsNullOrWhiteSpace(TestRunContext.StackApiKey))
            {
                return new StackResponse
                {
                    Stack = new StackModel { APIKey = TestRunContext.StackApiKey }
                };
            }

            string response = File.ReadAllText("./stackApiKey.txt");
            JObject jObject = JObject.Parse(response);
            return jObject.ToObject<StackResponse>(serializer);
        }
    }
}
