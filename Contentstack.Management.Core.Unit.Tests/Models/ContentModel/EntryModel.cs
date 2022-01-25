using System;
using Contentstack.Management.Core.Abstractions;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models.ContentModel
{
    public class EntryModel : IEntry
    {
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }
    }
}
