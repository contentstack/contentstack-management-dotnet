# Changelog

## [v1.0.0-beta.2](https://github.com/contentstack/contentstack-management-dotnet/tree/v1.0.0-beta.2)(2026-06-17)
 - **Test**
   - **Image format upload coverage**
     - Added `Test100_Should_Upload_JPEG_Image_Asset` — uploads `london.jpg` via `image/jpeg` MIME type and verifies `Created` status and `content_type` in response
     - Added `Test101_Should_Upload_JPEG_Extension_Image_Asset` — uploads `tokyo.jpeg` (`.jpeg` extension) and asserts `image/jpeg` content type
     - Added `Test102_Should_Upload_AVIF_Image_Asset` — uploads `dubai.avif` via `image/avif` MIME type and accepts `image/avif` or `application/octet-stream` content type
     - Added `Test103_Should_Upload_Multiple_Image_Formats_Sequentially` — uploads all three images in sequence and asserts `Created` on each
     - Added `Test104_Should_Fetch_Uploaded_Image_Asset_Metadata` — creates a JPEG asset and fetches it back, asserting `uid`, `filename`, `content_type`, and `file_size` fields are present
     - Added `Test105_Should_Update_Uploaded_Image_Asset` — creates a JPEG asset then replaces it with a different JPEG and verifies the `200 OK` update response
     - Test image files (`london.jpg`, `tokyo.jpeg`, `dubai.avif`) live under `Contentstack.Management.Core.Tests/Mock/assets/`

## [v1.0.0-beta.1](https://github.com/contentstack/contentstack-management-dotnet/tree/v1.0.0-beta.1)(2026-06-15)
 - **Feat**
   - **Branch support**
     - Added `Branch` model with `Create`, `CreateAsync`, `Fetch`, `FetchAsync`, `Delete`, `DeleteAsync`, and `Query` operations
     - `Stack.Branch(uid?)` accessor follows the same pattern as other stack resources
 - **Breaking Change**
   - **Complete migration from Newtonsoft.Json to System.Text.Json**
     - `Newtonsoft.Json` is no longer a dependency — remove it from your project once your own code no longer references it directly
     - `client.SerializerSettings` (`JsonSerializerSettings`) replaced by `client.SerializerOptions` (`JsonSerializerOptions`)
     - `response.OpenJObjectResponse()` removed — use `response.OpenJsonObjectResponse()` (`JsonObject`) or `response.OpenTResponse<T>()` instead
     - `JObjectParameterValue` now accepts `System.Text.Json.Nodes.JsonNode` instead of `Newtonsoft.Json.Linq.JObject`
     - All `[JsonProperty]` attributes replaced with `[JsonPropertyName]`; `[JsonObject(ItemNullValueHandling)]` removed in favour of `DefaultIgnoreCondition = WhenWritingNull` on `SerializerOptions`
     - All modules fully migrated to System.Text.Json: AuditLog, Branch, BulkOperation, ContentType, DeliveryToken, Entry, EntryVariant, Environment, Extension, GlobalField, Label, Locale, ManagementToken, Organization, Release, Role, Stack, Taxonomy, Term, User, VariantGroup, Webhook, and Workflow
     - OAuth auto token refresh wired into the request pipeline
     - Upgraded target framework to .NET 10
 - **New:** Multi-region endpoint resolution via `Endpoint.GetContentstackEndpoint(region, service)` — resolves Contentstack service URLs for all 7 supported regions (NA, EU, AU, Azure-NA, Azure-EU, GCP-NA, GCP-EU) and 18 service keys (contentManagement, contentDelivery, auth, graphqlDelivery, preview, images, assets, automate, launch, developerHub, brandKit, genAI, personalizeManagement, personalizeEdge, composableStudio, assetManagement, and more).
 - **New:** `omitHttps` flag strips the `https://` scheme from returned URLs — pass directly to `ContentstackClientOptions.Host` (e.g. `new ContentstackClientOptions { Host = Endpoint.GetContentstackEndpoint("eu", "contentManagement", omitHttps: true) }`).
 - **New:** Case-insensitive region alias support — `"us"`, `"NA"`, `"AWS-NA"`, `"azure_na"` all resolve correctly to the same region.
 - **New:** `regions.json` registry auto-downloaded from `artifacts.contentstack.com` on first use and cached on disk — no setup required. The SDK self-heals if the file is missing.
 - **New:** `Scripts/refresh-region.cs` bundled inside the NuGet package — automatically placed in your project's `Scripts/` folder on first `dotnet build`. Run `dotnet run Scripts/refresh-region.cs` anytime to pull the latest regions from CDN.

## [v0.10.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.10.0)
 - Feat
   - **Enhanced Error Handling and Test Coverage (DX-5436)**
     - Added comprehensive error handling across all models with enhanced `ContentstackErrorException`
     - Implemented negative test cases for all integration tests to validate error scenarios
     - Added testing infrastructure: `MockHttpStatusHandler`, `MockNetworkErrorHandler`, and `AssertLogger` helpers
     - Enhanced test coverage with error validation across Login, Organization, Stack, Release, Global Field, Content Type, Nested Global Field, Asset, Entry, Bulk Operation, Delivery Token, Taxonomy, Environment, Role, Workflow, Entry Variant, and Variant Group operations
     - Improved exception handling in `BaseModel` and service layers


## [v0.9.0](https://github.com/contentstack/contentstack-management-dotnet/tree/v0.9.0)
 - Fix
   - **Variant Group HTTP method correction**: Updated variant group link/unlink operations to use PUT method instead of POST for API compliance
   - Enhanced integration test coverage for variant group operations

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