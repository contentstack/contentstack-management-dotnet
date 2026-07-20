using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;
using RetryHandlerClass = Contentstack.Management.Core.Runtime.Pipeline.RetryHandler.RetryHandler;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline.RetryHandler
{
    [TestClass]
    public class RetryHandlerTest
    {
        private ExecutionContext CreateExecutionContext()
        {
            return new ExecutionContext(
                new RequestContext
                {
                    config = new ContentstackClientOptions(),
                    service = new MockService()
                },
                new ResponseContext());
        }

        [TestMethod]
        public async Task InvokeAsync_Success_NoRetry()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 3,
                MaxNetworkRetries = 2
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddSuccessResponse();
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, mockInnerHandler.CallCount);
            Assert.AreEqual(0, context.RequestContext.NetworkRetryCount);
            Assert.AreEqual(0, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_NetworkError_Retries_UpTo_MaxNetworkRetries()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 2,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddFailuresThenSuccess(2, MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, mockInnerHandler.CallCount); // 2 failures + 1 success
            Assert.AreEqual(2, context.RequestContext.NetworkRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_NetworkError_Exceeds_MaxNetworkRetries_Throws()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 2,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddException(MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            mockInnerHandler.AddException(MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            mockInnerHandler.AddException(MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            
            try
            {
                await handler.InvokeAsync<ContentstackResponse>(context);
                Assert.Fail("Should have thrown exception");
            }
            catch (SocketException)
            {
                // Expected
            }

            Assert.AreEqual(3, mockInnerHandler.CallCount); // 3 failures
            Assert.AreEqual(2, context.RequestContext.NetworkRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_HttpError_429_Retries_UpTo_RetryLimit()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2,
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, HttpStatusCode.TooManyRequests);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, mockInnerHandler.CallCount); // 2 failures + 1 success
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_HttpError_500_Retries_UpTo_RetryLimit()
        {
            var config = new RetryConfiguration
            {
                RetryOnHttpServerError = true,
                RetryLimit = 2,
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, HttpStatusCode.InternalServerError);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_HttpError_Exceeds_RetryLimit_Throws()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2,
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddResponse(HttpStatusCode.TooManyRequests);
            mockInnerHandler.AddResponse(HttpStatusCode.TooManyRequests);
            mockInnerHandler.AddResponse(HttpStatusCode.TooManyRequests);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            
            try
            {
                await handler.InvokeAsync<ContentstackResponse>(context);
                Assert.Fail("Should have thrown exception");
            }
            catch (ContentstackErrorException ex)
            {
                Assert.AreEqual(HttpStatusCode.TooManyRequests, ex.StatusCode);
            }

            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_NetworkError_Tracks_NetworkRetryCount()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 3,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddFailuresThenSuccess(1, MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.AreEqual(1, context.RequestContext.NetworkRetryCount);
            Assert.AreEqual(0, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_HttpError_Tracks_HttpRetryCount()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 3,
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(1, HttpStatusCode.TooManyRequests);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.AreEqual(0, context.RequestContext.NetworkRetryCount);
            Assert.AreEqual(1, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_NetworkError_Then_HttpError_Tracks_Both_Counts()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 3,
                RetryLimit = 3,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(10),
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddException(MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            mockInnerHandler.AddResponse(HttpStatusCode.TooManyRequests);
            mockInnerHandler.AddSuccessResponse();
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.AreEqual(1, context.RequestContext.NetworkRetryCount);
            Assert.AreEqual(1, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task InvokeAsync_Applies_NetworkRetryDelay()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 1,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(50),
                NetworkBackoffStrategy = BackoffStrategy.Fixed
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddFailuresThenSuccess(1, MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            // Use Stopwatch for higher-resolution elapsed time measurement.
            // DateTime.UtcNow has ~15ms resolution on Windows CI runners, which makes
            // a tight 50ms assertion unreliable. Stopwatch uses QueryPerformanceCounter
            // and is immune to system clock resolution.
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await handler.InvokeAsync<ContentstackResponse>(context);
            stopwatch.Stop();

            // The configured delay is 50ms (Fixed strategy, no exponential multiplier).
            // We assert >= 30ms rather than >= 50ms to provide a 20ms margin of safety
            // against OS timer resolution variance on Windows CI (system timer fires at
            // ~15.6ms intervals, so Task.Delay(50) can return as early as ~46ms).
            // A reading below 30ms would definitively indicate the delay path was skipped.
            Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromMilliseconds(30),
                $"Expected network retry delay of at least 30ms, but elapsed was {stopwatch.Elapsed.TotalMilliseconds:F1}ms");
        }

        [TestMethod]
        public async Task InvokeAsync_Applies_HttpRetryDelay()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 1,
                RetryDelay = TimeSpan.FromMilliseconds(50),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(50)
                }
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(1, HttpStatusCode.TooManyRequests);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            // Use Stopwatch for higher-resolution elapsed time measurement.
            // See InvokeAsync_Applies_NetworkRetryDelay for full explanation.
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await handler.InvokeAsync<ContentstackResponse>(context);
            stopwatch.Stop();

            // The configured base delay is 50ms with exponential backoff (retryCount=0 on first retry,
            // so effective delay = 50ms * 2^0 = 50ms). Assert >= 30ms for the same OS timer margin.
            // A reading below 30ms would definitively indicate the delay path was skipped.
            Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromMilliseconds(30),
                $"Expected HTTP retry delay of at least 30ms, but elapsed was {stopwatch.Elapsed.TotalMilliseconds:F1}ms");
        }

        [TestMethod]
        public async Task InvokeAsync_RequestId_Is_Generated()
        {
            var config = new RetryConfiguration();
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddSuccessResponse();
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.AreNotEqual(Guid.Empty, context.RequestContext.RequestId);
        }

        [TestMethod]
        public void InvokeSync_Success_NoRetry()
        {
            var config = new RetryConfiguration();
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddSuccessResponse();
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            handler.InvokeSync(context);

            Assert.AreEqual(1, mockInnerHandler.CallCount);
        }

        [TestMethod]
        public void InvokeSync_NetworkError_Retries()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 2,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddFailuresThenSuccess(2, MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            handler.InvokeSync(context);

            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.NetworkRetryCount);
        }

        [TestMethod]
        public void InvokeSync_HttpError_Retries()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2,
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new RetryHandlerClass(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, HttpStatusCode.TooManyRequests);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            handler.InvokeSync(context);

            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }
    }
}

