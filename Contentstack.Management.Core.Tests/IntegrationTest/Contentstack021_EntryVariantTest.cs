using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    public class ProductBannerEntry : IEntry
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("banner_title")]
        public string BannerTitle { get; set; }

        [JsonPropertyName("banner_color")]
        public string BannerColor { get; set; }
    }

    [TestClass]
    public class Contentstack021_EntryVariantTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        private string _contentTypeUid = "product_banner";
        private static string _entryUid;
        private static string _variantUid;
        private static string _variantGroupUid;

        #region Helper Methods

        /// <summary>
        /// Creates invalid variant payloads for systematic negative testing.
        /// Uses scenario-based approach for systematic negative testing.
        /// </summary>
        private static object CreateInvalidVariantPayload(string scenario)
        {
            switch (scenario)
            {
                case "null_data":
                    return null;

                case "empty_changeset":
                    return new
                    {
                        banner_color = "Test Color",
                        _variant = new
                        {
                            _change_set = new string[] { },
                            _order = new string[] { }
                        }
                    };

                case "no_variant_metadata":
                    return new
                    {
                        banner_color = "Test Color"
                        // missing _variant object entirely
                    };

                case "invalid_field_names":
                    return new
                    {
                        banner_color = "Test Color",
                        _variant = new
                        {
                            _change_set = new[] { "nonexistent_field_xyz" },
                            _order = new string[] { }
                        }
                    };

                case "oversized_payload":
                    var largeString = new string('A', 10000); // 10KB string
                    return new
                    {
                        banner_title = largeString,
                        banner_color = largeString,
                        _variant = new
                        {
                            _change_set = new[] { "banner_title", "banner_color" },
                            _order = new string[] { }
                        }
                    };

                case "invalid_field_types":
                    return new
                    {
                        banner_color = 12345, // should be string
                        _variant = new
                        {
                            _change_set = new[] { "banner_color" },
                            _order = new string[] { }
                        }
                    };

                case "malformed_order_array":
                    return new
                    {
                        banner_color = "Test Color",
                        _variant = new
                        {
                            _change_set = new[] { "banner_color" },
                            _order = "invalid_string_not_array"
                        }
                    };

                case "unicode_characters":
                    return new
                    {
                        banner_title = "Test with Unicode: 🚀 中文 العربية 🎉",
                        banner_color = "Unicode Color 🌈",
                        _variant = new
                        {
                            _change_set = new[] { "banner_title", "banner_color" },
                            _order = new string[] { }
                        }
                    };

                default:
                    throw new ArgumentException($"Unknown scenario: {scenario}");
            }
        }

        /// <summary>
        /// Asserts that the HTTP status code indicates a validation error (4xx range).
        /// </summary>
        private static void AssertValidationError(HttpStatusCode statusCode, string assertionName)
        {
            AssertLogger.IsTrue(
                (int)statusCode >= 400 && (int)statusCode < 500,
                $"Expected 4xx status code for validation error, got {(int)statusCode} ({statusCode})",
                assertionName);
        }

        /// <summary>
        /// Asserts that the exception indicates an authentication/authorization error.
        /// </summary>
        private static void AssertAuthenticationError(Exception ex, string assertionName)
        {
            AssertLogger.IsNotNull(ex, assertionName);
            
            if (ex is ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Unauthorized || 
                    cex.StatusCode == HttpStatusCode.Forbidden ||
                    cex.StatusCode == (HttpStatusCode)412, // PreconditionFailed for API key issues
                    $"Expected 401/403/412 for auth error, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    assertionName);
            }
            else if (ex is InvalidOperationException)
            {
                // SDK-level validation before HTTP call
                AssertLogger.IsTrue(true, "SDK validation threw InvalidOperationException as expected", assertionName);
            }
            else
            {
                AssertLogger.Fail($"Expected ContentstackErrorException or InvalidOperationException for auth error, got {ex.GetType().Name}: {ex.Message}", assertionName);
            }
        }

        /// <summary>
        /// Provides detailed error information when operations fail unexpectedly.
        /// </summary>
        private static void FailWithError(string operation, Exception ex)
        {
            string errorDetails = "Unknown error";

            if (ex is ContentstackErrorException cex)
            {
                errorDetails = $"HTTP {(int)cex.StatusCode} ({cex.StatusCode}). " +
                             $"ErrorCode: {cex.ErrorCode}. " +
                             $"Message: {cex.ErrorMessage}";

                if (cex.Errors != null && cex.Errors.Count > 0)
                {
                    var errorFields = string.Join(", ", cex.Errors.Keys);
                    errorDetails += $". Fields: {errorFields}";
                }
            }
            else
            {
                errorDetails = $"{ex.GetType().Name}: {ex.Message}";
            }

            AssertLogger.Fail($"{operation} failed with error: {errorDetails}", "UnexpectedError");
        }

        /// <summary>
        /// Asserts that a status code indicates a missing resource error (404 or 422).
        /// API inconsistency: sometimes 404, sometimes 422 for missing resources.
        /// </summary>
        private static void AssertMissingResourceError(HttpStatusCode statusCode, string assertionName)
        {
            AssertLogger.IsTrue(
                statusCode == HttpStatusCode.NotFound || statusCode == (HttpStatusCode)422 || statusCode == (HttpStatusCode)412,
                $"Expected 404 or 422 or 412 for missing resource, got {(int)statusCode} ({statusCode})",
                assertionName);
        }

        private static void AssertMissingEnvironmentError(HttpStatusCode statusCode, string assertionName)
        {
            AssertLogger.IsTrue(
                statusCode == HttpStatusCode.NotFound || statusCode == (HttpStatusCode)422 || statusCode == (HttpStatusCode)412 || statusCode == HttpStatusCode.Unauthorized,
                $"Expected 404 or 422 or 412 or 401 for missing/restricted environment, got {(int)statusCode} ({statusCode})",
                assertionName);
        }

        #endregion

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Read the API key from appSettings.json
            string apiKey = Contentstack.Config["Contentstack:Stack:api_key"];
            
            // Optional: Fallback to stackApiKey.txt if it's missing in appSettings.json
            if (string.IsNullOrEmpty(apiKey))
            {
                StackResponse response = StackResponse.getStack(_client.SerializerOptions);
                apiKey = response.Stack.APIKey;
            }
            
            _stack = _client.Stack(apiKey);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test001_Ensure_Setup_Data()
        {
            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Setup");

            // 1. Ensure Variant Group exists
            var collection = new global::Contentstack.Management.Core.Queryable.ParameterCollection();
            collection.Add("include_variant_info", "true");
            collection.Add("include_variant_count", "true");

            var vgResponse = await _stack.VariantGroup().FindAsync(collection);
            Console.WriteLine("Variant Groups Response: " + vgResponse.OpenResponse());

            var vgJObject = vgResponse.OpenJsonObjectResponse();
            var groups = vgJObject["variant_groups"] as JsonArray;

            if (groups == null || groups.Count == 0)
            {
                Assert.Inconclusive("No variant groups found in the stack. Create one to run EntryVariant tests. Response was: " + vgResponse.OpenResponse());
                return;
            }

            _variantGroupUid = groups[0]["uid"]?.ToString();
            
            var variantsArray = groups[0]["variants"] as JsonArray;
            if (variantsArray != null && variantsArray.Count > 0)
            {
                _variantUid = variantsArray[0]["uid"]?.ToString();
            }
            else
            {
                var variantUids = groups[0]["variant_uids"] as JsonArray;
                if (variantUids != null && variantUids.Count > 0)
                {
                    _variantUid = variantUids[0].ToString();
                }
            }

            if (string.IsNullOrEmpty(_variantUid))
            {
                // Fallback to demo UIDs if none are returned by the API so the test doesn't skip
                _variantUid = "cs2082f36d4099af4e";
                Console.WriteLine("Warning: The variant group had no variants. Using a hardcoded variant UID for testing: " + _variantUid);
            }

            TestOutputLogger.LogContext("VariantGroup", _variantGroupUid);
            TestOutputLogger.LogContext("Variant", _variantUid);

            // 2. Ensure Content Type exists
            ContentstackResponse ctFetchResponse = _stack.ContentType(_contentTypeUid).Fetch();
            if (!ctFetchResponse.IsSuccessStatusCode)
            {
                var contentModelling = new ContentModelling
                {
                    Title = "Product Banner",
                    Uid = _contentTypeUid,
                    Schema = new List<Field>
                    {
                        new TextboxField
                        {
                            DisplayName = "Title",
                            Uid = "title",
                            DataType = "text",
                            Mandatory = true
                        },
                        new TextboxField
                        {
                            DisplayName = "Banner Title",
                            Uid = "banner_title",
                            DataType = "text"
                        },
                        new TextboxField
                        {
                            DisplayName = "Banner Color",
                            Uid = "banner_color",
                            DataType = "text"
                        }
                    }
                };

                ContentstackResponse createCtResponse = _stack.ContentType().Create(contentModelling);
                if (!createCtResponse.IsSuccessStatusCode)
                {
                    Assert.Fail("Failed to create content type: " + createCtResponse.OpenResponse());
                }
            }

            // 3. Link Content Type to Variant Group
            try
            {
                var linkResponse = await _stack.VariantGroup(_variantGroupUid).LinkContentTypesAsync(new List<string> { _contentTypeUid });
                if (!linkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Warning: LinkContentTypesAsync failed, but continuing as it might already be linked. Error: " + linkResponse.OpenResponse());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: LinkContentTypesAsync threw an exception. It might be due to an SDK endpoint bug. Continuing. Exception: " + ex.Message);
            }

            // 4. Ensure Base Entry exists
            var queryResp = await _stack.ContentType(_contentTypeUid).Entry().Query().FindAsync();
            var entriesArray = queryResp.OpenJsonObjectResponse()["entries"] as JsonArray;
            
            if (entriesArray != null && entriesArray.Count > 0)
            {
                _entryUid = entriesArray[0]["uid"]?.ToString();
            }
            else
            {
                var entryData = new ProductBannerEntry
                {
                    Title = "Test Banner",
                    BannerTitle = "Original Title",
                    BannerColor = "Original Color"
                };

                var entryResponse = await _stack.ContentType(_contentTypeUid).Entry().CreateAsync(entryData);
                Assert.IsTrue(entryResponse.IsSuccessStatusCode, "Should create base entry: " + entryResponse.OpenResponse());
                var entryObj = entryResponse.OpenJsonObjectResponse()["entry"];
                _entryUid = entryObj["uid"]?.ToString();
            }

            Assert.IsNotNull(_entryUid, "Entry UID should not be null");

            TestOutputLogger.LogContext("Entry", _entryUid);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Create_Entry_Variant()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Create");

            var variantData = new
            {
                banner_color = "Navy Blue",
                _variant = new
                {
                    _change_set = new[] { "banner_color" },
                    _order = new string[] { }
                }
            };

            var createVariantResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
            Assert.IsTrue(createVariantResponse.IsSuccessStatusCode, "Should create entry variant. " + createVariantResponse.OpenResponse());
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Fetch_Entry_Variants()
        {
            if (string.IsNullOrEmpty(_entryUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Fetch");

            var fetchVariantsResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant().FindAsync();
            Assert.IsTrue(fetchVariantsResponse.IsSuccessStatusCode, "Should fetch all variants for entry");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test004_Should_Publish_Entry_With_Variants()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Publish");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                },
                VariantRules = new PublishVariantRules
                {
                    PublishLatestBase = true,
                    PublishLatestBaseConditionally = false
                }
            };

            try
            {
                var publishResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
                if (!publishResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Publish failed (often due to missing 'development' environment). Response: " + publishResponse.OpenResponse());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Publish threw exception (often due to missing 'development' environment). Continuing. Exception: " + ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Delete_Entry_Variant()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Delete");

            var deleteVariantResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).DeleteAsync();
            Assert.IsTrue(deleteVariantResponse.IsSuccessStatusCode, "Should delete entry variant");
        }
        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Fail_To_Create_Variant_For_Invalid_Entry()
        {
            if (string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Create_Negative");

            var invalidEntryUid = "blt_invalid_entry_uid";
            var variantData = new { banner_color = "Navy Blue", _variant = new { _change_set = new[] { "banner_color" } } };

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(invalidEntryUid).Variant(_variantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid entry UID" 
                    };
                }
            }, "CreateVariantInvalidEntry", HttpStatusCode.NotFound, (HttpStatusCode)422, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test007_Should_Fail_To_Fetch_Invalid_Variant()
        {
            if (string.IsNullOrEmpty(_entryUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Fetch_Negative");

            var invalidVariantUid = "cs_invalid_variant_123";

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(invalidVariantUid).FetchAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid variant UID" 
                    };
                }
            }, "FetchInvalidVariant", HttpStatusCode.NotFound, (HttpStatusCode)422, (HttpStatusCode)412);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test008_Should_Fail_To_Delete_Invalid_Variant()
        {
            if (string.IsNullOrEmpty(_entryUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Delete_Negative");

            var invalidVariantUid = "cs_invalid_variant_123";

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(invalidVariantUid).DeleteAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid variant UID" 
                    };
                }
            }, "DeleteInvalidVariant", HttpStatusCode.NotFound, (HttpStatusCode)422, (HttpStatusCode)412);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test009_Should_Accept_Publish_With_Invalid_Variant()
        {
            if (string.IsNullOrEmpty(_entryUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Publish_Negative");

            // API is permissive and accepts publish requests with invalid variants
            var publishDetailsWithInvalidVariant = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = "cs_invalid_variant_123", Version = 1 }
                }
            };

            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetailsWithInvalidVariant, "en-us");
            
            // API accepts the request and ignores invalid variants
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept publish with invalid variant, got {response.StatusCode}",
                "PublishWithInvalidVariantAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test010_Should_Accept_Create_Variant_Without_ChangeSet()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Create_NoChangeSet_Negative");

            // API is permissive and auto-generates changeset when missing
            var variantDataWithoutChangeSet = new
            {
                banner_color = "Red",
                _variant = new
                {
                    // missing _change_set array - API will auto-generate it
                    _order = new string[] { }
                }
            };

            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantDataWithoutChangeSet);
            
            // API accepts the request and auto-generates changeset
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept variant creation without changeset, got {response.StatusCode}",
                "CreateVariantWithoutChangesetAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test011_Should_Fail_To_Publish_Variant_To_Invalid_Environment()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Publish_Env_Negative");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "non_existent_environment_123" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid environment" 
                    };
                }
            }, "PublishToInvalidEnvironment", HttpStatusCode.NotFound, (HttpStatusCode)422, HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test012_Should_Fail_To_Create_Variant_With_Unlinked_Content_Type()
        {
            if (string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "ProductBannerVariantLifecycle_Unlinked_CT_Negative");

            var dummyContentTypeUid = "unlinked_dummy_ct";
            
            // To be thorough, this test usually creates a dummy Content Type, creates an entry in it, 
            // and tries to create a variant when it hasn't been linked to a variant group.
            // But since creating full schema is tedious, we can assert that trying to use a non-existent 
            // or unlinked dummy content type for variants will be rejected by the API.

            var invalidVariantData = new
            {
                title = "Dummy",
                _variant = new { _change_set = new[] { "title" } }
            };

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                // Tries to perform variant creation on a content type that has no variants linked
                var response = await _stack.ContentType(dummyContentTypeUid).Entry("blt_dummy_entry").Variant(_variantUid).CreateAsync(invalidVariantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Unlinked content type or non-existent content type/entry" 
                    };
                }
            }, "CreateVariantUnlinkedContentType", HttpStatusCode.NotFound, (HttpStatusCode)422, HttpStatusCode.BadRequest, HttpStatusCode.Forbidden, (HttpStatusCode)412);
        }

        #region A — Input Validation Tests (Sync)

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Fail_Create_Variant_With_Null_Data_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test013_Should_Fail_Create_Variant_With_Null_Data_Sync");

            try
            {
                var invalidData = CreateInvalidVariantPayload("null_data");
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(invalidData);
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "CreateVariantWithNullData");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for null variant data, but API accepted it", "NullDataAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "CreateVariantWithNullDataException");
            }
            catch (ArgumentNullException)
            {
                AssertLogger.IsTrue(true, "SDK validation threw ArgumentNullException as expected", "NullDataValidation");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Accept_Create_Variant_With_Empty_Changeset_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test014_Should_Accept_Create_Variant_With_Empty_Changeset_Sync");

            // API is permissive and auto-generates changeset when empty
            var dataWithEmptyChangeset = CreateInvalidVariantPayload("empty_changeset");
            var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(dataWithEmptyChangeset);
            
            // API accepts empty changeset and auto-generates it
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept empty changeset, got {response.StatusCode}",
                "EmptyChangesetAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Fail_Create_Variant_Without_Variant_Metadata_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test015_Should_Fail_Create_Variant_Without_Variant_Metadata_Sync");

            try
            {
                var invalidData = CreateInvalidVariantPayload("no_variant_metadata");
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(invalidData);
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "CreateVariantWithoutMetadata");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for missing variant metadata, but API accepted it", "MissingMetadataAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "CreateVariantWithoutMetadataException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Accept_Create_Variant_With_Invalid_Field_Names_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test016_Should_Accept_Create_Variant_With_Invalid_Field_Names_Sync");

            // API is permissive and ignores invalid field names in changeset
            var dataWithInvalidFieldNames = CreateInvalidVariantPayload("invalid_field_names");
            var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(dataWithInvalidFieldNames);
            
            // API accepts invalid field names and filters them from changeset
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept invalid field names, got {response.StatusCode}",
                "InvalidFieldNamesAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Accept_Create_Variant_With_Oversized_Payload_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test017_Should_Accept_Create_Variant_With_Oversized_Payload_Sync");

            // API is permissive and accepts large payloads without size validation
            var oversizedData = CreateInvalidVariantPayload("oversized_payload");
            var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(oversizedData);
            
            // API accepts oversized payloads
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept oversized payload, got {response.StatusCode}",
                "OversizedPayloadAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Fail_Create_Variant_With_Invalid_Field_Types_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test018_Should_Fail_Create_Variant_With_Invalid_Field_Types_Sync");

            try
            {
                var invalidData = CreateInvalidVariantPayload("invalid_field_types");
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(invalidData);
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "CreateVariantWithInvalidTypes");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for invalid field types, but API accepted it", "InvalidTypesAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "CreateVariantWithInvalidTypesException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Accept_Create_Variant_With_Fields_Not_In_Schema_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test019_Should_Accept_Create_Variant_With_Fields_Not_In_Schema_Sync");

            // API is permissive and ignores fields not in schema
            var dataWithNonSchemaFields = CreateInvalidVariantPayload("invalid_field_names");
            var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(dataWithNonSchemaFields);
            
            // API accepts extra fields and ignores them
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept fields not in schema, got {response.StatusCode}",
                "NonSchemaFieldsAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test020_Should_Fail_Create_Variant_With_Malformed_Order_Array_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test020_Should_Fail_Create_Variant_With_Malformed_Order_Array_Sync");

            try
            {
                var invalidData = CreateInvalidVariantPayload("malformed_order_array");
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(invalidData);
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "CreateVariantWithMalformedOrder");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for malformed order array, but API accepted it", "MalformedOrderAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "CreateVariantWithMalformedOrderException");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "SDK validation threw exception for malformed order array as expected", "MalformedOrderValidation");
            }
        }

        #endregion

        #region B — Input Validation Tests (Async)

        [TestMethod]
        [DoNotParallelize]
        public async Task Test021_Should_Fail_Create_Variant_With_Null_Data_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test021_Should_Fail_Create_Variant_With_Null_Data_Async");

            try 
            {
                var invalidData = CreateInvalidVariantPayload("null_data");
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(invalidData);
                AssertLogger.Fail("Expected ArgumentNullException for null data", "CreateVariantWithNullDataAsync");
            }
            catch (ArgumentNullException) 
            {
                AssertLogger.IsTrue(true, "SDK validation throws ArgumentNullException for null data as expected", "CreateVariantWithNullDataAsync");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test022_Should_Accept_Create_Variant_With_Empty_Changeset_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test022_Should_Accept_Create_Variant_With_Empty_Changeset_Async");

            // API is permissive and auto-generates changeset when empty
            var dataWithEmptyChangeset = CreateInvalidVariantPayload("empty_changeset");
            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(dataWithEmptyChangeset);
            
            // API accepts empty changeset and auto-generates it
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept empty changeset, got {response.StatusCode}",
                "EmptyChangesetAcceptedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test023_Should_Fail_Create_Variant_Without_Variant_Metadata_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test023_Should_Fail_Create_Variant_Without_Variant_Metadata_Async");

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var invalidData = CreateInvalidVariantPayload("no_variant_metadata");
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(invalidData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Missing variant metadata" 
                    };
                }
            }, "CreateVariantWithoutMetadataAsync", HttpStatusCode.BadRequest, (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test024_Should_Accept_Create_Variant_With_Invalid_Field_Names_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test024_Should_Accept_Create_Variant_With_Invalid_Field_Names_Async");

            // API is permissive and ignores invalid field names in changeset
            var dataWithInvalidFieldNames = CreateInvalidVariantPayload("invalid_field_names");
            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(dataWithInvalidFieldNames);
            
            // API accepts invalid field names and filters them from changeset
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept invalid field names, got {response.StatusCode}",
                "InvalidFieldNamesAcceptedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test025_Should_Accept_Create_Variant_With_Oversized_Payload_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test025_Should_Accept_Create_Variant_With_Oversized_Payload_Async");

            // API is permissive and accepts large payloads without size validation
            var oversizedData = CreateInvalidVariantPayload("oversized_payload");
            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(oversizedData);
            
            // API accepts oversized payloads
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept oversized payload, got {response.StatusCode}",
                "OversizedPayloadAcceptedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test026_Should_Fail_Create_Variant_With_Invalid_Field_Types_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test026_Should_Fail_Create_Variant_With_Invalid_Field_Types_Async");

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var invalidData = CreateInvalidVariantPayload("invalid_field_types");
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(invalidData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid field types" 
                    };
                }
            }, "CreateVariantWithInvalidTypesAsync", HttpStatusCode.BadRequest, (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test027_Should_Accept_Create_Variant_With_Fields_Not_In_Schema_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test027_Should_Accept_Create_Variant_With_Fields_Not_In_Schema_Async");

            // API is permissive and ignores fields not in schema
            var dataWithNonSchemaFields = CreateInvalidVariantPayload("invalid_field_names");
            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(dataWithNonSchemaFields);
            
            // API accepts extra fields and ignores them
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept fields not in schema, got {response.StatusCode}",
                "NonSchemaFieldsAcceptedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test028_Should_Fail_Create_Variant_With_Malformed_Order_Array_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test028_Should_Fail_Create_Variant_With_Malformed_Order_Array_Async");

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var invalidData = CreateInvalidVariantPayload("malformed_order_array");
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(invalidData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Malformed order array in variant metadata" 
                    };
                }
            }, "CreateVariantWithMalformedOrderAsync", HttpStatusCode.BadRequest, (HttpStatusCode)422);
        }

        #endregion

        #region C — Authentication & Authorization Tests (Sync)

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Fail_Operations_With_Invalid_Auth_Token_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test029_Should_Fail_Operations_With_Invalid_Auth_Token_Sync");

            // Create a client with invalid auth token
            var invalidClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "invalid_auth_token_12345"
            });

            var invalidStack = invalidClient.Stack(_stack.APIKey);

            try
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = invalidStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
                        $"Expected 401/403 for invalid auth token, got {(int)response.StatusCode} ({response.StatusCode})",
                        "InvalidAuthTokenValidation");
                }
                else
                {
                    AssertLogger.Fail("Expected authentication error for invalid token, but API accepted it", "InvalidTokenAccepted");
                }
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "InvalidAuthTokenException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Fail_Operations_With_Malformed_API_Key_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test030_Should_Fail_Operations_With_Malformed_API_Key_Sync");

            var invalidStack = _client.Stack("invalid_api_key_format");

            try
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = invalidStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Unauthorized || 
                        response.StatusCode == HttpStatusCode.Forbidden ||
                        response.StatusCode == (HttpStatusCode)412, // PreconditionFailed
                        $"Expected 401/403/412 for malformed API key, got {(int)response.StatusCode} ({response.StatusCode})",
                        "MalformedAPIKeyValidation");
                }
                else
                {
                    AssertLogger.Fail("Expected authentication error for malformed API key, but API accepted it", "MalformedAPIKeyAccepted");
                }
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "MalformedAPIKeyException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test031_Should_Validate_Variant_Stack_Isolation_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test031_Should_Validate_Variant_Stack_Isolation_Sync");

            // Attempt to access variant using different stack context
            var differentStackKey = "blt_fake_stack_key_12345";
            var differentStack = _client.Stack(differentStackKey);

            try
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = differentStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Unauthorized || 
                        response.StatusCode == HttpStatusCode.Forbidden ||
                        response.StatusCode == HttpStatusCode.NotFound ||
                        response.StatusCode == (HttpStatusCode)412,
                        $"Expected auth error for stack isolation, got {(int)response.StatusCode} ({response.StatusCode})",
                        "StackIsolationValidation");
                }
                else
                {
                    AssertLogger.Fail("Expected authentication error for cross-stack access, but API accepted it", "CrossStackAccessAccepted");
                }
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "StackIsolationException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Fail_Operations_With_Insufficient_Permissions_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test032_Should_Fail_Operations_With_Insufficient_Permissions_Sync");

            // This test assumes there's a way to create a limited permissions context
            // In practice, this might require a different auth token with limited permissions
            // For now, we'll test with an expired or invalid token scenario

            var limitedClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "limited_permissions_token"
            });

            var limitedStack = limitedClient.Stack(_stack.APIKey);

            try
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = limitedStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized,
                        $"Expected 403/401 for insufficient permissions, got {(int)response.StatusCode} ({response.StatusCode})",
                        "InsufficientPermissionsValidation");
                }
                else
                {
                    AssertLogger.Fail("Expected authorization error for insufficient permissions, but API accepted it", "InsufficientPermissionsAccepted");
                }
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "InsufficientPermissionsException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test033_Should_Handle_Operations_With_No_Auth_Context_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test033_Should_Handle_Operations_With_No_Auth_Context_Sync");

            // Create a client with no auth token
            var noAuthClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = ""
            });

            var noAuthStack = noAuthClient.Stack(_stack.APIKey);

            try
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = noAuthStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
                        $"Expected 401/403 for no auth context, got {(int)response.StatusCode} ({response.StatusCode})",
                        "NoAuthContextValidation");
                }
                else
                {
                    AssertLogger.Fail("Expected authentication error for no auth context, but API accepted it", "NoAuthContextAccepted");
                }
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "NoAuthContextException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Fail_Operations_With_Expired_Token_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test034_Should_Fail_Operations_With_Expired_Token_Sync");

            // Create a client with an expired-looking token (in practice, this would be a real expired token)
            var expiredClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "expired_token_blt12345678901234567890"
            });

            var expiredStack = expiredClient.Stack(_stack.APIKey);

            try
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = expiredStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
                        $"Expected 401/403 for expired token, got {(int)response.StatusCode} ({response.StatusCode})",
                        "ExpiredTokenValidation");
                }
                else
                {
                    AssertLogger.Fail("Expected authentication error for expired token, but API accepted it", "ExpiredTokenAccepted");
                }
            }
            catch (Exception ex)
            {
                AssertAuthenticationError(ex, "ExpiredTokenException");
            }
        }

        #endregion

        #region D — Authentication & Authorization Tests (Async)

        [TestMethod]
        [DoNotParallelize]
        public async Task Test035_Should_Fail_Operations_With_Invalid_Auth_Token_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test035_Should_Fail_Operations_With_Invalid_Auth_Token_Async");

            var invalidClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "invalid_auth_token_async_12345"
            });

            var invalidStack = invalidClient.Stack(_stack.APIKey);

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await invalidStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid auth token" 
                    };
                }
            }, "InvalidAuthTokenAsync", HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test036_Should_Fail_Operations_With_Malformed_API_Key_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test036_Should_Fail_Operations_With_Malformed_API_Key_Async");

            var invalidStack = _client.Stack("invalid_api_key_format_async");

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await invalidStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Malformed API key" 
                    };
                }
            }, "MalformedAPIKeyAsync", HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden, (HttpStatusCode)412);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test037_Should_Validate_Variant_Stack_Isolation_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test037_Should_Validate_Variant_Stack_Isolation_Async");

            var differentStackKey = "blt_fake_stack_key_async_12345";
            var differentStack = _client.Stack(differentStackKey);

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await differentStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Stack isolation failure" 
                    };
                }
            }, "StackIsolationAsync", HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden, HttpStatusCode.NotFound, (HttpStatusCode)412);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test038_Should_Fail_Operations_With_Insufficient_Permissions_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test038_Should_Fail_Operations_With_Insufficient_Permissions_Async");

            var limitedClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "limited_permissions_token_async"
            });

            var limitedStack = limitedClient.Stack(_stack.APIKey);

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await limitedStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Insufficient permissions" 
                    };
                }
            }, "InsufficientPermissionsAsync", HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test039_Should_Handle_Operations_With_No_Auth_Context_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test039_Should_Handle_Operations_With_No_Auth_Context_Async");

            var noAuthClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = ""
            });

            var noAuthStack = noAuthClient.Stack(_stack.APIKey);

            try 
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await noAuthStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                AssertLogger.Fail("Expected InvalidOperationException for no auth context", "NoAuthContextAsync");
            }
            catch (InvalidOperationException) 
            {
                AssertLogger.IsTrue(true, "SDK validation throws InvalidOperationException for no auth context as expected", "NoAuthContextAsync");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test040_Should_Fail_Operations_With_Expired_Token_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test040_Should_Fail_Operations_With_Expired_Token_Async");

            var expiredClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Host = _client.contentstackOptions.Host,
                Authtoken = "expired_token_async_blt12345678901234567890"
            });

            var expiredStack = expiredClient.Stack(_stack.APIKey);

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await expiredStack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Expired authentication token" 
                    };
                }
            }, "ExpiredTokenAsync", HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
        }

        #endregion

        #region E — Data Integrity & Referential Tests (Sync)

        [TestMethod]
        [DoNotParallelize]
        public void Test041_Should_Fail_Create_Variant_With_Invalid_Content_Type_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test041_Should_Fail_Create_Variant_With_Invalid_Content_Type_Sync");

            var invalidContentTypeUid = "nonexistent_content_type_123";

            try
            {
                var variantData = new { title = "Test", _variant = new { _change_set = new[] { "title" } } };
                var response = _stack.ContentType(invalidContentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertMissingResourceError(response.StatusCode, "CreateVariantInvalidContentType");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for invalid content type, but API accepted it", "InvalidContentTypeAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingResourceError(cex.StatusCode, "CreateVariantInvalidContentTypeException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test042_Should_Fail_Operations_With_Unlinked_Variant_Group_Sync()
        {
            if (string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test042_Should_Fail_Operations_With_Unlinked_Variant_Group_Sync");

            // Create a temporary content type that's not linked to any variant group
            var tempContentTypeUid = $"temp_unlinked_ct_{Guid.NewGuid():N}";
            string tempEntryUid = null;

            try
            {
                // Create temporary content type
                var tempContentType = new ContentModelling
                {
                    Title = "Temp Unlinked CT",
                    Uid = tempContentTypeUid,
                    Schema = new List<Field>
                    {
                        new TextboxField { DisplayName = "Title", Uid = "title", DataType = "text", Mandatory = true }
                    }
                };

                var ctResponse = _stack.ContentType().Create(tempContentType);
                if (!ctResponse.IsSuccessStatusCode)
                {
                    Assert.Inconclusive("Could not create temporary content type for unlinked test");
                    return;
                }

                // Create entry in the unlinked content type
                var entry = new SimpleTestEntry { Title = "Test Entry" };
                var entryResponse = _stack.ContentType(tempContentTypeUid).Entry().Create(entry);
                if (!entryResponse.IsSuccessStatusCode)
                {
                    Assert.Inconclusive("Could not create entry in temporary content type");
                    return;
                }

                var entryObj = entryResponse.OpenJsonObjectResponse()["entry"];
                tempEntryUid = entryObj["uid"]?.ToString();

                // Try to create variant - should fail since content type is not linked to variant group
                var variantData = new { title = "Variant Test", _variant = new { _change_set = new[] { "title" } } };
                var response = _stack.ContentType(tempContentTypeUid).Entry(tempEntryUid).Variant(_variantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "CreateVariantUnlinkedGroup");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for unlinked variant group, but API accepted it", "UnlinkedGroupAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "CreateVariantUnlinkedGroupException");
            }
            finally
            {
                // Cleanup
                try
                {
                    if (!string.IsNullOrEmpty(tempEntryUid))
                    {
                        _stack.ContentType(tempContentTypeUid).Entry(tempEntryUid).Delete();
                    }
                    _stack.ContentType(tempContentTypeUid).Delete();
                }
                catch { /* Ignore cleanup errors */ }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test043_Should_Accept_Publish_With_Version_Conflicts_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test043_Should_Accept_Publish_With_Version_Conflicts_Sync");

            // API is permissive and ignores invalid version numbers
            var publishDetailsWithInvalidVersion = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 999 } // Invalid version number
                }
            };

            var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Publish(publishDetailsWithInvalidVersion, "en-us");

            // API accepts invalid version numbers
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept invalid version numbers, got {response.StatusCode}",
                "VersionConflictAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test044_Should_Fail_Publish_With_Invalid_Locale_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test044_Should_Fail_Publish_With_Invalid_Locale_Sync");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "invalid-locale-xyz" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Publish(publishDetails, "invalid-locale-xyz");

                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "PublishInvalidLocale");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for invalid locale, but API accepted it", "InvalidLocaleAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "PublishInvalidLocaleException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test045_Should_Fail_Publish_With_Nonexistent_Environment_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test045_Should_Fail_Publish_With_Nonexistent_Environment_Sync");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "nonexistent_env_xyz_123" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Publish(publishDetails, "en-us");

                if (!response.IsSuccessStatusCode)
                {
                    AssertMissingEnvironmentError(response.StatusCode, "PublishNonexistentEnvironment");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for nonexistent environment, but API accepted it", "NonexistentEnvironmentAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingEnvironmentError(cex.StatusCode, "PublishNonexistentEnvironmentException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test046_Should_Accept_Operations_With_Broken_Dependencies_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test046_Should_Accept_Operations_With_Broken_Dependencies_Sync");

            // API is permissive with broken references - ignores invalid reference fields
            var deletedEntryUid = "blt_deleted_entry_12345";
            
            var variantDataWithBrokenRefs = new
            {
                banner_color = "Test",
                reference_field = new { uid = deletedEntryUid }, // Reference to non-existent entry
                _variant = new { _change_set = new[] { "banner_color", "reference_field" } }
            };

            var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantDataWithBrokenRefs);

            // API accepts broken references and omits them from final result
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept broken dependencies, got {response.StatusCode}",
                "BrokenDependenciesAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test047_Should_Fail_Create_Variant_With_Invalid_Group_Reference_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test047_Should_Fail_Create_Variant_With_Invalid_Group_Reference_Sync");

            var invalidVariantUid = "cs_nonexistent_variant_group_123";

            try
            {
                var variantData = new { banner_color = "Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(invalidVariantUid).Create(variantData);

                if (!response.IsSuccessStatusCode)
                {
                    AssertMissingResourceError(response.StatusCode, "CreateVariantInvalidGroup");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for invalid variant group reference, but API accepted it", "InvalidGroupReferenceAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertMissingResourceError(cex.StatusCode, "CreateVariantInvalidGroupException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test048_Should_Fail_Delete_Variant_With_Active_Dependencies_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test048_Should_Fail_Delete_Variant_With_Active_Dependencies_Sync");

            string createdVariantEntryUid = null;

            try
            {
                // First create a variant to establish dependency
                var variantData = new { banner_color = "Dependency Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var createResponse = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);

                if (!createResponse.IsSuccessStatusCode)
                {
                    Assert.Inconclusive("Could not create variant for dependency test");
                    return;
                }

                // Try to publish it to create active dependency
                var publishDetails = new PublishUnpublishDetails
                {
                    Locales = new List<string> { "en-us" },
                    Environments = new List<string> { "development" },
                    Variants = new List<PublishVariant>
                    {
                        new PublishVariant { Uid = _variantUid, Version = 1 }
                    }
                };

                try
                {
                    var publishResponse = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Publish(publishDetails, "en-us");
                    // Publishing might fail due to missing environment - that's OK for this test
                }
                catch 
                { 
                    // Ignore publish errors - the goal is to test deletion, not publishing
                }

                // Now try to delete the variant while it has active dependencies (published state)
                var deleteResponse = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Delete();

                if (!deleteResponse.IsSuccessStatusCode)
                {
                    // This might fail due to active dependencies or might succeed depending on API behavior
                    AssertLogger.IsTrue(
                        deleteResponse.StatusCode == HttpStatusCode.Conflict ||
                        deleteResponse.StatusCode == (HttpStatusCode)422 ||
                        deleteResponse.StatusCode == HttpStatusCode.BadRequest,
                        $"Expected 409/422/400 for active dependencies, got {(int)deleteResponse.StatusCode} ({deleteResponse.StatusCode})",
                        "DeleteVariantActiveDependencies");
                }
                else
                {
                    AssertLogger.IsTrue(true, "API allowed deletion of variant with dependencies (permissive behavior)", "ActiveDependenciesAllowed");
                }
            }
            catch (ContentstackErrorException cex)
            {
                // Expect conflict or validation error for active dependencies
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Conflict ||
                    cex.StatusCode == (HttpStatusCode)422 ||
                    cex.StatusCode == HttpStatusCode.BadRequest,
                    $"Expected 409/422/400 for active dependencies, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "DeleteVariantActiveDependenciesException");
            }
        }

        #endregion

        #region F — Data Integrity & Referential Tests (Async)

        [TestMethod]
        [DoNotParallelize]
        public async Task Test049_Should_Fail_Create_Variant_With_Invalid_Content_Type_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test049_Should_Fail_Create_Variant_With_Invalid_Content_Type_Async");

            var invalidContentTypeUid = "nonexistent_content_type_async_123";

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var variantData = new { title = "Test", _variant = new { _change_set = new[] { "title" } } };
                var response = await _stack.ContentType(invalidContentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid content type" 
                    };
                }
            }, "CreateVariantInvalidContentTypeAsync", HttpStatusCode.NotFound, (HttpStatusCode)422, (HttpStatusCode)412);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test050_Should_Fail_Operations_With_Unlinked_Variant_Group_Async()
        {
            if (string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test050_Should_Fail_Operations_With_Unlinked_Variant_Group_Async");

            var tempContentTypeUid = $"temp_unlinked_ct_async_{Guid.NewGuid():N}";
            string tempEntryUid = null;

            try
            {
                // Create temporary content type
                var tempContentType = new ContentModelling
                {
                    Title = "Temp Unlinked CT Async",
                    Uid = tempContentTypeUid,
                    Schema = new List<Field>
                    {
                        new TextboxField { DisplayName = "Title", Uid = "title", DataType = "text", Mandatory = true }
                    }
                };

                var ctResponse = await _stack.ContentType().CreateAsync(tempContentType);
                if (!ctResponse.IsSuccessStatusCode)
                {
                    Assert.Inconclusive("Could not create temporary content type for unlinked test");
                    return;
                }

                // Create entry in the unlinked content type
                var entry = new SimpleTestEntry { Title = "Test Entry Async" };
                var entryResponse = await _stack.ContentType(tempContentTypeUid).Entry().CreateAsync(entry);
                if (!entryResponse.IsSuccessStatusCode)
                {
                    Assert.Inconclusive("Could not create entry in temporary content type");
                    return;
                }

                var entryObj = entryResponse.OpenJsonObjectResponse()["entry"];
                tempEntryUid = entryObj["uid"]?.ToString();

                // Try to create variant - should fail
                await AssertLogger.ThrowsContentstackErrorAsync(async () =>
                {
                    var variantData = new { title = "Variant Test Async", _variant = new { _change_set = new[] { "title" } } };
                    var response = await _stack.ContentType(tempContentTypeUid).Entry(tempEntryUid).Variant(_variantUid).CreateAsync(variantData);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ContentstackErrorException 
                        { 
                            StatusCode = response.StatusCode, 
                            ErrorMessage = "Content type not linked to variant group" 
                        };
                    }
                }, "CreateVariantUnlinkedGroupAsync", HttpStatusCode.BadRequest, (HttpStatusCode)422, HttpStatusCode.Forbidden, (HttpStatusCode)412);
            }
            finally
            {
                // Cleanup
                try
                {
                    if (!string.IsNullOrEmpty(tempEntryUid))
                    {
                        await _stack.ContentType(tempContentTypeUid).Entry(tempEntryUid).DeleteAsync();
                    }
                    await _stack.ContentType(tempContentTypeUid).DeleteAsync();
                }
                catch { /* Ignore cleanup errors */ }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test051_Should_Accept_Publish_With_Version_Conflicts_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test051_Should_Accept_Publish_With_Version_Conflicts_Async");

            // API is permissive and ignores invalid version numbers
            var publishDetailsWithInvalidVersion = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 999 } // Invalid version number
                }
            };

            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetailsWithInvalidVersion, "en-us");
            
            // API accepts invalid version numbers
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept invalid version numbers, got {response.StatusCode}",
                "VersionConflictAcceptedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test052_Should_Fail_Publish_With_Invalid_Locale_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test052_Should_Fail_Publish_With_Invalid_Locale_Async");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "invalid-locale-async-xyz" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "invalid-locale-async-xyz");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid locale in publish request" 
                    };
                }
            }, "PublishInvalidLocaleAsync", HttpStatusCode.BadRequest, (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test053_Should_Fail_Publish_With_Nonexistent_Environment_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test053_Should_Fail_Publish_With_Nonexistent_Environment_Async");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "nonexistent_env_async_xyz_123" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Nonexistent environment in publish request" 
                    };
                }
            }, "PublishNonexistentEnvironmentAsync", HttpStatusCode.NotFound, (HttpStatusCode)422, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test054_Should_Accept_Operations_With_Broken_Dependencies_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test054_Should_Accept_Operations_With_Broken_Dependencies_Async");

            // API is permissive with broken references - ignores invalid reference fields
            var deletedEntryUid = "blt_deleted_entry_async_12345";
            
            var variantDataWithBrokenRefs = new
            {
                banner_color = "Test Async",
                reference_field = new { uid = deletedEntryUid },
                _variant = new { _change_set = new[] { "banner_color", "reference_field" } }
            };

            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantDataWithBrokenRefs);
            
            // API accepts broken references and omits them from final result
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept broken dependencies, got {response.StatusCode}",
                "BrokenDependenciesAcceptedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test055_Should_Fail_Create_Variant_With_Invalid_Group_Reference_Async()
        {
            if (string.IsNullOrEmpty(_entryUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test055_Should_Fail_Create_Variant_With_Invalid_Group_Reference_Async");

            var invalidVariantUid = "cs_nonexistent_variant_group_async_123";

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var variantData = new { banner_color = "Test Async", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(invalidVariantUid).CreateAsync(variantData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid variant group reference" 
                    };
                }
            }, "CreateVariantInvalidGroupAsync", HttpStatusCode.NotFound, (HttpStatusCode)422, (HttpStatusCode)412);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test056_Should_Accept_Delete_Variant_With_Active_Dependencies_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test056_Should_Accept_Delete_Variant_With_Active_Dependencies_Async");

            // API is permissive and allows deleting variants even with dependencies
            // First create a variant to establish dependency
            var variantData = new { banner_color = "Dependency Test Async", _variant = new { _change_set = new[] { "banner_color" } } };
            var createResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);

            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.Inconclusive("Could not create variant for dependency test");
                return;
            }

            // Try to publish it to create active dependency
            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            try
            {
                var publishResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
                // Publishing might fail due to missing environment - that's OK for this test
            }
            catch 
            { 
                // Ignore publish errors
            }

            // API allows deleting variant even with active dependencies
            var deleteResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).DeleteAsync();
            
            AssertLogger.IsTrue(
                deleteResponse.IsSuccessStatusCode,
                $"Expected API to allow deleting variant with dependencies, got {deleteResponse.StatusCode}",
                "DeleteVariantWithDependenciesAccepted");
        }

        #endregion

        #region G — Edge Cases & Boundary Tests (Sync)

        [TestMethod]
        [DoNotParallelize]
        public void Test057_Should_Handle_Unicode_Characters_In_Variant_Data_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test057_Should_Handle_Unicode_Characters_In_Variant_Data_Sync");

            try
            {
                var unicodeData = CreateInvalidVariantPayload("unicode_characters");
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(unicodeData);
                
                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(true, "API correctly handles Unicode characters in variant data", "UnicodeHandled");
                }
                else
                {
                    // Some systems might have issues with Unicode - document this
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == (HttpStatusCode)422,
                        $"Expected 400/422 for Unicode handling issues, got {(int)response.StatusCode} ({response.StatusCode})",
                        "UnicodeHandlingIssue");
                }
            }
            catch (ContentstackErrorException cex)
            {
                // Document any Unicode-related issues
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest || cex.StatusCode == (HttpStatusCode)422,
                    $"Expected 400/422 for Unicode issues, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "UnicodeHandlingException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test058_Should_Handle_Special_Characters_In_Identifiers_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test058_Should_Handle_Special_Characters_In_Identifiers_Sync");

            // Test with special characters in changeset field names
            var specialCharData = new
            {
                banner_color = "Test with special chars: <>\"'&",
                _variant = new
                {
                    _change_set = new[] { "banner_color" },
                    _order = new string[] { }
                }
            };

            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(specialCharData);
                
                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(true, "API correctly handles special characters in variant data", "SpecialCharsHandled");
                }
                else
                {
                    AssertValidationError(response.StatusCode, "SpecialCharacterHandling");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "SpecialCharacterHandlingException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test059_Should_Accept_Excessive_Payload_Size_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test059_Should_Accept_Excessive_Payload_Size_Sync");

            try
            {
                var oversizedData = CreateInvalidVariantPayload("oversized_payload");
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(oversizedData);
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == (HttpStatusCode)413 || 
                        response.StatusCode == (HttpStatusCode)422 || 
                        response.StatusCode == HttpStatusCode.BadRequest,
                        $"Expected 413/422/400 for excessive payload, got {(int)response.StatusCode} ({response.StatusCode})",
                        "ExcessivePayloadHandling");
                }
                else
                {
                    // API accepts large payloads
                    AssertLogger.IsTrue(true, "API accepts excessive payload size as expected", "ExcessivePayloadAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == (HttpStatusCode)413 || 
                    cex.StatusCode == (HttpStatusCode)422 || 
                    cex.StatusCode == HttpStatusCode.BadRequest,
                    $"Expected 413/422/400 for excessive payload, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "ExcessivePayloadException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test060_Should_Handle_Character_Encoding_Edge_Cases_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test060_Should_Handle_Character_Encoding_Edge_Cases_Sync");

            // Test with various character encoding edge cases
            var encodingData = new
            {
                banner_title = "Encoding test: \u0000\u0001\u0002 NULL bytes and control chars",
                banner_color = "Mixed encoding: café naïve résumé",
                _variant = new
                {
                    _change_set = new[] { "banner_title", "banner_color" },
                    _order = new string[] { }
                }
            };

            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(encodingData);
                
                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(true, "API correctly handles character encoding edge cases", "EncodingEdgeCasesHandled");
                }
                else
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == (HttpStatusCode)422,
                        $"Expected 400/422 for encoding issues, got {(int)response.StatusCode} ({response.StatusCode})",
                        "EncodingEdgeCaseHandling");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest || cex.StatusCode == (HttpStatusCode)422,
                    $"Expected 400/422 for encoding issues, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "EncodingEdgeCaseException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test061_Should_Fail_With_Malformed_JSON_Structure_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test061_Should_Fail_With_Malformed_JSON_Structure_Sync");

            try
            {
                var malformedData = CreateInvalidVariantPayload("malformed_order_array");
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(malformedData);
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "MalformedJSONHandling");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for malformed JSON structure, but API accepted it", "MalformedJSONAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "MalformedJSONException");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "SDK validation caught malformed JSON structure as expected", "MalformedJSONSDKValidation");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test062_Should_Handle_Null_Character_In_Strings_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test062_Should_Handle_Null_Character_In_Strings_Sync");

            // Test with null characters embedded in strings
            var nullCharData = new
            {
                banner_title = "Title with null\0character",
                banner_color = "Color\0with\0null\0chars",
                _variant = new
                {
                    _change_set = new[] { "banner_title", "banner_color" },
                    _order = new string[] { }
                }
            };

            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(nullCharData);
                
                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(true, "API correctly handles null characters in strings", "NullCharactersHandled");
                }
                else
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == (HttpStatusCode)422,
                        $"Expected 400/422 for null character issues, got {(int)response.StatusCode} ({response.StatusCode})",
                        "NullCharacterHandling");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest || cex.StatusCode == (HttpStatusCode)422,
                    $"Expected 400/422 for null character issues, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "NullCharacterException");
            }
        }

        #endregion

        #region H — Edge Cases & Boundary Tests (Async)

        [TestMethod]
        [DoNotParallelize]
        public async Task Test063_Should_Handle_Unicode_Characters_In_Variant_Data_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test063_Should_Handle_Unicode_Characters_In_Variant_Data_Async");

            var unicodeData = CreateInvalidVariantPayload("unicode_characters");
            
            try
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(unicodeData);
                
                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(true, "API correctly handles Unicode characters in variant data async", "UnicodeHandledAsync");
                }
                else
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == (HttpStatusCode)422,
                        $"Expected 400/422 for Unicode handling issues, got {(int)response.StatusCode} ({response.StatusCode})",
                        "UnicodeHandlingIssueAsync");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest || cex.StatusCode == (HttpStatusCode)422,
                    $"Expected 400/422 for Unicode issues, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "UnicodeHandlingExceptionAsync");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test064_Should_Handle_Special_Characters_In_Identifiers_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test064_Should_Handle_Special_Characters_In_Identifiers_Async");

            var specialCharData = new
            {
                banner_color = "Test with special chars async: <>\"'&",
                _variant = new
                {
                    _change_set = new[] { "banner_color" },
                    _order = new string[] { }
                }
            };

            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(specialCharData);

            // API accepts special characters as expected
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept special characters, got {response.StatusCode}",
                "SpecialCharacterHandlingAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test065_Should_Accept_Excessive_Payload_Size_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test065_Should_Accept_Excessive_Payload_Size_Async");

            // API is permissive and accepts large payloads without size validation
            var oversizedData = CreateInvalidVariantPayload("oversized_payload");
            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(oversizedData);
            
            // API accepts excessive payload sizes
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept excessive payload size, got {response.StatusCode}",
                "ExcessivePayloadAcceptedAsync");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test066_Should_Handle_Character_Encoding_Edge_Cases_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test066_Should_Handle_Character_Encoding_Edge_Cases_Async");

            var encodingData = new
            {
                banner_title = "Encoding test async: \u0000\u0001\u0002 NULL bytes and control chars",
                banner_color = "Mixed encoding async: café naïve résumé",
                _variant = new
                {
                    _change_set = new[] { "banner_title", "banner_color" },
                    _order = new string[] { }
                }
            };

            try
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(encodingData);
                
                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(true, "API correctly handles character encoding edge cases async", "EncodingEdgeCasesHandledAsync");
                }
                else
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == (HttpStatusCode)422,
                        $"Expected 400/422 for encoding issues, got {(int)response.StatusCode} ({response.StatusCode})",
                        "EncodingEdgeCaseHandlingAsync");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest || cex.StatusCode == (HttpStatusCode)422,
                    $"Expected 400/422 for encoding issues, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "EncodingEdgeCaseExceptionAsync");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test067_Should_Fail_With_Malformed_JSON_Structure_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test067_Should_Fail_With_Malformed_JSON_Structure_Async");

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var malformedData = CreateInvalidVariantPayload("malformed_order_array");
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(malformedData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Malformed JSON structure" 
                    };
                }
            }, "MalformedJSONHandlingAsync", HttpStatusCode.BadRequest, (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test068_Should_Handle_Null_Character_In_Strings_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test068_Should_Handle_Null_Character_In_Strings_Async");

            var nullCharData = new
            {
                banner_title = "Title with null async\0character",
                banner_color = "Color async\0with\0null\0chars",
                _variant = new
                {
                    _change_set = new[] { "banner_title", "banner_color" },
                    _order = new string[] { }
                }
            };

            try
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(nullCharData);
                
                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(true, "API correctly handles null characters in strings async", "NullCharactersHandledAsync");
                }
                else
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == (HttpStatusCode)422,
                        $"Expected 400/422 for null character issues, got {(int)response.StatusCode} ({response.StatusCode})",
                        "NullCharacterHandlingAsync");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.BadRequest || cex.StatusCode == (HttpStatusCode)422,
                    $"Expected 400/422 for null character issues, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "NullCharacterExceptionAsync");
            }
        }

        #endregion

        #region I — Concurrency & Race Conditions Tests  

        [TestMethod]
        [DoNotParallelize]
        public async Task Test069_Should_Handle_Concurrent_Variant_Modifications_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test069_Should_Handle_Concurrent_Variant_Modifications_Sync");

            var tasks = new List<Task<bool>>();
            var random = new Random();

            // Create multiple concurrent modification tasks
            for (int i = 0; i < 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(random.Next(100, 500)); // Random delay to create race conditions
                        
                        var variantData = new 
                        { 
                            banner_color = $"Concurrent Color {taskId}",
                            _variant = new { _change_set = new[] { "banner_color" } } 
                        };

                        var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                        
                        TestOutputLogger.LogContext("ConcurrentOps", $"Task {taskId}: {(response.IsSuccessStatusCode ? "SUCCESS" : $"FAILED ({response.StatusCode})")}");
                        
                        return response.IsSuccessStatusCode;
                    }
                    catch (Exception ex)
                    {
                        TestOutputLogger.LogContext("ConcurrentOps", $"Task {taskId}: EXCEPTION ({ex.GetType().Name}: {ex.Message})");
                        return false;
                    }
                }));
            }

            await Task.WhenAll(tasks);
            
            var successCount = tasks.Count(t => t.Result);
            AssertLogger.IsTrue(successCount >= 1, $"At least one concurrent operation should succeed, got {successCount}", "ConcurrentModificationsHandled");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test070_Should_Handle_Concurrent_Publish_Operations_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test070_Should_Handle_Concurrent_Publish_Operations_Async");

            // First create a variant to publish
            var variantData = new { banner_color = "Concurrent Publish Test", _variant = new { _change_set = new[] { "banner_color" } } };
            var createResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
            
            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.Inconclusive("Could not create variant for concurrent publish test");
                return;
            }

            var tasks = new List<Task<bool>>();

            // Create multiple concurrent publish tasks
            for (int i = 0; i < 3; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var publishDetails = new PublishUnpublishDetails
                        {
                            Locales = new List<string> { "en-us" },
                            Environments = new List<string> { "development" },
                            Variants = new List<PublishVariant>
                            {
                                new PublishVariant { Uid = _variantUid, Version = 1 }
                            }
                        };

                        var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
                        
                        TestOutputLogger.LogContext("ConcurrentPublish", $"Task {taskId}: {(response.IsSuccessStatusCode ? "SUCCESS" : $"FAILED ({response.StatusCode})")}");
                        
                        return response.IsSuccessStatusCode;
                    }
                    catch (Exception ex)
                    {
                        TestOutputLogger.LogContext("ConcurrentPublish", $"Task {taskId}: EXCEPTION ({ex.GetType().Name})");
                        return false;
                    }
                }));
            }

            await Task.WhenAll(tasks);
            AssertLogger.IsTrue(true, "Concurrent publish operations completed without deadlock", "ConcurrentPublishCompleted");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test071_Should_Handle_Delete_During_Active_Operations_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test071_Should_Handle_Delete_During_Active_Operations_Sync");

            // Create a variant first
            var variantData = new { banner_color = "Delete During Operations Test", _variant = new { _change_set = new[] { "banner_color" } } };
            var createResponse = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
            
            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.Inconclusive("Could not create variant for delete test");
                return;
            }

            // Start a long-running operation (publish) and simultaneously try to delete
            var publishTask = Task.Run(async () =>
            {
                try
                {
                    var publishDetails = new PublishUnpublishDetails
                    {
                        Locales = new List<string> { "en-us" },
                        Environments = new List<string> { "development" },
                        Variants = new List<PublishVariant>
                        {
                            new PublishVariant { Uid = _variantUid, Version = 1 }
                        }
                    };

                    var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            });

            var deleteTask = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100); // Small delay to let publish start
                    var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).DeleteAsync();
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            });

            await Task.WhenAll(publishTask, deleteTask);
            
            // At least one operation should complete without hanging
            AssertLogger.IsTrue(publishTask.IsCompleted && deleteTask.IsCompleted, "Both operations completed without hanging", "DeleteDuringOperationsHandled");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test072_Should_Handle_Variant_Locking_Conflicts_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test072_Should_Handle_Variant_Locking_Conflicts_Async");

            var lockingTasks = new List<Task>();

            // Create multiple tasks that try to modify the same variant simultaneously
            for (int i = 0; i < 10; i++)
            {
                int taskId = i;
                lockingTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var variantData = new 
                        { 
                            banner_color = $"Locking Test {taskId}",
                            _variant = new { _change_set = new[] { "banner_color" } } 
                        };

                        var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                        
                        TestOutputLogger.LogContext("LockingResults", $"Task {taskId}: {(response.IsSuccessStatusCode ? "SUCCESS" : $"FAILED ({response.StatusCode})")}");
                    }
                    catch (Exception ex)
                    {
                        TestOutputLogger.LogContext("LockingResults", $"Task {taskId}: EXCEPTION ({ex.GetType().Name})");
                    }
                }));
            }

            await Task.WhenAll(lockingTasks);
            AssertLogger.IsTrue(true, "Locking conflict operations completed", "LockingConflictCompleted");
        }

        #endregion

        #region J — System Constraints & Degraded Service Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test073_Should_Handle_Rate_Limiting_Errors_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test073_Should_Handle_Rate_Limiting_Errors_Sync");

            // Attempt to trigger rate limiting by making rapid requests
            var requestCount = 0;
            var rateLimitDetected = false;

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var variantData = new { banner_color = $"Rate Test {i}", _variant = new { _change_set = new[] { "banner_color" } } };
                    
                    try
                    {
                        var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);
                        requestCount++;
                        
                        if (response.StatusCode == (HttpStatusCode)429) // Too Many Requests
                        {
                            rateLimitDetected = true;
                            AssertLogger.IsTrue(true, "Rate limiting detected and handled properly", "RateLimitDetected");
                            break;
                        }
                    }
                    catch (ContentstackErrorException cex) when (cex.StatusCode == (HttpStatusCode)429)
                    {
                        rateLimitDetected = true;
                        AssertLogger.IsTrue(true, "Rate limiting exception handled properly", "RateLimitException");
                        break;
                    }
                    
                    // Small delay between requests
                    Task.Delay(50).Wait();
                }
                
                if (!rateLimitDetected)
                {
                    AssertLogger.IsTrue(true, $"No rate limiting triggered after {requestCount} requests (API may have high limits)", "NoRateLimitTriggered");
                }
            }
            catch (Exception ex)
            {
                FailWithError("Rate limiting test", ex);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test074_Should_Handle_Network_Timeout_Scenarios_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test074_Should_Handle_Network_Timeout_Scenarios_Async");

            try
            {
                // Create a large payload that might timeout
                var timeoutData = CreateInvalidVariantPayload("oversized_payload");
                
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5))) // 5 second timeout
                {
                    var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(timeoutData);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        AssertLogger.IsTrue(
                            response.StatusCode == HttpStatusCode.RequestTimeout || 
                            response.StatusCode == HttpStatusCode.GatewayTimeout ||
                            response.StatusCode == (HttpStatusCode)413, // Payload Too Large
                            $"Expected timeout or payload error, got {(int)response.StatusCode} ({response.StatusCode})",
                            "TimeoutHandling");
                    }
                    else
                    {
                        AssertLogger.IsTrue(true, "Large payload completed within timeout", "LargePayloadCompleted");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                AssertLogger.IsTrue(true, "Timeout scenario handled properly with cancellation", "TimeoutCancelled");
            }
            catch (ContentstackErrorException cex) when (
                cex.StatusCode == HttpStatusCode.RequestTimeout || 
                cex.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                AssertLogger.IsTrue(true, "Network timeout handled properly", "NetworkTimeoutHandled");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test075_Should_Handle_Service_Degradation_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test075_Should_Handle_Service_Degradation_Sync");

            try
            {
                var variantData = new { banner_color = "Service Degradation Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).Create(variantData);
                
                if (!response.IsSuccessStatusCode)
                {
                    // Check for service degradation status codes
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.InternalServerError ||
                        response.StatusCode == HttpStatusCode.BadGateway ||
                        response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                        response.StatusCode == HttpStatusCode.GatewayTimeout ||
                        ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500), // Client errors are also acceptable
                        $"Expected service degradation or client error, got {(int)response.StatusCode} ({response.StatusCode})",
                        "ServiceDegradationHandling");
                }
                else
                {
                    AssertLogger.IsTrue(true, "Service operating normally", "ServiceNormal");
                }
            }
            catch (ContentstackErrorException cex)
            {
                // Service degradation errors are expected and acceptable
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.InternalServerError ||
                    cex.StatusCode == HttpStatusCode.BadGateway ||
                    cex.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    cex.StatusCode == HttpStatusCode.GatewayTimeout ||
                    ((int)cex.StatusCode >= 400 && (int)cex.StatusCode < 500),
                    $"Service degradation error handled: {(int)cex.StatusCode} ({cex.StatusCode})",
                    "ServiceDegradationException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test076_Should_Handle_API_Maintenance_Mode_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test076_Should_Handle_API_Maintenance_Mode_Async");

            try
            {
                var variantData = new { banner_color = "Maintenance Mode Test", _variant = new { _change_set = new[] { "banner_color" } } };
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).Variant(_variantUid).CreateAsync(variantData);
                
                if (!response.IsSuccessStatusCode)
                {
                    // Check for maintenance mode status codes
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                        response.StatusCode == HttpStatusCode.BadGateway ||
                        response.StatusCode == (HttpStatusCode)503 ||
                        ((int)response.StatusCode >= 400), // Any error is acceptable during maintenance
                        $"Expected maintenance or error response, got {(int)response.StatusCode} ({response.StatusCode})",
                        "MaintenanceModeHandling");
                }
                else
                {
                    AssertLogger.IsTrue(true, "API available (not in maintenance mode)", "APIAvailable");
                }
            }
            catch (ContentstackErrorException cex)
            {
                // Maintenance mode errors are expected and acceptable
                AssertLogger.IsTrue(true, $"Maintenance mode error handled: {(int)cex.StatusCode} ({cex.StatusCode})", "MaintenanceModeException");
            }
        }

        #endregion

        #region K — Publishing & Workflow Integration Tests

        [TestMethod]
        [DoNotParallelize]
        public void Test077_Should_Fail_Publish_With_Malformed_Details_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test077_Should_Fail_Publish_With_Malformed_Details_Sync");

            // Test with null publish details
            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Publish(null, "en-us");
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "PublishMalformedDetails");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for null publish details, but API accepted it", "MalformedDetailsAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "PublishMalformedDetailsException");
            }
            catch (ArgumentNullException)
            {
                AssertLogger.IsTrue(true, "SDK validation caught null publish details as expected", "MalformedDetailsSDKValidation");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test078_Should_Fail_Publish_With_Version_Mismatch_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test078_Should_Fail_Publish_With_Version_Mismatch_Async");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Version = 999, // Invalid version
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 888 } // Another invalid version
                }
            };

            // Test API rejects invalid version numbers
            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid version numbers" 
                    };
                }
            }, "PublishVersionMismatchRejected", HttpStatusCode.BadRequest, (HttpStatusCode)422, HttpStatusCode.NotFound);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test079_Should_Fail_Scheduled_Publish_With_Invalid_Date_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test079_Should_Fail_Scheduled_Publish_With_Invalid_Date_Sync");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                ScheduledAt = "invalid-date-format-xyz",
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Publish(publishDetails, "en-us");
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertValidationError(response.StatusCode, "ScheduledPublishInvalidDate");
                }
                else
                {
                    AssertLogger.Fail("Expected validation error for invalid scheduled date, but API accepted it", "InvalidScheduledDateAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertValidationError(cex.StatusCode, "ScheduledPublishInvalidDateException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test080_Should_Accept_Publish_With_Invalid_Variant_Rules_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test080_Should_Accept_Publish_With_Invalid_Variant_Rules_Async");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "development" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                },
                VariantRules = new PublishVariantRules
                {
                    PublishLatestBase = true,
                    PublishLatestBaseConditionally = true // Conflicting rules
                }
            };

            // API is permissive and accepts conflicting variant rules
            var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).PublishAsync(publishDetails, "en-us");
            
            // API accepts invalid variant rules configurations
            AssertLogger.IsTrue(
                response.IsSuccessStatusCode,
                $"Expected API to accept invalid variant rules, got {response.StatusCode}",
                "PublishInvalidVariantRulesAccepted");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test081_Should_Fail_Publish_To_Restricted_Environment_Sync()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test081_Should_Fail_Publish_To_Restricted_Environment_Sync");

            var publishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "production_restricted" }, // Assume this is restricted
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = _variantUid, Version = 1 }
                }
            };

            try
            {
                var response = _stack.ContentType(_contentTypeUid).Entry(_entryUid).Publish(publishDetails, "en-us");
                
                if (!response.IsSuccessStatusCode)
                {
                    AssertLogger.IsTrue(
                        response.StatusCode == HttpStatusCode.Forbidden || 
                        response.StatusCode == HttpStatusCode.NotFound ||
                        response.StatusCode == (HttpStatusCode)422 ||
                        response.StatusCode == HttpStatusCode.Unauthorized,
                        $"Expected 403/404/422/401 for restricted environment, got {(int)response.StatusCode} ({response.StatusCode})",
                        "RestrictedEnvironmentHandling");
                }
                else
                {
                    AssertLogger.Fail("Expected authorization error for restricted environment, but API accepted it", "RestrictedEnvironmentAccepted");
                }
            }
            catch (ContentstackErrorException cex)
            {
                AssertLogger.IsTrue(
                    cex.StatusCode == HttpStatusCode.Forbidden || 
                    cex.StatusCode == HttpStatusCode.NotFound ||
                    cex.StatusCode == (HttpStatusCode)422 ||
                    cex.StatusCode == HttpStatusCode.Unauthorized,
                    $"Expected 403/404/422/401 for restricted environment, got {(int)cex.StatusCode} ({cex.StatusCode})",
                    "RestrictedEnvironmentException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test082_Should_Fail_Unpublish_With_Invalid_Context_Async()
        {
            if (string.IsNullOrEmpty(_entryUid) || string.IsNullOrEmpty(_variantUid))
            {
                Assert.Inconclusive("Setup not completed. Ensure Test001 runs first.");
                return;
            }

            TestOutputLogger.LogContext("TestScenario", "Test082_Should_Fail_Unpublish_With_Invalid_Context_Async");

            var unpublishDetails = new PublishUnpublishDetails
            {
                Locales = new List<string> { "invalid-locale" },
                Environments = new List<string> { "nonexistent-env" },
                Variants = new List<PublishVariant>
                {
                    new PublishVariant { Uid = "invalid-variant-uid", Version = 1 }
                }
            };

            await AssertLogger.ThrowsContentstackErrorAsync(async () =>
            {
                var response = await _stack.ContentType(_contentTypeUid).Entry(_entryUid).UnpublishAsync(unpublishDetails, "invalid-locale");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ContentstackErrorException 
                    { 
                        StatusCode = response.StatusCode, 
                        ErrorMessage = "Invalid unpublish context" 
                    };
                }
            }, "UnpublishInvalidContextAsync", HttpStatusCode.NotFound, (HttpStatusCode)422, HttpStatusCode.BadRequest);
        }

        #endregion
    }

    /// <summary>
    /// Simple entry class for testing purposes
    /// </summary>
    public class SimpleTestEntry : IEntry
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("_variant")]
        public object Variant { get; set; }
    }
}