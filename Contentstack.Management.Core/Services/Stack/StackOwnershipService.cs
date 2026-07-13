using System;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackOwnershipService : ContentstackService
    {
        private readonly string _email;

        #region Internal
        internal StackOwnershipService(Core.Models.Stack stack, string email, JsonSerializerOptions? stjOptions = null)
            : base(stjOptions ?? new JsonSerializerOptions(), stack)
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
            var requestData = new
            {
                transfer_to = _email
            };

            string jsonString = JsonSerializer.Serialize(requestData, SerializerOptions);
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}