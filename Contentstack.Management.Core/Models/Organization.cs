using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Organization;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    public class Organization
    {
        private readonly ContentstackClient _client;       

        #region Constructor
        public Organization(ContentstackClient contentstackClient, string uid = null)
        {
            _client = contentstackClient;
            Uid = uid;
        }
        #endregion

        #region Public
        public string Uid { get; set; }

        /// <summary>
        /// The Get all/single organizations call lists all organizations related to the system user in the order that they were created.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization();
        /// ContentstackResponse contentstackResponse = organization.GetOrganizations();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetOrganizations(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();

            var Organizations = new GetOrganizations(_client.serializer, parameters, this.Uid);

            return _client.InvokeSync(Organizations);
        }

        /// <summary>
        /// The Get all/single organizations call lists all organizations related to the system user in the order that they were created.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization();
        /// ContentstackResponse contentstackResponse = await organization.GetOrganizationsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> GetOrganizationsAsync(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();

            var Organizations = new GetOrganizations(_client.serializer, parameters, this.Uid);

            return _client.InvokeAsync<GetOrganizations, ContentstackResponse>(Organizations);
        }

        /// <summary>
        /// The Get all roles in an organization call gives the details of all the roles that are set to users in an Organization.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.Roles();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse Roles(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var Roles = new OrganizationRolesService(_client.serializer, this.Uid, parameters);

            return _client.InvokeSync(Roles);
        }

        /// <summary>
        /// The Get all roles in an organization call gives the details of all the roles that are set to users in an Organization.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.RolesAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> RolesAsync(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var Roles = new OrganizationRolesService(_client.serializer, this.Uid, parameters);

            return _client.InvokeAsync<OrganizationRolesService, ContentstackResponse>(Roles);
        }

        /// <summary>
        /// The Add users to organization call allows you to send invitations to add users to your organization. Only the owner or the admin of the organization can add users.
        /// </summary>
        /// <param name="orgInvite">List of User invitation.</param>
        /// <param name="stackInvite">Stack Uid with user invitation details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// UserInvitation invitation = new UserInvitation()
        /// {
        ///         Email = &quot;&lt;EMAIL&gt;&quot;,
        ///         Roles = new System.Collections.Generic.List&lt;string&gt;() { &quot;&lt;ROLE_UID&gt;&quot; }
        /// };
        /// ContentstackResponse contentstackResponse = organization.AddUser(new System.Collections.Generic.List&lt;UserInvitation&gt;()
        ///     {
        ///         invitation
        ///     },
        ///  new  Dictionary&lt;string, List&lt;UserInvitation&gt;&gt; ()
        ///     {
        ///           &quot;&lt;STACK_UID&gt;&quot;= invitation
        ///     });
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse AddUser(List<UserInvitation> orgInvite, Dictionary<string, List<UserInvitation>> stackInvite)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var userInviteService = new UserInvitationService(_client.serializer, this.Uid, "POST");

            if (orgInvite != null)
            {
                userInviteService.AddOrganizationInvite(orgInvite);
            }

            if (stackInvite != null)
            {
                userInviteService.AddStackInvite(stackInvite);
            }
            return _client.InvokeSync(userInviteService);
        }

        /// <summary>
        /// The Add users to organization call allows you to send invitations to add users to your organization. Only the owner or the admin of the organization can add users.
        /// </summary>
        /// <param name="orgInvite">List of User invitation.</param>
        /// <param name="stackInvite">Stack Uid with user invitation details.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// UserInvitation invitation = new UserInvitation()
        /// {
        ///         Email = &quot;&lt;EMAIL&gt;&quot;,
        ///         Roles = new System.Collections.Generic.List&lt;string&gt;() { &quot;&lt;ROLE_UID&gt;&quot; }
        /// };
        /// ContentstackResponse contentstackResponse = await organization.AddUserAsync(new System.Collections.Generic.List&lt;UserInvitation&gt;()
        ///     {
        ///         invitation
        ///     },
        ///  new  &lt;string, List&lt;UserInvitation&gt;&gt; ()
        ///     {
        ///           &quot;&lt;STACK_UID&gt;&quot;= invitation
        ///     }
        ///  );
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> AddUserAsync(List<UserInvitation> orgInvite, Dictionary<string, List<UserInvitation>> stackInvite)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var userInviteService = new UserInvitationService(_client.serializer, this.Uid, "POST");

            if (orgInvite != null)
            {
                userInviteService.AddOrganizationInvite(orgInvite);
            }

            if (stackInvite != null)
            {
                userInviteService.AddStackInvite(stackInvite);
            }
            return _client.InvokeAsync<UserInvitationService, ContentstackResponse>(userInviteService);
        }

        /// <summary>
        /// The Remove users from organization request allows you to remove existing users from your organization.
        /// </summary>
        /// <param name="emails">List of emails to be remove from the Organization</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.RemoveUser(new List() { &quot;&lt;EMAIL&gt;&quot; });
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse RemoveUser(List<string> emails)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();
            if (emails == null)
            {
                throw new ArgumentNullException("emails");
            }
            var userInviteService = new UserInvitationService(_client.serializer, this.Uid, "DELETE");
            userInviteService.RemoveUsers(emails);
            return _client.InvokeSync(userInviteService);
        }

        /// <summary>
        /// The Remove users from organization request allows you to remove existing users from your organization.
        /// </summary>
        /// <param name="emails">List of emails to be remove from the Organization</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.RemoveUserAsync(new List() { &quot;&lt;EMAIL&gt;&quot; });
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> RemoveUserAsync(List<string> emails)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();
            if (emails == null)
            {
                throw new ArgumentNullException("emails");
            }
            var userInviteService = new UserInvitationService(_client.serializer, this.Uid, "DELETE");
            userInviteService.RemoveUsers(emails);
            return _client.InvokeAsync<UserInvitationService, ContentstackResponse>(userInviteService);
        }

        /// <summary>
        /// he Resend pending organization invitation call allows you to resend Organization invitations to users who have not yet accepted the earlier invitation.
        /// Only the owner or the admin of the Organization can resend the invitation to add users to an Organization.
        /// </summary>
        /// <param name="shareUid">Uid for share invitation send to user.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.ResendInvitation(&quot;&lt;SHARE_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse ResendInvitation(string shareUid)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();
            
            var userInviteService = new ResendInvitationService(_client.serializer, this.Uid, shareUid);
            return _client.InvokeSync(userInviteService);
        }

        /// <summary>
        /// he Resend pending organization invitation call allows you to resend Organization invitations to users who have not yet accepted the earlier invitation.
        /// Only the owner or the admin of the Organization can resend the invitation to add users to an Organization.
        /// </summary>
        /// <param name="shareUid">Uid for share invitation send to user.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.ResendInvitationAsync(&quot;&lt;SHARE_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ResendInvitationAsync(string shareUid)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();
            
            var userInviteService = new ResendInvitationService(_client.serializer, this.Uid, shareUid);
            return _client.InvokeAsync<ResendInvitationService, ContentstackResponse>(userInviteService);
        }

        /// <summary>
        /// The Get all organization invitations call gives you a list of all the Organization invitations. 
        /// </summary>
        /// <param name="parameter">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.GetInvitations();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetInvitations(ParameterCollection parameter = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var userInviteService = new UserInvitationService(_client.serializer, this.Uid, "GET", parameter);

            return _client.InvokeSync(userInviteService);
        }

        /// <summary>
        /// The Get all organization invitations call gives you a list of all the Organization invitations. 
        /// </summary>
        /// <param name="parameter">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.GetInvitationsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> GetInvitationsAsync(ParameterCollection parameter = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var userInviteService = new UserInvitationService(_client.serializer, this.Uid, "GET", parameter);

            return _client.InvokeAsync<UserInvitationService, ContentstackResponse>(userInviteService);
        }

        /// <summary>
        /// The Transfer organization ownership call transfers the ownership of an Organization to another user.
        /// </summary>
        /// <param name="email">The email id of user for transfer.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.TransferOwnership(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse TransferOwnership(string email)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var service = new TransferOwnershipService(_client.serializer, this.Uid, email);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Transfer organization ownership call transfers the ownership of an Organization to another user.
        /// </summary>
        /// <param name="email">The email id of user for transfer.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.TransferOwnershipAsync(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> TransferOwnershipAsync(string email)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var service = new TransferOwnershipService(_client.serializer, this.Uid, email);

            return _client.InvokeAsync<TransferOwnershipService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The get Stacks call gets all the Stack within the Organization.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = organization.GetStacks();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetStacks(ParameterCollection parameter = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var service = new OrganizationStackService(_client.serializer, this.Uid, parameter);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The get Stacks call gets all the Stack within the Organization.
        /// </summary>
        /// <param name="parameter">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Organization organization = client.Organization(&quot;&lt;ORG_UID&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await organization.GetStacksAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> GetStacksAsync(ParameterCollection parameter = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfOrganizationUidNull();

            var service = new OrganizationStackService(_client.serializer, this.Uid, parameter);

            return _client.InvokeAsync<OrganizationStackService, ContentstackResponse>(service);
        }
        #endregion

        #region Private

        private void ThrowIfOrganizationUidNull()
        {
            if (string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException(CSConstants.MissingUID);
            }
        }
        #endregion
    }
}
