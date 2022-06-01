using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Contentstack.Management.Core.Abstractions;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Models.CustomExtension
{
    public class CustomWidgetModel : IExtensionInterface
    {
        public string Title { get; set; }
        public string Tags { get; set; }
        public ExtensionScope Scope { get; set; }
        public string ContentType { get; set; }

        internal ByteArrayContent byteArray;

        public CustomWidgetModel(string filePath, string contentType, string title, string tags = null, ExtensionScope scope = null) :
            this(File.OpenRead(filePath), contentType, title, tags, scope)
        { }

        public CustomWidgetModel(Stream stream, string contentType, string title, string tags = null, ExtensionScope scope = null) :
            this(getBytes(stream), contentType, title, tags, scope)
        { }

        public CustomWidgetModel(byte[] bytes, string contentType, string title, string tags = null, ExtensionScope scope = null) :
            this(getByteArray(bytes), contentType, title, tags, scope)
        { }

        public CustomWidgetModel(ByteArrayContent byteArray, string contentType, string title, string tags = null, ExtensionScope scope = null)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray", "Uploading content can not be null.");
            }
            Title = title;
            Tags = tags;
            Scope = scope;
            ContentType = contentType;
            this.byteArray = byteArray;
            this.byteArray.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);

        }

        static private byte[] getBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            return bytes;
        }

        static private ByteArrayContent getByteArray(byte[] bytes)
        {
            return new ByteArrayContent(bytes);
        }

        public HttpContent GetHttpContent()
        {
            MultipartFormDataContent content = new MultipartFormDataContent();

            content.Add(byteArray, "extension[upload]");

            if (Title != null)
            {
                content.Add(new StringContent(Title), "extension[title]", Title);
            }
              if (Tags != null)
            {
                content.Add(new StringContent(Tags), "extension[tags]", Tags);
            }
            if (Scope != null)
            {
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    JsonWriter writer = new JsonTextWriter(stringWriter);

                    JsonSerializer.Create().Serialize(writer, Scope);
                    string snippet = stringWriter.ToString();
                    StringContent jsonPart = new StringContent(snippet);
                    jsonPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    jsonPart.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    content.Add(jsonPart, "extension[scope]", snippet);
                }
            }
            content.Add(new StringContent("widget"), "extension[type]", "widget");
            return content;
        }
    }
}
