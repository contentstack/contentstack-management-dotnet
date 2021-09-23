using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class UpdateUserRoleService : ContentstackService
    {
        #region Internal
        private List<UserInvitation> _users;

        internal UpdateUserRoleService(JsonSerializer serializer, List<UserInvitation>userInvitation, string apiKey)
            : base(serializer, apiKey: apiKey)
        {
            if (userInvitation == null)
            {
                throw new ArgumentNullException("Uid and roles should be present.");
            }

            if (apiKey == null)
            {
                throw new ArgumentNullException("API Key should be present.");
            }
            this.ResourcePath = "stacks/users/roles";
            this.HttpMethod = "POST";
            _users = userInvitation;
        }

        public override void ContentBody()
        {
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("users");
                writer.WriteStartObject();
                foreach (UserInvitation invitation in this._users)
                {
                    writer.WritePropertyName(invitation.Uid);
                    writer.WriteStartArray();
                    foreach (string role in invitation.Roles)
                        writer.WriteValue(role);
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
                string snippet = stringWriter.ToString();
                this.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
        #endregion
    }
}