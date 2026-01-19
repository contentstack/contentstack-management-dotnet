using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline.RetryHandler
{
    [TestClass]
    public class DefaultRetryPolicyTest
    {
        [TestMethod]
        public void Constructor_With_RetryConfiguration_Sets_Properties()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 3,
                RetryDelay = TimeSpan.FromMilliseconds(200)
            };

            var policy = new DefaultRetryPolicy(config);

            Assert.AreEqual(3, policy.RetryLimit);
        }

        [TestMethod]
        public void Constructor_With_Legacy_Parameters_Sets_Properties()
        {
            var policy = new DefaultRetryPolicy(5, TimeSpan.FromMilliseconds(300));

            Assert.AreEqual(5, policy.RetryLimit);
        }

        [TestMethod]
        public void CanRetry_Respects_RetryOnError_From_Configuration()
        {
            var config = new RetryConfiguration
            {
                RetryOnError = true
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();

            var result = policy.CanRetry(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanRetry_Fallback_To_RetryOnError_Property()
        {
            var policy = new DefaultRetryPolicy(5, TimeSpan.FromMilliseconds(300));
            policy.RetryOnError = false;
            var context = CreateExecutionContext();

            var result = policy.CanRetry(context);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RetryForException_NetworkError_Respects_MaxNetworkRetries()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 2
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            var exception = MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset);

            context.RequestContext.NetworkRetryCount = 1;
            var result1 = policy.RetryForException(context, exception);
            Assert.IsTrue(result1);

            context.RequestContext.NetworkRetryCount = 2;
            var result2 = policy.RetryForException(context, exception);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void RetryForException_NetworkError_Increments_NetworkRetryCount()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 3
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            var exception = MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset);

            var result = policy.RetryForException(context, exception);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RetryForException_HttpError_5xx_Respects_RetryLimit()
        {
            var config = new RetryConfiguration
            {
                RetryOnHttpServerError = true,
                RetryLimit = 2
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            var exception = MockNetworkErrorGenerator.CreateContentstackErrorException(HttpStatusCode.InternalServerError);

            context.RequestContext.HttpRetryCount = 1;
            var result1 = policy.RetryForException(context, exception);
            Assert.IsTrue(result1);

            context.RequestContext.HttpRetryCount = 2;
            var result2 = policy.RetryForException(context, exception);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void RetryForException_HttpError_5xx_Increments_HttpRetryCount()
        {
            var config = new RetryConfiguration
            {
                RetryOnHttpServerError = true,
                RetryLimit = 5
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            var exception = MockNetworkErrorGenerator.CreateContentstackErrorException(HttpStatusCode.InternalServerError);

            var result = policy.RetryForException(context, exception);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RetryForException_HttpError_429_Respects_RetryLimit()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            var exception = MockNetworkErrorGenerator.CreateContentstackErrorException(HttpStatusCode.TooManyRequests);

            context.RequestContext.HttpRetryCount = 1;
            var result1 = policy.RetryForException(context, exception);
            Assert.IsTrue(result1);

            context.RequestContext.HttpRetryCount = 2;
            var result2 = policy.RetryForException(context, exception);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void RetryForException_NetworkError_Exceeds_MaxNetworkRetries_Returns_False()
        {
            var config = new RetryConfiguration
            {
                RetryOnNetworkFailure = true,
                RetryOnSocketFailure = true,
                MaxNetworkRetries = 1
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            context.RequestContext.NetworkRetryCount = 1;
            var exception = MockNetworkErrorGenerator.CreateSocketException(SocketError.ConnectionReset);

            var result = policy.RetryForException(context, exception);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RetryForException_HttpError_Exceeds_RetryLimit_Returns_False()
        {
            var config = new RetryConfiguration
            {
                RetryOnHttpServerError = true,
                RetryLimit = 1
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            var exception = MockNetworkErrorGenerator.CreateContentstackErrorException(HttpStatusCode.InternalServerError);

            context.RequestContext.HttpRetryCount = 0;
            var result0 = policy.RetryForException(context, exception);
            Assert.IsTrue(result0);

            context.RequestContext.HttpRetryCount = 1;
            var result = policy.RetryForException(context, exception);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RetryForException_NonRetryableException_Returns_False()
        {
            var config = new RetryConfiguration();
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            var exception = new ArgumentException("Invalid argument");

            var result = policy.RetryForException(context, exception);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RetryLimitExceeded_Checks_Both_Network_And_Http_Counts()
        {
            var config = new RetryConfiguration
            {
                MaxNetworkRetries = 2,
                RetryLimit = 3
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();

            context.RequestContext.NetworkRetryCount = 1;
            context.RequestContext.HttpRetryCount = 2;
            var result1 = policy.RetryLimitExceeded(context);
            Assert.IsFalse(result1);

            context.RequestContext.NetworkRetryCount = 2;
            context.RequestContext.HttpRetryCount = 3;
            var result2 = policy.RetryLimitExceeded(context);
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void WaitBeforeRetry_Uses_NetworkDelay_For_NetworkRetries()
        {
            var config = new RetryConfiguration
            {
                NetworkRetryDelay = TimeSpan.FromMilliseconds(50),
                NetworkBackoffStrategy = BackoffStrategy.Fixed
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            context.RequestContext.NetworkRetryCount = 1;

            var startTime = DateTime.UtcNow;
            policy.WaitBeforeRetry(context);
            var elapsed = DateTime.UtcNow - startTime;

            // Should wait approximately 50ms + jitter (0-100ms)
            Assert.IsTrue(elapsed >= TimeSpan.FromMilliseconds(50));
            Assert.IsTrue(elapsed <= TimeSpan.FromMilliseconds(200));
        }

        [TestMethod]
        public void WaitBeforeRetry_Uses_HttpDelay_For_HttpRetries()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(100),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(100)
                }
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            context.RequestContext.HttpRetryCount = 1;
            context.RequestContext.NetworkRetryCount = 0;

            var startTime = DateTime.UtcNow;
            policy.WaitBeforeRetry(context);
            var elapsed = DateTime.UtcNow - startTime;

            // Should wait approximately 200ms (100ms * 2^1) + jitter
            Assert.IsTrue(elapsed >= TimeSpan.FromMilliseconds(200));
            Assert.IsTrue(elapsed <= TimeSpan.FromMilliseconds(300));
        }

        [TestMethod]
        public void WaitBeforeRetry_Fallback_To_Legacy_Delay()
        {
            var policy = new DefaultRetryPolicy(5, TimeSpan.FromMilliseconds(150));
            var context = CreateExecutionContext();

            var startTime = DateTime.UtcNow;
            policy.WaitBeforeRetry(context);
            var elapsed = DateTime.UtcNow - startTime;

            // Should wait approximately 150ms
            Assert.IsTrue(elapsed >= TimeSpan.FromMilliseconds(150));
            Assert.IsTrue(elapsed <= TimeSpan.FromMilliseconds(200));
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Respects_Configuration()
        {
            var config = new RetryConfiguration
            {
                RetryCondition = (statusCode) => statusCode == HttpStatusCode.NotFound
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();

            var result = policy.ShouldRetryHttpStatusCode(HttpStatusCode.NotFound, context.RequestContext);
            Assert.IsTrue(result);

            var result2 = policy.ShouldRetryHttpStatusCode(HttpStatusCode.InternalServerError, context.RequestContext);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Respects_RetryLimit()
        {
            var config = new RetryConfiguration
            {
                RetryLimit = 2
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            context.RequestContext.HttpRetryCount = 2;

            var result = policy.ShouldRetryHttpStatusCode(HttpStatusCode.TooManyRequests, context.RequestContext);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHttpRetryDelay_Uses_DelayCalculator()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(200),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(200)
                }
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            context.RequestContext.HttpRetryCount = 1;

            var delay = policy.GetHttpRetryDelay(context.RequestContext, null);

            // Should be approximately 400ms (200ms * 2^1) + jitter
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(400));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(500));
        }

        [TestMethod]
        public void GetNetworkRetryDelay_Uses_DelayCalculator()
        {
            var config = new RetryConfiguration
            {
                NetworkRetryDelay = TimeSpan.FromMilliseconds(100),
                NetworkBackoffStrategy = BackoffStrategy.Exponential
            };
            var policy = new DefaultRetryPolicy(config);
            var context = CreateExecutionContext();
            context.RequestContext.NetworkRetryCount = 2;

            var delay = policy.GetNetworkRetryDelay(context.RequestContext);

            // Should be approximately 200ms (100ms * 2^1) + jitter
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(200));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(300));
        }

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
    }
}

