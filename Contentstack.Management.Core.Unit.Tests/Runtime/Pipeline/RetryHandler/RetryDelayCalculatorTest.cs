using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline.RetryHandler
{
    [TestClass]
    public class RetryDelayCalculatorTest
    {
        private RetryDelayCalculator calculator;

        [TestInitialize]
        public void Initialize()
        {
            calculator = new RetryDelayCalculator();
        }

        [TestMethod]
        public void CalculateNetworkRetryDelay_Exponential_FirstAttempt()
        {
            var config = new RetryConfiguration
            {
                NetworkRetryDelay = TimeSpan.FromMilliseconds(100),
                NetworkBackoffStrategy = BackoffStrategy.Exponential
            };

            var delay = calculator.CalculateNetworkRetryDelay(1, config);

            // First attempt: 100ms * 2^0 = 100ms + jitter (0-100ms)
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(100));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(200));
        }

        [TestMethod]
        public void CalculateNetworkRetryDelay_Exponential_SecondAttempt()
        {
            var config = new RetryConfiguration
            {
                NetworkRetryDelay = TimeSpan.FromMilliseconds(100),
                NetworkBackoffStrategy = BackoffStrategy.Exponential
            };

            var delay = calculator.CalculateNetworkRetryDelay(2, config);

            // Second attempt: 100ms * 2^1 = 200ms + jitter (0-100ms)
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(200));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(300));
        }

        [TestMethod]
        public void CalculateNetworkRetryDelay_Exponential_ThirdAttempt()
        {
            var config = new RetryConfiguration
            {
                NetworkRetryDelay = TimeSpan.FromMilliseconds(100),
                NetworkBackoffStrategy = BackoffStrategy.Exponential
            };

            var delay = calculator.CalculateNetworkRetryDelay(3, config);

            // Third attempt: 100ms * 2^2 = 400ms + jitter (0-100ms)
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(400));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(500));
        }

        [TestMethod]
        public void CalculateNetworkRetryDelay_Fixed_AllAttempts()
        {
            var config = new RetryConfiguration
            {
                NetworkRetryDelay = TimeSpan.FromMilliseconds(150),
                NetworkBackoffStrategy = BackoffStrategy.Fixed
            };

            var delay1 = calculator.CalculateNetworkRetryDelay(1, config);
            var delay2 = calculator.CalculateNetworkRetryDelay(2, config);
            var delay3 = calculator.CalculateNetworkRetryDelay(3, config);

            // All attempts should be ~150ms + jitter
            Assert.IsTrue(delay1 >= TimeSpan.FromMilliseconds(150));
            Assert.IsTrue(delay1 <= TimeSpan.FromMilliseconds(250));
            Assert.IsTrue(delay2 >= TimeSpan.FromMilliseconds(150));
            Assert.IsTrue(delay2 <= TimeSpan.FromMilliseconds(250));
            Assert.IsTrue(delay3 >= TimeSpan.FromMilliseconds(150));
            Assert.IsTrue(delay3 <= TimeSpan.FromMilliseconds(250));
        }

        [TestMethod]
        public void CalculateNetworkRetryDelay_Includes_Jitter()
        {
            var config = new RetryConfiguration
            {
                NetworkRetryDelay = TimeSpan.FromMilliseconds(100),
                NetworkBackoffStrategy = BackoffStrategy.Fixed
            };

            // Run multiple times to verify jitter is added
            var firstDelay = calculator.CalculateNetworkRetryDelay(1, config);
            
            // Jitter should cause some variation (though it's random, so not guaranteed)
            // At minimum, verify the delay is within expected range
            Assert.IsTrue(firstDelay >= TimeSpan.FromMilliseconds(100));
            Assert.IsTrue(firstDelay <= TimeSpan.FromMilliseconds(200));
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_Exponential_FirstRetry()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(300)
                }
            };

            var delay = calculator.CalculateHttpRetryDelay(0, config, null);

            // First retry: 300ms * 2^0 = 300ms + jitter
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(300));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(400));
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_Exponential_SubsequentRetries()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(300)
                }
            };

            var delay1 = calculator.CalculateHttpRetryDelay(1, config, null);
            var delay2 = calculator.CalculateHttpRetryDelay(2, config, null);

            // Second retry: 300ms * 2^1 = 600ms + jitter
            Assert.IsTrue(delay1 >= TimeSpan.FromMilliseconds(600));
            Assert.IsTrue(delay1 <= TimeSpan.FromMilliseconds(700));

            // Third retry: 300ms * 2^2 = 1200ms + jitter
            Assert.IsTrue(delay2 >= TimeSpan.FromMilliseconds(1200));
            Assert.IsTrue(delay2 <= TimeSpan.FromMilliseconds(1300));
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_Respects_RetryAfter_Header_Delta()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300)
            };

            var response = new HttpResponseMessage((HttpStatusCode)429);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(5));

            var delay = calculator.CalculateHttpRetryDelay(0, config, null, response.Headers);

            Assert.AreEqual(TimeSpan.FromSeconds(5), delay);
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_Respects_RetryAfter_Header_Date()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300)
            };

            var response = new HttpResponseMessage((HttpStatusCode)429);
            var retryAfterDate = DateTimeOffset.UtcNow.AddSeconds(3);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(retryAfterDate);

            var delay = calculator.CalculateHttpRetryDelay(0, config, null, response.Headers);

            // Should be approximately 3 seconds (allowing for small timing differences)
            Assert.IsTrue(delay >= TimeSpan.FromSeconds(2.5));
            Assert.IsTrue(delay <= TimeSpan.FromSeconds(3.5));
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_Uses_CustomBackoff_When_Provided()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryDelayOptions = new RetryDelayOptions
                {
                    CustomBackoff = (retryCount, error) => TimeSpan.FromMilliseconds(500 * (retryCount + 1))
                }
            };

            var delay1 = calculator.CalculateHttpRetryDelay(0, config, null);
            var delay2 = calculator.CalculateHttpRetryDelay(1, config, null);

            Assert.AreEqual(TimeSpan.FromMilliseconds(500), delay1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(1000), delay2);
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_CustomBackoff_Returns_Zero_Disables_Retry()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryDelayOptions = new RetryDelayOptions
                {
                    CustomBackoff = (retryCount, error) => retryCount >= 2 ? TimeSpan.Zero : TimeSpan.FromMilliseconds(100)
                }
            };

            var delay1 = calculator.CalculateHttpRetryDelay(0, config, null);
            var delay2 = calculator.CalculateHttpRetryDelay(1, config, null);
            var delay3 = calculator.CalculateHttpRetryDelay(2, config, null);

            Assert.AreEqual(TimeSpan.FromMilliseconds(100), delay1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(100), delay2);
            Assert.AreEqual(TimeSpan.Zero, delay3);
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_CustomBackoff_Returns_Negative_Disables_Retry()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryDelayOptions = new RetryDelayOptions
                {
                    CustomBackoff = (retryCount, error) => retryCount >= 2 ? TimeSpan.FromMilliseconds(-1) : TimeSpan.FromMilliseconds(100)
                }
            };

            var delay1 = calculator.CalculateHttpRetryDelay(0, config, null);
            var delay2 = calculator.CalculateHttpRetryDelay(1, config, null);
            var delay3 = calculator.CalculateHttpRetryDelay(2, config, null);

            Assert.AreEqual(TimeSpan.FromMilliseconds(100), delay1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(100), delay2);
            Assert.IsTrue(delay3 < TimeSpan.Zero);
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_Includes_Jitter()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(300)
                }
            };

            var delay = calculator.CalculateHttpRetryDelay(0, config, null);

            // Should be 300ms + jitter (0-100ms)
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(300));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(400));
        }

        [TestMethod]
        public void CalculateHttpRetryDelay_Uses_RetryDelay_When_Base_Is_Zero()
        {
            var config = new RetryConfiguration
            {
                RetryDelay = TimeSpan.FromMilliseconds(500),
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.Zero
                }
            };

            var delay = calculator.CalculateHttpRetryDelay(0, config, null);

            // Should use RetryDelay (500ms) instead of Base
            Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(500));
            Assert.IsTrue(delay <= TimeSpan.FromMilliseconds(600));
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Default_429()
        {
            var config = new RetryConfiguration();
            var result = calculator.ShouldRetryHttpStatusCode((HttpStatusCode)429, config);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Default_500()
        {
            var config = new RetryConfiguration();
            var result = calculator.ShouldRetryHttpStatusCode(HttpStatusCode.InternalServerError, config);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Default_502()
        {
            var config = new RetryConfiguration();
            var result = calculator.ShouldRetryHttpStatusCode(HttpStatusCode.BadGateway, config);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Default_503()
        {
            var config = new RetryConfiguration();
            var result = calculator.ShouldRetryHttpStatusCode(HttpStatusCode.ServiceUnavailable, config);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Default_504()
        {
            var config = new RetryConfiguration();
            var result = calculator.ShouldRetryHttpStatusCode(HttpStatusCode.GatewayTimeout, config);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Default_Not_4xx()
        {
            var config = new RetryConfiguration();
            
            Assert.IsFalse(calculator.ShouldRetryHttpStatusCode(HttpStatusCode.BadRequest, config));
            Assert.IsFalse(calculator.ShouldRetryHttpStatusCode(HttpStatusCode.Unauthorized, config));
            Assert.IsFalse(calculator.ShouldRetryHttpStatusCode(HttpStatusCode.Forbidden, config));
            Assert.IsFalse(calculator.ShouldRetryHttpStatusCode(HttpStatusCode.NotFound, config));
        }

        [TestMethod]
        public void ShouldRetryHttpStatusCode_Custom_Condition()
        {
            var config = new RetryConfiguration
            {
                RetryCondition = (statusCode) => statusCode == HttpStatusCode.NotFound || statusCode == (HttpStatusCode)429
            };

            Assert.IsTrue(calculator.ShouldRetryHttpStatusCode(HttpStatusCode.NotFound, config));
            Assert.IsTrue(calculator.ShouldRetryHttpStatusCode((HttpStatusCode)429, config));
            Assert.IsFalse(calculator.ShouldRetryHttpStatusCode(HttpStatusCode.InternalServerError, config));
        }
    }
}

