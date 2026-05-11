using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack004_ReleaseTest
    {
        private static ContentstackClient _client;
        private Stack _stack;
        private string _testReleaseName = "DotNet SDK Integration Test Release";
        private string _testReleaseDescription = "Release created for .NET SDK integration testing";

        /// <summary>Stable bogus UID for negative-path release API tests.</summary>
        private const string NonExistentReleaseUid = "non_existent_release_uid_12345";

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
        public async Task Initialize()
        {
            StackResponse response = StackResponse.getStack(_client.SerializerOptions);
            _stack = _client.Stack(response.Stack.APIKey);
        }

        /// <summary>
        /// Asserts a missing-release or generic not-found style API error (used for fetch/update/delete/clone source/deploy on bogus UID).
        /// </summary>
        private static void AssertReleaseNotFoundOrApiError(Exception ex, string assertContext)
        {
            if (ex is ContentstackErrorException csException)
            {
                Assert.IsTrue(
                    csException.ErrorMessage?.Contains("Release does not exist") == true ||
                    csException.ErrorCode == 141 ||
                    csException.Message?.Contains("Release does not exist") == true,
                    $"{assertContext}: Expected 'Release does not exist' or error 141, but got ErrorCode={csException.ErrorCode}, Message='{csException.Message}', ErrorMessage='{csException.ErrorMessage}'"
                );
            }
            else
            {
                Assert.IsTrue(
                    ex.Message?.Contains("Release does not exist") == true ||
                    ex.Message?.Contains("not found") == true ||
                    ex.Message?.Contains("404") == true,
                    $"{assertContext}: Expected not-found style error, but got: {ex.Message}"
                );
            }
        }

        private void ExpectReleaseNotFoundFailure(Func<ContentstackResponse> invoke, string context)
        {
            try
            {
                ContentstackResponse response = invoke();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertReleaseNotFoundOrApiError(ex, context);
            }
        }

        private async Task ExpectReleaseNotFoundFailureAsync(Func<Task<ContentstackResponse>> invokeAsync, string context)
        {
            try
            {
                ContentstackResponse response = await invokeAsync();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertReleaseNotFoundOrApiError(ex, context);
            }
        }

        /// <summary>
        /// Asserts a validation-style API error for create/update body issues (message wording varies by stack).
        /// </summary>
        private static void AssertValidationOrBadRequestApiError(Exception ex, string assertContext)
        {
            if (ex is ContentstackErrorException csException)
            {
                string combined = $"{csException.ErrorMessage} {csException.Message}".ToLowerInvariant();
                Assert.IsTrue(
                    combined.Contains("name") ||
                    combined.Contains("required") ||
                    combined.Contains("invalid") ||
                    combined.Contains("blank") ||
                    combined.Contains("empty") ||
                    (int)csException.StatusCode == 400 ||
                    (int)csException.StatusCode == 422,
                    $"{assertContext}: Expected validation/bad request, but got ErrorCode={csException.ErrorCode}, StatusCode={csException.StatusCode}, Message='{csException.Message}', ErrorMessage='{csException.ErrorMessage}'"
                );
            }
            else
            {
                string msg = ex.Message?.ToLowerInvariant() ?? string.Empty;
                Assert.IsTrue(
                    msg.Contains("name") ||
                    msg.Contains("required") ||
                    msg.Contains("invalid") ||
                    msg.Contains("400") ||
                    msg.Contains("422"),
                    $"{assertContext}: Expected validation-style error, but got: {ex.Message}"
                );
            }
        }

        private void ExpectValidationOrBadRequestFailure(Func<ContentstackResponse> invoke, string context)
        {
            try
            {
                ContentstackResponse response = invoke();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertValidationOrBadRequestApiError(ex, context);
            }
        }

        private async Task ExpectValidationOrBadRequestFailureAsync(Func<Task<ContentstackResponse>> invokeAsync, string context)
        {
            try
            {
                ContentstackResponse response = await invokeAsync();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertValidationOrBadRequestApiError(ex, context);
            }
        }

        /// <summary>
        /// Helper method to create a clean release for testing
        /// </summary>
        private string CreateTestRelease()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName,
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse contentstackResponse = _stack.Release().Create(releaseModel);
                var response = contentstackResponse.OpenJsonObjectResponse();

                if (!contentstackResponse.IsSuccessStatusCode || response?["release"] == null)
                {
                    throw new Exception($"Failed to create release. Status: {contentstackResponse.StatusCode}");
                }

                return response["release"]["uid"].ToString();
            }
            catch (Exception e)
            {
                throw new Exception($"CreateTestRelease failed: {e.Message}", e);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Release()
        {
            string releaseUid = null;
            try
            {
                releaseUid = CreateTestRelease();

                Assert.IsNotNull(releaseUid);
                
                ContentstackResponse contentstackResponse = _stack.Release(releaseUid).Fetch();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(_testReleaseName, response["release"]["name"].ToString());
                Assert.AreEqual(_testReleaseDescription, response["release"]["description"].ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try
                    {
                        _stack.Release(releaseUid).Delete();
                    }
                    catch
                    {
                       
                    }
                }
            }
        }

        /// <summary>
        /// Async helper method to create a clean release for testing
        /// </summary>
        private async Task<string> CreateTestReleaseAsync()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName,
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse contentstackResponse = await _stack.Release().CreateAsync(releaseModel);
                var response = contentstackResponse.OpenJsonObjectResponse();

                if (!contentstackResponse.IsSuccessStatusCode || response?["release"] == null)
                {
                    throw new Exception($"Failed to create release. Status: {contentstackResponse.StatusCode}");
                }

                return response["release"]["uid"].ToString();
            }
            catch (Exception e)
            {
                throw new Exception($"CreateTestReleaseAsync failed: {e.Message}", e);
            }
        }

        /// <summary>
        /// Helper method to create 6 numbered test releases
        /// </summary>
        private List<string> CreateSixNumberedReleases()
        {
            var releaseUids = new List<string>();
            
            try
            {
                for (int i = 1; i <= 6; i++)
                {
                    var releaseModel = new ReleaseModel
                    {
                        Name = $"{_testReleaseName} {i}",
                        Description = $"{_testReleaseDescription} (Number {i})",
                        Locked = false,
                        Archived = false
                    };

                    ContentstackResponse contentstackResponse = _stack.Release().Create(releaseModel);
                    var response = contentstackResponse.OpenJsonObjectResponse();

                    if (!contentstackResponse.IsSuccessStatusCode || response?["release"] == null)
                    {
                        throw new Exception($"Failed to create release {i}. Status: {contentstackResponse.StatusCode}");
                    }

                    releaseUids.Add(response["release"]["uid"].ToString());
                }
                
                return releaseUids;
            }
            catch (Exception e)
            {
                foreach (var uid in releaseUids)
                {
                    try
                    {
                        _stack.Release(uid).Delete();
                    }
                    catch
                    {
                    }
                }
                throw new Exception($"CreateSixNumberedReleases failed: {e.Message}", e);
            }
        }

        /// <summary>
        /// Async helper method to create 6 numbered test releases
        /// </summary>
        private async Task<List<string>> CreateSixNumberedReleasesAsync()
        {
            var releaseUids = new List<string>();
            
            try
            {
                for (int i = 1; i <= 6; i++)
                {
                    var releaseModel = new ReleaseModel
                    {
                        Name = $"{_testReleaseName} {i}",
                        Description = $"{_testReleaseDescription} (Number {i})",
                        Locked = false,
                        Archived = false
                    };

                    ContentstackResponse contentstackResponse = await _stack.Release().CreateAsync(releaseModel);
                    var response = contentstackResponse.OpenJsonObjectResponse();

                    if (!contentstackResponse.IsSuccessStatusCode || response?["release"] == null)
                    {
                        throw new Exception($"Failed to create release {i}. Status: {contentstackResponse.StatusCode}");
                    }

                    releaseUids.Add(response["release"]["uid"].ToString());
                }
                
                return releaseUids;
            }
            catch (Exception e)
            {
                foreach (var uid in releaseUids)
                {
                    try
                    {
                        await _stack.Release(uid).DeleteAsync();
                    }
                    catch
                    {
                    }
                }
                throw new Exception($"CreateSixNumberedReleasesAsync failed: {e.Message}", e);
            }
        }

        /// <summary>
        /// Helper method to clean up a list of releases
        /// </summary>
        private void CleanupReleases(List<string> releaseUids)
        {
            if (releaseUids != null)
            {
                foreach (var uid in releaseUids)
                {
                    try
                    {
                        _stack.Release(uid).Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Async helper method to clean up a list of releases
        /// </summary>
        private async Task CleanupReleasesAsync(List<string> releaseUids)
        {
            if (releaseUids != null)
            {
                foreach (var uid in releaseUids)
                {
                    try
                    {
                        await _stack.Release(uid).DeleteAsync();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Release_Async()
        {
            string releaseUid = null;
            try
            {
                releaseUid = await CreateTestReleaseAsync();

                Assert.IsNotNull(releaseUid);
                
                ContentstackResponse contentstackResponse = await _stack.Release(releaseUid).FetchAsync();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(_testReleaseName, response["release"]["name"].ToString());
                Assert.AreEqual(_testReleaseDescription, response["release"]["description"].ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release async failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try
                    {
                        await _stack.Release(releaseUid).DeleteAsync();
                    }
                    catch
                    {
                        // Ignore cleanup failures
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Query_All_Releases()
        {
            List<string> releaseUids = null;
            try
            {
                releaseUids = CreateSixNumberedReleases();

                ContentstackResponse contentstackResponse = _stack.Release().Query().Find();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["releases"]);
                
                var releases = response["releases"] as JsonArray;
                Assert.IsNotNull(releases);
                
                Assert.IsTrue(releases.Count >= 6, $"Expected at least 6 releases, but found {releases.Count}");
                
                var releaseNames = releases.Select(r => r["name"]?.ToString()).ToList();
                for (int i = 1; i <= 6; i++)
                {
                    string expectedName = $"{_testReleaseName} {i}";
                    Assert.IsTrue(releaseNames.Contains(expectedName), 
                        $"Expected to find release with name '{expectedName}' in query results");
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Query all releases failed: {e.Message}");
            }
            finally
            {
                CleanupReleases(releaseUids);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Query_All_Releases_Async()
        {
            List<string> releaseUids = null;
            try
            {
                releaseUids = await CreateSixNumberedReleasesAsync();

                ContentstackResponse contentstackResponse = await _stack.Release().Query().FindAsync();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["releases"]);
                
                var releases = response["releases"] as JsonArray;
                Assert.IsNotNull(releases);
                
                Assert.IsTrue(releases.Count >= 6, $"Expected at least 6 releases, but found {releases.Count}");
                
                var releaseNames = releases.Select(r => r["name"]?.ToString()).ToList();
                for (int i = 1; i <= 6; i++)
                {
                    string expectedName = $"{_testReleaseName} {i}";
                    Assert.IsTrue(releaseNames.Contains(expectedName), 
                        $"Expected to find release with name '{expectedName}' in query results");
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Query all releases async failed: {e.Message}");
            }
            finally
            {
                await CleanupReleasesAsync(releaseUids);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Fetch_Release()
        {
            List<string> releaseUids = null;
            try
            {
                releaseUids = CreateSixNumberedReleases();

                string releaseToFetch = releaseUids[2];
                ContentstackResponse contentstackResponse = _stack.Release(releaseToFetch).Fetch();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(releaseToFetch, response["release"]["uid"].ToString());
                Assert.AreEqual($"{_testReleaseName} 3", response["release"]["name"].ToString());
                Assert.AreEqual($"{_testReleaseDescription} (Number 3)", response["release"]["description"].ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Fetch release failed: {e.Message}");
            }
            finally
            {
                CleanupReleases(releaseUids);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Fetch_Release_Async()
        {
            List<string> releaseUids = null;
            try
            {
                releaseUids = await CreateSixNumberedReleasesAsync();

                string releaseToFetch = releaseUids[4];
                ContentstackResponse contentstackResponse = await _stack.Release(releaseToFetch).FetchAsync();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(releaseToFetch, response["release"]["uid"].ToString());
                Assert.AreEqual($"{_testReleaseName} 5", response["release"]["name"].ToString());
                Assert.AreEqual($"{_testReleaseDescription} (Number 5)", response["release"]["description"].ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Fetch release async failed: {e.Message}");
            }
            finally
            {
                await CleanupReleasesAsync(releaseUids);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Update_Release()
        {
            string releaseUid = null;
            try
            {
                releaseUid = CreateTestRelease();

                var updateModel = new ReleaseModel
                {
                    Name = _testReleaseName + " Updated",
                    Description = _testReleaseDescription + " (Updated)",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse contentstackResponse = _stack.Release(releaseUid).Update(updateModel);
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(_testReleaseName + " Updated", response["release"]["name"].ToString());
                Assert.AreEqual(_testReleaseDescription + " (Updated)", response["release"]["description"].ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Update release failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try
                    {
                        _stack.Release(releaseUid).Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Update_Release_Async()
        {
            string releaseUid = null;
            try
            {
                releaseUid = await CreateTestReleaseAsync();

                var updateModel = new ReleaseModel
                {
                    Name = _testReleaseName + " Updated Async",
                    Description = _testReleaseDescription + " (Updated Async)",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse contentstackResponse = await _stack.Release(releaseUid).UpdateAsync(updateModel);
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(_testReleaseName + " Updated Async", response["release"]["name"].ToString());
                Assert.AreEqual(_testReleaseDescription + " (Updated Async)", response["release"]["description"].ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Update release async failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try
                    {
                        await _stack.Release(releaseUid).DeleteAsync();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Clone_Release()
        {
            string originalReleaseUid = null;
            string clonedReleaseUid = null;
            try
            {
                originalReleaseUid = CreateTestRelease();

                string cloneName = _testReleaseName + " Cloned";
                string cloneDescription = _testReleaseDescription + " (Cloned)";

                ContentstackResponse contentstackResponse = _stack.Release(originalReleaseUid).Clone(cloneName, cloneDescription);
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(cloneName, response["release"]["name"].ToString());

                clonedReleaseUid = response["release"]["uid"].ToString();
                Assert.IsNotNull(clonedReleaseUid);
                Assert.AreNotEqual(originalReleaseUid, clonedReleaseUid);
            }
            catch (Exception e)
            {
                Assert.Fail($"Clone release failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(clonedReleaseUid))
                {
                    try
                    {
                        _stack.Release(clonedReleaseUid).Delete();
                    }
                    catch
                    {
                    }
                }
                if (!string.IsNullOrEmpty(originalReleaseUid))
                {
                    try
                    {
                        _stack.Release(originalReleaseUid).Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test010_Should_Clone_Release_Async()
        {
            string originalReleaseUid = null;
            string clonedReleaseUid = null;
            try
            {
                originalReleaseUid = await CreateTestReleaseAsync();

                string cloneName = _testReleaseName + " Cloned Async";
                string cloneDescription = _testReleaseDescription + " (Cloned Async)";

                ContentstackResponse contentstackResponse = await _stack.Release(originalReleaseUid).CloneAsync(cloneName, cloneDescription);
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);
                Assert.AreEqual(cloneName, response["release"]["name"].ToString());

                clonedReleaseUid = response["release"]["uid"].ToString();
                Assert.IsNotNull(clonedReleaseUid);
                Assert.AreNotEqual(originalReleaseUid, clonedReleaseUid);
            }
            catch (Exception e)
            {
                Assert.Fail($"Clone release async failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(clonedReleaseUid))
                {
                    try
                    {
                        await _stack.Release(clonedReleaseUid).DeleteAsync();
                    }
                    catch
                    {
                    }
                }
                if (!string.IsNullOrEmpty(originalReleaseUid))
                {
                    try
                    {
                        await _stack.Release(originalReleaseUid).DeleteAsync();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Query_Release_With_Parameters()
        {
            try
            {
                List<string> releaseUids = CreateSixNumberedReleases();
                var parameters = new ParameterCollection();
                parameters.Add("include_count", "true");
                parameters.Add("limit", "5");

                ContentstackResponse contentstackResponse = _stack.Release().Query().Limit(5).Find();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["releases"]);
                CleanupReleases(releaseUids);
            }
            catch (Exception e)
            {
                Assert.Fail($"Query release with parameters failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test012_Should_Query_Release_With_Parameters_Async()
        {
            try
            {
                List<string> releaseUids =await  CreateSixNumberedReleasesAsync();
                var parameters = new ParameterCollection();
                parameters.Add("include_count", "true");
                parameters.Add("limit", "5");

                ContentstackResponse contentstackResponse = await _stack.Release().Query().Limit(5).FindAsync();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["releases"]);
                await CleanupReleasesAsync(releaseUids);
            }
            catch (Exception e)
            {
                Assert.Fail($"Query release with parameters async failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Create_Release_With_ParameterCollection()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName + " With Params",
                    Description = _testReleaseDescription + " (With Parameters)",
                    Locked = false,
                    Archived = false
                };

                var parameters = new ParameterCollection();
                parameters.Add("include_count", "true");

                ContentstackResponse contentstackResponse = _stack.Release().Create(releaseModel, parameters);
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);

                string releaseUid = response["release"]["uid"].ToString();
                Assert.IsNotNull(releaseUid);

                _stack.Release(releaseUid).Delete();
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release with parameters failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test014_Should_Create_Release_With_ParameterCollection_Async()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName + " With Params Async",
                    Description = _testReleaseDescription + " (With Parameters Async)",
                    Locked = false,
                    Archived = false
                };

                var parameters = new ParameterCollection();
                parameters.Add("include_count", "true");

                ContentstackResponse contentstackResponse = await _stack.Release().CreateAsync(releaseModel, parameters);
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["release"]);

                string releaseUid = response["release"]["uid"].ToString();
                Assert.IsNotNull(releaseUid);

                await _stack.Release(releaseUid).DeleteAsync();
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release with parameters async failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Get_Release_Items()
        {
            string releaseUid = null;
            try
            {
                releaseUid = CreateTestRelease();

                ContentstackResponse contentstackResponse = _stack.Release(releaseUid).Item().GetAll();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail($"Get release items failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try
                    {
                        _stack.Release(releaseUid).Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test016_Should_Get_Release_Items_Async()
        {
            string releaseUid = null;
            try
            {
                releaseUid = await CreateTestReleaseAsync();

                ContentstackResponse contentstackResponse = await _stack.Release(releaseUid).Item().GetAllAsync();
                var response = contentstackResponse.OpenJsonObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail($"Get release items async failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try
                    {
                        await _stack.Release(releaseUid).DeleteAsync();
                    }
                    catch
                    {
                    }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Handle_Release_Not_Found()
        {
            try
            {
                ExpectReleaseNotFoundFailure(() => _stack.Release(NonExistentReleaseUid).Fetch(), "Fetch non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Handle release not found test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test018_Should_Handle_Release_Not_Found_Async()
        {
            try
            {
                await ExpectReleaseNotFoundFailureAsync(() => _stack.Release(NonExistentReleaseUid).FetchAsync(), "FetchAsync non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Handle release not found async test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Delete_Release()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName + " For Deletion",
                    Description = _testReleaseDescription + " (To be deleted)",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse createResponse = _stack.Release().Create(releaseModel);
                var createResponseJson = createResponse.OpenJsonObjectResponse();
                string releaseToDeleteUid = createResponseJson["release"]["uid"].ToString();

                ContentstackResponse contentstackResponse = _stack.Release(releaseToDeleteUid).Delete();

                Assert.IsNotNull(contentstackResponse);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail($"Delete release failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test020_Should_Delete_Release_Async()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName + " For Deletion Async",
                    Description = _testReleaseDescription + " (To be deleted async)",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse createResponse = await _stack.Release().CreateAsync(releaseModel);
                var createResponseJson = createResponse.OpenJsonObjectResponse();
                string releaseToDeleteUid = createResponseJson["release"]["uid"].ToString();

                ContentstackResponse contentstackResponse = await _stack.Release(releaseToDeleteUid).DeleteAsync();

                Assert.IsNotNull(contentstackResponse);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail($"Delete release async failed: {e.Message}");
            }
        }

        /// <summary>
        /// Verifies that Delete Release API succeeds when the SDK does not send Content-Type header (DELETE /releases/{uid}).
        /// Creates a release, deletes it without Content-Type, asserts success, then verifies the release is gone.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_Delete_Release_Without_Content_Type_Header()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName + " Delete Without Content-Type",
                    Description = _testReleaseDescription + " (Delete without Content-Type header)",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse createResponse = _stack.Release().Create(releaseModel);
                var createResponseJson = createResponse.OpenJsonObjectResponse();
                Assert.IsTrue(createResponse.IsSuccessStatusCode, "Create release must succeed.");
                string releaseToDeleteUid = createResponseJson["release"]["uid"].ToString();

                ContentstackResponse deleteResponse = _stack.Release(releaseToDeleteUid).Delete();

                Assert.IsNotNull(deleteResponse);
                Assert.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete release (without Content-Type) must succeed.");

                try
                {
                    var fetchResponse = _stack.Release(releaseToDeleteUid).Fetch();
                    Assert.IsFalse(fetchResponse.IsSuccessStatusCode, "Release must be gone after delete; Fetch should not succeed.");
                }
                catch (ContentstackErrorException)
                {
                    Assert.IsTrue(true, "Release not found after delete (exception path).");
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Delete release without Content-Type header failed: {e.Message}");
            }
        }

        /// <summary>
        /// Verifies that Delete Release API (async) succeeds when the SDK does not send Content-Type header (DELETE /releases/{uid}).
        /// Creates a release, deletes it without Content-Type, asserts success, then verifies the release is gone.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        public async Task Test022_Should_Delete_Release_Async_Without_Content_Type_Header()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = _testReleaseName + " Delete Async Without Content-Type",
                    Description = _testReleaseDescription + " (Delete async without Content-Type header)",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse createResponse = await _stack.Release().CreateAsync(releaseModel);
                var createResponseJson = createResponse.OpenJsonObjectResponse();
                Assert.IsTrue(createResponse.IsSuccessStatusCode, "Create release must succeed.");
                string releaseToDeleteUid = createResponseJson["release"]["uid"].ToString();

                ContentstackResponse deleteResponse = await _stack.Release(releaseToDeleteUid).DeleteAsync();

                Assert.IsNotNull(deleteResponse);
                Assert.IsTrue(deleteResponse.IsSuccessStatusCode, "Delete release async (without Content-Type) must succeed.");

                try
                {
                    var fetchResponse = await _stack.Release(releaseToDeleteUid).FetchAsync();
                    Assert.IsFalse(fetchResponse.IsSuccessStatusCode, "Release must be gone after delete; Fetch should not succeed.");
                }
                catch (ContentstackErrorException)
                {
                    Assert.IsTrue(true, "Release not found after delete (exception path).");
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Delete release async without Content-Type header failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_Fail_When_Create_Release_With_Null_Name()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = null,
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };
                ExpectValidationOrBadRequestFailure(() => _stack.Release().Create(releaseModel), "Create with null name");
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release null name negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test024_Should_Fail_When_Create_Release_With_Null_Name_Async()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = null,
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };
                await ExpectValidationOrBadRequestFailureAsync(() => _stack.Release().CreateAsync(releaseModel), "CreateAsync with null name");
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release null name async negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test025_Should_Fail_When_Create_Release_With_Empty_Name()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = string.Empty,
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };
                ExpectValidationOrBadRequestFailure(() => _stack.Release().Create(releaseModel), "Create with empty name");
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release empty name negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test026_Should_Fail_When_Create_Release_With_Empty_Name_Async()
        {
            try
            {
                var releaseModel = new ReleaseModel
                {
                    Name = string.Empty,
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };
                await ExpectValidationOrBadRequestFailureAsync(() => _stack.Release().CreateAsync(releaseModel), "CreateAsync with empty name");
            }
            catch (Exception e)
            {
                Assert.Fail($"Create release empty name async negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test027_Should_Fail_When_Update_Non_Existent_Release()
        {
            try
            {
                var updateModel = new ReleaseModel
                {
                    Name = _testReleaseName + " Ghost",
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };
                ExpectReleaseNotFoundFailure(() => _stack.Release(NonExistentReleaseUid).Update(updateModel), "Update non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Update non-existent release negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test028_Should_Fail_When_Update_Non_Existent_Release_Async()
        {
            try
            {
                var updateModel = new ReleaseModel
                {
                    Name = _testReleaseName + " Ghost Async",
                    Description = _testReleaseDescription,
                    Locked = false,
                    Archived = false
                };
                await ExpectReleaseNotFoundFailureAsync(() => _stack.Release(NonExistentReleaseUid).UpdateAsync(updateModel), "UpdateAsync non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Update non-existent release async negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Fail_When_Clone_Non_Existent_Release()
        {
            try
            {
                string cloneName = _testReleaseName + " Clone Target Should Never Exist";
                ExpectReleaseNotFoundFailure(() => _stack.Release(NonExistentReleaseUid).Clone(cloneName, _testReleaseDescription), "Clone non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Clone non-existent release negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test030_Should_Fail_When_Clone_Non_Existent_Release_Async()
        {
            try
            {
                string cloneName = _testReleaseName + " Clone Target Should Never Exist Async";
                await ExpectReleaseNotFoundFailureAsync(() => _stack.Release(NonExistentReleaseUid).CloneAsync(cloneName, _testReleaseDescription), "CloneAsync non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Clone non-existent release async negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test031_Should_Throw_When_Clone_With_Null_Name()
        {
            string releaseUid = null;
            try
            {
                releaseUid = CreateTestRelease();
                Assert.ThrowsException<ArgumentNullException>(() => _stack.Release(releaseUid).Clone(null, _testReleaseDescription));
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try { _stack.Release(releaseUid).Delete(); } catch { }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test032_Should_Throw_When_Clone_With_Null_Name_Async()
        {
            string releaseUid = null;
            try
            {
                releaseUid = await CreateTestReleaseAsync();
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _stack.Release(releaseUid).CloneAsync(null, _testReleaseDescription));
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try { await _stack.Release(releaseUid).DeleteAsync(); } catch { }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test033_Should_Fail_When_Get_Release_Items_For_Non_Existent_Release()
        {
            try
            {
                ExpectReleaseNotFoundFailure(() => _stack.Release(NonExistentReleaseUid).Item().GetAll(), "GetAll items non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Get release items for non-existent release failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test034_Should_Fail_When_Get_Release_Items_For_Non_Existent_Release_Async()
        {
            try
            {
                await ExpectReleaseNotFoundFailureAsync(() => _stack.Release(NonExistentReleaseUid).Item().GetAllAsync(), "GetAllAsync items non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Get release items async for non-existent release failed: {e.Message}");
            }
        }

        private static void AssertDeployOrEnvironmentOrNotFoundApiError(Exception ex, string assertContext)
        {
            if (ex is ContentstackErrorException csException)
            {
                string combined = $"{csException.ErrorMessage} {csException.Message}".ToLowerInvariant();
                Assert.IsTrue(
                    IsReleaseNotFoundContentstackError(csException) ||
                    combined.Contains("environment") ||
                    combined.Contains("locale") ||
                    combined.Contains("invalid") ||
                    combined.Contains("not found") ||
                    (int)csException.StatusCode == 400 ||
                    (int)csException.StatusCode == 422,
                    $"{assertContext}: Unexpected deploy error ErrorCode={csException.ErrorCode}, StatusCode={csException.StatusCode}, Message='{csException.Message}', ErrorMessage='{csException.ErrorMessage}'"
                );
            }
            else
            {
                string msg = ex.Message?.ToLowerInvariant() ?? string.Empty;
                Assert.IsTrue(
                    msg.Contains("release") ||
                    msg.Contains("environment") ||
                    msg.Contains("not found") ||
                    msg.Contains("400") ||
                    msg.Contains("422"),
                    $"{assertContext}: Unexpected deploy error: {ex.Message}"
                );
            }
        }

        private static bool IsReleaseNotFoundContentstackError(ContentstackErrorException csException)
        {
            return csException.ErrorMessage?.Contains("Release does not exist") == true ||
                   csException.ErrorCode == 141 ||
                   csException.Message?.Contains("Release does not exist") == true;
        }

        private void ExpectDeployFailure(Func<ContentstackResponse> invoke, string context)
        {
            try
            {
                ContentstackResponse response = invoke();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertDeployOrEnvironmentOrNotFoundApiError(ex, context);
            }
        }

        private async Task ExpectDeployFailureAsync(Func<Task<ContentstackResponse>> invokeAsync, string context)
        {
            try
            {
                ContentstackResponse response = await invokeAsync();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertDeployOrEnvironmentOrNotFoundApiError(ex, context);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test035_Should_Fail_When_Deploy_Non_Existent_Release()
        {
            try
            {
                var deployModel = new DeployModel
                {
                    Environments = new List<string> { "fake_environment_uid_for_negative_test" },
                    Locales = new List<string> { "en-us" }
                };
                ExpectDeployFailure(() => _stack.Release(NonExistentReleaseUid).Deploy(deployModel), "Deploy non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Deploy non-existent release negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test036_Should_Fail_When_Deploy_Non_Existent_Release_Async()
        {
            try
            {
                var deployModel = new DeployModel
                {
                    Environments = new List<string> { "fake_environment_uid_for_negative_test" },
                    Locales = new List<string> { "en-us" }
                };
                await ExpectDeployFailureAsync(() => _stack.Release(NonExistentReleaseUid).DeployAsync(deployModel), "DeployAsync non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Deploy non-existent release async negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test037_Should_Fail_When_Deploy_With_Invalid_Environment()
        {
            string releaseUid = null;
            try
            {
                releaseUid = CreateTestRelease();
                var deployModel = new DeployModel
                {
                    Environments = new List<string> { "fake_environment_uid_for_negative_test" },
                    Locales = new List<string> { "en-us" }
                };
                ExpectDeployFailure(() => _stack.Release(releaseUid).Deploy(deployModel), "Deploy with invalid environment");
            }
            catch (Exception e)
            {
                Assert.Fail($"Deploy invalid environment negative test failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try { _stack.Release(releaseUid).Delete(); } catch { }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test038_Should_Fail_When_Deploy_With_Invalid_Environment_Async()
        {
            string releaseUid = null;
            try
            {
                releaseUid = await CreateTestReleaseAsync();
                var deployModel = new DeployModel
                {
                    Environments = new List<string> { "fake_environment_uid_for_negative_test" },
                    Locales = new List<string> { "en-us" }
                };
                await ExpectDeployFailureAsync(() => _stack.Release(releaseUid).DeployAsync(deployModel), "DeployAsync with invalid environment");
            }
            catch (Exception e)
            {
                Assert.Fail($"Deploy invalid environment async negative test failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try { await _stack.Release(releaseUid).DeleteAsync(); } catch { }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test039_Should_Fail_When_Delete_Non_Existent_Release()
        {
            try
            {
                ExpectReleaseNotFoundFailure(() => _stack.Release(NonExistentReleaseUid).Delete(), "Delete non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Delete non-existent release negative test failed: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test040_Should_Fail_When_Delete_Non_Existent_Release_Async()
        {
            try
            {
                await ExpectReleaseNotFoundFailureAsync(() => _stack.Release(NonExistentReleaseUid).DeleteAsync(), "DeleteAsync non-existent release");
            }
            catch (Exception e)
            {
                Assert.Fail($"Delete non-existent release async negative test failed: {e.Message}");
            }
        }

        private static void AssertItemCreateOrReferenceApiError(Exception ex, string assertContext)
        {
            if (ex is ContentstackErrorException csException)
            {
                string combined = $"{csException.ErrorMessage} {csException.Message}".ToLowerInvariant();
                Assert.IsTrue(
                    combined.Contains("item") ||
                    combined.Contains("entry") ||
                    combined.Contains("asset") ||
                    combined.Contains("uid") ||
                    combined.Contains("invalid") ||
                    combined.Contains("not found") ||
                    (int)csException.StatusCode == 400 ||
                    (int)csException.StatusCode == 422,
                    $"{assertContext}: Unexpected item create error ErrorCode={csException.ErrorCode}, StatusCode={csException.StatusCode}, Message='{csException.Message}', ErrorMessage='{csException.ErrorMessage}'"
                );
            }
            else
            {
                string msg = ex.Message?.ToLowerInvariant() ?? string.Empty;
                Assert.IsTrue(
                    msg.Contains("item") ||
                    msg.Contains("entry") ||
                    msg.Contains("invalid") ||
                    msg.Contains("400") ||
                    msg.Contains("422"),
                    $"{assertContext}: Unexpected item create error: {ex.Message}"
                );
            }
        }

        private void ExpectItemCreateFailure(Func<ContentstackResponse> invoke, string context)
        {
            try
            {
                ContentstackResponse response = invoke();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertItemCreateOrReferenceApiError(ex, context);
            }
        }

        private async Task ExpectItemCreateFailureAsync(Func<Task<ContentstackResponse>> invokeAsync, string context)
        {
            try
            {
                ContentstackResponse response = await invokeAsync();
                Assert.IsFalse(response.IsSuccessStatusCode, context);
            }
            catch (Exception ex)
            {
                AssertItemCreateOrReferenceApiError(ex, context);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test041_Should_Fail_When_Add_Invalid_Item_To_Release()
        {
            string releaseUid = null;
            try
            {
                releaseUid = CreateTestRelease();
                var bogusItem = new ReleaseItemModel
                {
                    Uid = "bogus_entry_uid_nonexistent_12345",
                    ContentTypeUID = "bogus_content_type_uid",
                    Locale = "en-us",
                    Version = 1,
                    Action = "publish"
                };
                ExpectItemCreateFailure(() => _stack.Release(releaseUid).Item().Create(bogusItem), "Item Create with garbage UIDs");
            }
            catch (Exception e)
            {
                Assert.Fail($"Item create garbage negative test failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try { _stack.Release(releaseUid).Delete(); } catch { }
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test042_Should_Fail_When_Add_Invalid_Item_To_Release_Async()
        {
            string releaseUid = null;
            try
            {
                releaseUid = await CreateTestReleaseAsync();
                var bogusItem = new ReleaseItemModel
                {
                    Uid = "bogus_entry_uid_nonexistent_67890",
                    ContentTypeUID = "bogus_content_type_uid",
                    Locale = "en-us",
                    Version = 1,
                    Action = "publish"
                };
                await ExpectItemCreateFailureAsync(() => _stack.Release(releaseUid).Item().CreateAsync(bogusItem), "Item CreateAsync with garbage UIDs");
            }
            catch (Exception e)
            {
                Assert.Fail($"Item create garbage async negative test failed: {e.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(releaseUid))
                {
                    try { await _stack.Release(releaseUid).DeleteAsync(); } catch { }
                }
            }
        }
    }
} 