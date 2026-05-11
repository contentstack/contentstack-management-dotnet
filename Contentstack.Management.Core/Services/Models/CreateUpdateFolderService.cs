using System;
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
            JsonSerializerOptions serializerOptions,
            Core.Models.Stack stack,
            string name,
            string folderUid = null,
            string parentUId = null)
            : base(serializerOptions, stack)
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
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("asset");
                writer.WriteStartObject();
                if (!string.IsNullOrEmpty(_name))
                    writer.WriteString("name", _name);
                if (!string.IsNullOrEmpty(_parentUId))
                    writer.WriteString("parent_uid", _parentUId);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
        #endregion
    }
}
