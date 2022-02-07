using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackCreateUpdateService : ContentstackService
    {
        private string _name;
        private string _masterLocale;
        private string _description;
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
                    throw new ArgumentNullException("Should have Master Locale while creating the Stack.");
                }
                if (_name == null)
                {
                    throw new ArgumentNullException("Name for stack is mandatory while creating the Stack.");
                }
                this.Headers.Add("organization_uid", organizationUid);
                this.HttpMethod = "POST";
            }
            else 
            {
                throw new ArgumentNullException("Should have API Key or Organization UID to perform this operation.");
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
