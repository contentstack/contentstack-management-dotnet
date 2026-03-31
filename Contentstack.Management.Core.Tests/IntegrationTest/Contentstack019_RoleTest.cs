using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack019_RoleTest
    {
        /// <summary>
        /// UID that should not exist on any stack (for negative-path tests).
        /// </summary>
        private const string NonExistentRoleUid = "blt0000000000000000";

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

        /// <summary>
        /// Minimal role payload: branch rule on default branch "main".
        /// </summary>
        private static RoleModel BuildMinimalRoleModel(string uniqueName)
        {
            return new RoleModel
            {
                Name = uniqueName,
                Description = "Integration test role",
                DeployContent = true,
                Rules = new List<Rule>
                {
                    new BranchRules
                    {
                        Branches = new List<string> { "main" }
                    }
                }
            };
        }

        private static string ParseRoleUid(ContentstackResponse response)
        {
            var jo = response.OpenJObjectResponse();
            return jo?["role"]?["uid"]?.ToString();
        }

        private void SafeDelete(string roleUid)
        {
            if (string.IsNullOrEmpty(roleUid))
            {
                return;
            }

            try
            {
                _stack.Role(roleUid).Delete();
            }
            catch
            {
                // Best-effort cleanup; ignore if already deleted or API error
            }
        }

        private static bool RolesArrayContainsUid(JArray roles, string uid)
        {
            if (roles == null || string.IsNullOrEmpty(uid))
            {
                return false;
            }

            return roles.Any(r => r["uid"]?.ToString() == uid);
        }

        #region A — Sync happy path

        [TestMethod]
        public void Test001_Should_Create_Role_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_Role_Sync");
            string roleUid = null;
            string name = $"role_sync_create_{Guid.NewGuid():N}";
            try
            {
                var model = BuildMinimalRoleModel(name);
                ContentstackResponse response = _stack.Role().Create(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "Create role should succeed", "CreateSyncSuccess");
                roleUid = ParseRoleUid(response);
                AssertLogger.IsNotNull(roleUid, "role uid");

                var jo = response.OpenJObjectResponse();
                AssertLogger.AreEqual(name, jo["role"]?["name"]?.ToString(), "Response name should match", "RoleName");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public void Test002_Should_Fetch_Role_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Fetch_Role_Sync");
            string roleUid = null;
            string name = $"role_sync_fetch_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = _stack.Role().Create(BuildMinimalRoleModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForFetch");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                ContentstackResponse fetchResponse = _stack.Role(roleUid).Fetch();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "Fetch should succeed", "FetchSyncSuccess");

                var role = fetchResponse.OpenJObjectResponse()?["role"];
                AssertLogger.AreEqual(name, role?["name"]?.ToString(), "Fetched name should match", "FetchedName");
                AssertLogger.AreEqual(roleUid, role?["uid"]?.ToString(), "Fetched uid should match", "FetchedUid");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public void Test003_Should_Query_Roles_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Query_Roles_Sync");
            string roleUid = null;
            string name = $"role_sync_query_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = _stack.Role().Create(BuildMinimalRoleModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForQuery");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                ContentstackResponse queryResponse = _stack.Role().Query().Find();
                AssertLogger.IsTrue(queryResponse.IsSuccessStatusCode, "Query Find should succeed", "QueryFindSuccess");

                var roles = queryResponse.OpenJObjectResponse()?["roles"] as JArray;
                AssertLogger.IsNotNull(roles, "roles array");
                AssertLogger.IsTrue(
                    RolesArrayContainsUid(roles, roleUid),
                    "Query result should contain created role uid",
                    "ContainsUid");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public void Test004_Should_Update_Role_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Update_Role_Sync");
            string roleUid = null;
            string originalName = $"role_sync_update_{Guid.NewGuid():N}";
            string updatedName = $"{originalName}_updated";
            try
            {
                ContentstackResponse createResponse = _stack.Role().Create(BuildMinimalRoleModel(originalName));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForUpdate");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                var updateModel = BuildMinimalRoleModel(updatedName);
                ContentstackResponse updateResponse = _stack.Role(roleUid).Update(updateModel);
                AssertLogger.IsTrue(updateResponse.IsSuccessStatusCode, "Update should succeed", "UpdateSyncSuccess");

                ContentstackResponse fetchResponse = _stack.Role(roleUid).Fetch();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "Fetch after update should succeed", "FetchAfterUpdate");
                var role = fetchResponse.OpenJObjectResponse()?["role"];
                AssertLogger.AreEqual(updatedName, role?["name"]?.ToString(), "Name should reflect update", "UpdatedName");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public void Test005_Should_Delete_Role_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Delete_Role_Sync");
            string roleUid = null;
            string name = $"role_sync_delete_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = _stack.Role().Create(BuildMinimalRoleModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForDelete");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                ContentstackResponse deleteResponse = _stack.Role(roleUid).Delete();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete should succeed", "DeleteSyncSuccess");

                AssertLogger.ThrowsContentstackError(
                    () => _stack.Role(roleUid).Fetch(),
                    "FetchAfterDelete",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);

                roleUid = null;
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        #endregion

        #region B — Async happy path

        [TestMethod]
        public async Task Test006_Should_Create_Role_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Create_Role_Async");
            string roleUid = null;
            string name = $"role_async_create_{Guid.NewGuid():N}";
            try
            {
                var model = BuildMinimalRoleModel(name);
                ContentstackResponse response = await _stack.Role().CreateAsync(model);

                AssertLogger.IsTrue(response.IsSuccessStatusCode, "CreateAsync should succeed", "CreateAsyncSuccess");
                roleUid = ParseRoleUid(response);
                AssertLogger.IsNotNull(roleUid, "role uid");

                var jo = response.OpenJObjectResponse();
                AssertLogger.AreEqual(name, jo["role"]?["name"]?.ToString(), "Response name should match", "RoleName");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public async Task Test007_Should_Fetch_Role_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Fetch_Role_Async");
            string roleUid = null;
            string name = $"role_async_fetch_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = await _stack.Role().CreateAsync(BuildMinimalRoleModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForFetchAsync");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                ContentstackResponse fetchResponse = await _stack.Role(roleUid).FetchAsync();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "FetchAsync should succeed", "FetchAsyncSuccess");

                var role = fetchResponse.OpenJObjectResponse()?["role"];
                AssertLogger.AreEqual(name, role?["name"]?.ToString(), "Fetched name should match", "FetchedName");
                AssertLogger.AreEqual(roleUid, role?["uid"]?.ToString(), "Fetched uid should match", "FetchedUid");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public async Task Test008_Should_Query_Roles_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Query_Roles_Async");
            string roleUid = null;
            string name = $"role_async_query_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = await _stack.Role().CreateAsync(BuildMinimalRoleModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForQueryAsync");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                ContentstackResponse queryResponse = await _stack.Role().Query().FindAsync();
                AssertLogger.IsTrue(queryResponse.IsSuccessStatusCode, "Query FindAsync should succeed", "QueryFindAsyncSuccess");

                var roles = queryResponse.OpenJObjectResponse()?["roles"] as JArray;
                AssertLogger.IsNotNull(roles, "roles array");
                AssertLogger.IsTrue(
                    RolesArrayContainsUid(roles, roleUid),
                    "Query result should contain created role uid",
                    "ContainsUid");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public async Task Test009_Should_Update_Role_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Update_Role_Async");
            string roleUid = null;
            string originalName = $"role_async_update_{Guid.NewGuid():N}";
            string updatedName = $"{originalName}_updated";
            try
            {
                ContentstackResponse createResponse = await _stack.Role().CreateAsync(BuildMinimalRoleModel(originalName));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForUpdateAsync");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                var updateModel = BuildMinimalRoleModel(updatedName);
                ContentstackResponse updateResponse = await _stack.Role(roleUid).UpdateAsync(updateModel);
                AssertLogger.IsTrue(updateResponse.IsSuccessStatusCode, "UpdateAsync should succeed", "UpdateAsyncSuccess");

                ContentstackResponse fetchResponse = await _stack.Role(roleUid).FetchAsync();
                AssertLogger.IsTrue(fetchResponse.IsSuccessStatusCode, "FetchAsync after update should succeed", "FetchAsyncAfterUpdate");
                var role = fetchResponse.OpenJObjectResponse()?["role"];
                AssertLogger.AreEqual(updatedName, role?["name"]?.ToString(), "Name should reflect update", "UpdatedName");
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        [TestMethod]
        public async Task Test010_Should_Delete_Role_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test010_Should_Delete_Role_Async");
            string roleUid = null;
            string name = $"role_async_delete_{Guid.NewGuid():N}";
            try
            {
                ContentstackResponse createResponse = await _stack.Role().CreateAsync(BuildMinimalRoleModel(name));
                AssertLogger.IsTrue(createResponse.IsSuccessStatusCode, "Create should succeed", "CreateForDeleteAsync");
                roleUid = ParseRoleUid(createResponse);
                AssertLogger.IsNotNull(roleUid, "uid after create");

                ContentstackResponse deleteResponse = await _stack.Role(roleUid).DeleteAsync();
                AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode, "DeleteAsync should succeed", "DeleteAsyncSuccess");

                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.Role(roleUid).FetchAsync(),
                    "FetchAsyncAfterDelete",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);

                roleUid = null;
            }
            finally
            {
                SafeDelete(roleUid);
            }
        }

        #endregion

        #region C — Sync negative path

        [TestMethod]
        public void Test011_Should_Fail_Fetch_NonExistent_Role_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Fail_Fetch_NonExistent_Role_Sync");
            AssertLogger.ThrowsContentstackError(
                () => _stack.Role(NonExistentRoleUid).Fetch(),
                "FetchNonExistentSync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test012_Should_Fail_Update_NonExistent_Role_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test012_Should_Fail_Update_NonExistent_Role_Sync");
            var model = BuildMinimalRoleModel($"role_nonexistent_update_{Guid.NewGuid():N}");
            AssertLogger.ThrowsContentstackError(
                () => _stack.Role(NonExistentRoleUid).Update(model),
                "UpdateNonExistentSync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public void Test013_Should_Fail_Delete_NonExistent_Role_Sync()
        {
            TestOutputLogger.LogContext("TestScenario", "Test013_Should_Fail_Delete_NonExistent_Role_Sync");
            AssertLogger.ThrowsContentstackError(
                () => _stack.Role(NonExistentRoleUid).Delete(),
                "DeleteNonExistentSync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        #endregion

        #region D — Async negative path

        [TestMethod]
        public async Task Test014_Should_Fail_Fetch_NonExistent_Role_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test014_Should_Fail_Fetch_NonExistent_Role_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Role(NonExistentRoleUid).FetchAsync(),
                "FetchNonExistentAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public async Task Test015_Should_Fail_Update_NonExistent_Role_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test015_Should_Fail_Update_NonExistent_Role_Async");
            var model = BuildMinimalRoleModel($"role_nonexistent_update_async_{Guid.NewGuid():N}");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Role(NonExistentRoleUid).UpdateAsync(model),
                "UpdateNonExistentAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        public async Task Test016_Should_Fail_Delete_NonExistent_Role_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test016_Should_Fail_Delete_NonExistent_Role_Async");
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.Role(NonExistentRoleUid).DeleteAsync(),
                "DeleteNonExistentAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        #endregion
    }
}
