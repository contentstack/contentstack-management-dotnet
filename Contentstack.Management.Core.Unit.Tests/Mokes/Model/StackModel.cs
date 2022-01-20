using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Mokes.Model
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
    }

    public class Response
    {
        [JsonProperty("notice")]
        public string Notice { get; set; }
    }
}
