using System.Collections.Generic;
using Newtonsoft.Json;

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
        [JsonProperty(propertyName: "entries")]
        public List<BulkPublishEntry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the list of assets to publish/unpublish.
        /// </summary>
        [JsonProperty(propertyName: "assets")]
        public List<BulkPublishAsset> Assets { get; set; }

        /// <summary>
        /// Gets or sets the list of locales.
        /// </summary>
        [JsonProperty(propertyName: "locales")]
        public List<string> Locales { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of environments.
        /// </summary>
        [JsonProperty(propertyName: "environments")]
        public List<string> Environments { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the rules for the bulk operation.
        /// </summary>
        [JsonProperty(propertyName: "rules")]
        public BulkPublishRules Rules { get; set; }

        /// <summary>
        /// Gets or sets the scheduled time for the operation.
        /// </summary>
        [JsonProperty(propertyName: "scheduled_at")]
        public string ScheduledAt { get; set; }

        /// <summary>
        /// Gets or sets whether to publish with reference.
        /// </summary>
        [JsonProperty(propertyName: "publish_with_reference")]
        public bool PublishWithReference { get; set; }

        /// <summary>
        /// Determines whether to serialize the Entries property.
        /// </summary>
        /// <returns>True if Entries should be serialized, false otherwise.</returns>
        public bool ShouldSerializeEntries()
        {
            return Entries != null && Entries.Count > 0;
        }

        /// <summary>
        /// Determines whether to serialize the Assets property.
        /// </summary>
        /// <returns>True if Assets should be serialized, false otherwise.</returns>
        public bool ShouldSerializeAssets()
        {
            return Assets != null && Assets.Count > 0;
        }
    }

    /// <summary>
    /// Represents an entry for bulk publish/unpublish operations.
    /// </summary>
    public class BulkPublishEntry
    {
        /// <summary>
        /// Gets or sets the entry UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonProperty(propertyName: "content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        [JsonProperty(propertyName: "version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }
    }

    /// <summary>
    /// Represents rules for bulk publish operations.
    /// </summary>
    public class BulkPublishRules
    {
        /// <summary>
        /// Gets or sets the approvals setting.
        /// </summary>
        [JsonProperty(propertyName: "approvals")]
        public string Approvals { get; set; }
    }

    /// <summary>
    /// Represents an asset for bulk publish/unpublish operations.
    /// </summary>
    public class BulkPublishAsset
    {
        /// <summary>
        /// Gets or sets the asset UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
    }

    /// <summary>
    /// Represents details for bulk delete operations.
    /// </summary>
    public class BulkDeleteDetails
    {
        /// <summary>
        /// Gets or sets the list of entries to delete.
        /// </summary>
        [JsonProperty(propertyName: "entries")]
        public List<BulkDeleteEntry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the list of assets to delete.
        /// </summary>
        [JsonProperty(propertyName: "assets")]
        public List<BulkDeleteAsset> Assets { get; set; }

        /// <summary>
        /// Determines whether to serialize the Entries property.
        /// </summary>
        /// <returns>True if Entries should be serialized, false otherwise.</returns>
        public bool ShouldSerializeEntries()
        {
            return Entries != null && Entries.Count > 0;
        }

        /// <summary>
        /// Determines whether to serialize the Assets property.
        /// </summary>
        /// <returns>True if Assets should be serialized, false otherwise.</returns>
        public bool ShouldSerializeAssets()
        {
            return Assets != null && Assets.Count > 0;
        }
    }

    /// <summary>
    /// Represents an entry for bulk delete operations.
    /// </summary>
    public class BulkDeleteEntry
    {
        /// <summary>
        /// Gets or sets the entry UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonProperty(propertyName: "content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }
    }

    /// <summary>
    /// Represents an asset for bulk delete operations.
    /// </summary>
    public class BulkDeleteAsset
    {
        /// <summary>
        /// Gets or sets the asset UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }
    }

    /// <summary>
    /// Represents the body for bulk workflow update operations.
    /// </summary>
    public class BulkWorkflowUpdateBody
    {
        /// <summary>
        /// Gets or sets the list of entries to update.
        /// </summary>
        [JsonProperty(propertyName: "entries")]
        public List<BulkWorkflowEntry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the workflow stage information.
        /// </summary>
        [JsonProperty(propertyName: "workflow")]
        public BulkWorkflowStage Workflow { get; set; }

        /// <summary>
        /// Determines whether to serialize the Entries property.
        /// </summary>
        /// <returns>True if Entries should be serialized, false otherwise.</returns>
        public bool ShouldSerializeEntries()
        {
            return Entries != null && Entries.Count > 0;
        }
    }

    /// <summary>
    /// Represents an entry for bulk workflow update operations.
    /// </summary>
    public class BulkWorkflowEntry
    {
        /// <summary>
        /// Gets or sets the entry UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonProperty(propertyName: "content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }
    }

    /// <summary>
    /// Represents workflow stage information for bulk operations.
    /// </summary>
    public class BulkWorkflowStage
    {
        /// <summary>
        /// Gets or sets the workflow stage UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [JsonProperty(propertyName: "comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        [JsonProperty(propertyName: "due_date")]
        public string DueDate { get; set; }

        /// <summary>
        /// Gets or sets whether to notify.
        /// </summary>
        [JsonProperty(propertyName: "notify")]
        public bool Notify { get; set; }

        /// <summary>
        /// Gets or sets the list of assigned users.
        /// </summary>
        [JsonProperty(propertyName: "assigned_to")]
        public List<BulkWorkflowUser> AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the list of assigned roles.
        /// </summary>
        [JsonProperty(propertyName: "assigned_by_roles")]
        public List<BulkWorkflowRole> AssignedByRoles { get; set; }

        /// <summary>
        /// Determines whether to serialize the AssignedTo property.
        /// </summary>
        /// <returns>True if AssignedTo should be serialized, false otherwise.</returns>
        public bool ShouldSerializeAssignedTo()
        {
            return AssignedTo != null && AssignedTo.Count > 0;
        }

        /// <summary>
        /// Determines whether to serialize the AssignedByRoles property.
        /// </summary>
        /// <returns>True if AssignedByRoles should be serialized, false otherwise.</returns>
        public bool ShouldSerializeAssignedByRoles()
        {
            return AssignedByRoles != null && AssignedByRoles.Count > 0;
        }
    }

    /// <summary>
    /// Represents a user assigned to a workflow stage.
    /// </summary>
    public class BulkWorkflowUser
    {
        /// <summary>
        /// Gets or sets the user UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        [JsonProperty(propertyName: "email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Represents a role assigned to a workflow stage.
    /// </summary>
    public class BulkWorkflowRole
    {
        /// <summary>
        /// Gets or sets the role UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
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
        [JsonProperty(propertyName: "items")]
        public List<BulkAddItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the release UID for deployment operations.
        /// When specified, this enables release deployment mode (like JavaScript SDK).
        /// </summary>
        [JsonProperty(propertyName: "release")]
        public string Release { get; set; }

        /// <summary>
        /// Gets or sets the action to perform during deployment (publish, unpublish, etc.).
        /// Only used when Release is specified.
        /// </summary>
        [JsonProperty(propertyName: "action")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the list of locales for deployment.
        /// Only used when Release is specified.
        /// </summary>
        [JsonProperty(propertyName: "locale")]
        public List<string> Locale { get; set; }

        /// <summary>
        /// Gets or sets the reference flag for deployment.
        /// Only used when Release is specified.
        /// </summary>
        [JsonProperty(propertyName: "reference")]
        public bool? Reference { get; set; }

        /// <summary>
        /// Determines whether to serialize the Items property.
        /// </summary>
        /// <returns>True if Items should be serialized, false otherwise.</returns>
        public bool ShouldSerializeItems()
        {
            return Items != null && Items.Count > 0;
        }

        /// <summary>
        /// Determines whether to serialize the Release property.
        /// </summary>
        /// <returns>True if Release should be serialized, false otherwise.</returns>
        public bool ShouldSerializeRelease()
        {
            return !string.IsNullOrEmpty(Release);
        }

        /// <summary>
        /// Determines whether to serialize the Action property.
        /// </summary>
        /// <returns>True if Action should be serialized, false otherwise.</returns>
        public bool ShouldSerializeAction()
        {
            return !string.IsNullOrEmpty(Action);
        }

        /// <summary>
        /// Determines whether to serialize the Locale property.
        /// </summary>
        /// <returns>True if Locale should be serialized, false otherwise.</returns>
        public bool ShouldSerializeLocale()
        {
            return Locale != null && Locale.Count > 0;
        }

        /// <summary>
        /// Determines whether to serialize the Reference property.
        /// </summary>
        /// <returns>True if Reference should be serialized, false otherwise.</returns>
        public bool ShouldSerializeReference()
        {
            return Reference.HasValue;
        }

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
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [JsonProperty(propertyName: "content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content type UID for release deployment mode.
        /// This is an alias for ContentType with a different JSON property name.
        /// </summary>
        [JsonProperty(propertyName: "content_type_uid")]
        public string ContentTypeUid { get; set; }

        /// <summary>
        /// Gets or sets the version number for release deployment mode.
        /// Only used in enhanced release deployment operations.
        /// </summary>
        [JsonProperty(propertyName: "version")]
        public int? Version { get; set; }

        /// <summary>
        /// Gets or sets the locale for release deployment mode.
        /// Only used in enhanced release deployment operations.
        /// </summary>
        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the title for release deployment mode.
        /// Only used in enhanced release deployment operations.
        /// </summary>
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }

        /// <summary>
        /// Determines whether to serialize the ContentTypeUid property.
        /// </summary>
        /// <returns>True if ContentTypeUid should be serialized, false otherwise.</returns>
        public bool ShouldSerializeContentTypeUid()
        {
            return !string.IsNullOrEmpty(ContentTypeUid);
        }

        /// <summary>
        /// Determines whether to serialize the Version property.
        /// </summary>
        /// <returns>True if Version should be serialized, false otherwise.</returns>
        public bool ShouldSerializeVersion()
        {
            return Version.HasValue;
        }

        /// <summary>
        /// Determines whether to serialize the Locale property.
        /// </summary>
        /// <returns>True if Locale should be serialized, false otherwise.</returns>
        public bool ShouldSerializeLocale()
        {
            return !string.IsNullOrEmpty(Locale);
        }

        /// <summary>
        /// Determines whether to serialize the Title property.
        /// </summary>
        /// <returns>True if Title should be serialized, false otherwise.</returns>
        public bool ShouldSerializeTitle()
        {
            return !string.IsNullOrEmpty(Title);
        }
    }

    /// <summary>
    /// Represents data for bulk release items operations with complete request body structure.
    /// </summary>
    public class BulkReleaseItemsData
    {
        /// <summary>
        /// Gets or sets the release UID.
        /// </summary>
        [JsonProperty(propertyName: "release")]
        public string Release { get; set; }

        /// <summary>
        /// Gets or sets the action to perform (publish, unpublish, etc.).
        /// </summary>
        [JsonProperty(propertyName: "action")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the list of locales.
        /// </summary>
        [JsonProperty(propertyName: "locale")]
        public List<string> Locale { get; set; }

        /// <summary>
        /// Gets or sets the reference flag.
        /// </summary>
        [JsonProperty(propertyName: "reference")]
        public bool Reference { get; set; }

        /// <summary>
        /// Gets or sets the list of items to process.
        /// </summary>
        [JsonProperty(propertyName: "items")]
        public List<BulkReleaseItem> Items { get; set; }

        /// <summary>
        /// Determines whether to serialize the Locale property.
        /// </summary>
        /// <returns>True if Locale should be serialized, false otherwise.</returns>
        public bool ShouldSerializeLocale()
        {
            return Locale != null && Locale.Count > 0;
        }

        /// <summary>
        /// Determines whether to serialize the Items property.
        /// </summary>
        /// <returns>True if Items should be serialized, false otherwise.</returns>
        public bool ShouldSerializeItems()
        {
            return Items != null && Items.Count > 0;
        }
    }

    /// <summary>
    /// Represents an item for bulk release operations with enhanced properties.
    /// </summary>
    public class BulkReleaseItem
    {
        /// <summary>
        /// Gets or sets the content type UID.
        /// </summary>
        [JsonProperty(propertyName: "content_type_uid")]
        public string ContentTypeUid { get; set; }

        /// <summary>
        /// Gets or sets the item UID.
        /// </summary>
        [JsonProperty(propertyName: "uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        [JsonProperty(propertyName: "version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonProperty(propertyName: "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }
    }
} 