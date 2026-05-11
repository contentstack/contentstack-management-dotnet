using System;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Abstractions
{
        public interface IEntry
    {
        [JsonPropertyName("title")]
        string Title { get; set; }
    }
}
