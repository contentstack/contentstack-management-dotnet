using System;
using Contentstack.Management.Core.Models;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Tests.Model
{
    public class StackSettingsModel
    {
        public string Notice { get; set; }

        [JsonProperty("stack_settings")]
        public StackSettings StackSettings { get; set; }
    }
}
