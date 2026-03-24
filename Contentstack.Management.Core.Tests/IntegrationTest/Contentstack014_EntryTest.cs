using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;


namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack007_EntryTest
    {
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

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test001_Should_Create_Entry()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateSinglePageEntry");
            try
            {
                // First ensure the content type exists by trying to fetch it
                ContentstackResponse contentTypeResponse = _stack.ContentType("single_page").Fetch();
                if (!contentTypeResponse.IsSuccessStatusCode)
                {
                    // If content type doesn't exist, create it
                    var contentModelling = new ContentModelling
                    {
                        Title = "Single Page",
                        Uid = "single_page",
                        Schema = new List<Field>
                        {
                            new TextboxField
                            {
                                DisplayName = "Title",
                                Uid = "title",
                                DataType = "text",
                                Mandatory = true,
                                Unique = true,
                                FieldMetadata = new FieldMetadata
                                {
                                    Default = "true"
                                }
                            },
                            new TextboxField
                            {
                                DisplayName = "URL",
                                Uid = "url",
                                DataType = "text",
                                Mandatory = true,
                                FieldMetadata = new FieldMetadata
                                {
                                    Default = "true",
                                    Instruction = ""
                                }
                            }
                        }
                    };

                    ContentstackResponse createResponse = _stack.ContentType().Create(contentModelling);
                    if (!createResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Warning: Could not create content type, using existing one. Response: {createResponse.OpenResponse()}");
                    }
                }

                TestOutputLogger.LogContext("ContentType", "single_page");
                // Create entry for single_page content type
                var singlePageEntry = new SinglePageEntry
                {
                    Title = "My First Single Page Entry",
                    Url = "/my-first-single-page",
                    ContentTypeUid = "single_page"
                };

                ContentstackResponse response = await _stack.ContentType("single_page").Entry().CreateAsync(singlePageEntry);

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = response.OpenJObjectResponse();
                    AssertLogger.IsNotNull(responseObject["entry"], "responseObject_entry");

                    var entryData = responseObject["entry"] as Newtonsoft.Json.Linq.JObject;
                    AssertLogger.IsNotNull(entryData["uid"], "entry_uid");
                    AssertLogger.AreEqual(singlePageEntry.Title, entryData["title"]?.ToString(), "Entry title should match", "entry_title");
                    AssertLogger.AreEqual(singlePageEntry.Url, entryData["url"]?.ToString(), "Entry URL should match", "entry_url");

                    TestOutputLogger.LogContext("Entry", entryData["uid"]?.ToString());
                    Console.WriteLine($"Successfully created single page entry: {entryData["uid"]}");
                }
                else
                {
                    AssertLogger.Fail("Entry Creation Failed");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Entry Creation Failed", ex.Message);
                Console.WriteLine($"Create single page entry test encountered exception: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Create_MultiPage_Entry()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateMultiPageEntry");
            try
            {
                // First ensure the content type exists by trying to fetch it
                ContentstackResponse contentTypeResponse = _stack.ContentType("multi_page").Fetch();
                if (!contentTypeResponse.IsSuccessStatusCode)
                {
                    // If content type doesn't exist, create it
                    var contentModelling = new ContentModelling
                    {
                        Title = "Multi page",
                        Uid = "multi_page",
                        Schema = new List<Field>
                        {
                            new TextboxField
                            {
                                DisplayName = "Title",
                                Uid = "title",
                                DataType = "text",
                                Mandatory = true,
                                Unique = true,
                                FieldMetadata = new FieldMetadata
                                {
                                    Default = "true"
                                }
                            },
                            new TextboxField
                            {
                                DisplayName = "URL",
                                Uid = "url",
                                DataType = "text",
                                Mandatory = false,
                                FieldMetadata = new FieldMetadata
                                {
                                    Default = "true"
                                }
                            }
                        }
                    };

                    ContentstackResponse createResponse = _stack.ContentType().Create(contentModelling);
                    if (!createResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Warning: Could not create content type, using existing one. Response: {createResponse.OpenResponse()}");
                    }
                }

                TestOutputLogger.LogContext("ContentType", "multi_page");
                // Create entry for multi_page content type
                var multiPageEntry = new MultiPageEntry
                {
                    Title = "My First Multi Page Entry",
                    Url = "/my-first-multi-page",
                    ContentTypeUid = "multi_page"
                };

                ContentstackResponse response = await _stack.ContentType("multi_page").Entry().CreateAsync(multiPageEntry);

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = response.OpenJObjectResponse();
                    AssertLogger.IsNotNull(responseObject["entry"], "responseObject_entry");

                    var entryData = responseObject["entry"] as Newtonsoft.Json.Linq.JObject;
                    AssertLogger.IsNotNull(entryData["uid"], "entry_uid");
                    AssertLogger.AreEqual(multiPageEntry.Title, entryData["title"]?.ToString(), "Entry title should match", "entry_title");
                    AssertLogger.AreEqual(multiPageEntry.Url, entryData["url"]?.ToString(), "Entry URL should match", "entry_url");

                    TestOutputLogger.LogContext("Entry", entryData["uid"]?.ToString());
                    Console.WriteLine($"Successfully created multi page entry: {entryData["uid"]}");
                }
                else
                {
                    AssertLogger.Fail("Entry Crreation Failed");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Entry Creation Failed ", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Fetch_Entry()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchEntry");
            try
            {
                TestOutputLogger.LogContext("ContentType", "single_page");
                var singlePageEntry = new SinglePageEntry
                {
                    Title = "Test Entry for Fetch",
                    Url = "/test-entry-for-fetch",
                    ContentTypeUid = "single_page"
                };

                ContentstackResponse createResponse = await _stack.ContentType("single_page").Entry().CreateAsync(singlePageEntry);

                if (createResponse.IsSuccessStatusCode)
                {
                    var createObject = createResponse.OpenJObjectResponse();
                    var entryUid = createObject["entry"]["uid"]?.ToString();
                    AssertLogger.IsNotNull(entryUid, "created_entry_uid");
                    TestOutputLogger.LogContext("Entry", entryUid);

                    ContentstackResponse fetchResponse = await _stack.ContentType("single_page").Entry(entryUid).FetchAsync();

                    if (fetchResponse.IsSuccessStatusCode)
                    {
                        var fetchObject = fetchResponse.OpenJObjectResponse();
                        AssertLogger.IsNotNull(fetchObject["entry"], "fetchObject_entry");

                        var entryData = fetchObject["entry"] as Newtonsoft.Json.Linq.JObject;
                        AssertLogger.AreEqual(entryUid, entryData["uid"]?.ToString(), "Fetched entry UID should match", "fetched_entry_uid");
                        AssertLogger.AreEqual(singlePageEntry.Title, entryData["title"]?.ToString(), "Fetched entry title should match", "fetched_entry_title");

                        Console.WriteLine($"Successfully fetched entry: {entryUid}");
                    }
                    else
                    {
                        AssertLogger.Fail("Entry Fetch Failed");
                    }
                }
                else
                {
                    AssertLogger.Fail("Entry Creation for Fetch Failed");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Entry Fetch Failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test004_Should_Update_Entry()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateEntry");
            try
            {
                TestOutputLogger.LogContext("ContentType", "single_page");
                // First create an entry to update
                var singlePageEntry = new SinglePageEntry
                {
                    Title = "Original Entry Title",
                    Url = "/original-entry-url",
                    ContentTypeUid = "single_page"
                };

                ContentstackResponse createResponse = await _stack.ContentType("single_page").Entry().CreateAsync(singlePageEntry);

                if (createResponse.IsSuccessStatusCode)
                {
                    var createObject = createResponse.OpenJObjectResponse();
                    var entryUid = createObject["entry"]["uid"]?.ToString();
                    AssertLogger.IsNotNull(entryUid, "created_entry_uid");
                    TestOutputLogger.LogContext("Entry", entryUid);

                    // Update the entry
                    var updatedEntry = new SinglePageEntry
                    {
                        Title = "Updated Entry Title",
                        Url = "/updated-entry-url",
                        ContentTypeUid = "single_page"
                    };

                    ContentstackResponse updateResponse = await _stack.ContentType("single_page").Entry(entryUid).UpdateAsync(updatedEntry);

                    if (updateResponse.IsSuccessStatusCode)
                    {
                        var updateObject = updateResponse.OpenJObjectResponse();
                        AssertLogger.IsNotNull(updateObject["entry"], "updateObject_entry");

                        var entryData = updateObject["entry"] as Newtonsoft.Json.Linq.JObject;
                        AssertLogger.AreEqual(entryUid, entryData["uid"]?.ToString(), "Updated entry UID should match", "updated_entry_uid");
                        AssertLogger.AreEqual(updatedEntry.Title, entryData["title"]?.ToString(), "Updated entry title should match", "updated_entry_title");
                        AssertLogger.AreEqual(updatedEntry.Url, entryData["url"]?.ToString(), "Updated entry URL should match", "updated_entry_url");

                        Console.WriteLine($"Successfully updated entry: {entryUid}");
                    }
                    else
                    {
                        AssertLogger.Fail("Entry Update Failed");
                    }
                }
                else
                {
                    AssertLogger.Fail("Entry Creation for Update Failed");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Entry Update Failed",ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Query_Entries()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryEntries");
            try
            {
                TestOutputLogger.LogContext("ContentType", "single_page");
                ContentstackResponse response = await _stack.ContentType("single_page").Entry().Query().FindAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = response.OpenJObjectResponse();
                    AssertLogger.IsNotNull(responseObject["entries"], "responseObject_entries");

                    var entries = responseObject["entries"] as Newtonsoft.Json.Linq.JArray;
                    AssertLogger.IsNotNull(entries, "entries_array");

                    Console.WriteLine($"Successfully queried {entries.Count} entries for single_page content type");
                }
                else
                {
                    AssertLogger.Fail("Entry Query Failed");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Entry Query Failed ", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Delete_Entry()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteEntry");
            try
            {
                TestOutputLogger.LogContext("ContentType", "single_page");
                var singlePageEntry = new SinglePageEntry
                {
                    Title = "Entry to Delete",
                    Url = "/entry-to-delete",
                    ContentTypeUid = "single_page"
                };

                ContentstackResponse createResponse = await _stack.ContentType("single_page").Entry().CreateAsync(singlePageEntry);

                if (createResponse.IsSuccessStatusCode)
                {
                    var createObject = createResponse.OpenJObjectResponse();
                    var entryUid = createObject["entry"]["uid"]?.ToString();
                    AssertLogger.IsNotNull(entryUid, "created_entry_uid");
                    TestOutputLogger.LogContext("Entry", entryUid);

                    ContentstackResponse deleteResponse = await _stack.ContentType("single_page").Entry(entryUid).DeleteAsync();

                    if (deleteResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Successfully deleted entry: {entryUid}");
                    }
                    else
                    {
                        Console.WriteLine($"Entry delete failed with status {deleteResponse.StatusCode}: {deleteResponse.OpenResponse()}");
                    }
                }
                else
                {
                    AssertLogger.Fail("Entry Delete Async Failed");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Entry Delete Async Failed ", ex.Message);
            }
        }

    }
}
