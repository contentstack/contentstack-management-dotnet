using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Stack;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Models
{
    public class Stack
    {
        internal ContentstackClient client;
        public string APIKey { get; private set; }
        public string ManagementToken { get; private set; }
        public string BranchUid { get; private set; }

        #region Constructor
        public Stack(ContentstackClient contentstackClient, string apiKey = null, string managementToken = null, string branchUid = null)
        {
            client = contentstackClient;
            APIKey = apiKey;
            ManagementToken = managementToken;
            BranchUid = branchUid;
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();

            var service = new FetchStackService(client.serializer, this, parameters);

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();

            var service = new FetchStackService(client.serializer, this, parameters);

            return client.InvokeAsync<FetchStackService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new FetchStackService(client.serializer, this, parameters);
            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new FetchStackService(client.serializer, this, parameters);

            return client.InvokeAsync<FetchStackService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackOwnershipService(client.serializer, this, email);

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackOwnershipService(client.serializer, this, email);

            return client.InvokeAsync<StackOwnershipService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();
            this.ThrowInvalideName(name);
            this.ThrowInvalideLocale(masterLocale);
            this.ThrowInvalideOrganizationUid(organisationUid);

            var service = new StackCreateUpdateService(client.serializer, this, name, masterLocale, description, organizationUid: organisationUid);

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyNotEmpty();
            this.ThrowInvalideName(name);
            this.ThrowInvalideLocale(masterLocale);
            this.ThrowInvalideOrganizationUid(organisationUid);

            var service = new StackCreateUpdateService(client.serializer, this, name, masterLocale, description, organizationUid: organisationUid);

            return client.InvokeAsync<StackCreateUpdateService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            this.ThrowInvalideName(name);

            var service = new StackCreateUpdateService(client.serializer, this, name, description: description);

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            this.ThrowInvalideName(name);

            var service = new StackCreateUpdateService(client.serializer, this, name, description: description);

            return client.InvokeAsync<StackCreateUpdateService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new UpdateUserRoleService(client.serializer, this, usersRole);

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new UpdateUserRoleService(client.serializer, this, usersRole);

            return client.InvokeAsync<UpdateUserRoleService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(client.serializer, this);

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(client.serializer, this);

            return client.InvokeAsync<StackSettingsService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(client.serializer, this, "POST", new StackSettings()
            {
                StackVariables = new Dictionary<string, object>(),
                DiscreteVariables = new Dictionary<string, object>(),
                Rte = new Dictionary<string, object>()
            }); ;

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            var service = new StackSettingsService(client.serializer, this, "POST", new StackSettings()
            {
                StackVariables = new Dictionary<string, object>(),
                DiscreteVariables = new Dictionary<string, object>(),
                Rte = new Dictionary<string, object>()
            });

            return client.InvokeAsync<StackSettingsService, ContentstackResponse>(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (settings == null)
            {
                throw new ArgumentNullException("Settings can not be null.");
            }

            var service = new StackSettingsService(client.serializer, this, "POST", settings); ;

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (settings == null)
            {
                throw new ArgumentNullException("Settings can not be null.");
            }
            var service = new StackSettingsService(client.serializer, this, "POST", settings);

            return client.InvokeAsync<StackSettingsService, ContentstackResponse>(service);
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
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse Share(List<UserInvitation> invitations)
        {
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (invitations == null)
            {
                throw new ArgumentNullException("Invitations can not be null.");
            }

            var service = new StackShareService(client.serializer, this);
            service.AddUsers(invitations);

            return client.InvokeSync(service);
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
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (invitations == null)
            {
                throw new ArgumentNullException("Invitations can not be null.");
            }

            var service = new StackShareService(client.serializer, this);
            service.AddUsers(invitations);

            return client.InvokeAsync<StackShareService, ContentstackResponse>(service);
        }
        /// <summary>
        /// The Unshare a stack call unshares a stack with a user and removes the user account from the list of collaborators. 
        /// </summary>
        /// <param name="email"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.UnShare((&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse UnShare(string email)
        {
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (email == null)
            {
                throw new ArgumentNullException("Email can not be null.");
            }

            var service = new StackShareService(client.serializer, this); ;
            service.RemoveUsers(email);

            return client.InvokeSync(service);
        }

        /// <summary>
        /// The Unshare a stack call unshares a stack with a user and removes the user account from the list of collaborators. 
        /// </summary>
        /// <param name="email"></param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await stack.UnShareAsync(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UnShareAsync(string email)
        {
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (email == null)
            {
                throw new ArgumentNullException("Email can not be null.");
            }

            var service = new StackShareService(client.serializer, this);
            service.RemoveUsers(email);

            return client.InvokeAsync<StackShareService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Contentstack has a sophisticated multilingual capability. It allows you to create and publish entries in any language.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.Locale(&quot;&lt;LOCALE_CODE&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <param name="code">Locale code fot language</param>
        /// <returns>The <see cref="Models.Locale"/></returns>
        public Locale Locale(string code = null)
        {
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            return new Locale(this, code);
        }

        /// <summary>
        /// <see cref="Models.ContentType" /> defines the structure or schema of a page or a section of your web or mobile property.
        /// To create content for your application, you are required to first create a content type, and then create entries using the content type. 
        /// </summary>
        /// <param name="uid"> Optional content type uid.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.ContentType"/></returns>
        public ContentType ContentType(string uid = null)
        {
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            return new ContentType(this, uid);
        }
        /// <summary>
        /// <see cref="Models.Asset"/> efer to all the media files (images, videos, PDFs, audio files, and so on) uploaded in your Contentstack repository for future use. 
        /// </summary>
        /// <param name="uid">Optional asset uid.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.Asset(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.Asset"/></returns>
        public Asset Asset(string uid = null)
        {
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            return new Asset(this, uid);
        }

        /// <summary>
        /// A <see cref="Models.GlobalField" /> is a reusable field (or group of fields) that you can define once and reuse in any content type within your stack.
        /// This eliminates the need (and thereby time and efforts) to create the same set of fields repeatedly in multiple content types. 
        /// </summary>
        /// <param name="uid"> Optional global field uid.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// Stack stack = client.Stack(&quot;&lt;API_KEY&gt;&quot;);
        /// ContentstackResponse contentstackResponse = stack.GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Models.GlobalField"/></returns>
        public GlobalField GlobalField(string uid = null)
        {
            client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();

            return new GlobalField(this, uid);
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

        internal void ThrowIfAlreadyLoggedIn()
        {
            if (!string.IsNullOrEmpty(this.ManagementToken))
            {
                throw new InvalidOperationException(CSConstants.YouAreLoggedIn);
            }
            else
            {
                client.ThrowIfAlreadyLoggedIn();
            }
        }

        internal void ThrowIfNotLoggedIn()
        {
            if (string.IsNullOrEmpty(this.ManagementToken))
            {
                throw new InvalidOperationException(CSConstants.YouAreNotLoggedIn);
            }
            else
            {
                client.ThrowIfNotLoggedIn();
            }
        }
        #endregion
    }
}
