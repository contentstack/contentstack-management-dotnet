using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Organization;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    public class Organization
    {
        private ContentstackClient _client;
        public string uid;
        #region Constructor
        public Organization(ContentstackClient contentstackClient, string uid = null)
        {
            _client = contentstackClient;
            this.uid = uid;
        }
        #endregion

        #region Public
        /// <summary>
        /// The Get all/single organizations call lists all organizations related to the system user in the order that they were created.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns></returns>
        public ContentstackResponse GetOrganizations(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();

            var Organizations = new GetOrganizations(_client.serializer, parameters, this.uid);

            return _client.InvokeSync(Organizations);
        }

        /// <summary>
        /// The Get all/single organizations call lists all organizations related to the system user in the order that they were created.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> GetOrganizationsAsync(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();

            var Organizations = new GetOrganizations(_client.serializer, parameters, this.uid);

            return _client.InvokeAsync<GetOrganizations, ContentstackResponse>(Organizations);
        }

        public ContentstackResponse Roles(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var Roles = new OrgRoles(_client.serializer, this.uid, parameters);

            return _client.InvokeSync(Roles);
        }

        public Task<ContentstackResponse> RolesAsync(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var Roles = new OrgRoles(_client.serializer, this.uid, parameters);

            return _client.InvokeAsync<OrgRoles, ContentstackResponse>(Roles);
        }

        private void ThrowIfOrganizationUidNull()
        {
            if (string.IsNullOrEmpty(this.uid))
            {
                throw new InvalidOperationException(CSConstants.MissingUID);
            }
        }
        #endregion
    }
}
