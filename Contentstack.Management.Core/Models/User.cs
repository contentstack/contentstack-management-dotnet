using System.Threading.Tasks;
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


        #region Password

        /// <summary>
        /// The Forgot password call sends a request for a temporary password to log in to an account in case a user has forgotten the login password.
        /// </summary>
        /// <param name="email">The email for the account that user has forgotten the login password</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// User user = client.User();
        /// ContentstackResponse contentstackResponse = client.ForgotPassword(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse" />.</returns>
        public ContentstackResponse ForgotPassword(string email)
        {
            _client.ThrowIfAlreadyLoggedIn();

            var forgotPassword = new ForgotPasswordService(_client.serializer, email);

            return _client.InvokeSync(forgotPassword);
        }

        /// <summary>
        /// The Forgot password call sends a request for a temporary password to log in to an account in case a user has forgotten the login password.
        /// </summary>
        /// <param name="email">The email for the account that user has forgotten the login password</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// User user = client.User();
        /// ContentstackResponse contentstackResponse = await client.ForgotPasswordAsync(&quot;&lt;EMAIL&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> ForgotPasswordAsync(string email)
        {
            _client.ThrowIfAlreadyLoggedIn();

            var forgotPassword = new ForgotPasswordService(_client.serializer, email);

            return _client.InvokeAsync<ForgotPasswordService, ContentstackResponse>(forgotPassword);
        }

        /// <summary>
        /// The Reset password call sends a request for resetting the password of your Contentstack account.
        /// </summary>
        /// <param name="resetToken">The reset password token send to email.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="confirmPassword">The confirm password for the account.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// User user = client.User();
        /// ContentstackResponse contentstackResponse = client.ResetPassword(&quot;&lt;REST_TOKEN&gt;&quot;, &quot;&lt;PASSWORD&gt;&quot;, &quot;&lt;CONFIRM_PASSWORD&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse ResetPassword(string resetToken, string password, string confirmPassword)
        {
            _client.ThrowIfAlreadyLoggedIn();

            var resetPassword = new ResetPasswordService(_client.serializer, resetToken, password, confirmPassword);

            return _client.InvokeSync(resetPassword);
        }

        /// <summary>
        /// The Reset password call sends a request for resetting the password of your Contentstack account.
        /// </summary>
        /// <param name="resetToken">The reset password token send to email.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="confirmPassword">The confirm password for the account.</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// User user = client.User();
        /// ContentstackResponse contentstackResponse = await client.ResetPasswordAsync(&quot;&lt;REST_TOKEN&gt;&quot;, &quot;&lt;PASSWORD&gt;&quot;, &quot;&lt;CONFIRM_PASSWORD&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> ResetPasswordAsync(string resetToken, string password, string confirmPassword)
        {
            _client.ThrowIfAlreadyLoggedIn();

            var resetPassword = new ResetPasswordService(_client.serializer, resetToken, password, confirmPassword);

            return _client.InvokeAsync<ResetPasswordService, ContentstackResponse>(resetPassword);
        }
        #endregion

    }
}
