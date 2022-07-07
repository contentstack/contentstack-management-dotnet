using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline.RertyHandler
{
    public abstract partial class RetryPolicy
    {
        public bool RetryOnError { get; set; }
        public int RetryLimit { get; set; }
        

        public bool Retry(IExecutionContext executionContext, Exception exception)
        {

            bool canRetry = !RetryLimitExceeded(executionContext) && CanRetry(executionContext);

            if (canRetry && RetryForException(executionContext, exception))
            {
                return true;
            }

            return false;
        }

        protected abstract bool RetryForException(IExecutionContext excutionContext, Exception exception);

        protected abstract bool CanRetry(IExecutionContext excutionContext);

        protected abstract bool RetryLimitExceeded(IExecutionContext excutionContext);
        internal abstract void WaitBeforeRetry(IExecutionContext executionContext);
    }
}

