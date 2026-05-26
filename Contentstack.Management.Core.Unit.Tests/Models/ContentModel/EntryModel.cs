using System;
using Contentstack.Management.Core.Abstractions;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Unit.Tests.Models.ContentModel
{
    public class EntryModel : IEntry
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
