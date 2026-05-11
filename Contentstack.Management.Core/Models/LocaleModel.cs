using System.Text.Json.Serialization;
﻿using System;

namespace Contentstack.Management.Core.Models
{
        public class LocaleModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("fallback_locale")]
        public string FallbackLocale { get; set; }
    }
}
