using System;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Tests.Model
{
    public class StackSettingsModel
    {
        public string Notice { get; set; }

        [JsonPropertyName("stack_settings")]
        public StackSettings StackSettings { get; set; }
    }
}
