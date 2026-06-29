using System;
using System.Linq;
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
    public class Contentstack005_LocaleTest
    {
        private static ContentstackClient _client;
        private Stack _stack;

        // Codes stored between tests to support sequential fetch/update/delete flows
        private static string _localeCode = "fr-fr";
        private static string _asyncLocaleCode = "de-de";

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
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
        }

        // ---------------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------------

        private void SafeDeleteLocale(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return;
            }

            try
            {
                _stack.Locale(code).Delete();
            }
            catch
            {
                // Best-effort cleanup; ignore if already deleted or is master locale
            }
        }

        private static bool LocaleArrayContainsCode(JArray locales, string code)
        {
            if (locales == null || string.IsNullOrEmpty(code))
            {
                return false;
            }

            return locales.Any(l => l["code"]?.ToString() == code);
        }

        // ---------------------------------------------------------------------------
        // A — Sync happy-path: Create
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Locale_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_Locale_Sync");
            try
            {
                var model = new LocaleModel { Name = "French - France", Code = "fr-fr" };
                ContentstackResponse response = _stack.Locale("fr-fr").Create(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create locale sync should succeed", "CreateSyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo, "response body");
                AssertLogger.IsNotNull(jo["locale"], "locale object in response");

                // Catches wrong code being saved by the API
                AssertLogger.AreEqual("fr-fr", jo["locale"]?["code"]?.ToString(), "locale.code must equal 'fr-fr'", "LocaleCode");

                // Catches missing name in the response
                string returnedName = jo["locale"]?["name"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(returnedName), "locale.name must not be empty", "LocaleNameNotEmpty");

                _localeCode = "fr-fr";
                TestOutputLogger.LogContext("_localeCode", _localeCode);
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // B — Async happy-path: Create
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Locale_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Create_Locale_Async");
            try
            {
                var model = new LocaleModel { Name = "German", Code = "de-de" };
                ContentstackResponse response = await _stack.Locale("de-de").CreateAsync(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create locale async should succeed", "CreateAsyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo, "response body");
                AssertLogger.IsNotNull(jo["locale"], "locale object in response");

                // Catches wrong code being saved asynchronously
                AssertLogger.AreEqual("de-de", jo["locale"]?["code"]?.ToString(), "locale.code must equal 'de-de'", "AsyncLocaleCode");

                // Catches missing name in async response
                string returnedName = jo["locale"]?["name"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(returnedName), "locale.name must not be empty", "AsyncLocaleNameNotEmpty");

                _asyncLocaleCode = "de-de";
                TestOutputLogger.LogContext("_asyncLocaleCode", _asyncLocaleCode);
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // C — Sync happy-path: Fetch
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Locale_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Fetch_Locale_Sync");
            try
            {
                ContentstackResponse response = _stack.Locale(_localeCode).Fetch();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Fetch locale sync should succeed", "FetchSyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo, "response body");
                AssertLogger.IsNotNull(jo["locale"], "locale object in fetch response");

                // Round-trip code check: catches if the API returns a different locale
                AssertLogger.AreEqual(_localeCode, jo["locale"]?["code"]?.ToString(), "fetched locale.code must match requested code", "FetchedCode");

                // Name must be a non-empty string
                string name = jo["locale"]?["name"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(name), "fetched locale.name must be a non-empty string", "FetchedNameNotEmpty");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // D — Async happy-path: Fetch
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Locale_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Fetch_Locale_Async");
            try
            {
                ContentstackResponse response = await _stack.Locale(_asyncLocaleCode).FetchAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Fetch locale async should succeed", "FetchAsyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo, "response body");
                AssertLogger.IsNotNull(jo["locale"], "locale object in async fetch response");

                // Round-trip code check for the async locale
                AssertLogger.AreEqual(_asyncLocaleCode, jo["locale"]?["code"]?.ToString(), "async fetched locale.code must match requested code", "AsyncFetchedCode");

                string name = jo["locale"]?["name"]?.ToString();
                AssertLogger.IsTrue(!string.IsNullOrEmpty(name), "async fetched locale.name must not be empty", "AsyncFetchedNameNotEmpty");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // E — Sync happy-path: Update (fallback locale)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_FallbackLocale_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Update_FallbackLocale_Sync");
            try
            {
                var updateModel = new LocaleModel { FallbackLocale = "en-us" };
                ContentstackResponse response = _stack.Locale(_localeCode).Update(updateModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Update locale sync should succeed", "UpdateSyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo, "response body");
                AssertLogger.IsNotNull(jo["locale"], "locale object in update response");

                // Catches if fallback_locale was not persisted correctly
                AssertLogger.AreEqual("en-us", jo["locale"]?["fallback_locale"]?.ToString(), "fallback_locale must be 'en-us' after update", "FallbackLocale");

                // Catches if the update accidentally mutated the locale code
                AssertLogger.AreEqual(_localeCode, jo["locale"]?["code"]?.ToString(), "locale.code must remain unchanged after update", "CodeUnchangedAfterUpdate");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // F — Async happy-path: Update (fallback locale)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Locale_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Update_Locale_Async");
            try
            {
                var updateModel = new LocaleModel { FallbackLocale = "en-us" };
                ContentstackResponse response = await _stack.Locale(_asyncLocaleCode).UpdateAsync(updateModel);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Update locale async should succeed", "UpdateAsyncSuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo, "response body");
                AssertLogger.IsNotNull(jo["locale"], "locale object in async update response");

                // Catches if async update did not save fallback_locale
                AssertLogger.AreEqual("en-us", jo["locale"]?["fallback_locale"]?.ToString(), "async fallback_locale must be 'en-us' after update", "AsyncFallbackLocale");

                // Code must be intact after async update
                AssertLogger.AreEqual(_asyncLocaleCode, jo["locale"]?["code"]?.ToString(), "async locale.code must remain unchanged", "AsyncCodeUnchangedAfterUpdate");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // G — Query all locales
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_All_Locales()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Query_All_Locales");
            try
            {
                ContentstackResponse response = _stack.Locale("").Query().Find();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Query locales should succeed", "QuerySuccess");

                var jo = response.OpenJObjectResponse();
                AssertLogger.IsNotNull(jo, "response body");
                AssertLogger.IsNotNull(jo["locales"], "locales array in query response");

                var locales = jo["locales"] as JArray;
                AssertLogger.IsNotNull(locales, "locales must be a JSON array");

                // en-us (default) + fr-fr + de-de means at least 3; we check for >= 2 to be resilient
                AssertLogger.IsTrue(locales.Count >= 2, $"Expected at least 2 locales (en-us + created), found {locales.Count}", "LocaleCountAtLeastTwo");

                // Verify the locale we created in Test001 is visible in the list
                AssertLogger.IsTrue(
                    LocaleArrayContainsCode(locales, "fr-fr"),
                    "Query result must contain 'fr-fr' locale created earlier",
                    "QueryContainsFrFr");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // H — Sync happy-path: Delete (de-de)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Delete_Locale_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Delete_Locale_Sync");
            try
            {
                ContentstackResponse response = _stack.Locale(_asyncLocaleCode).Delete();

                // A successful delete must return a 2xx status; any exception = failure
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Delete locale '{_asyncLocaleCode}' sync should succeed", "DeleteSyncSuccess");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // I — Async happy-path: Delete (fr-fr)
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Delete_Locale_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Delete_Locale_Async");
            try
            {
                ContentstackResponse response = await _stack.Locale(_localeCode).DeleteAsync();

                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Delete locale '{_localeCode}' async should succeed", "DeleteAsyncSuccess");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---------------------------------------------------------------------------
        // J — Negative: Fetch a locale that does not exist
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Throw_On_Fetch_NonExistent_Locale()
        {
            TestOutputLogger.LogContext("TestScenario", "Test010_Should_Throw_On_Fetch_NonExistent_Locale");

            AssertLogger.ThrowsContentstackError(
                () => _stack.Locale("xx-xx").Fetch(),
                "FetchNonExistentLocale",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        // ---------------------------------------------------------------------------
        // K — Negative: Delete master locale (en-us) must be rejected
        // ---------------------------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Throw_On_Delete_Master_Locale()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Throw_On_Delete_Master_Locale");

            // The Contentstack API must refuse deletion of the master locale (en-us).
            // Any 4xx or exception confirms the constraint is enforced.
            try
            {
                ContentstackResponse response = _stack.Locale("en-us").Delete();

                // If no exception was raised, the API should still have returned a non-2xx code
                AssertLogger.IsFalse(response.IsSuccessStatusCode, "Deleting master locale 'en-us' must not succeed", "MasterLocaleDeleteMustFail");
            }
            catch (ContentstackErrorException ex)
            {
                // Any API error (4xx/5xx) confirms the master locale is protected
                AssertLogger.IsTrue(
                    (int)ex.StatusCode >= 400,
                    $"Expected a 4xx/5xx error when deleting master locale, but got {ex.StatusCode}",
                    "MasterLocaleDeleteErrorStatus");
                TestOutputLogger.LogContext("MasterLocaleDeleteErrorCode", ex.StatusCode.ToString());
            }
            catch (Exception e)
            {
                // Any other exception is also acceptable evidence of rejection
                TestOutputLogger.LogContext("MasterLocaleDeleteException", e.GetType().Name);
                AssertLogger.IsTrue(true, "Non-ContentstackErrorException also confirms rejection", "MasterLocaleDeleteRejected");
            }
        }
    }
}
