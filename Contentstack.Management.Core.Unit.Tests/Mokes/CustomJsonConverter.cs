using System;
using Contentstack.Management.Core.Attributes;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    [CsmJsonConverter("CustomAutoload")]
    public class CustomJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false; // Mock converter - not actually used for conversion
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    [CsmJsonConverter("CustomManualLoad", false)]
    public class CustomConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false; // Mock converter - not actually used for conversion
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
