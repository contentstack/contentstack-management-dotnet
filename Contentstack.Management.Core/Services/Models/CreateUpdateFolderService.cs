using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class CreateUpdateFolderService : ContentstackService
    {
        private readonly string _name;
        private readonly string _parentUId;

        #region Internal
        internal CreateUpdateFolderService(
            JsonSerializer serializer,
            Core.Models.Stack stack,
            string name,
            string folderUid = null,
            string parentUId = null)
            : base(serializer, stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", "Should have API Key to perform this operation.");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name", "Should have folder name.");
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
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("asset");
                writer.WriteStartObject();
                if (!string.IsNullOrEmpty(_name))
                {
                    writer.WritePropertyName("name");
                    writer.WriteValue(_name);
                }
                if (!string.IsNullOrEmpty(_parentUId))
                {
                    writer.WritePropertyName("parent_uid");
                    writer.WriteValue(_parentUId);
                }
                writer.WriteEndObject();
                writer.WriteEndObject();

                string snippet = stringWriter.ToString();
                this.ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
        #endregion
    }
}
