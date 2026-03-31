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
using Newtonsoft.Json.Linq;

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
            var jo = response.OpenJObjectResponse();
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

        private static bool EnvironmentsArrayContainsName(JArray environments, string name)
        {
            if (environments == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            return environments.Any(e => e["name"]?.ToString() == name);
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

                var jo = response.OpenJObjectResponse();
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

                string expectedUid = createResponse.OpenJObjectResponse()?["environment"]?["uid"]?.ToString();

                ContentstackResponse fetchResponse = _stack.Environment(name).Fetch();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "Fetch should succeed", "FetchSyncSuccess");

                var env = fetchResponse.OpenJObjectResponse()?["environment"];
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

                var environments = queryResponse.OpenJObjectResponse()?["environments"] as JArray;
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
                var env = fetchResponse.OpenJObjectResponse()?["environment"];
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

                var jo = response.OpenJObjectResponse();
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

                string expectedUid = createResponse.OpenJObjectResponse()?["environment"]?["uid"]?.ToString();

                ContentstackResponse fetchResponse = await _stack.Environment(name).FetchAsync();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "FetchAsync should succeed", "FetchAsyncSuccess");

                var env = fetchResponse.OpenJObjectResponse()?["environment"];
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

                var environments = queryResponse.OpenJObjectResponse()?["environments"] as JArray;
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
                var env = fetchResponse.OpenJObjectResponse()?["environment"];
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
    }
}
