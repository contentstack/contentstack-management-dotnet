using System;
using System.Text.Json;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Services.Stack.BulkOperation
{
    /// <summary>
    /// Service for bulk job status operations.
    /// </summary>
    internal class BulkJobStatusService : ContentstackService
    {
        private readonly string _jobId;
        private readonly string _bulkVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkJobStatusService"/> class.
        /// </summary>
        /// <param name="serializerOptions">The JSON serializer options.</param>
        /// <param name="stack">The stack instance.</param>
        /// <param name="jobId">The job ID.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        public BulkJobStatusService(JsonSerializerOptions serializerOptions, Contentstack.Management.Core.Models.Stack stack, string jobId, string bulkVersion = null)
            : base(serializerOptions, stack)
        {
            _jobId = jobId ?? throw new ArgumentNullException(nameof(jobId));
            _bulkVersion = bulkVersion;

            ResourcePath = $"/bulk/jobs/{_jobId}";
            HttpMethod = "GET";

            // Set bulk version header if provided
            if (!string.IsNullOrEmpty(_bulkVersion))
            {
                Headers["bulk_version"] = _bulkVersion;
            }
        }
    }
} 