using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contentstack.Management.Core.Tests.Helpers
{
    public enum NetworkErrorType
    {
        Timeout,
        ConnectionRefused,
        DnsFailure,
        SslError,
        NetworkUnreachable,
        ProxyError,
        ConnectionReset
    }

    public class MockNetworkErrorHandler : LoggingHttpHandler
    {
        private readonly NetworkErrorType _errorType;
        private readonly int _delayMs;

        public MockNetworkErrorHandler(NetworkErrorType errorType, int delayMs = 0)
        {
            _errorType = errorType;
            _delayMs = delayMs;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await CaptureRequest(request);
            }
            catch
            {
                // Never let logging break the request
            }

            if (_delayMs > 0)
            {
                await Task.Delay(_delayMs, cancellationToken);
            }

            // Simulate network errors
            switch (_errorType)
            {
                case NetworkErrorType.Timeout:
                    throw new TaskCanceledException("The operation was canceled.", 
                        new TimeoutException("The request timed out"));

                case NetworkErrorType.ConnectionRefused:
                    throw new HttpRequestException("Connection refused");

                case NetworkErrorType.DnsFailure:
                    throw new HttpRequestException("No such host is known");

                case NetworkErrorType.SslError:
                    throw new HttpRequestException("The SSL connection could not be established");

                case NetworkErrorType.NetworkUnreachable:
                    throw new HttpRequestException("Network is unreachable");

                case NetworkErrorType.ProxyError:
                    throw new HttpRequestException("Unable to connect to the remote server");

                case NetworkErrorType.ConnectionReset:
                    throw new HttpRequestException("An existing connection was forcibly closed by the remote host");

                default:
                    return await base.SendAsync(request, cancellationToken);
            }
        }

        private async Task CaptureRequest(HttpRequestMessage request)
        {
            try
            {
                TestOutputLogger.LogHttpRequest(
                    method: request.Method.ToString(),
                    url: request.RequestUri?.ToString() ?? "",
                    headers: new System.Collections.Generic.Dictionary<string, string>(),
                    body: request.Content != null ? await request.Content.ReadAsStringAsync() : "",
                    curlCommand: $"curl -X {request.Method} '{request.RequestUri}'",
                    sdkMethod: $"MockError:{_errorType}"
                );
            }
            catch
            {
                // Never let logging break the request
            }
        }
    }

    public class MockTimeoutHandler : LoggingHttpHandler
    {
        private readonly int _timeoutMs;

        public MockTimeoutHandler(int timeoutMs = 5000)
        {
            _timeoutMs = timeoutMs;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var timeoutCts = new CancellationTokenSource(_timeoutMs))
            using (var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, timeoutCts.Token))
            {
                try
                {
                    // Simulate a long delay that will cause timeout
                    await Task.Delay(int.MaxValue, combinedCts.Token);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    throw new TaskCanceledException("The operation was canceled due to timeout");
                }
            }
        }
    }
}