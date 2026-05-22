using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class CreateUpdateFolderService : ContentstackService
    {
        private readonly string _name;
        private readonly string _parentUId;

        #region Internal
        internal CreateUpdateFolderService(
            Core.Models.Stack stack,
            string name,
            string folderUid = null,
            string parentUId = null,
            JsonSerializerOptions stjOptions = null)
            : base(stjOptions ?? stack?.client?.SerializerOptions ?? new JsonSerializerOptions(), stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (name == null)
            {
                throw new ArgumentNullException("name", CSConstants.FolderNameRequired);
            }
            this.ResourcePath = "/assets/folders";

            _name = name;
            _parentUId = parentUId;

            if (folderUid != null)
            {
                this.HttpMethod = "PUT";
                this.ResourcePath = $"/assets/folders/{folderUid}";
            }
            else
            {
                this.HttpMethod = "POST";
                this.ResourcePath = "/assets/folders";
            }
        }

        public override void ContentBody()
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            
            writer.WriteStartObject();
            writer.WritePropertyName("asset");
            writer.WriteStartObject();
            
            if (!string.IsNullOrEmpty(_name))
            {
                writer.WritePropertyName("name");
                writer.WriteStringValue(_name);
            }
            
            if (!string.IsNullOrEmpty(_parentUId))
            {
                writer.WritePropertyName("parent_uid");
                writer.WriteStringValue(_parentUId);
            }
            
            writer.WriteEndObject();
            writer.WriteEndObject();
            writer.Flush();

            this.ByteContent = stream.ToArray();
        }
        #endregion
    }
}
