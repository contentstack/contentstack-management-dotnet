using System;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Services.User;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// <see cref="User" /> session consists of calls that will help you to sign in and sign out of your Contentstack account.
    /// </summary>
    public class User
    {
        private ContentstackClient _client;

        #region Constructor
        internal User(ContentstackClient contentstackClient)
        {
            _client = contentstackClient;
        }
        #endregion

        #region LoginMethod
        /// <summary>
        /// The Log in to your account request is used to sign in to your Contentstack account and obtain the authtoken.
        /// </summary>
        /// <param name="credentials">User credentials for login.</param>
        /// <param name="token">The optional 2FA token.</param>
        /// <returns></returns>
        public ContentstackResponse Login(ICredentials credentials, string token = null)
        {
            ThrowIfAlreadyLoggedIn();
            var Login = new LoginService(_client.serializer, credentials, token);
            
            return _client.InvokeSync(Login);
        }

        /// <summary>
        /// The Log in to your account request is used to sign in to your Contentstack account and obtain the authtoken.
        /// </summary>
        /// <param name="credentials">User credentials for login.</param>
        /// <param name="token">The optional 2FA token.</param>
        /// <returns></returns>
        public Task<ContentstackResponse> LoginAsync(ICredentials credentials, string token = null)
        {
            ThrowIfAlreadyLoggedIn();
            var Login = new LoginService(_client.serializer, credentials, token);
            
            return _client.InvokeAsync<LoginService, ContentstackResponse>(Login);
        }
        #endregion

        #region Throw Error

        private void ThrowIfAlreadyLoggedIn()
        {
            if (!string.IsNullOrEmpty(_client.contentstackOptions.Authtoken))
            {
                throw new InvalidOperationException(CSConstants.YouAreLoggedIn);
            }
        }

        private void ThrowIfNotLoggedIn()
        {
            if (string.IsNullOrEmpty(_client.contentstackOptions.Authtoken))
            {
                throw new InvalidOperationException(CSConstants.YouAreNotLoggedIn);
            }
        }
        #endregion

        #region LogoutMethod
        /// <summary>
        /// The Log out of your account call is used to sign out the user of Contentstack account
        /// </summary>
        /// <param name="authtoken">The optional authroken in case user want to logout.</param>
        /// <returns>The <see cref="ContentstackResponse" /></returns>
        public ContentstackResponse Logout(string authtoken = null)
        {
            var token = authtoken ?? _client.contentstackOptions.Authtoken;
            var logout = new LogoutService(_client.serializer, token);

            return _client.InvokeSync(logout);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authtoken">The optional authroken in case user want to logout.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> LogoutAsync(string authtoken = null)
        {
            var token = authtoken ?? _client.contentstackOptions.Authtoken;
            var logout = new LogoutService(_client.serializer, token);

            return _client.InvokeAsync<LogoutService, ContentstackResponse>(logout);
        }
        #endregion

        #region Password

        /// <summary>
        /// The Forgot password call sends a request for a temporary password to log in to an account in case a user has forgotten the login password.
        /// </summary>
        /// <param name="email">The email for the account that user has forgotten the login password</param>
        /// <returns>The <see cref="ContentstackResponse" /></returns>
        public ContentstackResponse ForgotPassword(string email)
        {
            ThrowIfAlreadyLoggedIn();

            var forgotPassword = new ForgotPasswordService(_client.serializer, email);

            return _client.InvokeSync(forgotPassword);
        }

        /// <summary>
        /// The Forgot password call sends a request for a temporary password to log in to an account in case a user has forgotten the login password.
        /// </summary>
        /// <param name="email">The email for the account that user has forgotten the login password</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> ForgotPasswordAsync(string email)
        {
            ThrowIfAlreadyLoggedIn();

            var forgotPassword = new ForgotPasswordService(_client.serializer, email);

            return _client.InvokeAsync<ForgotPasswordService, ContentstackResponse>(forgotPassword);
        }

        /// <summary>
        /// The Reset password call sends a request for resetting the password of your Contentstack account.
        /// </summary>
        /// <param name="resetToken">The reset password token send to email.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="confirmPassword">The confirm password for the account.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse ResetPassword(string resetToken, string password, string confirmPassword)
        {
            ThrowIfAlreadyLoggedIn();

            var resetPassword = new ResetPasswordService(_client.serializer, resetToken, password, confirmPassword);

            return _client.InvokeSync(resetPassword);
        }

        /// <summary>
        /// The Reset password call sends a request for resetting the password of your Contentstack account.
        /// </summary>
        /// <param name="resetToken">The reset password token send to email.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="confirmPassword">The confirm password for the account.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> ResetPasswordAsync(string resetToken, string password, string confirmPassword)
        {
            ThrowIfAlreadyLoggedIn();

            var resetPassword = new ResetPasswordService(_client.serializer, resetToken, password, confirmPassword);

            return _client.InvokeAsync<ResetPasswordService, ContentstackResponse>(resetPassword);
        }
        #endregion

        /// <summary>
        /// The Get user call returns comprehensive information of an existing user account.
        /// </summary>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse GetUser()
        {
            ThrowIfNotLoggedIn();

            var getUser = new GetLoggedInUserService(_client.serializer);

            return _client.InvokeSync(getUser);
        }

        /// <summary>
        /// The Get user call returns comprehensive information of an existing user account.
        /// </summary>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> GetUserAsync()
        {
            ThrowIfNotLoggedIn();

            var getUser = new GetLoggedInUserService(_client.serializer);

            return _client.InvokeAsync<GetLoggedInUserService, ContentstackResponse>(getUser);
        }
    }
}
