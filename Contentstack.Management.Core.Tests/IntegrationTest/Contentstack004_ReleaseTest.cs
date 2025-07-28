using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack004_ReleaseTest
    {
        private Stack _stack;
        private string _testReleaseName = "DotNet SDK Integration Test Release";
        private string _testReleaseDescription = "Release created for .NET SDK integration testing";

        [TestInitialize]
        public async Task Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                    var response = contentstackResponse.OpenJObjectResponse();

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
                    var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["releases"]);
                
                var releases = response["releases"] as JArray;
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
                var response = contentstackResponse.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(contentstackResponse.IsSuccessStatusCode);
                Assert.IsNotNull(response["releases"]);
                
                var releases = response["releases"] as JArray;
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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                var response = contentstackResponse.OpenJObjectResponse();

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
                string nonExistentUid = "non_existent_release_uid_12345";
                
                try
                {
                    ContentstackResponse contentstackResponse = _stack.Release(nonExistentUid).Fetch();
                    Assert.IsFalse(contentstackResponse.IsSuccessStatusCode);
                }
                catch (Exception ex)
                {
          
                    if (ex is ContentstackErrorException csException)
                    {
                        Assert.IsTrue(
                            csException.ErrorMessage?.Contains("Release does not exist") == true ||
                            csException.ErrorCode == 141 ||
                            csException.Message?.Contains("Release does not exist") == true,
                            $"Expected 'Release does not exist' error, but got: ErrorCode={csException.ErrorCode}, Message='{csException.Message}', ErrorMessage='{csException.ErrorMessage}'"
                        );
                    }
                    else
                    {
                        Assert.IsTrue(
                            ex.Message?.Contains("Release does not exist") == true ||
                            ex.Message?.Contains("not found") == true ||
                            ex.Message?.Contains("404") == true,
                            $"Expected 'Release does not exist' error, but got: {ex.Message}"
                        );
                    }
                }
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
                string nonExistentUid = "non_existent_release_uid_12345";
                
                try
                {
                    ContentstackResponse contentstackResponse = await _stack.Release(nonExistentUid).FetchAsync();
                    Assert.IsFalse(contentstackResponse.IsSuccessStatusCode);
                }
                catch (Exception ex)
                {
                    if (ex is ContentstackErrorException csException)
                    {
                        Assert.IsTrue(
                            csException.ErrorMessage?.Contains("Release does not exist") == true ||
                            csException.ErrorCode == 141 ||
                            csException.Message?.Contains("Release does not exist") == true,
                            $"Expected 'Release does not exist' error, but got: ErrorCode={csException.ErrorCode}, Message='{csException.Message}', ErrorMessage='{csException.ErrorMessage}'"
                        );
                    }
                    else
                    {
                        Assert.IsTrue(
                            ex.Message?.Contains("Release does not exist") == true ||
                            ex.Message?.Contains("not found") == true ||
                            ex.Message?.Contains("404") == true,
                            $"Expected 'Release does not exist' error, but got: {ex.Message}"
                        );
                    }
                }
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
                var createResponseJson = createResponse.OpenJObjectResponse();
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
                var createResponseJson = createResponse.OpenJObjectResponse();
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


    }
} 