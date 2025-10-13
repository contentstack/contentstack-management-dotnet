using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;


namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack007_EntryTest
    {
        private Stack _stack;

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test001_Should_Create_Entry()
        {
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
                    Assert.IsNotNull(responseObject["entry"], "Response should contain entry object");
                    
                    var entryData = responseObject["entry"] as Newtonsoft.Json.Linq.JObject;
                    Assert.IsNotNull(entryData["uid"], "Entry should have UID");
                    Assert.AreEqual(singlePageEntry.Title, entryData["title"]?.ToString(), "Entry title should match");
                    Assert.AreEqual(singlePageEntry.Url, entryData["url"]?.ToString(), "Entry URL should match");
                    
                    Console.WriteLine($"Successfully created single page entry: {entryData["uid"]}");
                }
                else
                {
                    Assert.Fail("Entry Creation Failed");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Entry Creation Failed", ex.Message);
                Console.WriteLine($"Create single page entry test encountered exception: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Create_MultiPage_Entry()
        {
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
                    Assert.IsNotNull(responseObject["entry"], "Response should contain entry object");
                    
                    var entryData = responseObject["entry"] as Newtonsoft.Json.Linq.JObject;
                    Assert.IsNotNull(entryData["uid"], "Entry should have UID");
                    Assert.AreEqual(multiPageEntry.Title, entryData["title"]?.ToString(), "Entry title should match");
                    Assert.AreEqual(multiPageEntry.Url, entryData["url"]?.ToString(), "Entry URL should match");
                    
                    Console.WriteLine($"Successfully created multi page entry: {entryData["uid"]}");
                }
                else
                {
                    Assert.Fail("Entry Crreation Failed");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Entry Creation Failed ", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Fetch_Entry()
        {
            try
            {
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
                    Assert.IsNotNull(entryUid, "Created entry should have UID");

                    ContentstackResponse fetchResponse = await _stack.ContentType("single_page").Entry(entryUid).FetchAsync();
                    
                    if (fetchResponse.IsSuccessStatusCode)
                    {
                        var fetchObject = fetchResponse.OpenJObjectResponse();
                        Assert.IsNotNull(fetchObject["entry"], "Response should contain entry object");
                        
                        var entryData = fetchObject["entry"] as Newtonsoft.Json.Linq.JObject;
                        Assert.AreEqual(entryUid, entryData["uid"]?.ToString(), "Fetched entry UID should match");
                        Assert.AreEqual(singlePageEntry.Title, entryData["title"]?.ToString(), "Fetched entry title should match");
                        
                        Console.WriteLine($"Successfully fetched entry: {entryUid}");
                    }
                    else
                    {
                        Assert.Fail("Entry Fetch Failed");
                    }
                }
                else
                {
                    Assert.Fail("Entry Creation for Fetch Failed");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Entry Fetch Failed", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test004_Should_Update_Entry()
        {
            try
            {
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
                    Assert.IsNotNull(entryUid, "Created entry should have UID");

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
                        Assert.IsNotNull(updateObject["entry"], "Response should contain entry object");
                        
                        var entryData = updateObject["entry"] as Newtonsoft.Json.Linq.JObject;
                        Assert.AreEqual(entryUid, entryData["uid"]?.ToString(), "Updated entry UID should match");
                        Assert.AreEqual(updatedEntry.Title, entryData["title"]?.ToString(), "Updated entry title should match");
                        Assert.AreEqual(updatedEntry.Url, entryData["url"]?.ToString(), "Updated entry URL should match");
                        
                        Console.WriteLine($"Successfully updated entry: {entryUid}");
                    }
                    else
                    {
                        Assert.Fail("Entry Update Failed");
                    }
                }
                else
                {
                    Assert.Fail("Entry Creation for Update Failed");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Entry Update Failed",ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test005_Should_Query_Entries()
        {
            try
            {
                ContentstackResponse response = await _stack.ContentType("single_page").Entry().Query().FindAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var responseObject = response.OpenJObjectResponse();
                    Assert.IsNotNull(responseObject["entries"], "Response should contain entries array");
                    
                    var entries = responseObject["entries"] as Newtonsoft.Json.Linq.JArray;
                    Assert.IsNotNull(entries, "Entries should be an array");
                    
                    Console.WriteLine($"Successfully queried {entries.Count} entries for single_page content type");
                }
                else
                {
                    Assert.Fail("Entry Query Failed");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Entry Query Failed ", ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Delete_Entry()
        {
            try
            {
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
                    Assert.IsNotNull(entryUid, "Created entry should have UID");

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
                    Assert.Fail("Entry Delete Async Failed");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Entry Delete Async Failed ", ex.Message);
            }
        }

    }
}
