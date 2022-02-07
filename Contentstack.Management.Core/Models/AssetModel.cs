using System;
using System.IO;
using System.Net.Http;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Models
{
    public class AssetModel: IUploadInterface
    {
        public string Title;
        public string Description;
        public string ParentUID;
        public string Tags;
        public string FileName;
        public string ContentType
        {
            get
            {
                return contentType;
            }
            set
            {
                contentType = value;
            }
        }

        internal string contentType;
        internal ByteArrayContent byteArray;

        public AssetModel(string fileName, string filePath, string contentType, string title = null, string description = null, string parentUID = null, string tags = null):
            this(fileName, File.OpenRead(filePath), contentType, title, description, parentUID, tags){ }

        public AssetModel(string fileName, Stream stream, string contentType, string title = null, string description = null, string parentUID = null, string tags = null):
            this(fileName, getBytes(stream), contentType, title, description, parentUID, tags){ }

        public AssetModel(string fileName, byte[] bytes, string contentType, string title = null, string description = null, string parentUID = null, string tags = null) :
            this(fileName, getByteArray(bytes), contentType, title, description, parentUID, tags){ }

        public AssetModel(string fileName, ByteArrayContent byteArray, string contentType, string title = null, string description = null, string parentUID = null, string tags = null)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("File name can not be null.");
            }
            if (byteArray == null)
            {
                throw new ArgumentNullException("Uploading content can not be null.");
            }
            FileName = fileName;
            Title = title;
            Description = description;
            ParentUID = parentUID;
            Tags = tags;
            ContentType = contentType;
            this.byteArray = byteArray;
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

            content.Add(byteArray, "asset[upload]", FileName);
            if (Title != null)
            {
                content.Add(new StringContent(Title), "asset[title]", Title);
            }
            if (Description != null)
            {
                content.Add(new StringContent(Description), "asset[description]", Description);
            }
            if (ParentUID != null)
            {
                content.Add(new StringContent(ParentUID), "asset[parent_uid]", ParentUID);
            }
            if (Tags != null)
            {
                content.Add(new StringContent(Tags), "asset[tags]", Tags);
            }
            return content;
        }
    }
}
