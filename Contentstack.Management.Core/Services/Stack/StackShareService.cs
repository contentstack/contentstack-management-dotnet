using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackShareService : ContentstackService
    {
        private List<UserInvitation> _invitations;

        private string _removeUser;

        internal StackShareService(JsonSerializer serializer, Core.Models.Stack stack) : base(serializer, stack)
        {
            if (string.IsNullOrEmpty(stack.APIKey))
            {
                throw new ArgumentNullException("apiKey");
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
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                if (_invitations != null)
                {
                    writer.WritePropertyName("emails");
                    writer.WriteStartArray();
                    foreach (UserInvitation user in _invitations)
                        writer.WriteValue(user.Email);
                    writer.WriteEndArray();

                    writer.WritePropertyName("roles");
                    writer.WriteStartObject();
                    foreach (UserInvitation invitation in _invitations)
                    {
                        writer.WritePropertyName(invitation.Email);
                        writer.WriteStartArray();
                        foreach (string role in invitation.Roles)
                            writer.WriteValue(role);
                        writer.WriteEndArray();
                    }
                    writer.WriteEndObject();
                }
                else if (_removeUser != null)
                {
                    writer.WritePropertyName("email");
                    writer.WriteValue(_removeUser);
                }
                writer.WriteEndObject();

                string snippet = stringWriter.ToString();
                this.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
    }
}
