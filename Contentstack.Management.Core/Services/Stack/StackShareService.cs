using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackShareService : ContentstackService
    {
        private List<UserInvitation> _invitations;

        private string _removeUser;

        internal StackShareService(Core.Models.Stack stack, JsonSerializerOptions stjOptions = null) : base(stjOptions ?? new JsonSerializerOptions(), stack)
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
            object requestData;
            
            if (_invitations != null)
            {
                requestData = new
                {
                    emails = _invitations.Select(u => u.Email).ToArray(),
                    roles = _invitations.ToDictionary(
                        invitation => invitation.Email,
                        invitation => invitation.Roles.ToArray()
                    )
                };
            }
            else if (_removeUser != null)
            {
                requestData = new
                {
                    email = _removeUser
                };
            }
            else
            {
                requestData = new { };
            }

            string jsonString = JsonSerializer.Serialize(requestData, SerializerOptions);
            this.ByteContent = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
