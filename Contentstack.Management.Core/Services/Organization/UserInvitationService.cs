using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class UserInvitationService : ContentstackService
    {
        private List<UserInvitation>? _organizationInvite;
        private Dictionary<string, List<UserInvitation>>? _stackInvite;
        private List<string>? _removeUsers;

        #region Internal
        internal UserInvitationService(JsonSerializerOptions serializerOptions, string? uid, string httpMethod = "GET", ParameterCollection? collection = null) : base(serializerOptions, collection: collection)
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
            using var ms = new MemoryStream();
            switch (this.HttpMethod)
            {
                case "POST":
                    WriteAddInvite(ms);
                    break;
                case "DELETE":
                    WriteRemoveUsers(ms);
                    break;
                default:
                    break;
            }

            this.ByteContent = ms.ToArray();
        }
        #endregion

        #region Private
        private void WriteRemoveUsers(Stream stream)
        {
            if (this._removeUsers == null || this._removeUsers.Count == 0)
                return;

            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("emails");
                writer.WriteStartArray();
                foreach (string email in _removeUsers)
                    writer.WriteStringValue(email);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        private void WriteAddInvite(Stream stream)
        {
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("share");
                writer.WriteStartObject();
                if (this._organizationInvite != null && this._organizationInvite.Count > 0)
                {
                    writer.WritePropertyName("users");
                    writer.WriteStartObject();
                    foreach (UserInvitation invitation in this._organizationInvite)
                    {
                        writer.WritePropertyName(invitation.Email!);
                        writer.WriteStartArray();
                        foreach (string role in invitation.Roles!)
                            writer.WriteStringValue(role);
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
                        WriteStackInvites(writer, key);
                    }
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
                writer.WriteEndObject();
            }
        }

        private void WriteStackInvites(Utf8JsonWriter writer, string key)
        {
            foreach (UserInvitation invitation in this._stackInvite![key])
            {
                writer.WritePropertyName(invitation.Email!);
                writer.WriteStartObject();
                writer.WritePropertyName(key);
                writer.WriteStartArray();
                foreach (string role in invitation.Roles!)
                    writer.WriteStringValue(role);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
        #endregion
    }
}
