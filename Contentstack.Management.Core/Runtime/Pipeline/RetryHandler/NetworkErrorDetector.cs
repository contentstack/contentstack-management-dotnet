using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    /// Service to detect and classify transient network errors.
    /// </summary>
    public class NetworkErrorDetector
    {
        /// <summary>
        /// Determines if an exception represents a transient network error.
        /// </summary>
        /// <param name="error">The exception to analyze.</param>
        /// <returns>NetworkErrorInfo if it's a transient network error, null otherwise.</returns>
        public NetworkErrorInfo IsTransientNetworkError(Exception error)
        {
            if (error == null)
                return null;

            // Check for SocketException
            if (error is SocketException socketException)
            {
                return DetectSocketError(socketException);
            }

            // Check for HttpRequestException with inner SocketException
            if (error is System.Net.Http.HttpRequestException httpRequestException)
            {
                if (httpRequestException.InnerException is SocketException innerSocketException)
                {
                    return DetectSocketError(innerSocketException);
                }
            }

            // Check for TaskCanceledException (timeout)
            if (error is TaskCanceledException taskCanceledException)
            {
                // Only treat as timeout if it's not a user cancellation
                // TaskCanceledException can occur due to timeout or cancellation
                // We check if the cancellation token was actually cancelled by the user
                if (taskCanceledException.CancellationToken.IsCancellationRequested == false)
                {
                    return new NetworkErrorInfo(NetworkErrorType.Timeout, true, error);
                }
            }

            // Check for TimeoutException
            if (error is TimeoutException)
            {
                return new NetworkErrorInfo(NetworkErrorType.Timeout, true, error);
            }

            // Check for ContentstackErrorException with 5xx status codes
            if (error is ContentstackErrorException contentstackError)
            {
                if (contentstackError.StatusCode >= HttpStatusCode.InternalServerError &&
                    contentstackError.StatusCode <= HttpStatusCode.GatewayTimeout)
                {
                    return new NetworkErrorInfo(NetworkErrorType.HttpServerError, true, error);
                }
            }

            return null;
        }

        /// <summary>
        /// Detects the type of socket error from a SocketException.
        /// </summary>
        private NetworkErrorInfo DetectSocketError(SocketException socketException)
        {
            bool isTransient = false;
            NetworkErrorType errorType = NetworkErrorType.SocketError;

            switch (socketException.SocketErrorCode)
            {
                // DNS-related errors
                case SocketError.HostNotFound:
                case SocketError.TryAgain:
                    errorType = NetworkErrorType.DnsFailure;
                    isTransient = true;
                    break;

                // Transient connection errors
                case SocketError.ConnectionReset:
                case SocketError.TimedOut:
                case SocketError.ConnectionRefused:
                case SocketError.NetworkUnreachable:
                case SocketError.HostUnreachable:
                case SocketError.NoBufferSpaceAvailable:
                    errorType = NetworkErrorType.SocketError;
                    isTransient = true;
                    break;

                // Other socket errors (may or may not be transient)
                default:
                    errorType = NetworkErrorType.SocketError;
                    // Most socket errors are transient, but some are not
                    // We'll be conservative and retry on most socket errors
                    isTransient = true;
                    break;
            }

            return new NetworkErrorInfo(errorType, isTransient, socketException);
        }

        /// <summary>
        /// Determines if a network error should be retried based on configuration.
        /// </summary>
        public bool ShouldRetryNetworkError(NetworkErrorInfo errorInfo, RetryConfiguration config)
        {
            if (errorInfo == null || !errorInfo.IsTransient)
                return false;

            if (!config.RetryOnNetworkFailure)
                return false;

            switch (errorInfo.ErrorType)
            {
                case NetworkErrorType.DnsFailure:
                    return config.RetryOnDnsFailure;

                case NetworkErrorType.SocketError:
                    return config.RetryOnSocketFailure;

                case NetworkErrorType.Timeout:
                    return config.RetryOnNetworkFailure;

                case NetworkErrorType.HttpServerError:
                    return config.RetryOnHttpServerError;

                default:
                    return config.RetryOnNetworkFailure;
            }
        }
    }
}

