using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Management.Core.Attributes;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    [CsmJsonConverter("CustomAutoload")]
    public class CustomJsonConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    [CsmJsonConverter("CustomManualLoad", false)]
    public class CustomConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
