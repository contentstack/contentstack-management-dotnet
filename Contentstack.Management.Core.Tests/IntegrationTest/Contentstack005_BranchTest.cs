using System;
using System.Net;
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

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);

            // Best-effort cleanup of any branches left over from a previous test run
            TryDeleteBranch(TestBranchUid);
            TryDeleteBranch(TestBranchUidAsync);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TryDeleteBranch(_createdBranchUid);
            TryDeleteBranch(_createdBranchUidAsync);

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
                ContentstackResponse response = _stack.Branch(_createdBranchUid).Fetch();
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
                ContentstackResponse response = await _stack.Branch(_createdBranchUidAsync).FetchAsync();
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
                ContentstackResponse response = _stack.Branch(_createdBranchUid).Delete(force);
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
                ContentstackResponse response = await _stack.Branch(_createdBranchUidAsync).DeleteAsync(force);
                var json = response.OpenJsonObjectResponse();
                AssertLogger.IsNotNull(json, "response");
                _createdBranchUidAsync = null;
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

        // ---- SDK validation tests (no API call) --------------------------------

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Throw_When_Query_Called_With_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Query_WithUid_Throws");
            Assert.ThrowsException<InvalidOperationException>(() =>
                _stack.Branch("some-uid").Query());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Throw_When_Fetch_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_NoUid_Throws_Sync");
            Assert.ThrowsException<ArgumentException>(() =>
                _stack.Branch().Fetch());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test014_Should_Throw_When_FetchAsync_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Fetch_NoUid_Throws_Async");
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _stack.Branch().FetchAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Throw_When_Delete_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_NoUid_Throws_Sync");
            Assert.ThrowsException<ArgumentException>(() =>
                _stack.Branch().Delete());
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test016_Should_Throw_When_DeleteAsync_Called_Without_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Delete_NoUid_Throws_Async");
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _stack.Branch().DeleteAsync());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Throw_When_Create_Called_With_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_WithUid_Throws_Sync");
            Assert.ThrowsException<InvalidOperationException>(() =>
                _stack.Branch("some-uid").Create(new BranchModel()));
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test018_Should_Throw_When_CreateAsync_Called_With_Uid()
        {
            TestOutputLogger.LogContext("TestScenario", "Branch_Create_WithUid_Throws_Async");
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
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

        // ---- Helper methods ---------------------------------------------------

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
