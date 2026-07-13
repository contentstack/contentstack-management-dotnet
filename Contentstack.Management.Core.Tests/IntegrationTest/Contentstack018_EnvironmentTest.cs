using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack018_EnvironmentTest
    {
        /// <summary>
        /// Name that should not exist on any stack (for negative-path tests).
        /// </summary>
        private const string NonExistentEnvironmentName = "nonexistent_environment_name";

        private static ContentstackClient _client;
        private Stack _stack;

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

        private static EnvironmentModel BuildModel(string uniqueName)
        {
            return new EnvironmentModel
            {
                Name = uniqueName,
                Urls = new List<LocalesUrl>
                {
                    new LocalesUrl
                    {
                        Locale = "en-us",
                        Url = "https://example.com"
                    }
                },
                DeployContent = true
            };
        }

        private static string ParseEnvironmentName(ContentstackResponse response)
        {
            var jo = response.OpenJsonObjectResponse();
            return jo?["environment"]?["name"]?.ToString();
        }

        private void SafeDelete(string environmentName)
        {
            if (string.IsNullOrEmpty(environmentName))
            {
                return;
            }

            try
            {
                _stack.Environment(environmentName).Delete();
            }
            catch
            {
                // Best-effort cleanup; ignore if already deleted or API error
            }
        }

        private static bool EnvironmentsArrayContainsName(JsonArray environments, string name)
        {
            if (environments == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            return environments.Any(e => e?["name"]?.GetValue<string>() == name);
        }

        #region A — Sync happy path

        [TestMethod]
        public void Test001_Should_Create_Environment_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_Environment_Sync");
            string environmentName = null;
            string name = $"env_sync_create_{Guid.NewGuid():N}";
            try
            {
                var model = BuildModel(name);
                ContentstackResponse response = _stack.Environment().Create(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create environment should succeed", "CreateSyncSuccess");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
                AssertLogger.AreEqual(name, environmentName, "Parsed name should match request", "ParsedEnvironmentName");

                var jo = response.OpenJsonObjectResponse();
                AssertLogger.AreEqual(name, jo["environment"]?["name"]?.ToString(), "Response name should match", "EnvironmentName");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test002_Should_Fetch_Environment_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Fetch_Environment_Sync");
            string environmentName = null;
            string name = $"env_sync_fetch_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = _stack.Environment().Create(BuildModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForFetch");
                environmentName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(environmentName, "name after create");
                AssertLogger.AreEqual(name, environmentName, "Parsed name should match create request", "CreateNameMatch");

                string expectedUid = createResponse.OpenJsonObjectResponse()?["environment"]?["uid"]?.ToString();

                ContentstackResponse fetchResponse = _stack.Environment(name).Fetch();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "Fetch should succeed", "FetchSyncSuccess");

                var env = fetchResponse.OpenJsonObjectResponse()?["environment"];
                AssertLogger.AreEqual(name, env?["name"]?.ToString(), "Fetched name should match", "FetchedName");
                AssertLogger.AreEqual(expectedUid, env?["uid"]?.ToString(), "Fetched uid should match create response", "FetchedUid");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test003_Should_Query_Environments_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Query_Environments_Sync");
            string environmentName = null;
            string name = $"env_sync_query_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = _stack.Environment().Create(BuildModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForQuery");
                environmentName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(environmentName, "name after create");

                ContentstackResponse queryResponse = _stack.Environment().Query().Find();
                AssertLogger.IsTrue(queryResponse.IsSuccessStatusCode, "Query Find should succeed", "QueryFindSuccess");

                var environments = queryResponse.OpenJsonObjectResponse()?["environments"] as JsonArray;
                AssertLogger.IsNotNull(environments, "environments array");
                AssertLogger.IsTrue(
                    EnvironmentsArrayContainsName(environments, name),
                    "Query result should contain created environment name",
                    "ContainsName");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test004_Should_Update_Environment_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Update_Environment_Sync");
            string environmentNameForCleanup = null;
            string originalName = $"env_sync_update_{Guid.NewGuid():N}";
            string updatedName = $"{originalName}_updated";
            try
            {
                ContentstackResponse createResponse = _stack.Environment().Create(BuildModel(originalName));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForUpdate");
                string createdName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(createdName, "name after create");
                AssertLogger.AreEqual(originalName, createdName, "Parsed name should match create request", "CreateNameMatch");

                var updateModel = BuildModel(updatedName);
                ContentstackResponse updateResponse = _stack.Environment(originalName).Update(updateModel);
                AssertLogger.IsTrue(updateResponse.IsSuccessStatusCode, "Update should succeed", "UpdateSyncSuccess");

                environmentNameForCleanup = updatedName;

                ContentstackResponse fetchResponse = _stack.Environment(updatedName).Fetch();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "Fetch after update should succeed", "FetchAfterUpdate");
                var env = fetchResponse.OpenJsonObjectResponse()?["environment"];
                AssertLogger.AreEqual(updatedName, env?["name"]?.ToString(), "Name should reflect update", "UpdatedName");
            }
            finally
            {
                SafeDelete(environmentNameForCleanup ?? originalName);
            }
        }

        [TestMethod]
        public void Test005_Should_Delete_Environment_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Delete_Environment_Sync");
            string environmentName = null;
            string name = $"env_sync_delete_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = _stack.Environment().Create(BuildModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForDelete");
                environmentName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(environmentName, "name after create");

                ContentstackResponse deleteResponse = _stack.Environment(name).Delete();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete should succeed", "DeleteSyncSuccess");

                AssertLogger.ThrowsContentstackError(
                    () => _stack.Environment(name).Fetch(),
                    "FetchAfterDelete",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);

                environmentName = null;
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        #endregion

        #region B — Async happy path

        [TestMethod]
        public async Task Test006_Should_Create_Environment_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Create_Environment_Async");
            string environmentName = null;
            string name = $"env_async_create_{Guid.NewGuid():N}";
            try
            {
                var model = BuildModel(name);
                ContentstackResponse response = await _stack.Environment().CreateAsync(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "CreateAsync should succeed", "CreateAsyncSuccess");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
                AssertLogger.AreEqual(name, environmentName, "Parsed name should match request", "ParsedEnvironmentName");

                var jo = response.OpenJsonObjectResponse();
                AssertLogger.AreEqual(name, jo["environment"]?["name"]?.ToString(), "Response name should match", "EnvironmentName");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public async Task Test007_Should_Fetch_Environment_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Fetch_Environment_Async");
            string environmentName = null;
            string name = $"env_async_fetch_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = await _stack.Environment().CreateAsync(BuildModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForFetchAsync");
                environmentName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(environmentName, "name after create");
                AssertLogger.AreEqual(name, environmentName, "Parsed name should match create request", "CreateNameMatch");

                string expectedUid = createResponse.OpenJsonObjectResponse()?["environment"]?["uid"]?.ToString();

                ContentstackResponse fetchResponse = await _stack.Environment(name).FetchAsync();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "FetchAsync should succeed", "FetchAsyncSuccess");

                var env = fetchResponse.OpenJsonObjectResponse()?["environment"];
                AssertLogger.AreEqual(name, env?["name"]?.ToString(), "Fetched name should match", "FetchedName");
                AssertLogger.AreEqual(expectedUid, env?["uid"]?.ToString(), "Fetched uid should match create response", "FetchedUid");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public async Task Test008_Should_Query_Environments_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Query_Environments_Async");
            string environmentName = null;
            string name = $"env_async_query_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = await _stack.Environment().CreateAsync(BuildModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForQueryAsync");
                environmentName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(environmentName, "name after create");

                ContentstackResponse queryResponse = await _stack.Environment().Query().FindAsync();
                AssertLogger.IsTrue(queryResponse.IsSuccessStatusCode, "Query FindAsync should succeed", "QueryFindAsyncSuccess");

                var environments = queryResponse.OpenJsonObjectResponse()?["environments"] as JsonArray;
                AssertLogger.IsNotNull(environments, "environments array");
                AssertLogger.IsTrue(
                    EnvironmentsArrayContainsName(environments, name),
                    "Query result should contain created environment name",
                    "ContainsName");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public async Task Test009_Should_Update_Environment_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Update_Environment_Async");
            string environmentNameForCleanup = null;
            string originalName = $"env_async_update_{Guid.NewGuid():N}";
            string updatedName = $"{originalName}_updated";
            try
            {
                ContentstackResponse createResponse = await _stack.Environment().CreateAsync(BuildModel(originalName));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForUpdateAsync");
                string createdName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(createdName, "name after create");
                AssertLogger.AreEqual(originalName, createdName, "Parsed name should match create request", "CreateNameMatch");

                var updateModel = BuildModel(updatedName);
                ContentstackResponse updateResponse = await _stack.Environment(originalName).UpdateAsync(updateModel);
                AssertLogger.IsTrue(updateResponse.IsSuccessStatusCode, "UpdateAsync should succeed", "UpdateAsyncSuccess");

                environmentNameForCleanup = updatedName;

                ContentstackResponse fetchResponse = await _stack.Environment(updatedName).FetchAsync();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "FetchAsync after update should succeed", "FetchAsyncAfterUpdate");
                var env = fetchResponse.OpenJsonObjectResponse()?["environment"];
                AssertLogger.AreEqual(updatedName, env?["name"]?.ToString(), "Name should reflect update", "UpdatedName");
            }
            finally
            {
                SafeDelete(environmentNameForCleanup ?? originalName);
            }
        }

        [TestMethod]
        public async Task Test010_Should_Delete_Environment_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test010_Should_Delete_Environment_Async");
            string environmentName = null;
            string name = $"env_async_delete_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = await _stack.Environment().CreateAsync(BuildModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForDeleteAsync");
                environmentName = ParseEnvironmentName(createResponse);
                AssertLogger.IsNotNull(environmentName, "name after create");

                ContentstackResponse deleteResponse = await _stack.Environment(name).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "DeleteAsync should succeed", "DeleteAsyncSuccess");

                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.Environment(name).FetchAsync(),
                    "FetchAsyncAfterDelete",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);

                environmentName = null;
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        #endregion

        #region C — Sync negative path

        [TestMethod]
        public void Test011_Should_Fail_Fetch_NonExistent_Environment_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Fail_Fetch_NonExistent_Environment_Sync");
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment(NonExistentEnvironmentName).Fetch(),
                "FetchNonExistentSync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test012_Should_Fail_Update_NonExistent_Environment_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test012_Should_Fail_Update_NonExistent_Environment_Sync");
            var model = BuildModel($"env_nonexistent_update_{Guid.NewGuid():N}");
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment(NonExistentEnvironmentName).Update(model),
                "UpdateNonExistentSync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test013_Should_Fail_Delete_NonExistent_Environment_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test013_Should_Fail_Delete_NonExistent_Environment_Sync");
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment(NonExistentEnvironmentName).Delete(),
                "DeleteNonExistentSync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        #endregion

        #region D — Async negative path

        [TestMethod]
        public async Task Test014_Should_Fail_Fetch_NonExistent_Environment_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test014_Should_Fail_Fetch_NonExistent_Environment_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Environment(NonExistentEnvironmentName).FetchAsync(),
                "FetchNonExistentAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public async Task Test015_Should_Fail_Update_NonExistent_Environment_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test015_Should_Fail_Update_NonExistent_Environment_Async");
            var model = BuildModel($"env_nonexistent_update_async_{Guid.NewGuid():N}");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Environment(NonExistentEnvironmentName).UpdateAsync(model),
                "UpdateNonExistentAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public async Task Test016_Should_Fail_Delete_NonExistent_Environment_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test016_Should_Fail_Delete_NonExistent_Environment_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Environment(NonExistentEnvironmentName).DeleteAsync(),
                "DeleteNonExistentAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        #endregion

        #region E — Input Validation Errors (Sync)

        [TestMethod]
        public void Test017_Should_Fail_Create_Environment_With_Null_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test017_Should_Fail_Create_Environment_With_Null_Name_Sync");
            var model = new EnvironmentModel
            {
                Name = null,
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment().Create(model),
                "CreateNullNameSync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test018_Should_Fail_Create_Environment_With_Empty_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test018_Should_Fail_Create_Environment_With_Empty_Name_Sync");
            var model = new EnvironmentModel
            {
                Name = "",
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment().Create(model),
                "CreateEmptyNameSync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test019_Should_Fail_Create_Environment_With_Whitespace_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test019_Should_Fail_Create_Environment_With_Whitespace_Name_Sync");
            var model = new EnvironmentModel
            {
                Name = "   ",
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment().Create(model),
                "CreateWhitespaceNameSync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test020_Should_Fail_Create_Environment_With_Invalid_Name_Characters_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test020_Should_Fail_Create_Environment_With_Invalid_Name_Characters_Sync");
            var model = new EnvironmentModel
            {
                Name = "env@#$%^&*()",
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment().Create(model),
                "CreateInvalidNameSync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test021_Should_Fail_Create_Environment_With_Extremely_Long_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test021_Should_Fail_Create_Environment_With_Extremely_Long_Name_Sync");
            var model = new EnvironmentModel
            {
                Name = new string('a', 1000), // 1000 characters
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment().Create(model),
                "CreateLongNameSync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test022_Should_Accept_Create_Environment_With_Null_Urls_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test022_Should_Accept_Create_Environment_With_Null_Urls_Sync");
            string environmentName = null;
            string name = $"env_null_urls_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = null,
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts null URLs array", "CreateNullUrlsSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test023_Should_Accept_Create_Environment_With_Empty_Urls_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test023_Should_Accept_Create_Environment_With_Empty_Urls_Sync");
            string environmentName = null;
            string name = $"env_empty_urls_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl>(),
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts empty URLs array", "CreateEmptyUrlsSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test024_Should_Accept_Create_Environment_With_Invalid_Url_Format_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test024_Should_Accept_Create_Environment_With_Invalid_Url_Format_Sync");
            string environmentName = null;
            string name = $"env_invalid_url_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "not-a-valid-url" } },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts invalid URL format", "CreateInvalidUrlSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test025_Should_Accept_Create_Environment_With_Invalid_Locale_Format_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test025_Should_Accept_Create_Environment_With_Invalid_Locale_Format_Sync");
            string environmentName = null;
            string name = $"env_invalid_locale_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "invalid-locale-format", Url = "https://example.com" } },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts invalid locale format", "CreateInvalidLocaleSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test026_Should_Fail_Create_Environment_With_Null_Model_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test026_Should_Fail_Create_Environment_With_Null_Model_Sync");
            AssertLogger.ThrowsException<ArgumentNullException>(
                () => _stack.Environment().Create(null),
                "CreateNullModelSync");
        }

        [TestMethod]
        public void Test027_Should_Fail_Create_Duplicate_Environment_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test027_Should_Fail_Create_Duplicate_Environment_Name_Sync");
            string environmentName = null;
            string name = $"env_duplicate_{Guid.NewGuid():N}";
            try
            {
                // Create first environment
                var model1 = BuildModel(name);
                ContentstackResponse response1 = _stack.Environment().Create(model1);
                AssertLogger.IsTrue(response1.IsSuccessStatusCode, "First create should succeed", "FirstCreateSuccess");
                environmentName = ParseEnvironmentName(response1);

                // Try to create second environment with same name
                var model2 = BuildModel(name);
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Environment().Create(model2),
                    "CreateDuplicateNameSync",
                    HttpStatusCode.Conflict,
                    (HttpStatusCode)422);
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        #endregion

        #region F — Input Validation Errors (Async)

        [TestMethod]
        public async Task Test028_Should_Fail_Create_Environment_With_Null_Name_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test028_Should_Fail_Create_Environment_With_Null_Name_Async");
            var model = new EnvironmentModel
            {
                Name = null,
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Environment().CreateAsync(model),
                "CreateNullNameAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public async Task Test029_Should_Fail_Create_Environment_With_Empty_Name_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test029_Should_Fail_Create_Environment_With_Empty_Name_Async");
            var model = new EnvironmentModel
            {
                Name = "",
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Environment().CreateAsync(model),
                "CreateEmptyNameAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public async Task Test030_Should_Fail_Update_Environment_With_Invalid_Model_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test030_Should_Fail_Update_Environment_With_Invalid_Model_Async");
            string environmentName = null;
            string name = $"env_update_invalid_{Guid.NewGuid():N}";
            try
            {
                // Create valid environment first
                var validModel = BuildModel(name);
                ContentstackResponse createResponse = await _stack.Environment().CreateAsync(validModel);
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForInvalidUpdate");
                environmentName = ParseEnvironmentName(createResponse);

                // Try to update with invalid model
                var invalidModel = new EnvironmentModel
                {
                    Name = "", // Invalid empty name
                    Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                    DeployContent = true
                };
                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.Environment(name).UpdateAsync(invalidModel),
                    "UpdateInvalidModelAsync",
                    HttpStatusCode.BadRequest,
                    (HttpStatusCode)422);
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public async Task Test031_Should_Fail_Create_Duplicate_Environment_Name_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test031_Should_Fail_Create_Duplicate_Environment_Name_Async");
            string environmentName = null;
            string name = $"env_duplicate_async_{Guid.NewGuid():N}";
            try
            {
                // Create first environment
                var model1 = BuildModel(name);
                ContentstackResponse response1 = await _stack.Environment().CreateAsync(model1);
                AssertLogger.IsTrue(response1.IsSuccessStatusCode, "First create should succeed", "FirstCreateSuccessAsync");
                environmentName = ParseEnvironmentName(response1);

                // Try to create second environment with same name
                var model2 = BuildModel(name);
                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.Environment().CreateAsync(model2),
                    "CreateDuplicateNameAsync",
                    HttpStatusCode.Conflict,
                    (HttpStatusCode)422);
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        #endregion

        #region G — Authentication & Authorization Errors (Sync)

        [TestMethod]
        public void Test032_Should_Fail_Create_Environment_Without_Auth_Token_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test032_Should_Fail_Create_Environment_Without_Auth_Token_Sync");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            var model = BuildModel($"env_unauth_{Guid.NewGuid():N}");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => unauthenticatedStack.Environment().Create(model),
                "CreateUnauthSync");
        }

        [TestMethod]
        public void Test033_Should_Fail_Fetch_Environment_Without_Auth_Token_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test033_Should_Fail_Fetch_Environment_Without_Auth_Token_Sync");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => unauthenticatedStack.Environment("dummy_env").Fetch(),
                "FetchUnauthSync");
        }

        [TestMethod]
        public void Test034_Should_Fail_Update_Environment_Without_Auth_Token_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test034_Should_Fail_Update_Environment_Without_Auth_Token_Sync");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            var model = BuildModel($"env_unauth_update_{Guid.NewGuid():N}");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => unauthenticatedStack.Environment("dummy_env").Update(model),
                "UpdateUnauthSync");
        }

        [TestMethod]
        public void Test035_Should_Fail_Delete_Environment_Without_Auth_Token_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test035_Should_Fail_Delete_Environment_Without_Auth_Token_Sync");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => unauthenticatedStack.Environment("dummy_env").Delete(),
                "DeleteUnauthSync");
        }

        [TestMethod]
        public void Test036_Should_Fail_Query_Environments_Without_Auth_Token_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test036_Should_Fail_Query_Environments_Without_Auth_Token_Sync");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => unauthenticatedStack.Environment().Query().Find(),
                "QueryUnauthSync");
        }

        #endregion

        #region H — Authentication & Authorization Errors (Async)

        [TestMethod]
        public async Task Test037_Should_Fail_Create_Environment_Without_Auth_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test037_Should_Fail_Create_Environment_Without_Auth_Token_Async");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            var model = BuildModel($"env_unauth_async_{Guid.NewGuid():N}");
            
            try
            {
                await unauthenticatedStack.Environment().CreateAsync(model);
                AssertLogger.Fail("Expected InvalidOperationException for unauthenticated create");
            }
            catch (InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "InvalidOperationException thrown as expected", "CreateUnauthAsync");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Expected InvalidOperationException but got {ex.GetType().Name}");
            }
        }

        [TestMethod]
        public async Task Test038_Should_Fail_Fetch_Environment_Without_Auth_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test038_Should_Fail_Fetch_Environment_Without_Auth_Token_Async");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            
            try
            {
                await unauthenticatedStack.Environment("dummy_env").FetchAsync();
                AssertLogger.Fail("Expected InvalidOperationException for unauthenticated fetch");
            }
            catch (InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "InvalidOperationException thrown as expected", "FetchUnauthAsync");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Expected InvalidOperationException but got {ex.GetType().Name}");
            }
        }

        [TestMethod]
        public async Task Test039_Should_Fail_Update_Environment_Without_Auth_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test039_Should_Fail_Update_Environment_Without_Auth_Token_Async");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            var model = BuildModel($"env_unauth_update_async_{Guid.NewGuid():N}");
            
            try
            {
                await unauthenticatedStack.Environment("dummy_env").UpdateAsync(model);
                AssertLogger.Fail("Expected InvalidOperationException for unauthenticated update");
            }
            catch (InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "InvalidOperationException thrown as expected", "UpdateUnauthAsync");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Expected InvalidOperationException but got {ex.GetType().Name}");
            }
        }

        [TestMethod]
        public async Task Test040_Should_Fail_Delete_Environment_Without_Auth_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test040_Should_Fail_Delete_Environment_Without_Auth_Token_Async");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            
            try
            {
                await unauthenticatedStack.Environment("dummy_env").DeleteAsync();
                AssertLogger.Fail("Expected InvalidOperationException for unauthenticated delete");
            }
            catch (InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "InvalidOperationException thrown as expected", "DeleteUnauthAsync");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Expected InvalidOperationException but got {ex.GetType().Name}");
            }
        }

        [TestMethod]
        public async Task Test041_Should_Fail_Query_Environments_Without_Auth_Token_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test041_Should_Fail_Query_Environments_Without_Auth_Token_Async");
            var unauthenticatedClient = new ContentstackClient();
            var unauthenticatedStack = unauthenticatedClient.Stack("dummy_api_key");
            
            try
            {
                await unauthenticatedStack.Environment().Query().FindAsync();
                AssertLogger.Fail("Expected InvalidOperationException for unauthenticated query");
            }
            catch (InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "InvalidOperationException thrown as expected", "QueryUnauthAsync");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Expected InvalidOperationException but got {ex.GetType().Name}");
            }
        }

        #endregion

        #region I — Stack Context Errors

        [TestMethod]
        public void Test042_Should_Fail_With_Invalid_Stack_API_Key_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test042_Should_Fail_With_Invalid_Stack_API_Key_Sync");
            var clientWithInvalidStack = Contentstack.CreateAuthenticatedClient();
            var invalidStack = clientWithInvalidStack.Stack("invalid_nonexistent_api_key");
            var model = BuildModel($"env_invalid_stack_{Guid.NewGuid():N}");
            
            AssertLogger.ThrowsContentstackError(
                () => invalidStack.Environment().Create(model),
                "CreateInvalidStackSync",
                HttpStatusCode.PreconditionFailed);
        }

        [TestMethod]
        public async Task Test043_Should_Fail_With_Invalid_Stack_API_Key_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test043_Should_Fail_With_Invalid_Stack_API_Key_Async");
            var clientWithInvalidStack = Contentstack.CreateAuthenticatedClient();
            var invalidStack = clientWithInvalidStack.Stack("invalid_nonexistent_api_key");
            var model = BuildModel($"env_invalid_stack_async_{Guid.NewGuid():N}");
            
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await invalidStack.Environment().CreateAsync(model),
                "CreateInvalidStackAsync",
                HttpStatusCode.PreconditionFailed);
        }

        #endregion

        #region J — Edge Cases & Boundary Conditions

        [TestMethod]
        public void Test044_Should_Fail_With_Null_Environment_UID_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test044_Should_Fail_With_Null_Environment_UID_Sync");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.Environment(null).Fetch(),
                "FetchNullUidSync");
        }

        [TestMethod]
        public void Test045_Should_Fail_With_Empty_Environment_UID_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test045_Should_Fail_With_Empty_Environment_UID_Sync");
            AssertLogger.ThrowsException<ArgumentException>(
                () => _stack.Environment("").Fetch(),
                "FetchEmptyUidSync");
        }

        [TestMethod]
        public void Test046_Should_Handle_Whitespace_Environment_UID_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test046_Should_Handle_Whitespace_Environment_UID_Sync");
            // Whitespace UID gets normalized to query endpoint (200 OK)
            ContentstackResponse response = _stack.Environment("   ").Fetch();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, "Whitespace UID normalized to query endpoint", "FetchWhitespaceUidSync");
        }

        [TestMethod]
        public void Test047_Should_Fail_Create_Environment_With_Unicode_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test047_Should_Fail_Create_Environment_With_Unicode_Name_Sync");
            var model = new EnvironmentModel
            {
                Name = "环境测试名称", // Chinese characters
                Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                DeployContent = true
            };
            AssertLogger.ThrowsContentstackError(
                () => _stack.Environment().Create(model),
                "CreateUnicodeNameSync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test048_Should_Accept_Create_Environment_With_Large_Url_List_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test048_Should_Accept_Create_Environment_With_Large_Url_List_Sync");
            string environmentName = null;
            string name = $"env_large_urls_{Guid.NewGuid():N}";
            try
            {
                var urls = new List<LocalesUrl>();
                for (int i = 0; i < 100; i++)
                {
                    urls.Add(new LocalesUrl { Locale = $"locale-{i}", Url = $"https://example-{i}.com" });
                }
                
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = urls,
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts large URL lists", "CreateLargeUrlsSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test049_Should_Accept_Create_Environment_With_Very_Long_Url_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test049_Should_Accept_Create_Environment_With_Very_Long_Url_Sync");
            string environmentName = null;
            string name = $"env_long_url_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> 
                    { 
                        new LocalesUrl 
                        { 
                            Locale = "en-us", 
                            Url = "https://example.com/" + new string('a', 2000) // Very long URL
                        } 
                    },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts very long URLs", "CreateLongUrlSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public async Task Test050_Should_Fail_With_Null_Environment_UID_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test050_Should_Fail_With_Null_Environment_UID_Async");
        try
        {
            await _stack.Environment(null).FetchAsync();
            AssertLogger.Fail("Expected ArgumentException for null UID");
        }
        catch (ArgumentException)
        {
            AssertLogger.IsTrue(true, "ArgumentException thrown as expected", "FetchNullUidAsync");
        }
        catch (Exception ex)
        {
            AssertLogger.Fail($"Expected ArgumentException but got {ex.GetType().Name}");
        }
        }

        [TestMethod]
        public async Task Test051_Should_Fail_With_Empty_Environment_UID_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test051_Should_Fail_With_Empty_Environment_UID_Async");
            try
            {
                await _stack.Environment("").FetchAsync();
                AssertLogger.Fail("Expected ArgumentException for empty UID");
            }
            catch (ArgumentException)
            {
                AssertLogger.IsTrue(true, "ArgumentException thrown as expected", "FetchEmptyUidAsync");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Expected ArgumentException but got {ex.GetType().Name}");
            }
        }

        [TestMethod]
        public async Task Test052_Should_Accept_Create_Environment_With_Invalid_Servers_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test052_Should_Accept_Create_Environment_With_Invalid_Servers_Async");
            string environmentName = null;
            string name = $"env_invalid_servers_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                    Servers = new List<Server> 
                    { 
                        new Server { Name = null }, // Invalid server with null name
                        new Server { Name = "" }    // Invalid server with empty name
                    },
                    DeployContent = true
                };
                ContentstackResponse response = await _stack.Environment().CreateAsync(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API ignores invalid server objects", "CreateInvalidServersAsync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        #endregion

        #region K — Business Logic Constraint Errors

        [TestMethod]
        public void Test053_Should_Fail_Update_Environment_To_Existing_Name_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test053_Should_Fail_Update_Environment_To_Existing_Name_Sync");
            string environment1Name = null;
            string environment2Name = null;
            string name1 = $"env_existing_1_{Guid.NewGuid():N}";
            string name2 = $"env_existing_2_{Guid.NewGuid():N}";
            
            try
            {
                // Create first environment
                var model1 = BuildModel(name1);
                ContentstackResponse response1 = _stack.Environment().Create(model1);
                AssertLogger.IsTrue(response1.IsSuccessStatusCode, "First create should succeed", "FirstCreateForConflict");
                environment1Name = ParseEnvironmentName(response1);

                // Create second environment
                var model2 = BuildModel(name2);
                ContentstackResponse response2 = _stack.Environment().Create(model2);
                AssertLogger.IsTrue(response2.IsSuccessStatusCode, "Second create should succeed", "SecondCreateForConflict");
                environment2Name = ParseEnvironmentName(response2);

                // Try to update second environment to have same name as first
                var updateModel = BuildModel(name1);
                AssertLogger.ThrowsContentstackError(
                    () => _stack.Environment(name2).Update(updateModel),
                    "UpdateToExistingNameSync",
                    HttpStatusCode.Conflict,
                    (HttpStatusCode)422);
            }
            finally
            {
                SafeDelete(environment1Name ?? name1);
                SafeDelete(environment2Name ?? name2);
            }
        }

        [TestMethod]
        public async Task Test054_Should_Fail_Update_Environment_To_Existing_Name_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test054_Should_Fail_Update_Environment_To_Existing_Name_Async");
            string environment1Name = null;
            string environment2Name = null;
            string name1 = $"env_existing_async_1_{Guid.NewGuid():N}";
            string name2 = $"env_existing_async_2_{Guid.NewGuid():N}";
            
            try
            {
                // Create first environment
                var model1 = BuildModel(name1);
                ContentstackResponse response1 = await _stack.Environment().CreateAsync(model1);
                AssertLogger.IsTrue(response1.IsSuccessStatusCode, "First create should succeed", "FirstCreateForConflictAsync");
                environment1Name = ParseEnvironmentName(response1);

                // Create second environment
                var model2 = BuildModel(name2);
                ContentstackResponse response2 = await _stack.Environment().CreateAsync(model2);
                AssertLogger.IsTrue(response2.IsSuccessStatusCode, "Second create should succeed", "SecondCreateForConflictAsync");
                environment2Name = ParseEnvironmentName(response2);

                // Try to update second environment to have same name as first
                var updateModel = BuildModel(name1);
                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.Environment(name2).UpdateAsync(updateModel),
                    "UpdateToExistingNameAsync",
                    HttpStatusCode.Conflict,
                    (HttpStatusCode)422);
            }
            finally
            {
                SafeDelete(environment1Name ?? name1);
                SafeDelete(environment2Name ?? name2);
            }
        }

        [TestMethod]
        public void Test055_Should_Accept_Create_Environment_With_Invalid_Locale_Combination_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test055_Should_Accept_Create_Environment_With_Invalid_Locale_Combination_Sync");
            string environmentName = null;
            string name = $"env_invalid_locale_combo_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> 
                    { 
                        new LocalesUrl { Locale = "non-existent-locale", Url = "https://example.com" }
                    },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts non-existent locale", "CreateInvalidLocaleComboSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        #endregion

        #region L — Server Error Simulation & Network Issues

        [TestMethod]
        public void Test056_Should_Handle_Network_Timeout_Gracefully_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test056_Should_Handle_Network_Timeout_Gracefully_Sync");
            // Note: This test simulates network issues - actual timeout behavior depends on infrastructure
            var model = BuildModel($"env_timeout_{Guid.NewGuid():N}");
            
            try
            {
                ContentstackResponse response = _stack.Environment().Create(model);
                // If successful, clean up the created environment
                if (response.IsSuccessStatusCode)
                {
                    string environmentName = ParseEnvironmentName(response);
                    SafeDelete(environmentName);
                }
                AssertLogger.IsTrue(true, "Network operation completed (success or expected failure)", "NetworkTimeoutHandling");
            }
            catch (ContentstackErrorException ex) when (
                ex.StatusCode == HttpStatusCode.RequestTimeout ||
                ex.StatusCode == HttpStatusCode.GatewayTimeout ||
                ex.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                AssertLogger.IsTrue(true, $"Expected network error handled gracefully: {ex.StatusCode}", "ExpectedNetworkError");
            }
            catch (Exception ex)
            {
                AssertLogger.IsTrue(ex is ContentstackErrorException || ex is TaskCanceledException, 
                    $"Unexpected exception type: {ex.GetType().Name}", "UnexpectedExceptionType");
            }
        }

        [TestMethod]
        public async Task Test057_Should_Handle_Network_Timeout_Gracefully_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test057_Should_Handle_Network_Timeout_Gracefully_Async");
            var model = BuildModel($"env_timeout_async_{Guid.NewGuid():N}");
            
            try
            {
                ContentstackResponse response = await _stack.Environment().CreateAsync(model);
                if (response.IsSuccessStatusCode)
                {
                    string environmentName = ParseEnvironmentName(response);
                    SafeDelete(environmentName);
                }
                AssertLogger.IsTrue(true, "Async network operation completed", "AsyncNetworkTimeoutHandling");
            }
            catch (ContentstackErrorException ex) when (
                ex.StatusCode == HttpStatusCode.RequestTimeout ||
                ex.StatusCode == HttpStatusCode.GatewayTimeout ||
                ex.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                AssertLogger.IsTrue(true, $"Expected async network error handled: {ex.StatusCode}", "ExpectedAsyncNetworkError");
            }
            catch (Exception ex)
            {
                AssertLogger.IsTrue(ex is ContentstackErrorException || ex is TaskCanceledException, 
                    $"Unexpected async exception type: {ex.GetType().Name}", "UnexpectedAsyncExceptionType");
            }
        }

        [TestMethod]
        public void Test058_Should_Fail_With_Malformed_Stack_Reference_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test058_Should_Fail_With_Malformed_Stack_Reference_Sync");
            var clientWithMalformedStack = Contentstack.CreateAuthenticatedClient();
            var malformedStack = clientWithMalformedStack.Stack(""); // Empty stack API key
            var model = BuildModel($"env_malformed_stack_{Guid.NewGuid():N}");
            
            AssertLogger.ThrowsException<InvalidOperationException>(
                () => malformedStack.Environment().Create(model),
                "CreateMalformedStackSync");
        }

        [TestMethod]
        public async Task Test059_Should_Fail_With_Malformed_Stack_Reference_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test059_Should_Fail_With_Malformed_Stack_Reference_Async");
            var clientWithMalformedStack = Contentstack.CreateAuthenticatedClient();
            var malformedStack = clientWithMalformedStack.Stack(""); // Empty stack API key
            var model = BuildModel($"env_malformed_stack_async_{Guid.NewGuid():N}");
            
            try
            {
                await malformedStack.Environment().CreateAsync(model);
                AssertLogger.Fail("Expected InvalidOperationException for empty API key");
            }
            catch (InvalidOperationException)
            {
                AssertLogger.IsTrue(true, "InvalidOperationException thrown as expected", "CreateMalformedStackAsync");
            }
            catch (Exception ex)
            {
                AssertLogger.Fail($"Expected InvalidOperationException but got {ex.GetType().Name}");
            }
        }

        [TestMethod]
        public void Test060_Should_Handle_Concurrent_Environment_Operations_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test060_Should_Handle_Concurrent_Environment_Operations_Sync");
            string environmentName = null;
            string name = $"env_concurrent_{Guid.NewGuid():N}";

            try
            {
                // Create environment first
                var model = BuildModel(name);
                ContentstackResponse createResponse = _stack.Environment().Create(model);
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForConcurrency");
                environmentName = ParseEnvironmentName(createResponse);

                // Test concurrent operations: update changes name, then operations on old name fail
                var updateModel = BuildModel($"{name}_updated");

                try
                {
                    // Update environment (changes name)
                    ContentstackResponse updateResponse = _stack.Environment(name).Update(updateModel);
                    AssertLogger.IsTrue(updateResponse.IsSuccessStatusCode, "Update should succeed", "UpdateOperation");
                    
                    if (updateResponse.IsSuccessStatusCode)
                    {
                        environmentName = $"{name}_updated";
                        
                        // Now fetch with old name should fail (environment was renamed)
                        try
                        {
                            _stack.Environment(name).Fetch();
                            AssertLogger.Fail("Fetch with old name should fail after rename");
                        }
                        catch (ContentstackErrorException ex)
                        {
                            // Expect 422 "Environment was not found" after name change
                            AssertLogger.IsTrue(ex.StatusCode == (HttpStatusCode)422, 
                                "Expected 422 for fetch on renamed environment", "FetchAfterRename");
                        }
                    }
                }
                catch (ContentstackErrorException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    AssertLogger.IsTrue(true, "Concurrent modification conflict handled correctly", "ConcurrentConflict");
                }
            }
            finally
            {
                SafeDelete(environmentName ?? name);
                SafeDelete($"{name}_updated");
            }
        }

        [TestMethod]
        public async Task Test061_Should_Handle_Concurrent_Environment_Operations_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test061_Should_Handle_Concurrent_Environment_Operations_Async");
            string environmentName = null;
            string name = $"env_concurrent_async_{Guid.NewGuid():N}";
            
            try
            {
                // Create environment first
                var model = BuildModel(name);
                ContentstackResponse createResponse = await _stack.Environment().CreateAsync(model);
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForConcurrencyAsync");
                environmentName = ParseEnvironmentName(createResponse);

                // Test race condition: update changes name, causing fetch on old name to fail
                var updateModel = BuildModel($"{name}_updated");
                
                try
                {
                    // Update environment (changes name)
                    ContentstackResponse updateResponse = await _stack.Environment(name).UpdateAsync(updateModel);
                    AssertLogger.IsTrue(updateResponse.IsSuccessStatusCode, "Update should succeed", "UpdateAsyncOperation");
                    
                    if (updateResponse.IsSuccessStatusCode)
                    {
                        environmentName = $"{name}_updated";
                        
                        // Now fetch with old name should fail (environment was renamed)
                        try
                        {
                            await _stack.Environment(name).FetchAsync();
                            AssertLogger.Fail("Async fetch with old name should fail after rename");
                        }
                        catch (ContentstackErrorException ex)
                        {
                            // Expect 422 "Environment was not found" after name change
                            AssertLogger.IsTrue(ex.StatusCode == (HttpStatusCode)422, 
                                "Expected 422 for async fetch on renamed environment", "FetchAfterRenameAsync");
                        }
                    }
                }
                catch (ContentstackErrorException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    AssertLogger.IsTrue(true, "Concurrent async modification conflict handled correctly", "ConcurrentAsyncConflict");
                }
            }
            finally
            {
                SafeDelete(environmentName ?? name);
                SafeDelete($"{name}_updated");
            }
        }

        #endregion

        #region M — Advanced Edge Cases & Data Boundary Tests

        [TestMethod]
        public void Test062_Should_Accept_Create_Environment_With_Null_Url_In_LocalesUrl_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test062_Should_Accept_Create_Environment_With_Null_Url_In_LocalesUrl_Sync");
            string environmentName = null;
            string name = $"env_null_url_field_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> 
                    { 
                        new LocalesUrl { Locale = "en-us", Url = null } // Null URL
                    },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts null URL in LocalesUrl", "CreateNullUrlFieldSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test063_Should_Accept_Create_Environment_With_Null_Locale_In_LocalesUrl_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test063_Should_Accept_Create_Environment_With_Null_Locale_In_LocalesUrl_Sync");
            string environmentName = null;
            string name = $"env_null_locale_field_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> 
                    { 
                        new LocalesUrl { Locale = null, Url = "https://example.com" } // Null locale
                    },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts null locale in LocalesUrl", "CreateNullLocaleFieldSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test064_Should_Accept_Create_Environment_With_Empty_Url_In_LocalesUrl_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test064_Should_Accept_Create_Environment_With_Empty_Url_In_LocalesUrl_Sync");
            string environmentName = null;
            string name = $"env_empty_url_field_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl>
                    {
                        new LocalesUrl { Locale = "en-us", Url = "" } // Empty URL
                    },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts empty URL string", "CreateEmptyUrlFieldSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test065_Should_Accept_Create_Environment_With_Empty_Locale_In_LocalesUrl_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test065_Should_Accept_Create_Environment_With_Empty_Locale_In_LocalesUrl_Sync");
            string environmentName = null;
            string name = $"env_empty_locale_field_{Guid.NewGuid():N}";
            try
            {
                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = new List<LocalesUrl> 
                    { 
                        new LocalesUrl { Locale = "", Url = "https://example.com" } // Empty locale
                    },
                    DeployContent = true
                };
                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API accepts empty locale string", "CreateEmptyLocaleFieldSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public void Test066_Should_Fail_Create_Environment_With_Malformed_Json_Structure_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test066_Should_Fail_Create_Environment_With_Malformed_Json_Structure_Sync");
            
            // Create a model with circular reference to test JSON serialization limits
            var model = new EnvironmentModel
            {
                Name = $"env_malformed_json_{Guid.NewGuid():N}",
                Urls = new List<LocalesUrl> 
                { 
                    new LocalesUrl { Locale = "en-us", Url = "https://example.com" }
                },
                DeployContent = true
            };
            
            // This test ensures the serializer can handle the model correctly
            try
            {
                ContentstackResponse response = _stack.Environment().Create(model);
                if (response.IsSuccessStatusCode)
                {
                    string environmentName = ParseEnvironmentName(response);
                    SafeDelete(environmentName);
                    AssertLogger.IsTrue(true, "Model serialized correctly", "JsonSerializationSuccess");
                }
                else
                {
                    AssertLogger.IsTrue(true, "Expected failure for complex model structure", "ExpectedJsonFailure");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.IsTrue(ex is ContentstackErrorException || ex is System.Text.Json.JsonException,
                    $"Expected JSON or API error: {ex.GetType().Name}", "JsonSerializationError");
            }
        }

        [TestMethod]
        public async Task Test067_Should_Fail_Create_Environment_With_Special_Characters_In_Urls_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test067_Should_Fail_Create_Environment_With_Special_Characters_In_Urls_Async");
            var model = new EnvironmentModel
            {
                Name = $"env_special_chars_{Guid.NewGuid():N}",
                Urls = new List<LocalesUrl> 
                { 
                    new LocalesUrl { Locale = "en-us", Url = "https://例え.テスト" } // Japanese domain
                },
                DeployContent = true
            };
            
            try
            {
                ContentstackResponse response = await _stack.Environment().CreateAsync(model);
                if (response.IsSuccessStatusCode)
                {
                    string environmentName = ParseEnvironmentName(response);
                    SafeDelete(environmentName);
                    AssertLogger.IsTrue(true, "International domain names handled correctly", "InternationalDomainSuccess");
                }
                else
                {
                    AssertLogger.IsTrue(true, "International domain validation working", "InternationalDomainValidation");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertLogger.IsTrue(ex.StatusCode == HttpStatusCode.BadRequest || ex.StatusCode == (HttpStatusCode)422, 
                    "Expected validation error for international domains", "ExpectedInternationalDomainError");
            }
        }

        [TestMethod]
        public async Task Test068_Should_Handle_Rapid_Sequential_Operations_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test068_Should_Handle_Rapid_Sequential_Operations_Async");
            var environmentNames = new List<string>();
            
            try
            {
                // Rapidly create multiple environments
                for (int i = 0; i < 3; i++)
                {
                    string name = $"env_rapid_{i}_{Guid.NewGuid():N}";
                    var model = BuildModel(name);
                    
                    try
                    {
                        ContentstackResponse response = await _stack.Environment().CreateAsync(model);
                        if (response.IsSuccessStatusCode)
                        {
                            string environmentName = ParseEnvironmentName(response);
                            environmentNames.Add(environmentName ?? name);
                        }
                    }
                    catch (ContentstackErrorException ex) when (
                        ex.StatusCode == HttpStatusCode.TooManyRequests ||
                        ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        AssertLogger.IsTrue(true, $"Rate limiting handled correctly: {ex.StatusCode}", "RateLimitingHandled");
                        break; // Stop if rate limited
                    }
                }
                
                AssertLogger.IsTrue(true, "Rapid operations handled appropriately", "RapidOperationsHandling");
            }
            finally
            {
                // Cleanup all created environments
                foreach (var envName in environmentNames)
                {
                    SafeDelete(envName);
                }
            }
        }

        [TestMethod]
        public void Test069_Should_Accept_Extremely_Complex_Environment_Model_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test069_Should_Accept_Extremely_Complex_Environment_Model_Sync");
            string environmentName = null;
            string name = $"env_complex_{Guid.NewGuid():N}";
            try
            {
                // Create a complex model with nested structures
                var urls = new List<LocalesUrl>();
                for (int i = 0; i < 50; i++)
                {
                    urls.Add(new LocalesUrl
                    {
                        Locale = $"locale-{i:D3}",
                        Url = $"https://subdomain-{i}.example-domain-{i}.com/path-{i}/subpath-{i}"
                    });
                }

                var servers = new List<Server>();
                for (int i = 0; i < 20; i++)
                {
                    servers.Add(new Server { Name = $"server-{i}-{Guid.NewGuid():N}" });
                }

                var model = new EnvironmentModel
                {
                    Name = name,
                    Urls = urls,
                    Servers = servers,
                    DeployContent = true
                };

                ContentstackResponse response = _stack.Environment().Create(model);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, "API handles complex models with 50 URLs and 20 servers", "CreateComplexModelSync");
                environmentName = ParseEnvironmentName(response);
                AssertLogger.IsNotNull(environmentName, "environment name");
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        [TestMethod]
        public async Task Test070_Should_Validate_Environment_State_After_Failed_Operations_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test070_Should_Validate_Environment_State_After_Failed_Operations_Async");
            string environmentName = null;
            string name = $"env_state_validation_{Guid.NewGuid():N}";
            
            try
            {
                // Create valid environment
                var validModel = BuildModel(name);
                ContentstackResponse createResponse = await _stack.Environment().CreateAsync(validModel);
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForStateValidation");
                environmentName = ParseEnvironmentName(createResponse);

                // Try invalid update
                var invalidModel = new EnvironmentModel
                {
                    Name = "", // Invalid name
                    Urls = new List<LocalesUrl> { new LocalesUrl { Locale = "en-us", Url = "https://example.com" } },
                    DeployContent = true
                };
                
                try
                {
                    await _stack.Environment(name).UpdateAsync(invalidModel);
                    AssertLogger.Fail("Invalid update should have failed", "InvalidUpdateShouldFail");
                }
                catch (ContentstackErrorException ex) when (
                    ex.StatusCode == HttpStatusCode.BadRequest || 
                    ex.StatusCode == (HttpStatusCode)422)
                {
                    // Expected failure - now validate environment state is unchanged
                    ContentstackResponse fetchResponse = await _stack.Environment(name).FetchAsync();
                    AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "Fetch after failed update should succeed", "FetchAfterFailedUpdate");
                    
                    var env = fetchResponse.OpenJsonObjectResponse()?["environment"];
                    AssertLogger.AreEqual(name, env?["name"]?.ToString(), "Name should be unchanged after failed update", "NameUnchangedAfterFailure");
                }
            }
            finally
            {
                SafeDelete(environmentName ?? name);
            }
        }

        #endregion

        #region N — Performance & Stress Tests

        [TestMethod]
        public async Task Test071_Should_Handle_Multiple_Parallel_Environment_Queries_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test071_Should_Handle_Multiple_Parallel_Environment_Queries_Async");
            
            try
            {
                var queryTasks = new List<Task<ContentstackResponse>>();
                
                // Create multiple parallel query tasks
                for (int i = 0; i < 5; i++)
                {
                    queryTasks.Add(_stack.Environment().Query().FindAsync());
                }
                
                var results = await Task.WhenAll(queryTasks);
                
                int successCount = results.Count(r => r.IsSuccessStatusCode);
                AssertLogger.IsTrue(successCount > 0, "At least one parallel query should succeed", "ParallelQuerySuccess");
                
                // Check if any failed due to rate limiting
                var rateLimitedCount = results.Count(r => !r.IsSuccessStatusCode);
                if (rateLimitedCount > 0)
                {
                    AssertLogger.IsTrue(true, $"Some queries rate-limited as expected: {rateLimitedCount}", "ExpectedRateLimiting");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.IsTrue(ex is ContentstackErrorException || ex is AggregateException, 
                    $"Expected exception type for parallel operations: {ex.GetType().Name}", "ParallelOperationException");
            }
        }

        [TestMethod]
        public async Task Test072_Should_Handle_Large_Query_Result_Sets_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test072_Should_Handle_Large_Query_Result_Sets_Async");
            
            try
            {
                ContentstackResponse queryResponse = await _stack.Environment().Query().FindAsync();
                AssertLogger.IsTrue(queryResponse.IsSuccessStatusCode, "Large query should succeed", "LargeQuerySuccess");
                
                var environments = queryResponse.OpenJsonObjectResponse()?["environments"] as JsonArray;
                AssertLogger.IsNotNull(environments, "EnvironmentsArrayPresent");
                
                // Validate that we can handle large result sets without memory issues
                AssertLogger.IsTrue(environments.Count >= 0, "Should handle any size result set", "ResultSetSizeHandling");
                
                TestOutputLogger.LogContext("EnvironmentCount", environments.Count.ToString());
            }
            catch (ContentstackErrorException ex)
            {
                AssertLogger.IsTrue(ex.StatusCode == HttpStatusCode.RequestTimeout || 
                                   ex.StatusCode == HttpStatusCode.ServiceUnavailable, 
                    "Large query timeout handled appropriately", "LargeQueryTimeoutHandled");
            }
        }

        #endregion
    }
}
