using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Utils
{
    internal static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Clone options and remove the first converter of type <typeparamref name="TConverter"/> to avoid re-entrancy in custom converters.
        /// </summary>
        public static JsonSerializerOptions WithoutConverter<TConverter>(this JsonSerializerOptions source)
            where TConverter : JsonConverter
        {
            var o = new JsonSerializerOptions(source);
            for (var i = o.Converters.Count - 1; i >= 0; i--)
            {
                if (o.Converters[i] is TConverter)
                {
                    o.Converters.RemoveAt(i);
                    break;
                }
            }

            return o;
        }
    }
}