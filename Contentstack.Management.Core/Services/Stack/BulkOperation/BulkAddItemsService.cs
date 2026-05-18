using System;
using System.Text;
using System.Text.Json;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Services.Stack.BulkOperation
{
    /// <summary>
    /// Service for bulk add items operations.
    /// </summary>
    internal class BulkAddItemsService : ContentstackService
    {
        private readonly BulkAddItemsData _data;
        private readonly string _bulkVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkAddItemsService"/> class.
        /// </summary>
        /// <param name="serializerOptions">The JSON serializer options.</param>
        /// <param name="stack">The stack instance.</param>
        /// <param name="data">The add items data.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        public BulkAddItemsService(JsonSerializerOptions serializerOptions, Contentstack.Management.Core.Models.Stack stack, BulkAddItemsData data, string bulkVersion = null)
            : base(serializerOptions, stack)
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
                var json = JsonSerializer.Serialize(_data, SerializerOptions);
                ByteContent = Encoding.UTF8.GetBytes(json);
            }
        }
    }
} 