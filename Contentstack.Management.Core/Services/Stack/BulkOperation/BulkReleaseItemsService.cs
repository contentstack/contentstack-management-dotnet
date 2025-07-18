using System;
using System.Text;
using Newtonsoft.Json;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Services.Stack.BulkOperation
{
    /// <summary>
    /// Service for bulk release items operations with complete request body structure.
    /// </summary>
    internal class BulkReleaseItemsService : ContentstackService
    {
        private readonly BulkReleaseItemsData _data;
        private readonly string _bulkVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkReleaseItemsService"/> class.
        /// </summary>
        /// <param name="serializer">The JSON serializer.</param>
        /// <param name="stack">The stack instance.</param>
        /// <param name="data">The release items data.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        public BulkReleaseItemsService(JsonSerializer serializer, Contentstack.Management.Core.Models.Stack stack, BulkReleaseItemsData data, string bulkVersion = null)
            : base(serializer, stack)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _bulkVersion = bulkVersion;

            ResourcePath = "/bulk/release/items";
            HttpMethod = "POST";

            // Set bulk version header if provided
            if (!string.IsNullOrEmpty(_bulkVersion))
            {
                Headers["bulk_version"] = _bulkVersion;
            }
        }

        /// <summary>
        /// Creates the content body for the request.
        /// </summary>
        public override void ContentBody()
        {
            if (_data != null)
            {
                var json = JsonConvert.SerializeObject(_data);
                ByteContent = Encoding.UTF8.GetBytes(json);
            }
        }
    }
} 