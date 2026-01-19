using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline.RetryHandler
{
    [TestClass]
    public class NetworkErrorDetectorTest
    {
        private NetworkErrorDetector detector;

        [TestInitialize]
        public void Initialize()
        {
            detector = new NetworkErrorDetector();
        }

        [TestMethod]
        public void Should_Detect_SocketException_ConnectionReset()
        {
            var exception = new SocketException((int)SocketError.ConnectionReset);
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.SocketError, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
            Assert.AreEqual(exception, result.OriginalException);
        }

        [TestMethod]
        public void Should_Detect_SocketException_TimedOut()
        {
            var exception = new SocketException((int)SocketError.TimedOut);
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.SocketError, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Detect_SocketException_ConnectionRefused()
        {
            var exception = new SocketException((int)SocketError.ConnectionRefused);
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.SocketError, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Detect_SocketException_HostNotFound()
        {
            var exception = new SocketException((int)SocketError.HostNotFound);
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.DnsFailure, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Detect_SocketException_TryAgain()
        {
            var exception = new SocketException((int)SocketError.TryAgain);
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.DnsFailure, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Detect_TaskCanceledException_Timeout()
        {
            var cts = new CancellationTokenSource();
            var exception = new TaskCanceledException("Operation timed out", null, cts.Token);
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.Timeout, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Not_Detect_TaskCanceledException_UserCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var exception = new TaskCanceledException("User cancelled", null, cts.Token);
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Detect_TimeoutException()
        {
            var exception = new TimeoutException("Operation timed out");
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.Timeout, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Detect_HttpRequestException_With_Inner_SocketException()
        {
            var socketException = new SocketException((int)SocketError.ConnectionReset);
            var httpException = new HttpRequestException("Network error", socketException);
            var result = detector.IsTransientNetworkError(httpException);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.SocketError, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Detect_ContentstackErrorException_5xx()
        {
            var exception = new ContentstackErrorException
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.HttpServerError, result.ErrorType);
            Assert.IsTrue(result.IsTransient);
        }

        [TestMethod]
        public void Should_Detect_ContentstackErrorException_502()
        {
            var exception = new ContentstackErrorException
            {
                StatusCode = HttpStatusCode.BadGateway
            };
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.HttpServerError, result.ErrorType);
        }

        [TestMethod]
        public void Should_Detect_ContentstackErrorException_503()
        {
            var exception = new ContentstackErrorException
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            };
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.HttpServerError, result.ErrorType);
        }

        [TestMethod]
        public void Should_Detect_ContentstackErrorException_504()
        {
            var exception = new ContentstackErrorException
            {
                StatusCode = HttpStatusCode.GatewayTimeout
            };
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNotNull(result);
            Assert.AreEqual(NetworkErrorType.HttpServerError, result.ErrorType);
        }

        [TestMethod]
        public void Should_Not_Detect_ContentstackErrorException_4xx()
        {
            var exception = new ContentstackErrorException
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Not_Detect_ContentstackErrorException_404()
        {
            var exception = new ContentstackErrorException
            {
                StatusCode = HttpStatusCode.NotFound
            };
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Return_Null_For_NonNetworkError()
        {
            var exception = new ArgumentException("Invalid argument");
            var result = detector.IsTransientNetworkError(exception);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Return_Null_For_Null()
        {
            var result = detector.IsTransientNetworkError(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldRetryNetworkError_Respects_Configuration()
        {
            var socketException = new SocketException((int)SocketError.ConnectionReset);
            var errorInfo = new NetworkErrorInfo(NetworkErrorType.SocketError, true, socketException);

            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true
            };

            var result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldRetryNetworkError_DnsFailure_Respects_RetryOnDnsFailure()
        {
            var socketException = new SocketException((int)SocketError.HostNotFound);
            var errorInfo = new NetworkErrorInfo(NetworkErrorType.DnsFailure, true, socketException);

            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnDnsFailure = true
            };

            var result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsTrue(result);

            config.RetryOnDnsFailure = false;
            result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldRetryNetworkError_SocketError_Respects_RetryOnSocketFailure()
        {
            var socketException = new SocketException((int)SocketError.ConnectionReset);
            var errorInfo = new NetworkErrorInfo(NetworkErrorType.SocketError, true, socketException);

            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true
            };

            var result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsTrue(result);

            config.RetryOnSocketFailure = false;
            result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldRetryNetworkError_HttpServerError_Respects_RetryOnHttpServerError()
        {
            var httpException = new ContentstackErrorException { StatusCode = HttpStatusCode.InternalServerError };
            var errorInfo = new NetworkErrorInfo(NetworkErrorType.HttpServerError, true, httpException);

            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnHttpServerError = true
            };

            var result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsTrue(result);

            config.RetryOnHttpServerError = false;
            result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldRetryNetworkError_Returns_False_When_RetryOnNetworkFailure_Is_False()
        {
            var socketException = new SocketException((int)SocketError.ConnectionReset);
            var errorInfo = new NetworkErrorInfo(NetworkErrorType.SocketError, true, socketException);

            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = false,
                RetryOnSocketFailure = true
            };

            var result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldRetryNetworkError_Returns_False_When_Not_Transient()
        {
            var exception = new ArgumentException("Not transient");
            var errorInfo = new NetworkErrorInfo(NetworkErrorType.Unknown, false, exception);

            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true
            };

            var result = detector.ShouldRetryNetworkError(errorInfo, config);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldRetryNetworkError_Returns_False_When_Null()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true
            };

            var result = detector.ShouldRetryNetworkError(null, config);
            Assert.IsFalse(result);
        }
    }
}

