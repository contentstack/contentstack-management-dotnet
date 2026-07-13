using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Represents details for bulk publish/unpublish operations.
    /// </summary>
    public class BulkPublishDetails
    {
        /// <summary>
        /// Gets or sets the list of entries to publish/unpublish.
        /// </summary>
        [JsonPropertyName("entries")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkPublishEntry>? Entries { get; set; }

        /// <summary>
        /// Gets or sets the list of assets to publish/unpublish.
        /// </summary>
        [JsonPropertyName("assets")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkPublishAsset>? Assets { get; set; }

        /// <summary>
        /// Gets or sets the list of locales.
        /// </summary>
        [JsonPropertyName("locales")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Locales { get; set; }

        /// <summary>
        /// Gets or sets the list of environments.
        /// </summary>
        [JsonPropertyName("environments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Environments { get; set; }

        /// <summary>
        /// Gets or sets the rules for the bulk operation.
        /// </summary>
        [JsonPropertyName("rules")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BulkPublishRules? Rules { get; set; }

        /// <summary>
        /// Gets or sets the scheduled time for the operation.
        /// </summary>
        [JsonPropertyName("scheduled_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ScheduledAt { get; set; }

        /// <summary>
        /// Gets or sets whether to publish with reference.
        /// </summary>
        [JsonPropertyName("publish_with_reference")]
        public bool PublishWithReference { get; set; }
    }

    /// <summary>
    /// Represents an entry for bulk publish/unpublish operations.
    /// </summary>
    public class BulkPublishEntry
    {
        /// <summary>
        /// Gets or sets the entry UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        [JsonPropertyName("version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
    }

    /// <summary>
    /// Represents rules for bulk publish operations.
    /// </summary>
    public class BulkPublishRules
    {
        /// <summary>
        /// Gets or sets the approvals setting.
        /// </summary>
        [JsonPropertyName("approvals")]
        public string? Approvals { get; set; }
    }

    /// <summary>
    /// Represents an asset for bulk publish/unpublish operations.
    /// </summary>
    public class BulkPublishAsset
    {
        /// <summary>
        /// Gets or sets the asset UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }

    /// <summary>
    /// Represents details for bulk delete operations.
    /// </summary>
    public class BulkDeleteDetails
    {
        /// <summary>
        /// Gets or sets the list of entries to delete.
        /// </summary>
        [JsonPropertyName("entries")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkDeleteEntry>? Entries { get; set; }

        /// <summary>
        /// Gets or sets the list of assets to delete.
        /// </summary>
        [JsonPropertyName("assets")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkDeleteAsset>? Assets { get; set; }
    }

    /// <summary>
    /// Represents an entry for bulk delete operations.
    /// </summary>
    public class BulkDeleteEntry
    {
        /// <summary>
        /// Gets or sets the entry UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
    }

    /// <summary>
    /// Represents an asset for bulk delete operations.
    /// </summary>
    public class BulkDeleteAsset
    {
        /// <summary>
        /// Gets or sets the asset UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }

    /// <summary>
    /// Represents the body for bulk workflow update operations.
    /// </summary>
    public class BulkWorkflowUpdateBody
    {
        /// <summary>
        /// Gets or sets the list of entries to update.
        /// </summary>
        [JsonPropertyName("entries")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkWorkflowEntry>? Entries { get; set; }

        /// <summary>
        /// Gets or sets the workflow stage information.
        /// </summary>
        [JsonPropertyName("workflow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BulkWorkflowStage? Workflow { get; set; }
    }

    /// <summary>
    /// Represents an entry for bulk workflow update operations.
    /// </summary>
    public class BulkWorkflowEntry
    {
        /// <summary>
        /// Gets or sets the entry UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
    }

    /// <summary>
    /// Represents workflow stage information for bulk operations.
    /// </summary>
    public class BulkWorkflowStage
    {
        /// <summary>
        /// Gets or sets the workflow stage UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [JsonPropertyName("comment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Comment { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        [JsonPropertyName("due_date")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DueDate { get; set; }

        /// <summary>
        /// Gets or sets whether to notify.
        /// </summary>
        [JsonPropertyName("notify")]
        public bool Notify { get; set; }

        /// <summary>
        /// Gets or sets the list of assigned users.
        /// </summary>
        [JsonPropertyName("assigned_to")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkWorkflowUser>? AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the list of assigned roles.
        /// </summary>
        [JsonPropertyName("assigned_by_roles")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkWorkflowRole>? AssignedByRoles { get; set; }
    }

    /// <summary>
    /// Represents a user assigned to a workflow stage.
    /// </summary>
    public class BulkWorkflowUser
    {
        /// <summary>
        /// Gets or sets the user UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    /// <summary>
    /// Represents a role assigned to a workflow stage.
    /// </summary>
    public class BulkWorkflowRole
    {
        /// <summary>
        /// Gets or sets the role UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    /// <summary>
    /// Represents data for bulk add/update items operations.
    /// Enhanced to support both simple adding to release and complex release deployment operations.
    /// </summary>
    public class BulkAddItemsData
    {
        /// <summary>
        /// Gets or sets the list of items to add/update.
        /// </summary>
        [JsonPropertyName("items")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkAddItem>? Items { get; set; }

        /// <summary>
        /// Gets or sets the release UID for deployment operations.
        /// When specified, this enables release deployment mode (like JavaScript SDK).
        /// </summary>
        [JsonPropertyName("release")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Release { get; set; }

        /// <summary>
        /// Gets or sets the action to perform during deployment (publish, unpublish, etc.).
        /// Only used when Release is specified.
        /// </summary>
        [JsonPropertyName("action")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the list of locales for deployment.
        /// Only used when Release is specified.
        /// </summary>
        [JsonPropertyName("locale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Locale { get; set; }

        /// <summary>
        /// Gets or sets the reference flag for deployment.
        /// Only used when Release is specified.
        /// </summary>
        [JsonPropertyName("reference")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Reference { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is configured for release deployment mode.
        /// </summary>
        /// <returns>True if this is a release deployment operation, false if simple add operation.</returns>
        public bool IsReleaseDeploymentMode()
        {
            return !string.IsNullOrEmpty(Release) && !string.IsNullOrEmpty(Action);
        }
    }

    /// <summary>
    /// Represents an item for bulk add/update operations.
    /// Enhanced to support both simple and complex release deployment properties.
    /// </summary>
    public class BulkAddItem
    {
        /// <summary>
        /// Gets or sets the item UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonPropertyName("content_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content type UID for release deployment mode.
        /// This is an alias for ContentType with a different JSON property name.
        /// </summary>
        [JsonPropertyName("content_type_uid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContentTypeUid { get; set; }

        /// <summary>
        /// Gets or sets the version number for release deployment mode.
        /// Only used in enhanced release deployment operations.
        /// </summary>
        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Version { get; set; }

        /// <summary>
        /// Gets or sets the locale for release deployment mode.
        /// Only used in enhanced release deployment operations.
        /// </summary>
        [JsonPropertyName("locale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; }

        /// <summary>
        /// Gets or sets the title for release deployment mode.
        /// Only used in enhanced release deployment operations.
        /// </summary>
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }
    }

    /// <summary>
    /// Represents data for bulk release items operations with complete request body structure.
    /// </summary>
    public class BulkReleaseItemsData
    {
        /// <summary>
        /// Gets or sets the release UID.
        /// </summary>
        [JsonPropertyName("release")]
        public string? Release { get; set; }

        /// <summary>
        /// Gets or sets the action to perform (publish, unpublish, etc.).
        /// </summary>
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the list of locales.
        /// </summary>
        [JsonPropertyName("locale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Locale { get; set; }

        /// <summary>
        /// Gets or sets the reference flag.
        /// </summary>
        [JsonPropertyName("reference")]
        public bool Reference { get; set; }

        /// <summary>
        /// Gets or sets the list of items to process.
        /// </summary>
        [JsonPropertyName("items")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkReleaseItem>? Items { get; set; }
    }

    /// <summary>
    /// Represents an item for bulk release operations with enhanced properties.
    /// </summary>
    public class BulkReleaseItem
    {
        /// <summary>
        /// Gets or sets the content type UID.
        /// </summary>
        [JsonPropertyName("content_type_uid")]
        public string? ContentTypeUid { get; set; }

        /// <summary>
        /// Gets or sets the item UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        [JsonPropertyName("version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
