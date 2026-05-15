using System;
using System.Text;
using Newtonsoft.Json;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Services.Stack.BulkOperation
{
    /// <summary>
    /// Service for bulk workflow update operations.
    /// </summary>
    internal class BulkWorkflowUpdateService : ContentstackService
    {
        private readonly BulkWorkflowUpdateBody _updateBody;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkWorkflowUpdateService"/> class.
        /// </summary>
        /// <param name="serializer">The JSON serializer.</param>
        /// <param name="stack">The stack instance.</param>
        /// <param name="updateBody">The workflow update body.</param>
        public BulkWorkflowUpdateService(JsonSerializer serializer, Contentstack.Management.Core.Models.Stack stack, BulkWorkflowUpdateBody updateBody)
            : base(serializer, stack)
        {
            _updateBody = updateBody ?? throw new ArgumentNullException(nameof(updateBody));

            ResourcePath = "/bulk/workflow";
            HttpMethod = "POST";
        }

        /// <summary>
        /// Creates the content body for the request.
        /// </summary>
        public override void ContentBody()
        {
            if (_updateBody != null)
            {
                var json = JsonConvert.SerializeObject(_updateBody);
                ByteContent = Encoding.UTF8.GetBytes(json);
            }
        }
    }
} 