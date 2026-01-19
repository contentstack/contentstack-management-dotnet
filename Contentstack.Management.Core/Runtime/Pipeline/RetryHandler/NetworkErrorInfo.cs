namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    /// Information about a detected network error.
    /// </summary>
    public class NetworkErrorInfo
    {
        /// <summary>
        /// The type of network error detected.
        /// </summary>
        public NetworkErrorType ErrorType { get; set; }

        /// <summary>
        /// Indicates if this is a transient error that should be retried.
        /// </summary>
        public bool IsTransient { get; set; }

        /// <summary>
        /// The original exception that caused this network error.
        /// </summary>
        public System.Exception OriginalException { get; set; }

        /// <summary>
        /// Creates a new NetworkErrorInfo instance.
        /// </summary>
        public NetworkErrorInfo(NetworkErrorType errorType, bool isTransient, System.Exception originalException)
        {
            ErrorType = errorType;
            IsTransient = isTransient;
            OriginalException = originalException;
        }
    }

    /// <summary>
    /// Types of network errors that can occur.
    /// </summary>
    public enum NetworkErrorType
    {
        /// <summary>
        /// DNS resolution failure.
        /// </summary>
        DnsFailure,

        /// <summary>
        /// Socket connection error (connection reset, refused, etc.).
        /// </summary>
        SocketError,

        /// <summary>
        /// Request timeout.
        /// </summary>
        Timeout,

        /// <summary>
        /// HTTP server error (5xx).
        /// </summary>
        HttpServerError,

        /// <summary>
        /// Unknown or unclassified error.
        /// </summary>
        Unknown
    }
}

