using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Utility to generate various network error exceptions for testing.
    /// </summary>
    public static class MockNetworkErrorGenerator
    {
        public static SocketException CreateSocketException(SocketError errorCode)
        {
            return new SocketException((int)errorCode);
        }

        public static HttpRequestException CreateHttpRequestExceptionWithSocketException(SocketError socketError)
        {
            var socketException = CreateSocketException(socketError);
            return new HttpRequestException("Network error", socketException);
        }

        public static TaskCanceledException CreateTaskCanceledExceptionTimeout()
        {
            var cts = new CancellationTokenSource();
            return new TaskCanceledException("Operation timed out", null, cts.Token);
        }

        public static TaskCanceledException CreateTaskCanceledExceptionUserCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            return new TaskCanceledException("User cancelled", null, cts.Token);
        }

        public static TimeoutException CreateTimeoutException()
        {
            return new TimeoutException("Operation timed out");
        }

        public static ContentstackErrorException CreateContentstackErrorException(HttpStatusCode statusCode)
        {
            return new ContentstackErrorException
            {
                StatusCode = statusCode,
                Message = $"HTTP {statusCode} error"
            };
        }
    }
}

