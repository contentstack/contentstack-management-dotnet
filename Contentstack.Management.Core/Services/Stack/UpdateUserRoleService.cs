using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class UpdateUserRoleService : ContentstackService
    {
        #region Internal
        private readonly List<UserInvitation> _users;

        internal UpdateUserRoleService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, List<UserInvitation>userInvitation)
            : base(serializerOptions, stack)
        {
            if (userInvitation == null)
            {
                throw new ArgumentNullException("userInvitation", CSConstants.UserInvitationDetailsRequired);
            }

            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            this.ResourcePath = "stacks/users/roles";
            this.HttpMethod = "POST";
            _users = userInvitation;
        }

        public override void ContentBody()
        {
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("users");
                writer.WriteStartObject();
                foreach (UserInvitation invitation in this._users)
                {
                    writer.WritePropertyName(invitation.Uid);
                    writer.WriteStartArray();
                    foreach (string role in invitation.Roles)
                        writer.WriteStringValue(role);
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            this.ByteContent = ms.ToArray();
        }
        #endregion
    }
}
