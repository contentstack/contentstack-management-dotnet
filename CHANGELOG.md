# Changelog

## [v0.8.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.8.0)
 - Feat
   - **Entry Variant support**
     - `EntryVariant` model for create, fetch, find, update, and delete on entry variant endpoints
     - `Entry.Variant(uid)` to access variant operations for a given entry
     - Publish with variants: `PublishVariant`, `PublishVariantRules`, and `Variants` / `VariantRules` on `PublishUnpublishDetails`; serialization updated in `PublishUnpublishService`
     - Unit tests for `EntryVariant` and publish payload serialization; integration tests (`Contentstack021_EntryVariantTest`) for Product Banner lifecycle and negative cases

## [v0.7.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.7.0)
 - Feat
   - **Bulk publish/unpublish: query parameters (DX-3233)**
     - `skip_workflow_stage_check` and `approvals` are now sent as query parameters instead of headers for bulk publish and bulk unpublish
     - Unit tests updated to assert on `QueryResources` for these flags (BulkPublishServiceTest, BulkUnpublishServiceTest, BulkOperationServicesTest)
     - Integration tests: bulk publish with skipWorkflowStage and approvals (Test003a), bulk unpublish with skipWorkflowStage and approvals (Test004a), and helper `EnsureBulkTestContentTypeAndEntriesAsync()` so bulk tests can run in any order

## [v0.6.1](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.6.1) (2026-02-02)
 - Fix
   - Release DELETE request no longer includes Content-Type header to comply with API requirements

## [v0.6.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.6.0)
 - Enhancement
   - Refactor retry policy implementation to improve exception handling  and retry logic across various scenarios
   - Improved error messages

## [v0.5.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.5.0)
 - Feat
   - **Variant Group Management**: Added comprehensive support for variant group operations
     - Added `VariantGroup` model class with Find, LinkContentTypes and UnlinkContentTypes methods
     - Comprehensive unit test coverage with 33+ tests covering all functionality and edge cases

## [v0.4.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.4.0)
 - Feat
   - **MFA Support**: Added Multi-Factor Authentication (MFA) support for login operations
     - Added `mfaSecret` parameter to `Login` and `LoginAsync` methods for TOTP generation
     - Automatic TOTP token generation from Base32-encoded MFA secrets using Otp.NET library
     - Comprehensive test coverage for MFA functionality including unit and integration tests
     - Supports both explicit token and MFA secret-based authentication flows
   - Added Support for OAuth
    - Added Comprehensive test coverage for OAuth Functionality in Unit Test cases.
    - Supports both Login with and without OAuth Flows 

## [v0.3.2](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.3.2)
 - Fix
   - Added Test cases for the Release

## [v0.3.1](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.3.1)
 - Fix
   - Fixed apiVersion param in Publish/Unpublish methods

## [v0.3.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.3.0)
  - Feat
    - Bulk Operations: 
      - Added Support for the bulk operations of Publish, Unpublish, Delete, Workflow Update, addRelease Items, Update Release Items, Job-Status both sync and async methods
    - Nested Global Fields: 
      Added the support for the nested global fields for all the CRUD Operations

## [v0.2.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.2.0)
 - Fix
   - Fixed the Single Publish issue with specific entry version (Changing the type from String to Int)

## [v0.1.12](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.12)
 - Fix
   - Fix the Delivery Token URL
   - Made the Summary More Readable

## [v0.1.11](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.11)
 - Feat
   - Add support for custom Http client and IHttpClientFactory

## [v0.1.10](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.10)
 - Feat
   - Add support for apiVersion in bulk publish unpublish methods

## [v0.1.9](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.9)
 - Fix
   - Media header now added only to Assets API methods and removed from all others for both Sync and Async calls.


## [v0.1.8](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.8)
 - Fix
   - Strong named assemblies

## [v0.1.7](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.7) (2024-02-11)
 - Feature
   - Parameter support in References and ReferencesAsync methods

## [v0.1.6](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.6) (2024-02-11)
 - Fix
   - TextNode Deserializer

## [v0.1.5](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.5) (2024-02-11)
 - Adds JsonConverters to Serializer for JSON Rte 

## [v0.1.4](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.4) (2024-01-22)
 - EarlyAccess Header support and AddQuery method in ParamCollection

## [v0.1.3](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.3) (2023-04-04)

## [v0.1.2](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.2) (2023-03-07)

## [v0.1.1](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.1) (2022-12-16)

## [v0.1.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.1.0) (2022-10-19)