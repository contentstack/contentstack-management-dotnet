using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Stack;
using Contentstack.Management.Core.Services.Stack.BulkOperation;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents bulk operations for entries and assets in Contentstack.
    /// </summary>
    public class BulkOperation
    {
        private readonly Stack _stack;

        internal BulkOperation(Stack stack)
        {
            _stack = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        /// <summary>
        /// Publishes multiple entries and assets in bulk.
        /// </summary>
        /// <param name="details">The publish details containing entries, assets, locales, and environments.</param>
        /// <param name="skipWorkflowStage">Set to true to skip workflow stage checks.</param>
        /// <param name="approvals">Set to true to publish entries that don't require approval.</param>
        /// <param name="isNested">Set to true for nested publish operations.</param>
        /// <param name="apiVersion">The API version to use.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// var publishDetails = new BulkPublishDetails
        /// {
        ///     Entries = new List<BulkPublishEntry>
        ///     {
        ///         new BulkPublishEntry { Uid = "entry_uid", ContentTypeUid = "content_type_uid", Version = 1, Locale = "en-us" }
        ///     },
        ///     Assets = new List<BulkPublishAsset>
        ///     {
        ///         new BulkPublishAsset { Uid = "asset_uid" }
        ///     },
        ///     Locales = new List<string> { "en-us" },
        ///     Environments = new List<string> { "environment_uid" }
        /// };
        /// 
        /// ContentstackResponse response = stack.BulkOperation().Publish(publishDetails);
        /// </code></pre>
        /// </example>
        public ContentstackResponse Publish(BulkPublishDetails details, bool skipWorkflowStage = false, bool approvals = false, bool isNested = false, string apiVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkPublishService(_stack.client.serializer, _stack, details, skipWorkflowStage, approvals, isNested);
            return _stack.client.InvokeSync(service, false, apiVersion);
        }

        /// <summary>
        /// Publishes multiple entries and assets in bulk asynchronously.
        /// </summary>
        /// <param name="details">The publish details containing entries, assets, locales, and environments.</param>
        /// <param name="skipWorkflowStage">Set to true to skip workflow stage checks.</param>
        /// <param name="approvals">Set to true to publish entries that don't require approval.</param>
        /// <param name="isNested">Set to true for nested publish operations.</param>
        /// <param name="apiVersion">The API version to use.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> PublishAsync(BulkPublishDetails details, bool skipWorkflowStage = false, bool approvals = false, bool isNested = false, string apiVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkPublishService(_stack.client.serializer, _stack, details, skipWorkflowStage, approvals, isNested);
            return _stack.client.InvokeAsync<BulkPublishService, ContentstackResponse>(service, false, apiVersion);
        }

        /// <summary>
        /// Unpublishes multiple entries and assets in bulk.
        /// </summary>
        /// <param name="details">The unpublish details containing entries, assets, locales, and environments.</param>
        /// <param name="skipWorkflowStage">Set to true to skip workflow stage checks.</param>
        /// <param name="approvals">Set to true to unpublish entries that don't require approval.</param>
        /// <param name="isNested">Set to true for nested unpublish operations.</param>
        /// <param name="apiVersion">The API version to use.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// var unpublishDetails = new BulkPublishDetails
        /// {
        ///     Entries = new List<BulkPublishEntry>
        ///     {
        ///         new BulkPublishEntry { Uid = "entry_uid", ContentTypeUid = "content_type_uid", Locale = "en-us" }
        ///     },
        ///     Assets = new List<BulkPublishAsset>
        ///     {
        ///         new BulkPublishAsset { Uid = "asset_uid" }
        ///     },
        ///     Locales = new List<string> { "en-us" },
        ///     Environments = new List<string> { "environment_uid" }
        /// };
        /// 
        /// ContentstackResponse response = stack.BulkOperation().Unpublish(unpublishDetails);
        /// </code></pre>
        /// </example>
        public ContentstackResponse Unpublish(BulkPublishDetails details, bool skipWorkflowStage = false, bool approvals = false, bool isNested = false, string apiVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkUnpublishService(_stack.client.serializer, _stack, details, skipWorkflowStage, approvals, isNested);
            return _stack.client.InvokeSync(service, false, apiVersion);
        }

        /// <summary>
        /// Unpublishes multiple entries and assets in bulk asynchronously.
        /// </summary>
        /// <param name="details">The unpublish details containing entries, assets, locales, and environments.</param>
        /// <param name="skipWorkflowStage">Set to true to skip workflow stage checks.</param>
        /// <param name="approvals">Set to true to unpublish entries that don't require approval.</param>
        /// <param name="isNested">Set to true for nested unpublish operations.</param>
        /// <param name="apiVersion">The API version to use.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UnpublishAsync(BulkPublishDetails details, bool skipWorkflowStage = false, bool approvals = false, bool isNested = false, string apiVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkUnpublishService(_stack.client.serializer, _stack, details, skipWorkflowStage, approvals, isNested);
            return _stack.client.InvokeAsync<BulkUnpublishService, ContentstackResponse>(service, false, apiVersion);
        }

        /// <summary>
        /// Deletes multiple entries and assets in bulk.
        /// </summary>
        /// <param name="details">The delete details containing entries and assets to delete.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// var deleteDetails = new BulkDeleteDetails
        /// {
        ///     Entries = new List<BulkDeleteEntry>
        ///     {
        ///         new BulkDeleteEntry { Uid = "entry_uid", ContentType = "content_type_uid", Locale = "en-us" }
        ///     },
        ///     Assets = new List<BulkDeleteAsset>
        ///     {
        ///         new BulkDeleteAsset { Uid = "asset_uid" }
        ///     }
        /// };
        /// 
        /// ContentstackResponse response = stack.BulkOperation().Delete(deleteDetails);
        /// </code></pre>
        /// </example>
        public ContentstackResponse Delete(BulkDeleteDetails details)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkDeleteService(_stack.client.serializer, _stack, details);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Deletes multiple entries and assets in bulk asynchronously.
        /// </summary>
        /// <param name="details">The delete details containing entries and assets to delete.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> DeleteAsync(BulkDeleteDetails details)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkDeleteService(_stack.client.serializer, _stack, details);
            return _stack.client.InvokeAsync<BulkDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Updates workflow stages for multiple entries in bulk.
        /// </summary>
        /// <param name="updateBody">The update body containing entries and workflow information.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// var updateBody = new BulkWorkflowUpdateBody
        /// {
        ///     Entries = new List<BulkWorkflowEntry>
        ///     {
        ///         new BulkWorkflowEntry { Uid = "entry_uid", ContentType = "content_type_uid", Locale = "en-us" }
        ///     },
        ///     Workflow = new BulkWorkflowStage
        ///     {
        ///         Comment = "Workflow-related Comments",
        ///         DueDate = "Thu Dec 01 2018",
        ///         Notify = false,
        ///         Uid = "workflow_stage_uid"
        ///     }
        /// };
        /// 
        /// ContentstackResponse response = stack.BulkOperation().Update(updateBody);
        /// </code></pre>
        /// </example>
        public ContentstackResponse Update(BulkWorkflowUpdateBody updateBody)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkWorkflowUpdateService(_stack.client.serializer, _stack, updateBody);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Updates workflow stages for multiple entries in bulk asynchronously.
        /// </summary>
        /// <param name="updateBody">The update body containing entries and workflow information.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UpdateAsync(BulkWorkflowUpdateBody updateBody)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkWorkflowUpdateService(_stack.client.serializer, _stack, updateBody);
            return _stack.client.InvokeAsync<BulkWorkflowUpdateService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Adds multiple items to a release in bulk with enhanced capabilities.
        /// Automatically detects whether to perform simple add or deployment operation based on data properties.
        /// When Release property is set in data, performs deployment operation (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items. If Release property is set, performs deployment operation.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// // Simple add mode
        /// var simpleData = new BulkAddItemsData
        /// {
        ///     Items = new List<BulkAddItem>
        ///     {
        ///         new BulkAddItem { Uid = "entry_uid", ContentType = "content_type_uid" }
        ///     }
        /// };
        /// ContentstackResponse response = stack.BulkOperation().AddItems(simpleData);
        /// 
        /// // Deployment mode (like JavaScript SDK)
        /// var deployData = new BulkAddItemsData
        /// {
        ///     Release = "release_uid",
        ///     Action = "publish",
        ///     Locale = new List<string> { "en-us" },
        ///     Reference = true,
        ///     Items = new List<BulkAddItem>
        ///     {
        ///         new BulkAddItem 
        ///         { 
        ///             Uid = "entry_uid", 
        ///             ContentTypeUid = "content_type_uid",
        ///             Version = 1,
        ///             Locale = "en-us",
        ///             Title = "My Entry"
        ///         }
        ///     }
        /// };
        /// ContentstackResponse response = stack.BulkOperation().AddItems(deployData, "2.0");
        /// </code></pre>
        /// </example>
        public ContentstackResponse AddItems(BulkAddItemsData data, string bulkVersion = "1.0")
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkAddItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Adds multiple items to a release in bulk with enhanced deployment capabilities.
        /// Supports both simple adding to release and complex release deployment operations (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items and optional deployment configuration.</param>
        /// <param name="releaseUid">The release UID for deployment operations. If specified, enables deployment mode.</param>
        /// <param name="action">The action to perform (publish, unpublish, etc.). Required when releaseUid is specified.</param>
        /// <param name="locales">The list of locales for deployment. Only used when releaseUid is specified.</param>
        /// <param name="reference">Whether to include references. Only used when releaseUid is specified.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// // Enhanced deployment mode
        /// var deployData = new BulkAddItemsData
        /// {
        ///     Items = new List<BulkAddItem>
        ///     {
        ///         new BulkAddItem 
        ///         { 
        ///             Uid = "entry_uid", 
        ///             ContentTypeUid = "content_type_uid",
        ///             Version = 1,
        ///             Locale = "en-us",
        ///             Title = "My Entry"
        ///         }
        ///     }
        /// };
        /// ContentstackResponse response = stack.BulkOperation().AddItemsWithDeployment(deployData, "release_uid", "publish", new List<string> { "en-us" }, true, "2.0");
        /// </code></pre>
        /// </example>
        public ContentstackResponse AddItemsWithDeployment(BulkAddItemsData data, string releaseUid, string action, List<string> locales = null, bool? reference = null, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            // Configure the data object for deployment
            data.Release = releaseUid;
            data.Action = action;
            data.Locale = locales;
            data.Reference = reference;

            var service = new BulkAddItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Adds multiple items to a release in bulk asynchronously with enhanced capabilities.
        /// Automatically detects whether to perform simple add or deployment operation based on data properties.
        /// When Release property is set in data, performs deployment operation (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items. If Release property is set, performs deployment operation.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> AddItemsAsync(BulkAddItemsData data, string bulkVersion = "1.0")
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkAddItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeAsync<BulkAddItemsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Adds multiple items to a release in bulk asynchronously with enhanced deployment capabilities.
        /// Supports both simple adding to release and complex release deployment operations (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items and optional deployment configuration.</param>
        /// <param name="releaseUid">The release UID for deployment operations. Required for deployment mode.</param>
        /// <param name="action">The action to perform (publish, unpublish, etc.). Required when releaseUid is specified.</param>
        /// <param name="locales">The list of locales for deployment. Only used when releaseUid is specified.</param>
        /// <param name="reference">Whether to include references. Only used when releaseUid is specified.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> AddItemsWithDeploymentAsync(BulkAddItemsData data, string releaseUid, string action, List<string> locales = null, bool? reference = null, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            // Configure the data object for deployment
            data.Release = releaseUid;
            data.Action = action;
            data.Locale = locales;
            data.Reference = reference;

            var service = new BulkAddItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeAsync<BulkAddItemsService, ContentstackResponse>(service);
        }



        /// <summary>
        /// Updates multiple items in a release in bulk with enhanced capabilities.
        /// Automatically detects whether to perform simple update or deployment operation based on data properties.
        /// When Release property is set in data, performs deployment operation (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items. If Release property is set, performs deployment operation.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// // Simple update mode
        /// var simpleData = new BulkAddItemsData
        /// {
        ///     Items = new List<BulkAddItem>
        ///     {
        ///         new BulkAddItem { Uid = "entry_uid", ContentType = "content_type_uid" }
        ///     }
        /// };
        /// ContentstackResponse response = stack.BulkOperation().UpdateItems(simpleData);
        /// 
        /// // Deployment mode (like JavaScript SDK)
        /// var deployData = new BulkAddItemsData
        /// {
        ///     Release = "release_uid",
        ///     Action = "unpublish",
        ///     Locale = new List<string> { "en-us" },
        ///     Reference = false,
        ///     Items = new List<BulkAddItem>
        ///     {
        ///         new BulkAddItem 
        ///         { 
        ///             Uid = "entry_uid", 
        ///             ContentTypeUid = "content_type_uid",
        ///             Version = 1,
        ///             Locale = "en-us",
        ///             Title = "My Entry"
        ///         }
        ///     }
        /// };
        /// ContentstackResponse response = stack.BulkOperation().UpdateItems(deployData, "2.0");
        /// </code></pre>
        /// </example>
        public ContentstackResponse UpdateItems(BulkAddItemsData data, string bulkVersion = "1.0")
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkUpdateItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Updates multiple items in a release in bulk with enhanced deployment capabilities.
        /// Supports both simple updating in release and complex release deployment operations (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items and optional deployment configuration.</param>
        /// <param name="releaseUid">The release UID for deployment operations. Required for deployment mode.</param>
        /// <param name="action">The action to perform (publish, unpublish, etc.). Required when releaseUid is specified.</param>
        /// <param name="locales">The list of locales for deployment. Only used when releaseUid is specified.</param>
        /// <param name="reference">Whether to include references. Only used when releaseUid is specified.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// // Enhanced deployment mode
        /// var deployData = new BulkAddItemsData
        /// {
        ///     Items = new List<BulkAddItem>
        ///     {
        ///         new BulkAddItem 
        ///         { 
        ///             Uid = "entry_uid", 
        ///             ContentTypeUid = "content_type_uid",
        ///             Version = 2,
        ///             Locale = "en-us",
        ///             Title = "Updated Entry"
        ///         }
        ///     }
        /// };
        /// ContentstackResponse response = stack.BulkOperation().UpdateItemsWithDeployment(deployData, "release_uid", "publish", new List<string> { "en-us" }, true, "2.0");
        /// </code></pre>
        /// </example>
        public ContentstackResponse UpdateItemsWithDeployment(BulkAddItemsData data, string releaseUid, string action, List<string> locales = null, bool? reference = null, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            // Configure the data object for deployment
            data.Release = releaseUid;
            data.Action = action;
            data.Locale = locales;
            data.Reference = reference;

            var service = new BulkUpdateItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Updates multiple items in a release in bulk asynchronously with enhanced capabilities.
        /// Automatically detects whether to perform simple update or deployment operation based on data properties.
        /// When Release property is set in data, performs deployment operation (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items. If Release property is set, performs deployment operation.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UpdateItemsAsync(BulkAddItemsData data, string bulkVersion = "1.0")
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkUpdateItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeAsync<BulkUpdateItemsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Updates multiple items in a release in bulk asynchronously with enhanced deployment capabilities.
        /// Supports both simple updating in release and complex release deployment operations (like JavaScript SDK).
        /// </summary>
        /// <param name="data">The data containing items and optional deployment configuration.</param>
        /// <param name="releaseUid">The release UID for deployment operations. Required for deployment mode.</param>
        /// <param name="action">The action to perform (publish, unpublish, etc.). Required when releaseUid is specified.</param>
        /// <param name="locales">The list of locales for deployment. Only used when releaseUid is specified.</param>
        /// <param name="reference">Whether to include references. Only used when releaseUid is specified.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UpdateItemsWithDeploymentAsync(BulkAddItemsData data, string releaseUid, string action, List<string> locales = null, bool? reference = null, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            // Configure the data object for deployment
            data.Release = releaseUid;
            data.Action = action;
            data.Locale = locales;
            data.Reference = reference;

            var service = new BulkUpdateItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeAsync<BulkUpdateItemsService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Checks the status of a bulk job.
        /// </summary>
        /// <param name="jobId">The ID of the job.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// ContentstackResponse response = stack.BulkOperation().JobStatus("job_id", "1.0");
        /// </code></pre>
        /// </example>
        public ContentstackResponse JobStatus(string jobId, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkJobStatusService(_stack.client.serializer, _stack, jobId, bulkVersion);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Checks the status of a bulk job asynchronously.
        /// </summary>
        /// <param name="jobId">The ID of the job.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> JobStatusAsync(string jobId, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkJobStatusService(_stack.client.serializer, _stack, jobId, bulkVersion);
            return _stack.client.InvokeAsync<BulkJobStatusService, ContentstackResponse>(service);
        }

        /// <summary>
        /// Performs bulk release items operations with complete request body structure.
        /// </summary>
        /// <param name="data">The release items data containing release, action, locale, reference, and items.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// Stack stack = client.Stack("<API_KEY>");
        /// 
        /// var releaseData = new BulkReleaseItemsData
        /// {
        ///     Release = "release_uid",
        ///     Action = "publish",
        ///     Locale = new List<string> { "en-us" },
        ///     Reference = true,
        ///     Items = new List<BulkReleaseItem>
        ///     {
        ///         new BulkReleaseItem
        ///         {
        ///             ContentTypeUid = "ct_1",
        ///             Uid = "uid",
        ///             Version = 2,
        ///             Locale = "en-us",
        ///             Title = "validation test"
        ///         }
        ///     }
        /// };
        /// 
        /// ContentstackResponse response = stack.BulkOperation().ReleaseItems(releaseData);
        /// </code></pre>
        /// </example>
        public ContentstackResponse ReleaseItems(BulkReleaseItemsData data, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkReleaseItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Performs bulk release items operations with complete request body structure asynchronously.
        /// </summary>
        /// <param name="data">The release items data containing release, action, locale, reference, and items.</param>
        /// <param name="bulkVersion">The bulk version.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ReleaseItemsAsync(BulkReleaseItemsData data, string bulkVersion = null)
        {
            _stack.ThrowIfNotLoggedIn();
            _stack.ThrowIfAPIKeyEmpty();

            var service = new BulkReleaseItemsService(_stack.client.serializer, _stack, data, bulkVersion);
            return _stack.client.InvokeAsync<BulkReleaseItemsService, ContentstackResponse>(service);
        }
    }
} 