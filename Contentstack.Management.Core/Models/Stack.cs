using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Stack;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    public class Stack
    {
        private ContentstackClient _client;
        public string APIKey { get; private set; }
        public string ManagementToken { get; private set; }

        #region Constructor
        public Stack(ContentstackClient contentstackClient, string apiKey = null, string managementToken = null)
        {
            _client = contentstackClient;
            APIKey = apiKey;
            ManagementToken = managementToken;
        }
        #endregion

        #region Public
        /// <summary>
        /// The Get all stacks call fetches the list of all stacks owned by and shared with a particular user account.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack();
        /// ContentstackResponse contentstackResponse = stack.GetAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetAll(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();

            var service = new FetchStackService(_client.serializer, parameters);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Get all stacks call fetches the list of all stacks owned by and shared with a particular user account.
        /// </summary>
        /// <param name="collection">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack =  client.Stack();
        /// ContentstackResponse contentstackResponse = await stack.GetAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> GetAllAsync(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();

            var service = new FetchStackService(_client.serializer, parameters);

            return _client.InvokeAsync<FetchStackService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The Get a single stack call fetches comprehensive details of a specific stack.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse Fetch(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new FetchStackService(_client.serializer, parameters, this.APIKey);
            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Get a single stack call fetches comprehensive details of a specific stack.
        /// </summary>
        /// <param name="parameters">URI query parameters</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await stack.FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> FetchAsync(ParameterCollection parameters = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new FetchStackService(_client.serializer, parameters, this.APIKey);

            return _client.InvokeAsync<FetchStackService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Transfer stack ownership to other users call sends the specified user an email invitation for accepting the ownership of a particular stack.
        /// </summary>
        /// <param name="email">The email id of user for transfer.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.TransferOwnership(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse TransferOwnership(string email)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackOwnershipService(_client.serializer, this.APIKey, email);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Transfer stack ownership to other users call sends the specified user an email invitation for accepting the ownership of a particular stack.
        /// </summary>
        /// <param name="email">The email id of user for transfer.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await stack.TransferOwnershipAsync(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> TransferOwnershipAsync(string email)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackOwnershipService(_client.serializer, this.APIKey, email);

            return _client.InvokeAsync<StackOwnershipService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Create stack call creates a new stack in your Contentstack account.
        /// </summary>
        /// <param name="name">The name for Stack.</param>
        /// <param name="masterLocale">The Master Locale for Stack</param>
        /// <param name="organisationUid">The Organization Uid in which you want to create Stack.</param>
        /// <param name="description">The description for the Stack.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack();
        /// ContentstackResponse contentstackResponse = stack.Create(&quot;&lt;STACK_NAME&gt;&quot;, &quot;&lt;LOCALE&gt;&quot;, &quot;&lt;ORG_UID&gt;&quot;, &quot;&lt;DESCRIPTION&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse Create(string name, string masterLocale, string organisationUid, string description = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();
            this.ThrowInvalideName(name);
            this.ThrowInvalideLocale(masterLocale);
            this.ThrowInvalideOrganizationUid(organisationUid);

            var service = new StackCreateUpdateService(_client.serializer, name, masterLocale, description, organizationUid: organisationUid);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Create stack call creates a new stack in your Contentstack account.
        /// </summary>
        /// <param name="name">The name for Stack.</param>
        /// <param name="masterLocale">The Master Locale for Stack</param>
        /// <param name="organisationUid">The Organization Uid in which you want to create Stack.</param>
        /// <param name="description">The description for the Stack.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack();
        /// ContentstackResponse contentstackResponse = await stack.CreateAsync(&quot;&lt;STACK_NAME&gt;&quot;, &quot;&lt;LOCALE&gt;&quot;, &quot;&lt;ORG_UID&gt;&quot;, &quot;&lt;DESCRIPTION&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> CreateAsync(string name, string masterLocale, string organisationUid, string description = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();
            this.ThrowInvalideName(name);
            this.ThrowInvalideLocale(masterLocale);
            this.ThrowInvalideOrganizationUid(organisationUid);

            var service = new StackCreateUpdateService(_client.serializer, name, masterLocale, description, organizationUid: organisationUid);

            return _client.InvokeAsync<StackCreateUpdateService, ContentstackResponse>(service);
        }

        /// <summary>
        /// TThe Update stack call lets you update the name and description of an existing stack.
        /// </summary>
        /// <param name="name">The name for Stack.</param>
        /// <param name="masterLocale">The Master Locale for Stack</param>
        /// <param name="organisationUid">The Organization Uid in which you want to create Stack.</param>
        /// <param name="description">The description for the Stack.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.Update(&quot;&lt;STACK_NAME&gt;&quot;, &quot;&lt;DESCRIPTION&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse Update(string name, string description = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            this.ThrowInvalideName(name);

            var service = new StackCreateUpdateService(_client.serializer, name, description: description, apiKey: this.APIKey);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// TThe Update stack call lets you update the name and description of an existing stack.
        /// </summary>
        /// <param name="name">The name for Stack.</param>
        /// <param name="masterLocale">The Master Locale for Stack</param>
        /// <param name="organisationUid">The Organization Uid in which you want to create Stack.</param>
        /// <param name="description">The description for the Stack.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await stack.UpdateAsync(&quot;&lt;STACK_NAME&gt;&quot;, &quot;&lt;DESCRIPTION&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UpdateAsync(string name, string description = null)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            this.ThrowInvalideName(name);

            var service = new StackCreateUpdateService(_client.serializer, name, description: description, apiKey: this.APIKey);

            return _client.InvokeAsync<StackCreateUpdateService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Update User Role API Request updates the roles of an existing user account.
        /// </summary>
        /// <param name="usersRole">List of users uid and roles to assign users.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// UserInvitation invitation = new UserInvitation()
        /// {
        ///         Uid = &quot;&lt;USER_ID&gt;&quot;,
        ///         Roles = new System.Collections.Generic.List&lt;string&gt;() { &quot;&lt;ROLE_UID&gt;&quot; }
        /// };
        /// ContentstackResponse contentstackResponse = stack.UpdateUserRole(new List&lt;UserInvitation&gt;() {
        ///     invitation
        /// });
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse UpdateUserRole(List<UserInvitation> usersRole)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new UpdateUserRoleService(_client.serializer, usersRole, apiKey: this.APIKey);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Update User Role API Request updates the roles of an existing user account.
        /// </summary>
        /// <param name="usersRole"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// UserInvitation invitation = new UserInvitation()
        /// {
        ///         Uid = &quot;&lt;USER_ID&gt;&quot;,
        ///         Roles = new System.Collections.Generic.List&lt;string&gt;() { &quot;&lt;ROLE_UID&gt;&quot; }
        /// };
        /// ContentstackResponse contentstackResponse = await stack.UpdateUserRoleAsync(new List&lt;UserInvitation&gt;() {
        ///     invitation
        /// });
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UpdateUserRoleAsync(List<UserInvitation> usersRole)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new UpdateUserRoleService(_client.serializer, usersRole, apiKey: this.APIKey);

            return _client.InvokeAsync<UpdateUserRoleService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Get stack settings call retrieves the configuration settings of an existing stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.Settings();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse Settings()
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(_client.serializer, this.APIKey);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Get stack settings call retrieves the configuration settings of an existing stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await stack.SettingsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> SettingsAsync()
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(_client.serializer, this.APIKey);

            return _client.InvokeAsync<StackSettingsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Reset stack settings call resets your stack to default settings, and additionally, lets you add parameters to or modify the settings of an existing stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.Settings();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse ResetSettings()
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(_client.serializer, this.APIKey, "POST", new StackSettings()
            {
                StackVariables = new Dictionary<string, object>(),
                DiscreteVariables = new Dictionary<string, object>(),
                Rte = new Dictionary<string, object>()
            }); ;

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Reset stack settings call resets your stack to default settings, and additionally, lets you add parameters to or modify the settings of an existing stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await stack.SettingsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ResetSettingsAsync()
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(_client.serializer, this.APIKey, "POST", new StackSettings()
            {
                StackVariables = new Dictionary<string, object>(),
                DiscreteVariables = new Dictionary<string, object>(),
                Rte = new Dictionary<string, object>()
            });

            return _client.InvokeAsync<StackSettingsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Add stack settings request lets you add additional settings for your existing stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.Settings(&quot;&lt;STACK_SETTINGS&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse AddSettings(StackSettings settings)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (settings == null)
            {
                throw new ArgumentNullException("Settings can not be null.");
            }

            var service = new StackSettingsService(_client.serializer, this.APIKey, "POST", settings); ;

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Add stack settings request lets you add additional settings for your existing stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await stack.SettingsAsync(&quot;&lt;STACK_SETTINGS&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> AddSettingsAsync(StackSettings settings)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (settings == null)
            {
                throw new ArgumentNullException("Settings can not be null.");
            }
            var service = new StackSettingsService(_client.serializer, this.APIKey, "POST", settings);

            return _client.InvokeAsync<StackSettingsService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The Share a stack call shares a stack with the specified user to collaborate on the stack.
        /// </summary>
        /// <param name="invitations"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// UserInvitation invitation = new UserInvitation()
        /// {
        ///         Email = &quot;&lt;EMAIL&gt;&quot;,
        ///         Roles = new System.Collections.Generic.List&lt;string&gt;() { &quot;&lt;ROLE_UID&gt;&quot; }
        /// };
        /// ContentstackResponse contentstackResponse = stack.Share(invitation);
        /// </code></pre>
        /// </example>
        /// <returns></returns>
        public ContentstackResponse Share(List<UserInvitation> invitations)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (invitations == null)
            {
                throw new ArgumentNullException("Invitations can not be null.");
            }

            var service = new StackShareService(_client.serializer, this.APIKey);
            service.AddUsers(invitations);

            return _client.InvokeSync(service);
        }
        /// <summary>
        /// The Share a stack call shares a stack with the specified user to collaborate on the stack.
        /// </summary>
        /// <param name="invitations"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// UserInvitation invitation = new UserInvitation()
        /// {
        ///         Email = &quot;&lt;EMAIL&gt;&quot;,
        ///         Roles = new System.Collections.Generic.List&lt;string&gt;() { &quot;&lt;ROLE_UID&gt;&quot; }
        /// };
        /// ContentstackResponse contentstackResponse = await stack.ShareAsync(invitation);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ShareAsync(List<UserInvitation> invitations)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (invitations == null)
            {
                throw new ArgumentNullException("Invitations can not be null.");
            }

            var service = new StackShareService(_client.serializer, this.APIKey);
            service.AddUsers(invitations);

            return _client.InvokeAsync<StackShareService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The Unshare a stack call unshares a stack with a user and removes the user account from the list of collaborators. 
        /// </summary>
        /// <param name="email"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.UnShare((&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns></returns>
        public ContentstackResponse UnShare(string email)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (email == null)
            {
                throw new ArgumentNullException("Email can not be null.");
            }

            var service = new StackShareService(_client.serializer, this.APIKey); ;
            service.RemoveUsers(email);

            return _client.InvokeSync(service);
        }

        /// <summary>
        /// The Unshare a stack call unshares a stack with a user and removes the user account from the list of collaborators. 
        /// </summary>
        /// <param name="email"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.UnShareAsync(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns></returns>
        public Task<ContentstackResponse> UnShareAsync(string email)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (email == null)
            {
                throw new ArgumentNullException("Email can not be null.");
            }

            var service = new StackShareService(_client.serializer, this.APIKey);
            service.RemoveUsers(email);

            return _client.InvokeAsync<StackShareService, ContentstackResponse>(service);
        }

        #endregion

        #region Throw Error

        internal void ThrowIfAPIKeyNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.APIKey))
            {
                throw new InvalidOperationException(CSConstants.APIKey);
            }
        }

        internal void ThrowIfAPIKeyEmpty()
        {
            if (string.IsNullOrEmpty(this.APIKey))
            {
                throw new InvalidOperationException(CSConstants.MissingAPIKey);
            }
        }
        internal void ThrowInvalideName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Invalide name for the Stack.");
            }
        }

        internal void ThrowInvalideLocale(string locale)
        {
            if (string.IsNullOrEmpty(locale))
            {
                throw new ArgumentNullException("Invalide name for the Stack.");
            }
        }

        internal void ThrowInvalideOrganizationUid(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("Invalide Organization UID.");
            }
        }

        #endregion
    }
}
