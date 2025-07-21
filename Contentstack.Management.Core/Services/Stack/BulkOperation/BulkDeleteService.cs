using System;
using System.Text;
using Newtonsoft.Json;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Services.Stack.BulkOperation
{
    /// <summary>
    /// Service for bulk delete operations.
    /// </summary>
    internal class BulkDeleteService : ContentstackService
    {
        private readonly BulkDeleteDetails _details;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeleteService"/> class.
        /// </summary>
        /// <param name="serializer">The JSON serializer.</param>
        /// <param name="stack">The stack instance.</param>
        /// <param name="details">The delete details.</param>
        public BulkDeleteService(JsonSerializer serializer, Contentstack.Management.Core.Models.Stack stack, BulkDeleteDetails details)
            : base(serializer, stack)
        {
            _details = details ?? throw new ArgumentNullException(nameof(details));

            ResourcePath = "/bulk/delete";
            HttpMethod = "POST";
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