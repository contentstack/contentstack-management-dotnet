using System;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models.Fields
{
    public class DateField : Field
    {
        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }
        [JsonPropertyName("endDate")]
        public string EndDate { get; set; }
    }
}
