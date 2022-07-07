using System;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;

namespace Contentstack.Management.Core.Runtime.Pipeline
{
    /// <summary>
    /// Interface for a handler in a pipeline.
    /// </summary>
    public interface IPipelineHandler
    {
        /// <summary>
        /// The logger used to log messages.
        /// </summary>
        ILogManager LogManager { get; set; }

        /// <summary>
        /// The inner handler which is called after the current 
        /// handler completes it's processing.
        /// </summary>
        IPipelineHandler InnerHandler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executionContext"></param>

        void InvokeSync(IExecutionContext executionContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext);
    }
}
