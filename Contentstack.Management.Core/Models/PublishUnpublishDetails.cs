using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models
{
    public class PublishUnpublishDetails
    {
        public List<string> Locales { get; set; }

        public List<string> Environments { get; set; }

        public string Version { get; set; }

        public string ScheduledAt { get; set; }

    }
}
