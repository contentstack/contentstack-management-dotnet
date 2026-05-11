using System;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackOwnershipService : ContentstackService
    {
        private readonly string _email;

        #region Internal
        internal StackOwnershipService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string email)
            : base(serializerOptions, stack)
        {
            if (string.IsNullOrEmpty(stack.APIKey))
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email", CSConstants.EmailRequired);
            }
            this._email = email;
            this.ResourcePath = "stacks/transfer_ownership";
            this.HttpMethod = "POST";
        }
        #endregion

        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WriteString("transfer_to", _email);
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
    }
}
