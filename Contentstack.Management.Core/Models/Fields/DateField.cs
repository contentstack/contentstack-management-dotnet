using System.Text.Json.Serialization;
﻿using System;

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
