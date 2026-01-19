namespace Contentstack.Management.Core.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    /// Defines the backoff strategy for retry delays.
    /// </summary>
    public enum BackoffStrategy
    {
        /// <summary>
        /// Fixed delay with jitter.
        /// </summary>
        Fixed,

        /// <summary>
        /// Exponential backoff with jitter (delay = baseDelay * 2^(attempt-1) + jitter).
        /// </summary>
        Exponential
    }
}

