using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackShareService : ContentstackService
    {
        private List<UserInvitation> _invitations;

        private string _removeUser;

        internal StackShareService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack) : base(serializerOptions, stack)
        {
            if (string.IsNullOrEmpty(stack.APIKey))
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            HttpMethod = "POST";
        }

        internal void AddUsers(List<UserInvitation> invitations)
        {
            this._invitations = invitations;
            ResourcePath = "stacks/share";
        }

        internal void RemoveUsers(string emails)
        {
            this._removeUser = emails;
            ResourcePath = "stacks/unshare";
        }

        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                if (_invitations != null)
                {
                    writer.WritePropertyName("emails");
                    writer.WriteStartArray();
                    foreach (UserInvitation user in _invitations)
                        writer.WriteStringValue(user.Email);
                    writer.WriteEndArray();

                    writer.WritePropertyName("roles");
                    writer.WriteStartObject();
                    foreach (UserInvitation invitation in _invitations)
                    {
                        writer.WritePropertyName(invitation.Email);
                        writer.WriteStartArray();
                        foreach (string role in invitation.Roles)
                            writer.WriteStringValue(role);
                        writer.WriteEndArray();
                    }
                    writer.WriteEndObject();
                }
                else if (_removeUser != null)
                {
                    writer.WriteString("email", _removeUser);
                }
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
    }
}
