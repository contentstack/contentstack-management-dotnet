using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack005_BranchTest
    {
        private static ContentstackClient _client;
        private static Stack _stack;

        private static string _createdBranchUid;
        private static string _createdBranchUidAsync;

        private const string SourceBranch = "main";
        private const string TestBranchUid = "dotnet_int_br";       // max 15 chars, alphanumeric+underscore
        private const string TestBranchUidAsync = "dotnet_int_br_a"; // max 15 chars, alphanumeric+underscore
        private const string NonExistentBranchUid = "dotnet_no_br";

        // publish_config.environments: names (not UIDs) that must exist on the test stack.
        // Set these to two real environment names on the stack before running the live suite.
        private const string EnvironmentOne = "development";
        private const string EnvironmentTwo = "staging";
        private const string UnknownEnvironment = "dotnet_no_env_99999";
        private const string EnvTwoBranchUid = "dotnet_env_two";     // max 15 chars, alphanumeric+underscore
        private const string EnvNoneBranchUid = "dotnet_env_none";   // max 15 chars, alphanumeric+underscore
        private const string EnvUnknownBranchUid = "dotnet_env_unk"; // max 15 chars, alphanumeric+underscore
        private const string EnvMixBranchUid = "dotnet_env_mix";     // max 15 chars, alphanumeric+underscore

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);

            // Best-effort cleanup of any branches left over from a previous test run
            TryDeleteBranch(TestBranchUid);
            TryDeleteBranch(TestBranchUidAsync);
            TryDeleteBranch(EnvTwoBranchUid);
            TryDeleteBranch(EnvNoneBranchUid);
            TryDeleteBranch(EnvUnknownBranchUid);
            TryDeleteBranch(EnvMixBranchUid);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TryDeleteBranch(_createdBranchUid);
            TryDeleteBranch(_createdBranchUidAsync);
            TryDeleteBranch(EnvTwoBranchUid);
            TryDeleteBranch(EnvNoneBranchUid);
            TryDeleteBranch(EnvUnknownBranchUid);
            TryDeleteBranch(EnvMixBranchUid);

            try { _client?.Logout(); } catch { }
            _client = null;
        }

        private static void TryDeleteBranch(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            var force = new ParameterCollection();
            force.Add("force", true);
            try { _stack.Branch(uid).Delete(force); } catch { }
        }

        // ---- Query tests -------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Query_All_Branches()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_QueryAll_Sync");
            try
            {
                ContentstackResponse response = _stack.Branch().Query().Find();
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Query_All_Branches_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_QueryAll_Async");
            try
            {
                ContentstackResponse response = await _stack.Branch().Query().FindAsync();
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Query_Branches_With_Limit_Skip_And_IncludeCount()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_QueryWithParams_Sync");
            try
            {
                ContentstackResponse response = _stack.Branch().Query()
                    .Limit(5).Skip(0).IncludeCount().Find();
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---- Create tests ------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Create_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_Sync");
            try
            {
                var model = new BranchModel { Uid = TestBranchUid, Source = SourceBranch };
                ContentstackResponse response = _stack.Branch().Create(model);
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
                _createdBranchUid = TestBranchUid;
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Create_Branch_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_Async");
            try
            {
                var model = new BranchModel { Uid = TestBranchUidAsync, Source = SourceBranch };
                ContentstackResponse response = await _stack.Branch().CreateAsync(model);
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
                _createdBranchUidAsync = TestBranchUidAsync;
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---- Fetch tests -------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Fetch_Main_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_Main_Sync");
            try
            {
                ContentstackResponse response = _stack.Branch(SourceBranch).Fetch();
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Fetch_Main_Branch_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_Main_Async");
            try
            {
                ContentstackResponse response = await _stack.Branch(SourceBranch).FetchAsync();
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Fetch_Created_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_Created_Sync");
            if (string.IsNullOrEmpty(_createdBranchUid))
                Assert.Inconclusive("Test004 did not create branch — skipping.");
            try
            {
                ContentstackResponse response = RetryOnBranchProvisioning(() => _stack.Branch(_createdBranchUid).Fetch());
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Fetch_Created_Branch_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_Created_Async");
            if (string.IsNullOrEmpty(_createdBranchUidAsync))
                Assert.Inconclusive("Test005 did not create branch — skipping.");
            try
            {
                ContentstackResponse response = await RetryOnBranchProvisioningAsync(() => _stack.Branch(_createdBranchUidAsync).FetchAsync());
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---- Delete tests ------------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Delete_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_Sync");
            if (string.IsNullOrEmpty(_createdBranchUid))
                Assert.Inconclusive("Test004 did not create branch — skipping.");
            try
            {
                var force = new ParameterCollection();
                force.Add("force", true);
                ContentstackResponse response = RetryOnBranchProvisioning(() => _stack.Branch(_createdBranchUid).Delete(force));
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
                _createdBranchUid = null;
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test011_Should_Delete_Branch_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_Async");
            if (string.IsNullOrEmpty(_createdBranchUidAsync))
                Assert.Inconclusive("Test005 did not create branch — skipping.");
            try
            {
                var force = new ParameterCollection();
                force.Add("force", true);
                ContentstackResponse response = await RetryOnBranchProvisioningAsync(() => _stack.Branch(_createdBranchUidAsync).DeleteAsync(force));
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
                _createdBranchUidAsync = null;
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---- Branch provisioning retry helpers ---------------------------------
        // Branch creation is async server-side: POST /branches returns 201 immediately
        // with `"deleted_at": "in-progress"` while the branch is still being provisioned.
        // Fetch/Delete against a branch in that state returns a 422 "branch is not valid"
        // error until the provisioning job completes, so retry with backoff instead of
        // failing on the first attempt.

        private static bool IsBranchProvisioningError(ContentstackErrorException ex)
        {
            return ex.StatusCode == (HttpStatusCode)422 && ex.Message != null &&
                (ex.Message.Contains("Failed to fetch Branch") || ex.Message.Contains("Branch delete failed") || ex.Message.Contains("is not valid"));
        }

        private static ContentstackResponse RetryOnBranchProvisioning(Func<ContentstackResponse> action, int maxAttempts = 6, int delayMs = 1000)
        {
            for (int attempt = 1; attempt < maxAttempts; attempt++)
            {
                try
                {
                    return action();
                }
                catch (ContentstackErrorException ex) when (IsBranchProvisioningError(ex))
                {
                    Task.Delay(delayMs).Wait();
                }
            }
            return action();
        }

        private static async Task<ContentstackResponse> RetryOnBranchProvisioningAsync(Func<Task<ContentstackResponse>> action, int maxAttempts = 6, int delayMs = 1000)
        {
            for (int attempt = 1; attempt < maxAttempts; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (ContentstackErrorException ex) when (IsBranchProvisioningError(ex))
                {
                    await Task.Delay(delayMs);
                }
            }
            return await action();
        }

        // ---- SDK validation tests (no API call) --------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Throw_When_Query_Called_With_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Query_WithUid_Throws");
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                _stack.Branch("some-uid").Query());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Throw_When_Fetch_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_NoUid_Throws_Sync");
            Assert.ThrowsExactly<ArgumentException>(() =>
                _stack.Branch().Fetch());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test014_Should_Throw_When_FetchAsync_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_NoUid_Throws_Async");
            await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _stack.Branch().FetchAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Throw_When_Delete_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_NoUid_Throws_Sync");
            Assert.ThrowsExactly<ArgumentException>(() =>
                _stack.Branch().Delete());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test016_Should_Throw_When_DeleteAsync_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_NoUid_Throws_Async");
            await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _stack.Branch().DeleteAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Throw_When_Create_Called_With_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_WithUid_Throws_Sync");
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                _stack.Branch("some-uid").Create(new BranchModel()));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test018_Should_Throw_When_CreateAsync_Called_With_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_WithUid_Throws_Async");
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                _stack.Branch("some-uid").CreateAsync(new BranchModel()));
        }

        // ---- API error tests ---------------------------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Fail_Fetch_NonExistent_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_NonExistent_Sync");
            try
            {
                _stack.Branch(NonExistentBranchUid).Fetch();
                AssertLogger.Fail("Expected API error for non-existent branch fetch");
            }
            catch (ContentstackErrorException ex)
            {
                AssertBranchApiError(ex, "Branch_Fetch_NonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test020_Should_Fail_FetchAsync_NonExistent_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_NonExistent_Async");
            try
            {
                await _stack.Branch(NonExistentBranchUid).FetchAsync();
                AssertLogger.Fail("Expected API error for non-existent branch fetch");
            }
            catch (ContentstackErrorException ex)
            {
                AssertBranchApiError(ex, "Branch_FetchAsync_NonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Fail_Delete_NonExistent_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_NonExistent_Sync");
            try
            {
                _stack.Branch(NonExistentBranchUid).Delete();
                AssertLogger.Fail("Expected API error for non-existent branch delete");
            }
            catch (ContentstackErrorException ex)
            {
                AssertBranchApiError(ex, "Branch_Delete_NonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test022_Should_Fail_DeleteAsync_NonExistent_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_NonExistent_Async");
            try
            {
                await _stack.Branch(NonExistentBranchUid).DeleteAsync();
                AssertLogger.Fail("Expected API error for non-existent branch delete");
            }
            catch (ContentstackErrorException ex)
            {
                AssertBranchApiError(ex, "Branch_DeleteAsync_NonExistent");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_Fail_Create_With_Invalid_Source_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_InvalidSource_Sync");
            var model = new BranchModel
            {
                Uid = "dotnet_inv_src",
                Source = "non_existent_source_branch_99999"
            };
            try
            {
                _stack.Branch().Create(model);
                AssertLogger.Fail("Expected API error for invalid source branch");
            }
            catch (ContentstackErrorException ex)
            {
                AssertBranchApiError(ex, "Branch_Create_InvalidSource");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test024_Should_Fail_CreateAsync_With_Invalid_Source_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_InvalidSource_Async");
            var model = new BranchModel
            {
                Uid = "dotnet_inv_a",
                Source = "non_existent_source_branch_88888"
            };
            try
            {
                await _stack.Branch().CreateAsync(model);
                AssertLogger.Fail("Expected API error for invalid source branch");
            }
            catch (ContentstackErrorException ex)
            {
                AssertBranchApiError(ex, "Branch_CreateAsync_InvalidSource");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test025_Should_Fail_Create_Duplicate_Branch()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_Duplicate_Sync");
            // main always exists; creating it again must fail
            var model = new BranchModel { Uid = SourceBranch, Source = SourceBranch };
            try
            {
                _stack.Branch().Create(model);
                AssertLogger.Fail("Expected API error for duplicate branch creation");
            }
            catch (ContentstackErrorException ex)
            {
                AssertBranchApiError(ex, "Branch_Create_Duplicate");
            }
        }

        // ---- Create with publish_config.environments ---------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Create_Branch_With_Two_Environment_Names()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_TwoEnvironments");
            try
            {
                var model = NewEnvironmentBranch(EnvTwoBranchUid, EnvironmentOne, EnvironmentTwo);
                ContentstackResponse response = _stack.Branch().Create(model);
                AssertLogger.IsNotNull(response.OpenJsonObjectResponse(), "response");
                AssertEnvironmentsEqual(response, "Branch_Create_TwoEnvironments", EnvironmentOne, EnvironmentTwo);
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test027_Should_Create_Branch_With_Empty_Environment_List()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_EmptyEnvironments");
            try
            {
                var model = NewEnvironmentBranch(EnvNoneBranchUid);
                ContentstackResponse response = _stack.Branch().Create(model);
                AssertLogger.IsNotNull(response.OpenJsonObjectResponse(), "response");
                // [] must read back as an empty list (present, not null), so assert it is not null first
                AssertEnvironmentsEqual(response, "Branch_Create_EmptyEnvironments");
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Reject_Create_With_Unknown_Environment_Name()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_UnknownEnvironment");
            var model = NewEnvironmentBranch(EnvUnknownBranchUid, UnknownEnvironment);
            try
            {
                _stack.Branch().Create(model);
                AssertLogger.Fail("Expected API error for an unknown environment name");
            }
            catch (ContentstackErrorException ex)
            {
                AssertEnvironmentRejected(ex, EnvUnknownBranchUid, "Branch_Create_UnknownEnvironment");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Reject_Create_With_Mix_Of_Valid_And_Unknown_Environment_Names()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_MixedEnvironments");
            var model = NewEnvironmentBranch(EnvMixBranchUid, EnvironmentOne, UnknownEnvironment);
            try
            {
                _stack.Branch().Create(model);
                AssertLogger.Fail("Expected API error for a mix of valid and unknown environment names");
            }
            catch (ContentstackErrorException ex)
            {
                AssertEnvironmentRejected(ex, EnvMixBranchUid, "Branch_Create_MixedEnvironments");
            }
        }

        // ---- Helper methods ---------------------------------------------------

        private static BranchModel NewEnvironmentBranch(string uid, params string[] environments)
        {
            return new BranchModel
            {
                Uid = uid,
                Source = SourceBranch,
                PublishConfig = new BranchPublishConfigModel { Environments = new List<string>(environments) }
            };
        }

        private static List<string> EnvironmentsOf(ContentstackResponse response)
        {
            JsonNode node = response.OpenJsonObjectResponse()["branch"]?["publish_config"]?["environments"];
            return node?.AsArray().Select(n => n!.GetValue<string>()).ToList();
        }

        private static void AssertEnvironmentsEqual(ContentstackResponse response, string assertionName, params string[] expected)
        {
            List<string> names = EnvironmentsOf(response);
            AssertLogger.IsNotNull(names, "publish_config.environments");
            AssertLogger.IsTrue(
                names.SequenceEqual(expected),
                $"Expected environments [{string.Join(", ", expected)}] but got [{string.Join(", ", names)}]",
                assertionName);
        }

        private static void AssertEnvironmentRejected(ContentstackErrorException ex, string uid, string assertionName)
        {
            AssertLogger.IsTrue(
                ex.StatusCode == (HttpStatusCode)422,
                $"Expected 422, got {(int)ex.StatusCode} ({ex.StatusCode})",
                assertionName);
            AssertLogger.IsTrue(
                ex.Errors != null && ex.Errors.ContainsKey("publish_config.environments"),
                "Expected the API error to be keyed publish_config.environments",
                assertionName);

            // No branch may be created when the request is rejected
            bool exists;
            try
            {
                _stack.Branch(uid).Fetch();
                exists = true;
            }
            catch (ContentstackErrorException)
            {
                exists = false;
            }
            AssertLogger.IsTrue(!exists, "A rejected create must not leave a branch behind", assertionName);
        }

        private static void AssertBranchApiError(ContentstackErrorException ex, string assertionName)
        {
            AssertLogger.IsTrue(
                ex.StatusCode == HttpStatusCode.NotFound ||
                ex.StatusCode == HttpStatusCode.BadRequest ||
                ex.StatusCode == (HttpStatusCode)422 ||
                ex.StatusCode == HttpStatusCode.Conflict ||
                ex.StatusCode == HttpStatusCode.Forbidden,
                $"Expected branch API error, got {(int)ex.StatusCode} ({ex.StatusCode})",
                assertionName);
        }
    }
}
