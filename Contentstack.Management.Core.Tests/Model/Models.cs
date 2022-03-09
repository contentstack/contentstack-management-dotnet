using Newtonsoft.Json;
using Contentstack.Management.Core.Models;
using System.Collections.Generic;

namespace Contentstack.Management.Core.Tests.Model
{
    public class GlobalFieldModel
    {
        [JsonProperty("global_field")]
        public ContentModelling Modelling { get; set; }
    }
    public class GlobalFieldsModel
    {
        [JsonProperty("global_fields")]
        public List<ContentModelling> Modellings { get; set; }
    }

    public class ContentTypeModel
    {
        [JsonProperty("content_type")]
        public ContentModelling Modelling { get; set; }
    }
    public class ContentTypesModel
    {
        [JsonProperty("content_types")]
        public List<ContentModelling> Modellings { get; set; }
    }
}

