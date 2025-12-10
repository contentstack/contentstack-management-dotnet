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
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, (HttpStatusCode)429);
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
            var handler = new ContentstackRetryHandler(policy);
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
            catch (ContentstackErrorException ex)
            {
                Assert.AreEqual((HttpStatusCode)429, ex.StatusCode);
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
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddException(MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            mockInnerHandler.AddResponse((HttpStatusCode)429);
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
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddFailuresThenSuccess(1, MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset));
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var startTime = DateTime.UtcNow;
            await handler.InvokeAsync<ContentstackResponse>(context);
            var elapsed = DateTime.UtcNow - startTime;

            // Should have waited at least 50ms + jitter
            Assert.IsTrue(elapsed >= TimeSpan.FromMilliseconds(50));
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
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(1, HttpStatusCode.TooManyRequests);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            var startTime = DateTime.UtcNow;
            await handler.InvokeAsync<ContentstackResponse>(context);
            var elapsed = DateTime.UtcNow - startTime;

            // Should have waited at least 50ms + jitter
            Assert.IsTrue(elapsed >= TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public async Task InvokeAsync_RequestId_Is_Generated()
        {
            var config = new RetryConfiguration();
            var policy = new DefaultRetryPolicy(config);
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
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
            var handler = new ContentstackRetryHandler(policy);
            var mockInnerHandler = new MockHttpHandlerWithRetries();
            mockInnerHandler.AddHttpErrorsThenSuccess(2, (HttpStatusCode)429);
            handler.InnerHandler = mockInnerHandler;
            handler.LogManager = LogManager.EmptyLogger;

            var context = CreateExecutionContext();
            handler.InvokeSync(context);

            Assert.AreEqual(3, mockInnerHandler.CallCount);
            Assert.AreEqual(2, context.RequestContext.HttpRetryCount);
        }
    }
}

