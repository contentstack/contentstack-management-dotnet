using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Organization
{
    internal class UserInvitationService : ContentstackService
    {
        private List<UserInvitation> _organizationInvite;
        private Dictionary<string, List<UserInvitation>> _stackInvite;
        private List<string> _removeUsers;

        #region Internal
        internal UserInvitationService(JsonSerializer serializer, string uid, string httpMethod = "GET", ParameterCollection collection = null) : base(serializer, collection)
        {
            if (uid == null)
            {
                throw new ArgumentNullException("uid");
            }

            if (httpMethod == null)
            {
                throw new ArgumentNullException("HttpMethod");
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
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                switch (this.HttpMethod)
                {
                    case "POST":
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
                                writer.WriteEndObject();
                            }
                            writer.WriteEndObject();
                            writer.WriteEndObject();
                        }
                        break;
                    case "DELETE":
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
                        break;
                    default:
                        break;
                }
                string snippet = stringWriter.ToString();
                this.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }
        }
        #endregion
    }
}
