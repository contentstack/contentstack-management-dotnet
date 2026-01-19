using System;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline.RetryHandler;

namespace Contentstack.Management.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Mock retry policy for testing RetryHandler in isolation.
    /// </summary>
    public class MockRetryPolicy : RetryPolicy
    {
        public bool ShouldRetryValue { get; set; } = true;
        public bool CanRetryValue { get; set; } = true;
        public bool RetryLimitExceededValue { get; set; } = false;
        public TimeSpan WaitDelay { get; set; } = TimeSpan.FromMilliseconds(100);
        public Exception LastException { get; private set; }
        public int RetryCallCount { get; private set; }

        public MockRetryPolicy()
        {
            RetryOnError = true;
            RetryLimit = 5;
        }

        public override bool RetryForException(IExecutionContext executionContext, Exception exception)
        {
            LastException = exception;
            RetryCallCount++;
            return ShouldRetryValue;
        }

        public override bool CanRetry(IExecutionContext executionContext)
        {
            return CanRetryValue;
        }

        public override bool RetryLimitExceeded(IExecutionContext executionContext)
        {
            return RetryLimitExceededValue;
        }

        internal override void WaitBeforeRetry(IExecutionContext executionContext)
        {
            System.Threading.Tasks.Task.Delay(WaitDelay).Wait();
        }
    }
}

