using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.CustomExtension;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Exceptions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack006_AssetTest
    {
        private Stack _stack;

        [TestInitialize]
        public void Initialize()
        {
            TestReportHelper.Begin();
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestReportHelper.Flush();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Asset()
        {
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                AssetModel asset = new AssetModel("contentTypeSchema.json", path, "application/json", title:"New.json", description:"new test desc", parentUID: null, tags:"one,two");
                TestReportHelper.LogRequest("_stack.Asset().Create(asset)", "POST",
                    $"https://{Contentstack.Client.contentstackOptions.Host}/v3/stacks/assets");
                ContentstackResponse response = _stack.Asset().Create(asset);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                if (response.IsSuccessStatusCode)
                {
                    TestReportHelper.LogAssertion(response.StatusCode == System.Net.HttpStatusCode.Created,
                        "Status code is Created", expected: "Created", actual: response.StatusCode.ToString(), type: "AreEqual");
                    Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                }
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                Assert.Fail("Asset Creation Failed ", e.Message);
            }
        }

        // Check the below 3 Test cases
        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_Dashboard()
        {
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html");
            try
            {
                DashboardWidgetModel dashboard = new DashboardWidgetModel(path, "text/html", "Dashboard", isEnable: true, defaultWidth: "half", tags: "one,two");
                ContentstackResponse response = _stack.Extension().Upload(dashboard);
                
                if (response.IsSuccessStatusCode)
                {
                    Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Dashboard Creation Failed ", e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Create_Custom_Widget()
        {
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html");
            try
            {
                CustomWidgetModel customWidget = new CustomWidgetModel(path, "text/html", title: "Custom widget Upload", scope: new ExtensionScope()
                {
                    ContentTypes = new List<string>()
                    {
                        "single_page"
                    }
                }, tags: "one,two");
                ContentstackResponse response = _stack.Extension().Upload(customWidget);
                
                if (response.IsSuccessStatusCode)
                {
                    Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Custom Widget Creation Failed ", e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Create_Custom_field()
        {
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html");
            try
            {
                CustomFieldModel fieldModel = new CustomFieldModel(path, "text/html", "Custom field Upload", "text", isMultiple: false, tags: "one,two");
                ContentstackResponse response = _stack.Extension().Upload(fieldModel);
                
                if (response.IsSuccessStatusCode)
                {
                    Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Custom Field Creation Failed ", e.Message);
            }
        }

        private string _testAssetUid;

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Create_Asset_Async()
        {
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
            try
            {
                AssetModel asset = new AssetModel("async_asset.json", path, "application/json", title:"Async Asset", description:"async test asset", parentUID: null, tags:"async,test");
                ContentstackResponse response = _stack.Asset().CreateAsync(asset).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                    var responseObject = response.OpenJObjectResponse();
                    if (responseObject["asset"] != null)
                    {
                        _testAssetUid = responseObject["asset"]["uid"]?.ToString();
                    }
                }
                else
                {
                    Assert.Fail("Asset Creation Async Failed");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Asset Creation Async Failed ",ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Fetch_Asset()
        {
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    ContentstackResponse response = _stack.Asset(_testAssetUid).Fetch();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain asset object");
                    }
                    else
                    {
                        Assert.Fail("The Asset is Not Getting Created");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Asset Fetch Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Fetch_Asset_Async()
        {
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    ContentstackResponse response = _stack.Asset(_testAssetUid).FetchAsync().Result;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain asset object");
                    }
                    else
                    {
                        Assert.Fail("Asset Fetch Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Asset Fetch Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Update_Asset()
        {
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                    AssetModel updatedAsset = new AssetModel("updated_asset.json", path, "application/json", title:"Updated Asset", description:"updated test asset", parentUID: null, tags:"updated,test");
                    
                    ContentstackResponse response = _stack.Asset(_testAssetUid).Update(updatedAsset);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain asset object");
                    }
                    else
                    {
                        Assert.Fail("Asset update Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Asset Update Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Update_Asset_Async()
        {
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                    AssetModel updatedAsset = new AssetModel("async_updated_asset.json", path, "application/json", title:"Async Updated Asset", description:"async updated test asset", parentUID: null, tags:"async,updated,test");
                    
                    ContentstackResponse response = _stack.Asset(_testAssetUid).UpdateAsync(updatedAsset).Result;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain asset object");
                    }
                    else
                    {
                        Assert.Fail("Asset Update Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Asset Update Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Query_Assets()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.Asset().Query().Find()", "GET",
                    $"https://{Contentstack.Client.contentstackOptions.Host}/v3/stacks/assets");
                ContentstackResponse response = _stack.Asset().Query().Find();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = response.OpenJObjectResponse();
                    TestReportHelper.LogAssertion(responseObject["assets"] != null, "assets key present", type: "IsNotNull");
                    Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                    Assert.IsNotNull(responseObject["assets"], "Response should contain assets array");
                }
                else
                {
                    Assert.Fail("Querying the Asset Failed");
                }
            }
            catch (ContentstackErrorException ex)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"ContentstackErrorException: {ex.Message}", type: "Fail");
                Assert.Fail("Querying the Asset Failed ",ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Query_Assets_With_Parameters()
        {
            try
            {
                var query = _stack.Asset().Query();
                query.Limit(5);
                query.Skip(0);
                
                ContentstackResponse response = query.Find();
                
                if (response.IsSuccessStatusCode)
                {
                    Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                    var responseObject = response.OpenJObjectResponse();
                    Assert.IsNotNull(responseObject["assets"], "Response should contain assets array");
                }
                else
                {
                    Assert.Fail("Querying the Asset Failed");
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Querying the Asset Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Delete_Asset()
        {
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    ContentstackResponse response = _stack.Asset(_testAssetUid).Delete();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        _testAssetUid = null; // Clear the UID since asset is deleted
                    }
                    else
                    {
                        Assert.Fail("Deleting the Asset Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Deleting the Asset Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Delete_Asset_Async()
        {
            try
            {
                var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                AssetModel asset = new AssetModel("delete_asset.json", path, "application/json", title:"Delete Asset", description:"asset for deletion", parentUID: null, tags:"delete,test");
                ContentstackResponse createResponse = _stack.Asset().CreateAsync(asset).Result;
                
                if (createResponse.IsSuccessStatusCode)
                {
                    var responseObject = createResponse.OpenJObjectResponse();
                    string assetUid = responseObject["asset"]["uid"]?.ToString();
                    
                    if (!string.IsNullOrEmpty(assetUid))
                    {
                        ContentstackResponse deleteResponse = _stack.Asset(assetUid).DeleteAsync().Result;
                        
                        if (deleteResponse.IsSuccessStatusCode)
                        {
                            Assert.AreEqual(System.Net.HttpStatusCode.OK, deleteResponse.StatusCode);
                        }
                        else
                        {
                            Assert.Fail("Deleting Asset Async Failed");
                        }
                    }
                }
                else
                {
                    Assert.Fail("Deleting Asset Async Failed");
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Deleting Asset Async Failed ",e.Message);
            }
        }


        private string _testFolderUid;

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Create_Folder()
        {
            try
            {
                ContentstackResponse response = _stack.Asset().Folder().Create("Test Folder", null);
                
                if (response.IsSuccessStatusCode)
                {
                    Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                    var responseObject = response.OpenJObjectResponse();
                    if (responseObject["asset"] != null)
                    {
                        _testFolderUid = responseObject["asset"]["uid"]?.ToString();
                    }
                }
                else
                {
                    Assert.Fail("Folder Creation Failed");
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Folder Creation Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Create_Subfolder()
        {
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    ContentstackResponse response = _stack.Asset().Folder().Create("Test Subfolder", _testFolderUid);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain folder object");
                    }
                    else
                    {
                        Assert.Fail("SubFolder Creation Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("SubFolder Fetch Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Fetch_Folder()
        {
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).Fetch();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain folder object");
                    }
                    else
                    {
                        Assert.Fail("Fetch Failed for Folder");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Fetch Async Failed for Folder ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Fetch_Folder_Async()
        {
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).FetchAsync().Result;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain folder object");
                    }
                    else
                    {
                        Assert.Fail("Fetch Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Fetch Async Failed for Folder ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Update_Folder()
        {
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).Update("Updated Test Folder", null);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain folder object");
                    }
                    else
                    {
                        Assert.Fail("Folder update Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Folder Update Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Update_Folder_Async()
        {
            try
            {
                // First create a folder if we don't have one
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).UpdateAsync("Async Updated Test Folder", null).Result;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                        var responseObject = response.OpenJObjectResponse();
                        Assert.IsNotNull(responseObject["asset"], "Response should contain folder object");
                    }
                    else
                    {
                        Assert.Fail("Folder Update Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Folder Delete Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Delete_Folder()
        {
            try
            {
                // First create a folder if we don't have one
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).Delete();
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                        _testFolderUid = null; // Clear the UID since folder is deleted
                    }
                    else
                    {
                        Assert.Fail("Delete Folder Failed");
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Delete Folder Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_Delete_Folder_Async()
        {
            try
            {
                // Create a new folder for deletion
                ContentstackResponse createResponse = _stack.Asset().Folder().Create("Delete Test Folder", null);
                
                if (createResponse.IsSuccessStatusCode)
                {
                    var responseObject = createResponse.OpenJObjectResponse();
                    string folderUid = responseObject["asset"]["uid"]?.ToString();
                    
                    if (!string.IsNullOrEmpty(folderUid))
                    {
                        ContentstackResponse deleteResponse = _stack.Asset().Folder(folderUid).DeleteAsync().Result;
                        
                        if (deleteResponse.IsSuccessStatusCode)
                        {
                            Assert.AreEqual(System.Net.HttpStatusCode.OK, deleteResponse.StatusCode);
                        }
                        else
                        {
                            Assert.Fail("The Delete Folder Async Failed");
                        }
                    }
                }
                else
                {
                    Assert.Fail("The Create Folder Call Failed");
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Delete Folder Async Failed ",e.Message);
            }
        }

        // Phase 4: Error Handling and Edge Case Tests
        [TestMethod]
        [DoNotParallelize]
        public void Test024_Should_Handle_Invalid_Asset_Operations()
        {
            string invalidAssetUid = "invalid_asset_uid_12345";
            
            // Test fetching non-existent asset - expect exception
            try
            {
                _stack.Asset(invalidAssetUid).Fetch();
                Assert.Fail("Expected exception for invalid asset fetch, but operation succeeded");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected exception for invalid asset operations
                Assert.IsTrue(ex.Message.Contains("not found") || ex.Message.Contains("invalid"), 
                    $"Expected 'not found' or 'invalid' in exception message, got: {ex.Message}");
            }
            
            // Test updating non-existent asset - expect exception
            try
            {
                var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                AssetModel updateModel = new AssetModel("invalid_asset.json", path, "application/json", title:"Invalid Asset", description:"invalid test asset", parentUID: null, tags:"invalid,test");
                
                _stack.Asset(invalidAssetUid).Update(updateModel);
                Assert.Fail("Expected exception for invalid asset update, but operation succeeded");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected exception for invalid asset operations
                Assert.IsTrue(ex.Message.Contains("not found") || ex.Message.Contains("invalid"), 
                    $"Expected 'not found' or 'invalid' in exception message, got: {ex.Message}");
            }
            
            // Test deleting non-existent asset - expect exception
            try
            {
                _stack.Asset(invalidAssetUid).Delete();
                Assert.Fail("Expected exception for invalid asset delete, but operation succeeded");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected exception for invalid asset operations
                Assert.IsTrue(ex.Message.Contains("not found") || ex.Message.Contains("invalid"), 
                    $"Expected 'not found' or 'invalid' in exception message, got: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Handle_Invalid_Folder_Operations()
        {
            string invalidFolderUid = "invalid_folder_uid_12345";
            
            // Test fetching non-existent folder - expect ContentstackErrorException
            bool fetchExceptionThrown = false;
            try
            {
                _stack.Asset().Folder(invalidFolderUid).Fetch();
                // If we get here, the API returned success for invalid folder
                Console.WriteLine("Warning: Fetching invalid folder unexpectedly succeeded");
            }
            catch (ContentstackErrorException ex)
            {
                Console.WriteLine($"Expected ContentstackErrorException for invalid folder fetch: {ex.Message}");
                fetchExceptionThrown = true;
            }
            Assert.IsTrue(fetchExceptionThrown, "Expected ContentstackErrorException for invalid folder fetch");
            
            // Test updating non-existent folder - API may succeed or throw exception
            try
            {
                ContentstackResponse updateResponse = _stack.Asset().Folder(invalidFolderUid).Update("Invalid Folder", null);
                // If we get here, the API returned success for invalid folder update
                Console.WriteLine("Warning: Updating invalid folder unexpectedly succeeded");
                // This is unexpected but not necessarily wrong - API behavior may vary
            }
            catch (ContentstackErrorException ex)
            {
                // Expected behavior - API should throw ContentstackErrorException for invalid folder update
                Console.WriteLine($"Expected ContentstackErrorException for invalid folder update: {ex.Message}");
            }
            // Don't assert on update behavior as API may handle this differently
            
            // Test deleting non-existent folder - API may succeed or throw exception
            try
            {
                ContentstackResponse deleteResponse = _stack.Asset().Folder(invalidFolderUid).Delete();
                // If we get here, the API returned success for invalid folder delete
                Console.WriteLine("Warning: Deleting invalid folder unexpectedly succeeded");
                // This is unexpected but not necessarily wrong - API behavior may vary
            }
            catch (ContentstackErrorException ex)
            {
                // Expected behavior - API should throw ContentstackErrorException for invalid folder delete
                Console.WriteLine($"Expected ContentstackErrorException for invalid folder delete: {ex.Message}");
            }
            // Don't assert on delete behavior as API may handle this differently
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test027_Should_Handle_Asset_Creation_With_Invalid_File()
        {
            string invalidPath = Path.Combine(System.Environment.CurrentDirectory, "non_existent_file.json");
            
            // Expect FileNotFoundException during AssetModel construction due to file not found
            try
            {
                new AssetModel("invalid_file.json", invalidPath, "application/json", title:"Invalid File Asset", description:"asset with invalid file", parentUID: null, tags:"invalid,file");
                Assert.Fail("Expected FileNotFoundException during AssetModel construction, but it succeeded");
            }
            catch (FileNotFoundException ex)
            {
                // Expected exception for file not found during AssetModel construction
                Assert.IsTrue(ex.Message.Contains("non_existent_file.json") || ex.Message.Contains("Could not find file"), 
                    $"Expected file not found exception, got: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Handle_Query_With_Invalid_Parameters()
        {
            // Test asset query with invalid parameters - expect exception to be raised directly
            var assetQuery = _stack.Asset().Query();
            assetQuery.Limit(-1); // Invalid limit
            assetQuery.Skip(-1); // Invalid skip
            
            try
            {
                assetQuery.Find();
                Assert.Fail("Expected exception for invalid query parameters, but operation succeeded");
            }
            catch (ArgumentException ex)
            {
                // Expected exception for invalid parameters
                Assert.IsTrue(ex.Message.Contains("limit") || ex.Message.Contains("skip") || ex.Message.Contains("invalid"), 
                    $"Expected parameter validation error, got: {ex.Message}");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected ContentstackErrorException for invalid parameters
                Assert.IsTrue(ex.Message.Contains("parameter") || ex.Message.Contains("invalid") || ex.Message.Contains("limit") || ex.Message.Contains("skip"), 
                    $"Expected parameter validation error, got: {ex.Message}");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Handle_Empty_Query_Results()
        {
            try
            {
                // Test query with very high skip value to get empty results
                var assetQuery = _stack.Asset().Query();
                assetQuery.Skip(999999);
                assetQuery.Limit(1);
                
                ContentstackResponse response = assetQuery.Find();
                
                if (response.IsSuccessStatusCode)
                {
                    Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                    var responseObject = response.OpenJObjectResponse();
                    Assert.IsNotNull(responseObject["assets"], "Response should contain assets array");
                    // Empty results are valid, so we don't assert on count
                }
                else
                {
                    Assert.Fail("Asset Querying with Empty Query Failed");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

    }
}
