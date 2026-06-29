// NOTE: The Contentstack .NET Management SDK (as of this writing) does NOT expose a Branch()
// method on the Stack class, nor does it include a Branch model in Contentstack.Management.Core.Models.
// The internal service infrastructure (InvokeSync / InvokeAsync) is also marked internal and
// cannot be called from the test project.
//
// All tests below are decorated with [Ignore] and carry a descriptive comment explaining the
// intended behaviour. When Branch API support is added to the SDK, remove the [Ignore] attribute
// and uncomment / adjust the SDK call to match the actual API surface.
//
// Intended SDK pattern (once available):
//   _stack.Branch()                    // no uid => create / query context
//   _stack.Branch(uid)                 // with uid => fetch / delete context
//   _stack.Branch().Create(model)      // POST /stacks/branches
//   _stack.Branch(uid).Fetch()         // GET  /stacks/branches/{uid}
//   _stack.Branch(uid).FetchAsync()    // async variant
//   _stack.Branch().Query().Find()     // GET  /stacks/branches
//   _stack.Branch(uid).Delete()        // DELETE /stacks/branches/{uid}

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    [DoNotParallelize]
    public class Contentstack009_BranchTest
    {
        private static ContentstackClient _client;
        private static Core.Models.Stack _stack;

        // Branch UID: "test-br-" + 8-char lowercase hex suffix derived from a Guid.
        private static readonly string _branchUid =
            "test-br-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToLowerInvariant();

        // Tracks whether Test001 successfully created the branch so ClassCleanup knows whether to attempt deletion.
        private static bool _createdBranch = false;

        // -----------------------------------------------------------------------
        // Class setup / teardown
        // -----------------------------------------------------------------------

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // If the create test succeeded and the delete test did not clean up, try to remove the branch.
            if (_createdBranch)
            {
                SafeDeleteBranch(_branchUid);
            }

            try { _client?.Logout(); } catch { }
            _client = null;
        }

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// Best-effort synchronous branch deletion used only in cleanup.
        /// Swallows all exceptions so that cleanup never fails the test run.
        /// </summary>
        private static void SafeDeleteBranch(string uid)
        {
            // SDK does not expose Branch operations yet — placeholder body only.
            // Once Branch() is available on Stack, replace the body with:
            //   _stack.Branch(uid).Delete();
            _ = uid; // suppress unused-variable warning
        }

        /// <summary>
        /// Asserts that the exception is a <see cref="ContentstackErrorException"/> with an
        /// HTTP status code that indicates the resource was not found or a validation failure.
        /// Acceptable codes: 400, 404, 409, 422.
        /// </summary>
        private static void AssertBranchErrorException(Exception ex, string context)
        {
            if (ex is ContentstackErrorException cex)
            {
                int status = (int)cex.StatusCode;
                AssertLogger.IsTrue(
                    status == 400 || status == 404 || status == 409 || status == 422,
                    $"{context}: Expected 400/404/409/422 but got {status} — {cex.Message}",
                    $"{context}_StatusCode");
            }
            else
            {
                string msg = ex.Message?.ToLowerInvariant() ?? string.Empty;
                AssertLogger.IsTrue(
                    msg.Contains("not found") || msg.Contains("404") ||
                    msg.Contains("conflict") || msg.Contains("409") ||
                    msg.Contains("invalid") || msg.Contains("400") ||
                    msg.Contains("422"),
                    $"{context}: Unexpected exception — {ex.GetType().Name}: {ex.Message}",
                    $"{context}_FallbackMessage");
            }
        }

        // -----------------------------------------------------------------------
        // Tests
        // -----------------------------------------------------------------------

        /// <summary>
        /// Creates a new branch with uid=_branchUid sourced from "main".
        /// Verifies the response contains the expected uid and source fields.
        /// Sets _createdBranch = true on success; waits 5 s for branch provisioning.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public async Task Test001_Should_Create_Branch_From_Main()
        {
            // ----- Arrange -----
            // var model = new JObject
            // {
            //     ["uid"]    = _branchUid,
            //     ["source"] = "main"
            // };

            // ----- Act -----
            // ContentstackResponse contentstackResponse = _stack.Branch().Create(model);
            // JObject response = contentstackResponse.OpenJObjectResponse();

            // ----- Assert -----
            // AssertLogger.IsNotNull(response, "Create branch response must not be null", "CreateBranchResponse");
            // AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Create branch must return 2xx", "CreateBranchStatus");
            // AssertLogger.IsNotNull(response["branch"], "Response must contain 'branch' key", "CreateBranchKey");
            // AssertLogger.AreEqual(_branchUid, response["branch"]["uid"]?.ToString(),
            //     $"Created branch uid must equal '{_branchUid}'", "CreateBranchUid");
            //
            // string source = response["branch"]["source"]?.ToString()
            //              ?? response["branch"]["uid"]?.ToString();     // some APIs echo "uid":"main"
            // AssertLogger.IsTrue(source == "main",
            //     $"Expected source 'main' but got '{source}'", "CreateBranchSource");
            //
            // _createdBranch = true;

            // Branches take 2-5 s to provision after creation.
            // await Task.Delay(5000);

            await Task.CompletedTask; // remove when implementing
            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }

        /// <summary>
        /// Fetches the branch created in Test001 (synchronous) and verifies the uid round-trips correctly.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public void Test002_Should_Fetch_Branch_Sync()
        {
            // ----- Act -----
            // ContentstackResponse contentstackResponse = _stack.Branch(_branchUid).Fetch();
            // JObject response = contentstackResponse.OpenJObjectResponse();

            // ----- Assert -----
            // AssertLogger.IsNotNull(response, "Fetch branch response must not be null", "FetchBranchResponse");
            // AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Fetch branch must return 2xx", "FetchBranchStatus");
            // AssertLogger.IsNotNull(response["branch"], "Response must contain 'branch' key", "FetchBranchKey");
            // AssertLogger.AreEqual(_branchUid, response["branch"]["uid"]?.ToString(),
            //     "Fetched branch uid must match _branchUid (round-trip check)", "FetchBranchUid");

            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }

        /// <summary>
        /// Fetches the branch created in Test001 (asynchronous) and verifies the uid round-trips correctly.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public async Task Test003_Should_Fetch_Branch_Async()
        {
            // ----- Act -----
            // ContentstackResponse contentstackResponse = await _stack.Branch(_branchUid).FetchAsync();
            // JObject response = contentstackResponse.OpenJObjectResponse();

            // ----- Assert -----
            // AssertLogger.IsNotNull(response, "FetchAsync branch response must not be null", "FetchAsyncBranchResponse");
            // AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "FetchAsync branch must return 2xx", "FetchAsyncBranchStatus");
            // AssertLogger.IsNotNull(response["branch"], "Response must contain 'branch' key", "FetchAsyncBranchKey");
            // AssertLogger.AreEqual(_branchUid, response["branch"]["uid"]?.ToString(),
            //     "Async-fetched branch uid must match _branchUid (round-trip check)", "FetchAsyncBranchUid");

            await Task.CompletedTask; // remove when implementing
            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }

        /// <summary>
        /// Queries all branches and verifies that both "main" and _branchUid appear in the list.
        /// Guards against a branch being created but not appearing in listing (eventual-consistency issue).
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public void Test004_Should_Query_Branches_Includes_Main_And_Created()
        {
            // ----- Act -----
            // ContentstackResponse contentstackResponse = _stack.Branch().Query().Find();
            // JObject response = contentstackResponse.OpenJObjectResponse();

            // ----- Assert -----
            // AssertLogger.IsNotNull(response, "Query branches response must not be null", "QueryBranchesResponse");
            // AssertLogger.IsTrue(contentstackResponse.IsSuccessStatusCode, "Query branches must return 2xx", "QueryBranchesStatus");
            // AssertLogger.IsNotNull(response["branches"], "Response must contain 'branches' key", "QueryBranchesKey");
            //
            // JArray branches = response["branches"] as JArray;
            // AssertLogger.IsNotNull(branches, "Branches value must be a JSON array", "QueryBranchesArray");
            //
            // bool hasMain = branches.Any(b => b["uid"]?.ToString() == "main");
            // AssertLogger.IsTrue(hasMain,
            //     "Branches list must contain the 'main' branch", "QueryBranchesHasMain");
            //
            // bool hasCreated = branches.Any(b => b["uid"]?.ToString() == _branchUid);
            // AssertLogger.IsTrue(hasCreated,
            //     $"Branches list must contain the created branch '{_branchUid}'", "QueryBranchesHasCreated");

            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }

        /// <summary>
        /// Attempts to fetch a branch with a uid that does not exist.
        /// Expects a <see cref="ContentstackErrorException"/> with HTTP 404 or 422.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public void Test005_Should_Throw_On_Fetch_NonExistent_Branch()
        {
            const string nonExistentUid = "nonexistent-branch-9999";

            // try
            // {
            //     ContentstackResponse contentstackResponse = _stack.Branch(nonExistentUid).Fetch();
            //     AssertLogger.IsFalse(contentstackResponse.IsSuccessStatusCode,
            //         "Fetching a non-existent branch must not return 2xx",
            //         "FetchNonExistentBranchStatus");
            // }
            // catch (Exception ex)
            // {
            //     AssertBranchErrorException(ex, "FetchNonExistentBranch");
            // }

            _ = nonExistentUid; // suppress unused-variable warning
            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }

        /// <summary>
        /// Attempts to create a branch with the same uid as the one created in Test001.
        /// Expects a <see cref="ContentstackErrorException"/> with HTTP 409/422/400 (conflict / already exists).
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public void Test006_Should_Throw_On_Create_Duplicate_Branch()
        {
            // var model = new JObject
            // {
            //     ["uid"]    = _branchUid,
            //     ["source"] = "main"
            // };
            //
            // try
            // {
            //     ContentstackResponse contentstackResponse = _stack.Branch().Create(model);
            //     AssertLogger.IsFalse(contentstackResponse.IsSuccessStatusCode,
            //         "Creating a duplicate branch must not return 2xx",
            //         "DuplicateBranchStatus");
            // }
            // catch (Exception ex)
            // {
            //     AssertBranchErrorException(ex, "CreateDuplicateBranch");
            // }

            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }

        /// <summary>
        /// Deletes the branch created in Test001 (synchronous).
        /// Verifies the delete succeeds, then confirms a subsequent fetch raises an error.
        /// Sets _createdBranch = false to skip ClassCleanup deletion.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public void Test007_Should_Delete_Branch_Sync()
        {
            // ----- Act: delete -----
            // ContentstackResponse deleteResponse = _stack.Branch(_branchUid).Delete();
            // AssertLogger.IsTrue(deleteResponse.IsSuccessStatusCode,
            //     "Delete branch must return 2xx", "DeleteBranchStatus");
            //
            // _createdBranch = false;
            //
            // ----- Assert: branch is gone -----
            // try
            // {
            //     ContentstackResponse fetchResponse = _stack.Branch(_branchUid).Fetch();
            //     AssertLogger.IsFalse(fetchResponse.IsSuccessStatusCode,
            //         "Branch must be gone after deletion; Fetch must not return 2xx",
            //         "DeletedBranchFetchStatus");
            // }
            // catch (Exception ex)
            // {
            //     AssertBranchErrorException(ex, "FetchAfterDelete");
            // }

            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }

        /// <summary>
        /// Attempts to delete the "main" branch, which is protected.
        /// Expects a <see cref="ContentstackErrorException"/> with HTTP 400 or 422.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        [Ignore("Branch API is not yet exposed in the .NET Management SDK. " +
                "Remove [Ignore] and implement when Stack.Branch() is available.")]
        public void Test008_Should_Throw_On_Delete_Main_Branch()
        {
            // try
            // {
            //     ContentstackResponse deleteResponse = _stack.Branch("main").Delete();
            //     AssertLogger.IsFalse(deleteResponse.IsSuccessStatusCode,
            //         "Deleting the 'main' branch must not return 2xx (it is protected)",
            //         "DeleteMainBranchStatus");
            // }
            // catch (Exception ex)
            // {
            //     // 400 or 422 expected — main branch deletion should be rejected by the API.
            //     AssertBranchErrorException(ex, "DeleteMainBranch");
            // }

            Assert.Inconclusive("Branch API not yet exposed in SDK — test skipped.");
        }
    }
}
