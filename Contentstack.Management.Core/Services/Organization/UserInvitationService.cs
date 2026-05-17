using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class UserInvitationService : ContentstackService
    {
        private List<UserInvitation> _organizationInvite;
        private Dictionary<string, List<UserInvitation>> _stackInvite;
        private List<string> _removeUsers;

        #region Internal
        internal UserInvitationService(Newtonsoft.Json.JsonSerializer serializer, string uid, string httpMethod = "GET", ParameterCollection collection = null, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft) : base(serializer, collection: collection, stjOptions: stjOptions, serializationMode: serializationMode)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid", CSConstants.OrganizationUIDRequired);
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod", CSConstants.HTTPMethodRequired);
            }
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
            this.ResourcePath = "/organizations/{organization_uid}/share";
            this.HttpMethod = httpMethod;
            this.AddPathResource("{organization_uid}", uid);
        }

        internal void AddOrganizationInvite(List<UserInvitation> orgInvite)
        {
            this._organizationInvite = orgInvite;
        }

        internal void AddStackInvite(Dictionary<string, List<UserInvitation>> stackInvite)
        {
            this._stackInvite = stackInvite;
        }

        internal void RemoveUsers(List<string> emails)
        {
            this._removeUsers = emails;
        }

        public override void ContentBody()
        {
            object requestObject = null;
            
            switch (this.HttpMethod)
            {
                case "POST":
                    requestObject = BuildAddUserRequestObject();
                    break;
                case "DELETE":
                    requestObject = BuildRemoveUserRequestObject();
                    break;
                default:
                    break;
            }

            if (requestObject != null)
            {
                var mode = GetSerializationMode();
                WriteObjectWithBothEngines(requestObject, mode, GetSerializerSettings(), GetStjOptions(), out byte[] content);
                this.ByteContent = content;
            }
        }
        #endregion

        #region Private
        
        private object BuildRemoveUserRequestObject()
        {
            if (_removeUsers != null && _removeUsers.Count > 0)
            {
                return new { emails = _removeUsers };
            }
            return null;
        }

        private object BuildAddUserRequestObject()
        {
            var share = new Dictionary<string, object>();

            if (_organizationInvite != null && _organizationInvite.Count > 0)
            {
                var users = new Dictionary<string, List<string>>();
                foreach (UserInvitation invitation in _organizationInvite)
                {
                    users[invitation.Email] = invitation.Roles;
                }
                share["users"] = users;
            }

            if (_stackInvite != null && _stackInvite.Keys.Count > 0)
            {
                var stacks = new Dictionary<string, object>();
                foreach (string key in _stackInvite.Keys)
                {
                    foreach (UserInvitation invitation in _stackInvite[key])
                    {
                        stacks[invitation.Email] = new Dictionary<string, List<string>> { [key] = invitation.Roles };
                    }
                }
                share["stacks"] = stacks;
            }

            return new { share };
        }
        private void removeUserJsonWriter(StringWriter stringWriter)
        {
            if (this._removeUsers != null && this._removeUsers.Count > 0)
            {
                JsonWriter writer = new JsonTextWriter(stringWriter);
                writer.WriteStartObject();
                writer.WritePropertyName("emails");
                writer.WriteStartArray();
                foreach (string email in _removeUsers)
                    writer.WriteValue(email);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        private void addUserJsonWriter(StringWriter stringWriter)
        {
            JsonWriter writer = new JsonTextWriter(stringWriter);
            writer.WriteStartObject();
            writer.WritePropertyName("share");
            writer.WriteStartObject();
            if (this._organizationInvite != null && this._organizationInvite.Count > 0)
            {
                writer.WritePropertyName("users");
                writer.WriteStartObject();
                foreach (UserInvitation invitation in this._organizationInvite)
                {
                    writer.WritePropertyName(invitation.Email);
                    writer.WriteStartArray();
                    foreach (string role in invitation.Roles)
                        writer.WriteValue(role);

                    writer.WriteEndArray();

                }
                writer.WriteEndObject();

            }
            if (this._stackInvite != null && this._stackInvite.Keys.Count > 0)
            {
                writer.WritePropertyName("stacks");
                writer.WriteStartObject();
                foreach (string key in this._stackInvite.Keys)
                {
                    UserInvitationJsonWriter(writer, key);
                }
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        private void UserInvitationJsonWriter(JsonWriter writer, string key)
        {
            foreach (UserInvitation invitation in this._stackInvite[key])
            {
                writer.WritePropertyName(invitation.Email);
                writer.WriteStartObject();
                writer.WritePropertyName(key);
                writer.WriteStartArray();
                foreach (string role in invitation.Roles)
                    writer.WriteValue(role);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
        #endregion
    }
}
