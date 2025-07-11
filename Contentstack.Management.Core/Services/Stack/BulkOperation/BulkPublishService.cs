using System;
using System.Text;
using Newtonsoft.Json;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Services.Stack.BulkOperation
{
    /// <summary>
    /// Service for bulk publish operations.
    /// </summary>
    internal class BulkPublishService : ContentstackService
    {
        private readonly BulkPublishDetails _details;
        private readonly bool _skipWorkflowStage;
        private readonly bool _approvals;
        private readonly bool _isNested;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkPublishService"/> class.
        /// </summary>
        /// <param name="serializer">The JSON serializer.</param>
        /// <param name="stack">The stack instance.</param>
        /// <param name="details">The publish details.</param>
        /// <param name="skipWorkflowStage">Whether to skip workflow stage checks.</param>
        /// <param name="approvals">Whether to include approvals.</param>
        /// <param name="isNested">Whether this is a nested operation.</param>
        public BulkPublishService(JsonSerializer serializer, Contentstack.Management.Core.Models.Stack stack, BulkPublishDetails details, bool skipWorkflowStage = false, bool approvals = false, bool isNested = false)
            : base(serializer, stack)
        {
            _details = details ?? throw new ArgumentNullException(nameof(details));
            _skipWorkflowStage = skipWorkflowStage;
            _approvals = approvals;
            _isNested = isNested;

            ResourcePath = "/bulk/publish";
            HttpMethod = "POST";

            // Set headers based on parameters
            if (_skipWorkflowStage)
            {
                Headers["skip_workflow_stage_check"] = "true";
            }

            if (_approvals)
            {
                Headers["approvals"] = "true";
            }

            if (_isNested)
            {
                AddQueryResource("nested", "true");
                AddQueryResource("event_type", "bulk");
            }
        }

        /// <summary>
        /// Creates the content body for the request.
        /// </summary>
        public override void ContentBody()
        {
            if (_details != null)
            {
                var json = JsonConvert.SerializeObject(_details);
                ByteContent = Encoding.UTF8.GetBytes(json);
            }
        }
    }
} 