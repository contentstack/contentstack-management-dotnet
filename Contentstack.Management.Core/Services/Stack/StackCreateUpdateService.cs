using System;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackCreateUpdateService : ContentstackService
    {
        private readonly string _name;
        private readonly string _masterLocale;
        private readonly string _description;

        #region Internal
        internal StackCreateUpdateService(
            JsonSerializerOptions serializerOptions,
            Core.Models.Stack stack,
            string name,
            string masterLocale = null,
            string description = null,
            string organizationUid = null)
            : base(serializerOptions, stack)
        {
            this.ResourcePath = "/stacks";

            _name = name;
            _masterLocale = masterLocale;
            _description = description;

            if (stack.APIKey != null)
            {
                this.HttpMethod = "PUT";
            }
            else if (!string.IsNullOrEmpty(organizationUid))
            {
                if (masterLocale == null)
                {
                    throw new ArgumentNullException("masterLocale", CSConstants.MasterLocaleRequired);
                }
                if (_name == null)
                {
                    throw new ArgumentNullException("name", CSConstants.StackNameRequired);
                }
                this.Headers.Add("organization_uid", organizationUid);
                this.HttpMethod = "POST";
            }
            else 
            {
                throw new ArgumentNullException("stack", CSConstants.APIKeyOrOrgUIDRequired);
            }
        }

        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("stack");
                writer.WriteStartObject();
                if (!string.IsNullOrEmpty(_name))
                    writer.WriteString("name", _name);
                if (!string.IsNullOrEmpty(_description))
                    writer.WriteString("description", _description);
                if (this.HttpMethod == "POST")
                    writer.WriteString("master_locale", _masterLocale);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
        #endregion
    }
}
