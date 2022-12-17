using System;
using System.IO;
using System.Net.Http;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Models.CustomExtension
{
    public class DashboardWidgetModel : IExtensionInterface
    {
        public string Title { get; set; }
        public string Tags { get; set; }
        public string ContentType { get; set; }
        public string DefaultWidth { get; set; }
        public bool Enable { get; set; }
        
        internal ByteArrayContent byteArray;

        public DashboardWidgetModel(string filePath, string contentType, string title, bool isEnable = false, string defaultWidth = null, string tags = null) :
            this(File.OpenRead(filePath), contentType, title, isEnable, defaultWidth, tags)
        { }

        public DashboardWidgetModel(Stream stream, string contentType, string title, bool isEnable = false, string defaultWidth = null, string tags = null) :
            this(getBytes(stream), contentType, title, isEnable, defaultWidth, tags)
        { }

        public DashboardWidgetModel(byte[] bytes, string contentType, string title, bool isEnable = false, string defaultWidth = null, string tags = null) :
            this(getByteArray(bytes), contentType, title, isEnable, defaultWidth, tags)
        { }

        public DashboardWidgetModel(ByteArrayContent byteArray, string contentType, string title, bool isEnable = false, string defaultWidth = null, string tags = null)
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
            Tags = tags;
            ContentType = contentType;
            DefaultWidth = defaultWidth;
            Enable = isEnable;
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
            
            if (Tags != null)
            {
                content.Add(new StringContent(Tags), "extension[tags]");
            }
            if (DefaultWidth != null)
            {
                content.Add(new StringContent(DefaultWidth), "extension[default_width]");
            }
            content.Add(new StringContent(Enable ? "true" : "false"), "extension[enable]");
            content.Add(new StringContent("dashboard"), "extension[type]");
            return content;
        }
    }
}
