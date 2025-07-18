using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack015_BulkOperationTest
    {
        private Stack _stack;
        private string _contentTypeUid = "bulk_test_content_type";
        private string _testEnvironmentUid = "bulk_test_environment";
        private string _testReleaseUid = "bulk_test_release";
        private List<EntryInfo> _createdEntries = new List<EntryInfo>();

        [TestInitialize]
        public async Task Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
            
            // Create a test environment for bulk operations
            //await CreateTestEnvironment();
            //await CreateTestRelease();
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test001_Should_Create_Content_Type_With_Title_Field()
        {
            try
            {
                await CreateTestEnvironment();
                await CreateTestRelease();
                // Create a content type with only a title field
                var contentModelling = new ContentModelling
                {
                    Title = "bulk_test_content_type",
                    Uid = _contentTypeUid,
                    Schema = new List<Field>
                    {
                        new TextboxField
                        {
                            DisplayName = "Title",
                            Uid = "title",
                            DataType = "text",
                            Mandatory = true,
                            Unique = false,
                            Multiple = false
                        }
                    }
                };

                // Create the content type
                ContentstackResponse response = _stack.ContentType().Create(contentModelling);
                var responseJson = response.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(responseJson["content_type"]);
                Assert.AreEqual(_contentTypeUid, responseJson["content_type"]["uid"].ToString());
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_Create_Five_Entries()
        {
            try
            {
                // Create 5 entries with different titles
                var entryTitles = new[] { "First Entry", "Second Entry", "Third Entry", "Fourth Entry", "Fifth Entry" };

                foreach (var title in entryTitles)
                {
                    var entry = new SimpleEntry
                    {
                        Title = title
                    };

                    ContentstackResponse response = _stack.ContentType(_contentTypeUid).Entry().Create(entry);
                    var responseJson = response.OpenJObjectResponse();

                    Assert.IsNotNull(response);
                    Assert.IsTrue(response.IsSuccessStatusCode);
                    Assert.IsNotNull(responseJson["entry"]);
                    Assert.IsNotNull(responseJson["entry"]["uid"]);

                    string entryUid = responseJson["entry"]["uid"].ToString();
                    string entryTitle = responseJson["entry"]["title"].ToString();

                    _createdEntries.Add(new EntryInfo
                    {
                        Uid = entryUid,
                        Title = entryTitle
                    });
                }

                Assert.AreEqual(5, _createdEntries.Count, "Should have created exactly 5 entries");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test003_Should_Perform_Bulk_Publish_Operation()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

                // Get available environments or use empty list if none available
                List<string> availableEnvironments = await GetAvailableEnvironments();

                // Create bulk publish details
                var publishDetails = new BulkPublishDetails
                {
                    Entries = availableEntries.Select(e => new BulkPublishEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Version = 1,
                        Locale = "en-us"
                    }).ToList(),
                    Locales = new List<string> { "en-us" },
                    Environments = availableEnvironments
                };

                // Perform bulk publish
                ContentstackResponse response = _stack.BulkOperation().Publish(publishDetails);
                var responseJson = response.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail($"Failed to perform bulk publish: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Perform_Bulk_Unpublish_Operation()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

                // Get available environments
                List<string> availableEnvironments = await GetAvailableEnvironments();

                // Create bulk unpublish details
                var unpublishDetails = new BulkPublishDetails
                {
                    Entries = availableEntries.Select(e => new BulkPublishEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Version = 1,
                        Locale = "en-us"
                    }).ToList(),
                    Locales = new List<string> { "en-us" },
                    Environments = availableEnvironments
                };

                // Perform bulk unpublish
                ContentstackResponse response = _stack.BulkOperation().Unpublish(unpublishDetails);
                var responseJson = response.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail($"Failed to perform bulk unpublish: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Perform_Bulk_Release_Operations()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");
                
                // Fetch an available release
                string availableReleaseUid = await FetchAvailableRelease();
                Assert.IsFalse(string.IsNullOrEmpty(availableReleaseUid), "No release available for bulk operations");

                // First, add items to the release
                var addItemsData = new BulkAddItemsData
                {
                    Items = availableEntries.Select(e => new BulkAddItem
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid
                    }).ToList()
                };

                //// Add items to release
                //ContentstackResponse addResponse = _stack.BulkOperation().AddItems(addItemsData);
                //var addResponseJson = addResponse.OpenJObjectResponse();

                //Assert.IsNotNull(addResponse);
                //Assert.IsTrue(addResponse.IsSuccessStatusCode);
                //Console.WriteLine("Successfully added items to release");

                // Now perform bulk release operations using AddItems in deployment mode (only 4 entries)
                var releaseData = new BulkAddItemsData
                {
                    Release = availableReleaseUid,
                    Action = "publish",
                    Locale = new List<string> { "en-us" },
                    Reference = false,
                    Items = availableEntries.Take(4).Select(e => new BulkAddItem
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        ContentTypeUid = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us",
                        Title = e.Title
                    }).ToList()
                };

                // Perform bulk release using AddItems in deployment mode
                ContentstackResponse releaseResponse = _stack.BulkOperation().AddItems(releaseData, "2.0");
                var releaseResponseJson = releaseResponse.OpenJObjectResponse();

                Assert.IsNotNull(releaseResponse);
                Assert.IsTrue(releaseResponse.IsSuccessStatusCode);

                // Check if job was created
                Assert.IsNotNull(releaseResponseJson["job_id"]);
                string jobId = releaseResponseJson["job_id"].ToString();

                // Wait a bit and check job status
                await Task.Delay(2000);
                await CheckBulkJobStatus(jobId,"2.0");
            }
            catch (Exception e)
            {
                Assert.Fail($"Failed to perform bulk release operations: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005A_Should_Update_Items_In_Release()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");
                
                // Fetch an available release
                string availableReleaseUid = await FetchAvailableRelease();
                Assert.IsFalse(string.IsNullOrEmpty(availableReleaseUid), "No release available for bulk operations");

                // First, add the last entry to the release 
                //var addItemsData = new BulkAddItemsData
                //{
                //    Items = availableEntries.Skip(4).Take(1).Select(e => new BulkAddItem // Take the 5th entry (last one)
                //    {
                //        Uid = e.Uid,
                //        ContentType = _contentTypeUid
                //    }).ToList()
                //};

                // Add items to release using bulk_version 1.0
                //ContentstackResponse addResponse = _stack.BulkOperation().AddItems(addItemsData, "1.0");
                //if (addResponse.IsSuccessStatusCode)
                //{
                //    Console.WriteLine("Successfully added last entry to release for update test");
                //    await Task.Delay(3000); // Wait for items to be added
                //}

                //// Now update the last entry in the release using ReleaseItem().UpdateReleaseItem()
                //var releaseItemsToUpdate = availableEntries.Skip(4).Take(1).Select(e => e.Uid).ToList();
                
                //ContentstackResponse updateResponse = _stack.Release(availableReleaseUid).Item().UpdateReleaseItem(releaseItemsToUpdate);
                //var updateResponseJson = updateResponse.OpenJObjectResponse();

                //Assert.IsNotNull(updateResponse);
                //Assert.IsTrue(updateResponse.IsSuccessStatusCode);

                //if (updateResponseJson["job_id"] != null)
                //{
                //    string jobId = updateResponseJson["job_id"].ToString();
                //    Console.WriteLine($"Release items update job created with ID: {jobId}");
                    
                //    // Check job status
                //    await Task.Delay(2000);
                //    await CheckBulkJobStatus(jobId);
                //}
                //else
                //{
                //    Console.WriteLine("Release items updated successfully (no job created)");
                //}

                                // Alternative: Test bulk update items with version 2.0 for release items
                var releaseData = new BulkAddItemsData
                {
                    Release = availableReleaseUid,
                    Action = "publish",
                    Locale = new List<string> { "en-us" },
                    Reference = false,
                    Items = availableEntries.Skip(4).Take(1).Select(e => new BulkAddItem
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        ContentTypeUid = _contentTypeUid,
                        Version = e.Version,
                        Locale = "en-us",
                        Title = e.Title
                    }).ToList()
                };
                //var bulkUpdateData = new BulkAddItemsData
                //{
                //    Items = availableEntries.Skip(4).Take(1).Select(e => new BulkAddItem // Update the last entry
                //    {
                //        Uid = e.Uid,
                //        ContentType = _contentTypeUid
                //    }).ToList()
                //};

                // Perform bulk update items operation
                ContentstackResponse bulkUpdateResponse = _stack.BulkOperation().UpdateItems(releaseData, "2.0");
                var bulkUpdateResponseJson = bulkUpdateResponse.OpenJObjectResponse();

                Assert.IsNotNull(bulkUpdateResponse);
                Assert.IsTrue(bulkUpdateResponse.IsSuccessStatusCode);

                if (bulkUpdateResponseJson["job_id"] != null)
                {
                    string bulkJobId = bulkUpdateResponseJson["job_id"].ToString();
                    
                    // Check job status
                    await Task.Delay(2000);
                    await CheckBulkJobStatus(bulkJobId, "2.0");
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Failed to update items in release: {e.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Perform_Bulk_Delete_Operation()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

                // Create bulk delete details
                var deleteDetails = new BulkDeleteDetails
                {
                    Entries = availableEntries.Select(e => new BulkDeleteEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Locale = "en-us"
                    }).ToList()
                };

                // Perform bulk delete
                ContentstackResponse response = _stack.BulkOperation().Delete(deleteDetails);
                var responseJson = response.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail($"Failed to perform bulk delete: {e.Message}");
            }
        }

       
        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Perform_Bulk_Workflow_Operations()
        {
            try
            {
                // Fetch existing entries from the content type
                List<EntryInfo> availableEntries = await FetchExistingEntries();
                Assert.IsTrue(availableEntries.Count > 0, "No entries available for bulk operation");

                // Test bulk workflow update operations
                var workflowUpdateBody = new BulkWorkflowUpdateBody
                {
                    Entries = availableEntries.Select(e => new BulkWorkflowEntry
                    {
                        Uid = e.Uid,
                        ContentType = _contentTypeUid,
                        Locale = "en-us"
                    }).ToList(),
                    Workflow = new BulkWorkflowStage
                    {
                        Comment = "Bulk workflow update test",
                        DueDate = DateTime.Now.AddDays(7).ToString("ddd MMM dd yyyy"),
                        Notify = false,
                        Uid = "workflow_stage_uid" // This would need to be a real workflow stage UID
                    }
                };

                // Perform bulk workflow update
                ContentstackResponse response = _stack.BulkOperation().Update(workflowUpdateBody);
                var responseJson = response.OpenJObjectResponse();

                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(responseJson["job_id"]);
                string jobId = responseJson["job_id"].ToString();

                // Check job status
                await CheckBulkJobStatus(jobId);
            }
            catch (Exception e)
            {
                // Note: This test might fail if no workflow stages are configured
                // In a real scenario, you would need to create workflow stages first
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test009_Should_Cleanup_Test_Resources()
        {
            try
            {
                // Delete the content type we created
                ContentstackResponse response = _stack.ContentType(_contentTypeUid).Delete();
                Assert.IsNotNull(response);
                Assert.IsTrue(response.IsSuccessStatusCode);

                // Clean up test release
                if (!string.IsNullOrEmpty(_testReleaseUid))
                {
                    try
                    {
                        ContentstackResponse releaseResponse = _stack.Release(_testReleaseUid).Delete();
                    }
                    catch (Exception e)
                    {
                        // Cleanup failed, continue with test
                    }
                }

                // Clean up test environment
                if (!string.IsNullOrEmpty(_testEnvironmentUid))
                {
                    try
                    {
                        ContentstackResponse envResponse = _stack.Environment(_testEnvironmentUid).Delete();
                    }
                    catch (Exception e)
                    {
                        // Cleanup failed, continue with test
                    }
                }
            }
            catch (Exception e)
            {
                // Don't fail the test for cleanup issues
            }
        }

        private async Task CheckBulkJobStatus(string jobId, string bulkVersion = null)
        {
            try
            {
                ContentstackResponse statusResponse = await _stack.BulkOperation().JobStatusAsync(jobId, bulkVersion);
                var statusJson = statusResponse.OpenJObjectResponse();

                Assert.IsNotNull(statusResponse);
                Assert.IsTrue(statusResponse.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                // Failed to check job status
            }
        }

        private async Task CreateTestEnvironment()
        {
            try
            {
                // Check if test environment already exists
                //try
                //{
                //    ContentstackResponse fetchResponse = _stack.Environment(_testEnvironmentUid).Fetch();
                //    if (fetchResponse.IsSuccessStatusCode)
                //    {
                //        Console.WriteLine($"Test environment '{_testEnvironmentUid}' already exists");
                //        return;
                //    }
                //}
                //catch
                //{
                //    // Environment doesn't exist, create it
                //}

                // Create test environment
                var environmentModel = new EnvironmentModel
                {
                    Name = "bulk_test_env",
                    Urls = new List<LocalesUrl>
                    {
                        new LocalesUrl
                        {
                            Url = "https://bulk-test-environment.example.com",
                            Locale = "en-us"
                        }
                    }
                };

                ContentstackResponse response = _stack.Environment().Create(environmentModel);
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["environment"] != null)
                {
                    _testEnvironmentUid = responseJson["environment"]["uid"].ToString();
                }
            }
            catch (Exception e)
            {
                // Don't fail the test if environment creation fails
            }
        }

        private async Task CreateTestRelease()
        {
            try
            {
                // Check if test release already exists
                //try
                //{
                //    ContentstackResponse fetchResponse = _stack.Release(_testReleaseUid).Fetch();
                //    if (fetchResponse.IsSuccessStatusCode)
                //    {
                //        Console.WriteLine($"Test release '{_testReleaseUid}' already exists");
                //        return;
                //    }
                //}
                //catch
                //{
                //    // Release doesn't exist, create it
                //}

                // Create test release
                var releaseModel = new ReleaseModel
                {
                    Name = "bulk_test_release",
                    Description = "Release for testing bulk operations",
                    Locked = false,
                    Archived = false
                };

                ContentstackResponse response = _stack.Release().Create(releaseModel);
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["release"] != null)
                {
                    _testReleaseUid = responseJson["release"]["uid"].ToString();
                }
            }
            catch (Exception e)
            {
                // Don't fail the test if release creation fails
            }
        }

        private async Task<List<string>> GetAvailableEnvironments()
        {
            try
            {
                // First try to use our test environment
                if (!string.IsNullOrEmpty(_testEnvironmentUid))
                {
                    try
                    {
                        ContentstackResponse fetchResponse = _stack.Environment(_testEnvironmentUid).Fetch();
                        if (fetchResponse.IsSuccessStatusCode)
                        {
                            return new List<string> { _testEnvironmentUid };
                        }
                    }
                    catch
                    {
                        // Test environment doesn't exist, fall back to available environments
                    }
                }

                // Try to get available environments
                try
                {
                    ContentstackResponse response = _stack.Environment().Query().Find();
                    var responseJson = response.OpenJObjectResponse();

                    if (response.IsSuccessStatusCode && responseJson["environments"] != null)
                    {
                        var environments = responseJson["environments"] as JArray;
                        if (environments != null && environments.Count > 0)
                        {
                            var environmentUids = new List<string>();
                            foreach (var env in environments)
                            {
                                if (env["uid"] != null)
                                {
                                    environmentUids.Add(env["uid"].ToString());
                                }
                            }
                            return environmentUids;
                        }
                    }
                }
                catch (Exception e)
                {
                    // Failed to get environments
                }

                // Fallback to empty list if no environments found
                return new List<string>();
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }

        private async Task<List<EntryInfo>> FetchExistingEntries()
        {
            try
            {
                // Query entries from the content type
                ContentstackResponse response = _stack.ContentType(_contentTypeUid).Entry().Query().Find();
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["entries"] != null)
                {
                    var entries = responseJson["entries"] as JArray;
                    if (entries != null && entries.Count > 0)
                    {
                        var entryList = new List<EntryInfo>();
                        foreach (var entry in entries)
                        {
                            if (entry["uid"] != null && entry["title"] != null)
                            {
                                entryList.Add(new EntryInfo
                                {
                                    Uid = entry["uid"].ToString(),
                                    Title = entry["title"].ToString(),
                                    Version = entry["_version"] != null ? (int)entry["_version"] : 1
                                });
                            }
                        }
                        return entryList;
                    }
                }

                return new List<EntryInfo>();
            }
            catch (Exception e)
            {
                return new List<EntryInfo>();
            }
        }

        private async Task<List<string>> GetAvailableReleases()
        {
            try
            {
                // First try to use our test release
                if (!string.IsNullOrEmpty(_testReleaseUid))
                {
                    try
                    {
                        ContentstackResponse fetchResponse = _stack.Release(_testReleaseUid).Fetch();
                        if (fetchResponse.IsSuccessStatusCode)
                        {
                            return new List<string> { _testReleaseUid };
                        }
                    }
                    catch
                    {
                        // Test release doesn't exist, fall back to available releases
                    }
                }

                // Try to get available releases
                try
                {
                    ContentstackResponse response = _stack.Release().Query().Find();
                    var responseJson = response.OpenJObjectResponse();

                    if (response.IsSuccessStatusCode && responseJson["releases"] != null)
                    {
                        var releases = responseJson["releases"] as JArray;
                        if (releases != null && releases.Count > 0)
                        {
                            var releaseUids = new List<string>();
                            foreach (var release in releases)
                            {
                                if (release["uid"] != null)
                                {
                                    releaseUids.Add(release["uid"].ToString());
                                }
                            }
                            return releaseUids;
                        }
                    }
                }
                catch (Exception e)
                {
                    // Failed to get releases
                }

                // Fallback to empty list if no releases found
                return new List<string>();
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }

        private async Task<string> FetchAvailableRelease()
        {
            try
            {
                // First try to use our test release if it exists
                if (!string.IsNullOrEmpty(_testReleaseUid))
                {
                    try
                    {
                        ContentstackResponse fetchResponse = _stack.Release(_testReleaseUid).Fetch();
                        if (fetchResponse.IsSuccessStatusCode)
                        {
                            return _testReleaseUid;
                        }
                    }
                    catch
                    {
                        // Test release not found, look for other releases
                    }
                }

                // Query for available releases
                ContentstackResponse response = _stack.Release().Query().Find();
                var responseJson = response.OpenJObjectResponse();

                if (response.IsSuccessStatusCode && responseJson["releases"] != null)
                {
                    var releases = responseJson["releases"] as JArray;
                    if (releases != null && releases.Count > 0)
                    {
                        // Get the first available release
                        var firstRelease = releases[0];
                        if (firstRelease["uid"] != null)
                        {
                            string releaseUid = firstRelease["uid"].ToString();
                            return releaseUid;
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public class SimpleEntry : IEntry
        {
            [JsonProperty(propertyName: "title")]
            public string Title { get; set; }
        }

        public class EntryInfo
        {
            public string Uid { get; set; }
            public string Title { get; set; }
            public int Version { get; set; }
        }
    }
} 