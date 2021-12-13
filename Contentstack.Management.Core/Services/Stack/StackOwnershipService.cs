using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackOwnershipService : ContentstackService
    {
        private string _email;

        #region Internal
        internal StackOwnershipService(JsonSerializer serializer, Core.Models.Stack stack, string email)
            : base(serializer, stack)
        {
            if (string.IsNullOrEmpty(stack.APIKey))
            {
                throw new ArgumentNullException("apiKey");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
            this._email = email;
            this.ResourcePath = "stacks/transfer_ownership";
            this.HttpMethod = "POST";
        }
        #endregion

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("transfer_to");
                writer.WriteValue(_email);
                writer.WriteEndObject();
                string snippet = stringWriter.ToString();
                this.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}