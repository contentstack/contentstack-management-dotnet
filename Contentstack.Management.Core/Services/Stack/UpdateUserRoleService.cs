using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class UpdateUserRoleService : ContentstackService
    {
        #region Internal
        private readonly List<UserInvitation> _users;

        internal UpdateUserRoleService(Core.Models.Stack stack, List<UserInvitation> userInvitation, JsonSerializerOptions? stjOptions = null)
            : base(stjOptions ?? new JsonSerializerOptions(), stack)
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
            var requestData = new
            {
                users = _users.ToDictionary(
                    invitation => invitation.Uid!,
                    invitation => invitation.Roles!.ToArray()
                )
            };

            string jsonString = JsonSerializer.Serialize(requestData, SerializerOptions);
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
        #endregion
    }
}