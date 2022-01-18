using System;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.Fields
{
    public class DateField : Field
    {
        [JsonProperty(propertyName: "startDate")]
        public string StartDate { get; set; }
        [JsonProperty(propertyName: "endDate")]
        public string EndDate { get; set; }
    }
}
