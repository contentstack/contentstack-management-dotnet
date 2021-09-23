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

        public ContentstackResponse Share(List<UserInvitation> invitations)
        {
            _client.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (invitations == null)
            {
                throw new ArgumentNullException("Invitations can not be null.");
            }

            var service = new StackShareService(_client.serializer, this.APIKey); ;
            service.AddUsers(invitations);

            return _client.InvokeSync(service);
        }

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
