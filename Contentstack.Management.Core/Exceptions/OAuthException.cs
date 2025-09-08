using System;

namespace Contentstack.Management.Core.Exceptions
{
    /// <summary>
    /// Base exception class for OAuth-related errors in the Contentstack Management SDK.
    /// </summary>
    public class OAuthException : ContentstackException
    {
        /// <summary>
        /// Initializes a new instance of the OAuthException class.
        /// </summary>
        public OAuthException() : base("An OAuth error occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OAuthException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public OAuthException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when OAuth configuration is invalid or missing required parameters.
    /// </summary>
    public class OAuthConfigurationException : OAuthException
    {
        /// <summary>
        /// Initializes a new instance of the OAuthConfigurationException class.
        /// </summary>
        public OAuthConfigurationException() : base("OAuth configuration error occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthConfigurationException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OAuthConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthConfigurationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public OAuthConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when OAuth token operations fail.
    /// </summary>
    public class OAuthTokenException : OAuthException
    {
        /// <summary>
        /// Initializes a new instance of the OAuthTokenException class.
        /// </summary>
        public OAuthTokenException() : base("OAuth token error occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthTokenException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OAuthTokenException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthTokenException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public OAuthTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when OAuth authorization fails.
    /// </summary>
    public class OAuthAuthorizationException : OAuthException
    {
        /// <summary>
        /// Initializes a new instance of the OAuthAuthorizationException class.
        /// </summary>
        public OAuthAuthorizationException() : base("OAuth authorization error occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthAuthorizationException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OAuthAuthorizationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthAuthorizationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public OAuthAuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when OAuth token refresh fails.
    /// </summary>
    public class OAuthTokenRefreshException : OAuthTokenException
    {
        /// <summary>
        /// Initializes a new instance of the OAuthTokenRefreshException class.
        /// </summary>
        public OAuthTokenRefreshException() : base("OAuth token refresh error occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthTokenRefreshException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OAuthTokenRefreshException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthTokenRefreshException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public OAuthTokenRefreshException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
