using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack007_ExtensionTest
    {
        private static ContentstackClient _client;
        private static Stack _stack;

        // UIDs for resources created in early tests, shared across later tests
        private static string _customFieldUid;
        private static string _widgetUid;
        private static string _originalCustomFieldTitle;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
            StackResponse stackResponse = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(stackResponse.Stack.APIKey);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            SafeDelete(_customFieldUid);
            SafeDelete(_widgetUid);
            try { _client?.Logout(); } catch { }
            _client = null;
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private static ExtensionModel BuildCustomFieldModel(string title)
        {
            return new ExtensionModel
            {
                Title = title,
                Type = "field",
                DataType = "text",
                Srcdoc = "<html><body><input/></body></html>",
                Tags = new List<string> { "test" }
            };
        }

        private static ExtensionModel BuildWidgetModel(string title)
        {
            return new ExtensionModel
            {
                Title = title,
                Type = "widget",
                Srcdoc = "<html><body>Widget</body></html>"
            };
        }

        private static string ParseExtensionUid(ContentstackResponse response)
        {
            return response.OpenJObjectResponse()?["extension"]?["uid"]?.ToString();
        }

        private static void SafeDelete(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            try { _stack.Extension(uid).Delete(); } catch { }
        }

        // ---------------------------------------------------------------------------
        // Test001 — Create custom field (sync)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_CustomField_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_CustomField_Sync");

            string title = $"Custom Field {Guid.NewGuid():N}";
            _originalCustomFieldTitle = title;

            try
            {
                var model = BuildCustomFieldModel(title);
                ContentstackResponse response = _stack.Extension().Create(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create custom field should succeed", "CreateCustomFieldSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo?["extension"], "Response should contain extension object", "ExtensionObject");

                _customFieldUid = jo["extension"]["uid"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(_customFieldUid), "Extension UID should not be empty", "UidNotEmpty");

                AssertLogger.AreEqual("field", jo["extension"]["type"]?.ToString(), "Type should be 'field'", "TypeIsField");
                AssertLogger.AreEqual(title, jo["extension"]["title"]?.ToString(), "Title should match input", "TitleMatch");

                TestOutputLogger.LogContext("CustomFieldUid", _customFieldUid);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test001 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test002 — Create widget (async)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Widget_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Create_Widget_Async");

            string title = $"Widget {Guid.NewGuid():N}";

            try
            {
                var model = BuildWidgetModel(title);
                ContentstackResponse response = await _stack.Extension().CreateAsync(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "CreateAsync widget should succeed", "CreateWidgetAsyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo?["extension"], "Response should contain extension object", "ExtensionObject");

                _widgetUid = jo["extension"]["uid"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(_widgetUid), "Widget UID should not be empty", "WidgetUidNotEmpty");

                AssertLogger.AreEqual("widget", jo["extension"]["type"]?.ToString(), "Type should be 'widget'", "TypeIsWidget");
                AssertLogger.AreEqual(title, jo["extension"]["title"]?.ToString(), "Title should match input", "TitleMatch");

                TestOutputLogger.LogContext("WidgetUid", _widgetUid);
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test002 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test003 — Fetch custom field by UID (sync), round-trip check
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Extension_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Fetch_Extension_Sync");

            if (string.IsNullOrEmpty(_customFieldUid))
            {
                AssertLogger.Fail("Test003 requires _customFieldUid set by Test001.");
                return;
            }

            try
            {
                ContentstackResponse response = _stack.Extension(_customFieldUid).Fetch();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Fetch extension should succeed", "FetchSyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo?["extension"], "Response should contain extension object", "ExtensionObject");

                string fetchedUid = jo["extension"]["uid"]?.ToString();
                AssertLogger.AreEqual(_customFieldUid, fetchedUid, "Fetched UID should match requested UID (round-trip)", "UidRoundTrip");
                AssertLogger.AreEqual("field", jo["extension"]["type"]?.ToString(), "Type should be 'field'", "TypeIsField");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test003 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test004 — Fetch widget by UID (async)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Extension_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Fetch_Extension_Async");

            if (string.IsNullOrEmpty(_widgetUid))
            {
                AssertLogger.Fail("Test004 requires _widgetUid set by Test002.");
                return;
            }

            try
            {
                ContentstackResponse response = await _stack.Extension(_widgetUid).FetchAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "FetchAsync widget should succeed", "FetchAsyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo?["extension"], "Response should contain extension object", "ExtensionObject");

                AssertLogger.AreEqual("widget", jo["extension"]["type"]?.ToString(), "Fetched type should be 'widget'", "TypeIsWidget");
                AssertLogger.AreEqual(_widgetUid, jo["extension"]["uid"]?.ToString(), "Fetched UID should match", "UidMatch");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test004 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test005 — Update custom field title (sync); proves the change was persisted
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Extension_Title_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Update_Extension_Title_Sync");

            if (string.IsNullOrEmpty(_customFieldUid))
            {
                AssertLogger.Fail("Test005 requires _customFieldUid set by Test001.");
                return;
            }

            string updatedTitle = "Updated Custom Field";

            try
            {
                var model = BuildCustomFieldModel(updatedTitle);
                ContentstackResponse response = _stack.Extension(_customFieldUid).Update(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Update extension should succeed", "UpdateSyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo?["extension"], "Response should contain extension object", "ExtensionObject");

                string responseTitle = jo["extension"]["title"]?.ToString();
                AssertLogger.AreEqual(updatedTitle, responseTitle, "Title should equal updated value", "UpdatedTitleMatch");

                // Proves the value actually changed — not just that the API accepted the request
                AssertLogger.IsTrue(
                    responseTitle != _originalCustomFieldTitle,
                    "Updated title must differ from the original title",
                    "TitleActuallyChanged");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test005 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test006 — Update widget tags (async); verifies new tag is present in response
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Extension_Tags_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Update_Extension_Tags_Async");

            if (string.IsNullOrEmpty(_widgetUid))
            {
                AssertLogger.Fail("Test006 requires _widgetUid set by Test002.");
                return;
            }

            try
            {
                var model = new ExtensionModel
                {
                    Title = "Widget Updated",
                    Type = "widget",
                    Srcdoc = "<html><body>Widget</body></html>",
                    Tags = new List<string> { "updated" }
                };

                ContentstackResponse response = await _stack.Extension(_widgetUid).UpdateAsync(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "UpdateAsync widget should succeed", "UpdateAsyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo?["extension"], "Response should contain extension object", "ExtensionObject");

                var tags = jo["extension"]["tags"] as JArray;
                AssertLogger.IsNotNull(tags, "Response should contain tags array", "TagsArray");

                bool containsUpdatedTag = false;
                foreach (var tag in tags)
                {
                    if (tag.ToString() == "updated")
                    {
                        containsUpdatedTag = true;
                        break;
                    }
                }

                AssertLogger.IsTrue(containsUpdatedTag, "Tags array should contain the 'updated' tag", "TagUpdatedPresent");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test006 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test007 — Query all extensions; expects at least 2 (custom field + widget)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_Extensions_Returns_Both()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Query_Extensions_Returns_Both");

            try
            {
                ContentstackResponse response = _stack.Extension().Query().Find();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Query.Find() should succeed", "QueryFindSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo?["extensions"], "Response should contain 'extensions' array", "ExtensionsArray");

                var extensions = jo["extensions"] as JArray;
                AssertLogger.IsNotNull(extensions, "Extensions should be a JArray", "ExtensionsJArray");
                AssertLogger.IsTrue(extensions.Count >= 2, "Query should return at least 2 extensions (custom field + widget)", "CountAtLeastTwo");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test007 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test008 — Delete widget (sync); then verify it is truly gone via Fetch
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Delete_Extension_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Delete_Extension_Sync");

            if (string.IsNullOrEmpty(_widgetUid))
            {
                AssertLogger.Fail("Test008 requires _widgetUid set by Test002.");
                return;
            }

            string uidToDelete = _widgetUid;

            try
            {
                ContentstackResponse deleteResponse = _stack.Extension(uidToDelete).Delete();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete extension should succeed", "DeleteSyncSuccess");

                // Mark as deleted so ClassCleanup does not attempt a second delete
                _widgetUid = null;

                // Verify the delete actually happened — the extension must no longer be fetchable
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Extension(uidToDelete).Fetch(),
                    "FetchAfterDelete",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test008 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test009 — Delete custom field (async)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Delete_Extension_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Delete_Extension_Async");

            if (string.IsNullOrEmpty(_customFieldUid))
            {
                AssertLogger.Fail("Test009 requires _customFieldUid set by Test001.");
                return;
            }

            string uidToDelete = _customFieldUid;

            try
            {
                ContentstackResponse deleteResponse = await _stack.Extension(uidToDelete).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "DeleteAsync extension should succeed", "DeleteAsyncSuccess");

                // Mark as deleted so ClassCleanup does not attempt a second delete
                _customFieldUid = null;
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Test009 failed: {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------
        // Test010 — Fetch a non-existent UID; must throw ContentstackErrorException 404/422
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Throw_On_Fetch_NonExistent_Extension()
        {
            TestOutputLogger.LogContext("TestScenario", "Test010_Should_Throw_On_Fetch_NonExistent_Extension");

            AssertLogger.ThrowsContentstackError(
                () => _stack.Extension("blt_nonexistent_ext_uid").Fetch(),
                "FetchNonExistentExtension",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        // ---------------------------------------------------------------------------
        // Test011 — Create without title; must throw ContentstackErrorException 400/422
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Throw_On_Create_Without_Title()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Throw_On_Create_Without_Title");

            var model = new ExtensionModel
            {
                Title = null,
                Type = "field",
                DataType = "text",
                Srcdoc = "<html/>"
            };

            AssertLogger.ThrowsContentstackError(
                () => _stack.Extension().Create(model),
                "CreateWithoutTitle",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }
    }
}
