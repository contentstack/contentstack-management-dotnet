using System.Collections.Generic;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Tests.Model
{
    public class GlobalFieldModel
    {
        [JsonPropertyName("global_field")]
        public ContentModelling Modelling { get; set; }
    }

    public class GlobalFieldsModel
    {
        [JsonPropertyName("global_fields")]
        public List<ContentModelling> Modellings { get; set; }
    }

    public class ContentTypeModel
    {
        [JsonPropertyName("content_type")]
        public ContentModelling Modelling { get; set; }
    }

    public class ContentTypesModel
    {
        [JsonPropertyName("content_types")]
        public List<ContentModelling> Modellings { get; set; }
    }

    public class TaxonomyResponseModel
    {
        [JsonPropertyName("taxonomy")]
        public TaxonomyModel Taxonomy { get; set; }
    }

    public class TaxonomiesResponseModel
    {
        [JsonPropertyName("taxonomies")]
        public List<TaxonomyModel> Taxonomies { get; set; }
    }

    public class TermResponseModel
    {
        [JsonPropertyName("term")]
        public TermModel Term { get; set; }
    }

    public class TermsResponseModel
    {
        [JsonPropertyName("terms")]
        public List<TermModel> Terms { get; set; }

        [JsonPropertyName("count")]
        public int? Count { get; set; }
    }
}
