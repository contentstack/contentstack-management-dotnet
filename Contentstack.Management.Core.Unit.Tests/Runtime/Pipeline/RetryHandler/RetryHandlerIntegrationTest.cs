using System;
using System.Net;
using System.Net.Sockets;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;
using ContentstackRetryHandler = Contentstack.Management.Core.Runtime.Pipeline.RetryHandler.RetryHandler;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline.RetryHandler
{
    [TestClass]
    public class RetryHandlerIntegrationTest
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
        public async Task EndToEnd_NetworkError_Retries_And_Succeeds()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 3,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(10),
                NetworkBackoffStrategy = BackoffStrategy.Exponential
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddFailuresThenSuccess(2, MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessStatusCode);
            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.NetworkRetryCount);
        }

        [TestMethod]
        public async Task EndToEnd_HttpError_Retries_And_Succeeds()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 3,
                RetryDelay = TimeSpan.FromMilliseconds(10),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(10)
                }
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, HttpStatusCode.InternalServerError);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessStatusCode);
            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task EndToEnd_Mixed_Network_And_Http_Errors()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 3,
                RetryLimit = 3,
                RetryOnHttpServerError = true,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(10),
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddException(MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            mockInnerHandler.AddResponse(HttpStatusCode.InternalServerError);
            mockInnerHandler.AddResponse((HttpStatusCode)429);
            mockInnerHandler.AddSuccessResponse();
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessStatusCode);
            Assert.AreEqual(4, mockInnerHandler.CallCount);
            Assert.AreEqual(1, context.RequestContext.NetworkRetryCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task EndToEnd_Respects_RetryConfiguration()
        {
            var config = new RetryConfiguration
            {
                RetryOnError = false
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
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

            // Should not retry when RetryOnError is false
            Assert.AreEqual(1, mockInnerHandler.CallCount);
        }

        [TestMethod]
        public async Task EndToEnd_ExponentialBackoff_Delays_Increase()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 2,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(50),
                NetworkBackoffStrategy = BackoffStrategy.Exponential
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddFailuresThenSuccess(2, MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var startTime = DateTime.UtcNow;
            await handler.InvokeAsync<ContentstackResponse>(context);
            var totalElapsed = DateTime.UtcNow - startTime;

            // First retry: ~50ms, second retry: ~100ms (exponential)
            // Total should be at least 150ms + jitter
            Assert.IsTrue(totalElapsed >= TimeSpan.FromMilliseconds(150));
        }

        [TestMethod]
        public async Task EndToEnd_RetryLimit_Stops_Retries()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2,
                RetryDelay = TimeSpan.FromMilliseconds(10)
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddResponse((HttpStatusCode)429);
            mockInnerHandler.AddResponse((HttpStatusCode)429);
            mockInnerHandler.AddResponse((HttpStatusCode)429);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            
            try
            {
                await handler.InvokeAsync<ContentstackResponse>(context);
                Assert.Fail("Should have thrown exception");
            }
            catch (ContentstackErrorException)
            {
                // Expected
            }

            // Should stop after 2 retries (3 total calls)
            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task EndToEnd_With_CustomRetryCondition()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2,
                RetryDelay = TimeSpan.FromMilliseconds(10),
                RetryCondition = (statusCode) => statusCode == HttpStatusCode.NotFound
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, HttpStatusCode.NotFound);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var result = await handler.InvokeAsync<ContentstackResponse>(context);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessStatusCode);
            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }

        [TestMethod]
        public async Task EndToEnd_With_CustomBackoff()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2,
                RetryDelay = TimeSpan.FromMilliseconds(10),
                RetryDelayOptions = new RetryDelayOptions
                {
                    CustomBackoff = (retryCount, error) => TimeSpan.FromMilliseconds(100 * (retryCount + 1))
                }
            };
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, (HttpStatusCode)429);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var startTime = DateTime.UtcNow;
            await handler.InvokeAsync<ContentstackResponse>(context);
            var elapsed = DateTime.UtcNow - startTime;

            // Custom backoff: first retry 100ms, second retry 200ms = 300ms total
            Assert.IsTrue(elapsed >= TimeSpan.FromMilliseconds(300));
            Assert.AreEqual(3, mockInnerHandler.CallCount);
        }
    }
}

