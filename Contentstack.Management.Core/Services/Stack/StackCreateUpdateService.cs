using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
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
            JsonSerializer serializer,
            Core.Models.Stack stack,
            string name,
            string masterLocale = null,
            string description = null,
            string organizationUid = null)
            : base(serializer, stack)
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
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("stack");
                writer.WriteStartObject();
                if (!string.IsNullOrEmpty(_name))
                {
                    writer.WritePropertyName("name");
                    writer.WriteValue(_name);
                }
                if (!string.IsNullOrEmpty(_description))
                {
                    writer.WritePropertyName("description");
                    writer.WriteValue(_description);
                }
                switch (this.HttpMethod)
                {
                    case "POST":
                        writer.WritePropertyName("master_locale");
                        writer.WriteValue(_masterLocale);
                        break;
                    default:
                        break;
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
