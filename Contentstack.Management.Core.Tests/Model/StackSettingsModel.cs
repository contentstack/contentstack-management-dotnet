using System;
using Contentstack.Management.Core.Models;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Tests.Model
{
    public class StackSettingsModel
    {
        public string Notice { get; set; }

        [JsonPropertyName("stack_settings")]
        public StackSettings StackSettings { get; set; }
    }
}
