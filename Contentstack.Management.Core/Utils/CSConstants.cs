using System;

namespace Contentstack.Management.Core.Utils
{
    public static class CSConstants
    {
        #region Internal Constants
        internal const long ContentBufferSize = 1024 * 1024 * 1024;
        internal readonly static TimeSpan Timeout = TimeSpan.FromSeconds(30);
        internal readonly static TimeSpan Delay = TimeSpan.FromMilliseconds(300);
        internal const string Slash = "/";
        internal const char SlashChar = '/';
        #endregion

        #region Internal Message
        
        #region Authentication Messages
        internal const string YouAreLoggedIn = "You are already logged in.";
        internal const string YouAreNotLoggedIn = "You are not logged in. Log in and try again.";
        internal const string LoginCredentialsRequired = "Login credentials are required. Provide a valid email and password and try again.";
        internal const string AuthenticationTokenRequired = "Authentication token is required to log out. Provide a valid token and try again.";
        internal const string ResetPasswordTokenRequired = "Reset password token is required. Provide a valid token and try again.";
        internal const string NewPasswordRequired = "New password is required. Provide a valid password and try again.";
        internal const string PasswordMismatch = "New password and confirm password do not match. Enter the same password in both fields and try again.";
        #endregion

        #region OAuth Messages
        internal const string OAuthOptionsRequired = "OAuth options cannot be null.";
        internal const string OAuthTokensRequired = "OAuth tokens cannot be null.";
        internal const string AccessTokenRequired = "Access token cannot be null or empty.";
        internal const string ClientIDRequired = "Client ID cannot be null or empty.";
        #endregion

        #region HTTP Client Messages
        internal const string HttpClientRequired = "HTTP client is required. Initialize the HTTP client and try again.";
        #endregion

        #region API Key Messages
        internal const string MissingAPIKey = "API Key is required. Provide a valid API Key and try again.";
        internal const string InvalidAPIKey = "API Key is invalid. Provide a valid API Key and try again.";
        internal const string APIKeyOrOrgUIDRequired = "API Key or Organization UID is required to perform this operation. Provide a valid value and try again.";
        #endregion

        #region UID Messages
        internal const string MissingUID = "UID is required. Provide a valid UID and try again.";
        internal const string AssetUIDRequired = "Asset UID is required. Provide a valid Asset UID and try again.";
        internal const string AuditLogUIDRequired = "Audit Log UID is required. Provide a valid Audit Log UID and try again.";
        internal const string ExtensionUIDRequired = "Extension UID is required. Provide a valid Extension UID and try again.";
        internal const string FolderUIDRequired = "Folder UID is required. Provide a valid Folder UID and try again.";
        internal const string OrganizationUIDRequired = "Organization UID is required. Provide a valid Organization UID and try again.";
        internal const string PublishQueueUIDRequired = "Publish Queue UID is required. Provide a valid Publish Queue UID and try again.";
        internal const string ReleaseItemUIDRequired = "Release Item UID is required. Provide a valid Release Item UID and try again.";
        internal const string VersionUIDRequired = "Version UID is required. Provide a valid Version UID and try again.";
        internal const string ShareUIDRequired = "Share UID is required. Provide a valid Share UID and try again.";
        internal const string ReleaseUIDRequired = "Release UID is required. Provide a valid Release UID and try again.";
        internal const string JobIDRequired = "Job ID is required to fetch the bulk job status. Provide a valid Job ID and try again.";
        #endregion

        #region Operation Not Allowed Messages
        internal const string AssetAlreadyExists = "An asset with this unique ID already exists. Use a different ID and try again.";
        internal const string OperationNotAllowedOnModel = "Operation not allowed on this model. Update your request and try again.";
        internal const string OperationNotAllowedOnAuditLogs = "Operation not allowed on audit logs. Update your request and try again.";
        internal const string OperationNotAllowedOnExtension = "Operation not allowed on extension. Update your request and try again.";
        internal const string OperationNotAllowedOnFolder = "Operation not allowed on folder. Update your request and try again.";
        internal const string OperationNotAllowedOnPublishQueue = "Operation not allowed on publish queue. Update your request and try again.";
        internal const string OperationNotAllowedForVersion = "Operation not allowed for this Version.";
        #endregion

        #region File and Upload Messages
        internal const string FileNameRequired = "File Name is required. Provide a valid File Name and try again.";
        internal const string UploadContentRequired = "Upload content is required. Provide valid upload content and try again.";
        internal const string WidgetTitleRequired = "Widget Title is required. Provide a valid Widget Title and try again.";
        #endregion

        #region Field and Model Messages
        internal const string FieldNameRequired = "Field Name is required for this service. Provide a valid Field Name and try again.";
        internal const string DataModelRequired = "Data Model is required for this service. Provide a valid Data Model and try again.";
        internal const string ModelRequired = "Model is required for this service. Provide a valid Model and try again.";
        #endregion

        #region Stack Messages
        internal const string StackRequired = "Stack is required. Initialize the Stack and try again.";
        internal const string StackSettingsRequired = "Stack settings are required. Provide valid Stack settings and try again.";
        internal const string InvitationsRequired = "Invitations are required. Provide valid Invitations and try again.";
        internal const string EmailRequired = "Email is required. Provide a valid Email and try again.";
        internal const string StackNameInvalid = "Stack Name is invalid. Provide a valid Stack Name and try again.";
        internal const string LocaleInvalid = "Locale is invalid for this Stack. Provide a valid Locale and try again.";
        internal const string OrganizationUIDInvalid = "Organization UID is invalid. Provide a valid Organization UID and try again.";
        internal const string MasterLocaleRequired = "Master Locale is required when creating the Stack. Provide a valid Master Locale and try again.";
        internal const string StackNameRequired = "Stack Name is required when creating the Stack. Provide a valid Stack Name and try again.";
        #endregion

        #region Release Messages
        internal const string ReleaseNameInvalid = "Release Name is invalid. Provide a valid Release Name and try again.";
        #endregion

        #region Workflow Messages
        internal const string ContentTypeRequired = "Content Type is required. Provide a valid Content Type and try again.";
        #endregion

        #region Query Messages
        internal const string ResourcePathRequired = "Resource Path is required. Provide a valid Resource Path and try again.";
        internal const string ParameterTypeNotSupported = "Parameter value type is not supported. Provide a supported value type and try again.";
        #endregion

        #region Pipeline Messages
        internal const string InnerHandlerNotSet = "Inner Handler is not set. Configure an Inner Handler and try again.";
        internal const string ResponseNotReturned = "Response was not returned and no exception was thrown. Try again. Retry the request and check your network.";
        #endregion

        #region Serializer Messages
        internal const string JSONSerializerError = "JSON serializer error. Check the serializer configuration and try again.";
        #endregion

        #region Bulk Operation Messages
        internal const string BulkOperationStackRequired = "Bulk operation failed. Initialize the stack before running this operation.";
        internal const string BulkAddDataRequired = "Data payload is required for bulk addition. Provide a list of objects and try again.";
        internal const string BulkDeleteDetailsRequired = "Delete details are required for bulk deletion. Provide a list of objects with the required fields and try again.";
        internal const string BulkPublishDetailsRequired = "Publish details are required for bulk publish. Provide a list of item objects with the required fields and try again.";
        internal const string BulkUnpublishDetailsRequired = "Unpublish details are required for bulk unpublish. Provide a list of item objects with the required fields and try again.";
        internal const string BulkReleaseItemsDataRequired = "Data payload is required for bulk release items. Provide a valid list of item objects and try again.";
        internal const string BulkWorkflowUpdateBodyRequired = "Request body is required for bulk workflow update. Provide a valid payload with the required workflow fields and try again.";
        internal const string BulkUpdateDataRequired = "Data payload is required for bulk update. Provide a valid list of item objects with the required fields and try again.";
        #endregion

        #region Organization Messages
        internal const string EmailsRequired = "Emails are required. Provide a list of valid email addresses and try again.";
        internal const string UserInvitationDetailsRequired = "User invitation details are required. Provide a valid UID and Roles to update the user role and try again.";
        #endregion

        #region Service Messages
        internal const string FolderNameRequired = "Folder Name is required. Provide a valid Folder Name and try again.";
        internal const string PublishDetailsRequired = "Publish details are required. Provide valid publish details and try again.";
        internal const string HTTPMethodRequired = "HTTP method is required. Provide a valid HTTP method and try again.";
        internal const string ReleaseItemsRequired = "Release Items are required. Provide valid Release Items and try again.";
        #endregion

        #region Legacy Messages (for backward compatibility)
        internal const string RemoveUserEmailError = "Please enter email id to remove from org.";
        internal const string OrgShareUIDMissing = "Please enter share uid to resend invitation.";
        internal const string APIKey = "API Key should be empty.";
        #endregion
        #endregion
    }
}
