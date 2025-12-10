using System;
using System.Net;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline.RetryHandler
{
    [TestClass]
    public class RetryConfigurationTest
    {
        [TestMethod]
        public void FromOptions_Creates_Configuration_With_All_Properties()
        {
            var options = new ContentstackClientOptions
            {
                RetryOnError = true,
                RetryLimit = 5,
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryOnNetworkFailure = true,
                RetryOnDnsFailure = true,
                RetryOnSocketFailure = true,
                RetryOnHttpServerError = true,
                MaxNetworkRetries = 3,
                NetworkRetryDelay = TimeSpan.FromMilliseconds(100),
                NetworkBackoffStrategy = BackoffStrategy.Exponential,
                RetryCondition = (statusCode) => statusCode == (HttpStatusCode)429,
                RetryDelayOptions = new RetryDelayOptions
                {
                    Base = TimeSpan.FromMilliseconds(300),
                    CustomBackoff = (retryCount, error) => TimeSpan.FromMilliseconds(500)
                }
            };

            var config = RetryConfiguration.FromOptions(options);

            Assert.AreEqual(options.RetryOnError, config.RetryOnError);
            Assert.AreEqual(options.RetryLimit, config.RetryLimit);
            Assert.AreEqual(options.RetryDelay, config.RetryDelay);
            Assert.AreEqual(options.RetryOnNetworkFailure, config.RetryOnNetworkFailure);
            Assert.AreEqual(options.RetryOnDnsFailure, config.RetryOnDnsFailure);
            Assert.AreEqual(options.RetryOnSocketFailure, config.RetryOnSocketFailure);
            Assert.AreEqual(options.RetryOnHttpServerError, config.RetryOnHttpServerError);
            Assert.AreEqual(options.MaxNetworkRetries, config.MaxNetworkRetries);
            Assert.AreEqual(options.NetworkRetryDelay, config.NetworkRetryDelay);
            Assert.AreEqual(options.NetworkBackoffStrategy, config.NetworkBackoffStrategy);
            Assert.AreEqual(options.RetryCondition, config.RetryCondition);
            Assert.IsNotNull(config.RetryDelayOptions);
            Assert.AreEqual(options.RetryDelayOptions.Base, config.RetryDelayOptions.Base);
            Assert.AreEqual(options.RetryDelayOptions.CustomBackoff, config.RetryDelayOptions.CustomBackoff);
        }

        [TestMethod]
        public void FromOptions_Handles_Null_RetryDelayOptions()
        {
            var options = new ContentstackClientOptions
            {
                RetryDelay = TimeSpan.FromMilliseconds(300),
                RetryDelayOptions = null
            };

            var config = RetryConfiguration.FromOptions(options);

            Assert.IsNotNull(config.RetryDelayOptions);
            Assert.AreEqual(options.RetryDelay, config.RetryDelayOptions.Base);
        }

        [TestMethod]
        public void FromOptions_Sets_RetryDelayOptions_Base_From_RetryDelay()
        {
            var options = new ContentstackClientOptions
            {
                RetryDelay = TimeSpan.FromMilliseconds(500),
                RetryDelayOptions = null
            };

            var config = RetryConfiguration.FromOptions(options);

            Assert.AreEqual(TimeSpan.FromMilliseconds(500), config.RetryDelayOptions.Base);
        }

        [TestMethod]
        public void Default_Values_Are_Correct()
        {
            var config = new RetryConfiguration();

            Assert.IsTrue(config.RetryOnError);
            Assert.AreEqual(5, config.RetryLimit);
            Assert.AreEqual(TimeSpan.FromMilliseconds(300), config.RetryDelay);
            Assert.IsTrue(config.RetryOnNetworkFailure);
            Assert.IsTrue(config.RetryOnDnsFailure);
            Assert.IsTrue(config.RetryOnSocketFailure);
            Assert.IsTrue(config.RetryOnHttpServerError);
            Assert.AreEqual(3, config.MaxNetworkRetries);
            Assert.AreEqual(TimeSpan.FromMilliseconds(100), config.NetworkRetryDelay);
            Assert.AreEqual(BackoffStrategy.Exponential, config.NetworkBackoffStrategy);
            Assert.IsNull(config.RetryCondition);
            Assert.IsNotNull(config.RetryDelayOptions);
        }

        [TestMethod]
        public void RetryDelayOptions_Default_Values()
        {
            var options = new RetryDelayOptions();

            Assert.AreEqual(TimeSpan.FromMilliseconds(300), options.Base);
            Assert.IsNull(options.CustomBackoff);
        }
    }
}

