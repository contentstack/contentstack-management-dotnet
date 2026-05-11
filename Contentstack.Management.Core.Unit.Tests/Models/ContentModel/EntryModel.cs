using System.Text.Json.Serialization;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Unit.Tests.Models.ContentModel
{
    public class EntryModel : IEntry
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
