using System;
using System.IO;
using System.Net.Http;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Models.CustomExtension
{
    public class CustomFieldModel : IExtensionInterface
    {
        public string Title { get; set; }
        public string DataType { get; set; }
        public string Tags { get; set; }
        public bool Multiple { get; set; }
        public string ContentType { get; set; }

        internal ByteArrayContent byteArray;

        public CustomFieldModel(string filePath, string contentType, string title, string dataType, bool isMultiple = false, string tags = null) :
            this(File.OpenRead(filePath), contentType, title, dataType, isMultiple, tags)
        { }

        public CustomFieldModel(Stream stream, string contentType, string title, string dataType, bool isMultiple = false, string tags = null) :
            this(getBytes(stream), contentType, title, dataType, isMultiple, tags)
        { }

        public CustomFieldModel(byte[] bytes, string contentType, string title, string dataType, bool isMultiple = false, string tags = null) :
            this(getByteArray(bytes), contentType, title, dataType, isMultiple, tags)
        { }

        public CustomFieldModel(ByteArrayContent byteArray, string contentType, string title, string dataType, bool isMultiple = false, string tags = null)
        {

            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray", "Uploading content can not be null.");
            }
            if (title == null)
            {
                throw new ArgumentNullException("title", "Title for widget is required.");
            }
            Title = title;
            DataType = dataType;
            Multiple = isMultiple;
            Tags = tags;
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

            content.Add(byteArray, "extension[upload]", Title);
            content.Add(new StringContent(Title), "extension[title]");
            
            if (DataType != null)
            {
                content.Add(new StringContent(DataType), "extension[data_type]");
            }
            if (Tags != null)
            {
                content.Add(new StringContent(Tags), "extension[tags]");
            }
            content.Add(new StringContent(Multiple ? "true" : "false"), "extension[multiple]");
            content.Add(new StringContent("field"), "extension[type]");

            return content;
        }
    }
}
