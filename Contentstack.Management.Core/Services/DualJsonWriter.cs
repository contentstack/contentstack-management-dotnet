using System;
using System.IO;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services
{
    /// <summary>
    /// Helper class to store Utf8JsonWriter with its underlying stream.
    /// </summary>
    internal class Utf8JsonWriterInfo
    {
        public MemoryStream MemoryStream { get; set; }
        public Utf8JsonWriter Writer { get; set; }
    }

    /// <summary>
    /// Abstract base class that provides dual JSON writing capabilities 
    /// for both Newtonsoft.Json and System.Text.Json serialization engines.
    /// </summary>
    public abstract class DualJsonWriter
    {
        /// <summary>
        /// Serializes an object using the specified serialization mode and outputs the result as byte content.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="mode">The serialization mode to use.</param>
        /// <param name="newtonsoftSettings">Newtonsoft.Json serializer settings.</param>
        /// <param name="stjOptions">System.Text.Json serializer options.</param>
        /// <param name="content">The resulting byte content.</param>
        protected void WriteObjectWithBothEngines(object obj, SerializationMode mode, 
            JsonSerializerSettings newtonsoftSettings, JsonSerializerOptions stjOptions,
            out byte[] content)
        {
            string json = mode switch
            {
                SerializationMode.SystemTextJson => System.Text.Json.JsonSerializer.Serialize(obj, stjOptions),
                _ => JsonConvert.SerializeObject(obj, newtonsoftSettings)
            };
            content = Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Initializes a JSON writer based on the serialization mode for manual JSON construction.
        /// Note: For complex scenarios, prefer using WriteObjectWithBothEngines with DTOs instead.
        /// </summary>
        /// <param name="mode">The serialization mode to use.</param>
        /// <param name="writer">The JSON writer object (either JsonTextWriter or stream-based writer info).</param>
        /// <param name="stringWriter">The string writer (used with Newtonsoft).</param>
        protected void WriteManualObjectStart(SerializationMode mode, out object writer, out StringWriter stringWriter)
        {
            stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            
            if (mode == SerializationMode.SystemTextJson)
            {
                // For STJ, we'll store the MemoryStream and Utf8JsonWriter together
                var memoryStream = new MemoryStream();
                var utf8Writer = new Utf8JsonWriter(memoryStream);
                writer = new Utf8JsonWriterInfo { MemoryStream = memoryStream, Writer = utf8Writer };
            }
            else
            {
                // For Newtonsoft, use JsonTextWriter
                writer = new JsonTextWriter(stringWriter);
            }
        }

        /// <summary>
        /// Completes the manual JSON writing and returns the byte content.
        /// </summary>
        /// <param name="mode">The serialization mode used.</param>
        /// <param name="writer">The JSON writer object.</param>
        /// <param name="stringWriter">The string writer (used with Newtonsoft).</param>
        /// <param name="content">The resulting byte content.</param>
        protected void CompleteManualWrite(SerializationMode mode, object writer, StringWriter stringWriter, out byte[] content)
        {
            if (mode == SerializationMode.SystemTextJson && writer is Utf8JsonWriterInfo writerInfo)
            {
                writerInfo.Writer.Flush();
                content = writerInfo.MemoryStream.ToArray();
                writerInfo.Writer.Dispose();
                writerInfo.MemoryStream.Dispose();
            }
            else
            {
                string json = stringWriter.ToString();
                content = Encoding.UTF8.GetBytes(json);
                stringWriter.Dispose();
                if (writer is JsonTextWriter jsonWriter)
                {
                    jsonWriter.Close();
                }
            }
        }
    }
}