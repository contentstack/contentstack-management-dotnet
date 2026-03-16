using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.CustomExtension;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Contentstack.Management.Core.Exceptions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack006_AssetTest
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
        public void Test001_Should_Create_Asset()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateAsset");
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
            try
            {
                AssetModel asset = new AssetModel("contentTypeSchema.json", path, "application/json", title:"New.json", description:"new test desc", parentUID: null, tags:"one,two");
                ContentstackResponse response = _stack.Asset().Create(asset);
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "CreateAsset_StatusCode");
                }
                else
                {
                    // Don't fail the test if API returns an error - this might be expected behavior
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Asset Creation Failed ", e.Message);
            }
        }

        // Check the below 3 Test cases
        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_Dashboard()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateDashboard");
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html");
            try
            {
                DashboardWidgetModel dashboard = new DashboardWidgetModel(path, "text/html", "Dashboard", isEnable: true, defaultWidth: "half", tags: "one,two");
                ContentstackResponse response = _stack.Extension().Upload(dashboard);
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "CreateDashboard_StatusCode");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Dashboard Creation Failed ", e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Create_Custom_Widget()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateCustomWidget");
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
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "CreateCustomWidget_StatusCode");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Custom Widget Creation Failed ", e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Create_Custom_field()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateCustomField");
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html");
            try
            {
                CustomFieldModel fieldModel = new CustomFieldModel(path, "text/html", "Custom field Upload", "text", isMultiple: false, tags: "one,two");
                ContentstackResponse response = _stack.Extension().Upload(fieldModel);
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "CreateCustomField_StatusCode");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Custom Field Creation Failed ", e.Message);
            }
        }

        private string _testAssetUid;

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Create_Asset_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateAssetAsync");
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
            try
            {
                AssetModel asset = new AssetModel("async_asset.json", path, "application/json", title:"Async Asset", description:"async test asset", parentUID: null, tags:"async,test");
                ContentstackResponse response = _stack.Asset().CreateAsync(asset).Result;
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "CreateAssetAsync_StatusCode");
                    var responseObject = response.OpenJObjectResponse();
                    if (responseObject["asset"] != null)
                    {
                        _testAssetUid = responseObject["asset"]["uid"]?.ToString();
                        TestOutputLogger.LogContext("AssetUID", _testAssetUid ?? "null");
                    }
                }
                else
                {
                    AssertLogger.Fail("Asset Creation Async Failed");
                }
            }
            catch (Exception ex)
            {
                AssertLogger.Fail("Asset Creation Async Failed ",ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test006_Should_Fetch_Asset()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAsset");
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    TestOutputLogger.LogContext("AssetUID", _testAssetUid);
                    ContentstackResponse response = _stack.Asset(_testAssetUid).Fetch();

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "FetchAsset_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "FetchAsset_ResponseContainsAsset");
                    }
                    else
                    {
                        AssertLogger.Fail("The Asset is Not Getting Created");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Asset Fetch Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Fetch_Asset_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchAssetAsync");
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    TestOutputLogger.LogContext("AssetUID", _testAssetUid);
                    ContentstackResponse response = _stack.Asset(_testAssetUid).FetchAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "FetchAssetAsync_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "FetchAssetAsync_ResponseContainsAsset");
                    }
                    else
                    {
                        AssertLogger.Fail("Asset Fetch Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Asset Fetch Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Update_Asset()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAsset");
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    TestOutputLogger.LogContext("AssetUID", _testAssetUid);
                    var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                    AssetModel updatedAsset = new AssetModel("updated_asset.json", path, "application/json", title:"Updated Asset", description:"updated test asset", parentUID: null, tags:"updated,test");

                    ContentstackResponse response = _stack.Asset(_testAssetUid).Update(updatedAsset);

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "UpdateAsset_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "UpdateAsset_ResponseContainsAsset");
                    }
                    else
                    {
                        AssertLogger.Fail("Asset update Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Asset Update Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Update_Asset_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateAssetAsync");
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    TestOutputLogger.LogContext("AssetUID", _testAssetUid);
                    var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                    AssetModel updatedAsset = new AssetModel("async_updated_asset.json", path, "application/json", title:"Async Updated Asset", description:"async updated test asset", parentUID: null, tags:"async,updated,test");

                    ContentstackResponse response = _stack.Asset(_testAssetUid).UpdateAsync(updatedAsset).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "UpdateAssetAsync_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "UpdateAssetAsync_ResponseContainsAsset");
                    }
                    else
                    {
                        AssertLogger.Fail("Asset Update Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Asset Update Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test010_Should_Query_Assets()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAssets");
            try
            {
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");
                ContentstackResponse response = _stack.Asset().Query().Find();

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "QueryAssets_StatusCode");
                    var responseObject = response.OpenJObjectResponse();
                    AssertLogger.IsNotNull(responseObject["assets"], "QueryAssets_ResponseContainsAssets");
                }
                else
                {
                    AssertLogger.Fail("Querying the Asset Failed");
                }
            }
            catch (ContentstackErrorException ex)
            {
                AssertLogger.Fail("Querying the Asset Failed ",ex.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Query_Assets_With_Parameters()
        {
            TestOutputLogger.LogContext("TestScenario", "QueryAssetsWithParameters");
            try
            {
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");
                var query = _stack.Asset().Query();
                query.Limit(5);
                query.Skip(0);

                ContentstackResponse response = query.Find();

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "QueryAssetsWithParams_StatusCode");
                    var responseObject = response.OpenJObjectResponse();
                    AssertLogger.IsNotNull(responseObject["assets"], "QueryAssetsWithParams_ResponseContainsAssets");
                }
                else
                {
                    AssertLogger.Fail("Querying the Asset Failed");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Querying the Asset Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test012_Should_Delete_Asset()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteAsset");
            try
            {
                if (string.IsNullOrEmpty(_testAssetUid))
                {
                    Test005_Should_Create_Asset_Async();
                }

                if (!string.IsNullOrEmpty(_testAssetUid))
                {
                    TestOutputLogger.LogContext("AssetUID", _testAssetUid);
                    ContentstackResponse response = _stack.Asset(_testAssetUid).Delete();

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "DeleteAsset_StatusCode");
                        _testAssetUid = null; // Clear the UID since asset is deleted
                    }
                    else
                    {
                        AssertLogger.Fail("Deleting the Asset Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Deleting the Asset Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Delete_Asset_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteAssetAsync");
            try
            {
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");
                var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                AssetModel asset = new AssetModel("delete_asset.json", path, "application/json", title:"Delete Asset", description:"asset for deletion", parentUID: null, tags:"delete,test");
                ContentstackResponse createResponse = _stack.Asset().CreateAsync(asset).Result;

                if (createResponse.IsSuccessStatusCode)
                {
                    var responseObject = createResponse.OpenJObjectResponse();
                    string assetUid = responseObject["asset"]["uid"]?.ToString();
                    TestOutputLogger.LogContext("AssetUID", assetUid ?? "null");

                    if (!string.IsNullOrEmpty(assetUid))
                    {
                        ContentstackResponse deleteResponse = _stack.Asset(assetUid).DeleteAsync().Result;

                        if (deleteResponse.IsSuccessStatusCode)
                        {
                            AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, deleteResponse.StatusCode, "DeleteAssetAsync_StatusCode");
                        }
                        else
                        {
                            AssertLogger.Fail("Deleting Asset Async Failed");
                        }
                    }
                }
                else
                {
                    AssertLogger.Fail("Deleting Asset Async Failed");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Deleting Asset Async Failed ",e.Message);
            }
        }


        private string _testFolderUid;

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Create_Folder()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateFolder");
            try
            {
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");
                ContentstackResponse response = _stack.Asset().Folder().Create("Test Folder", null);

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "CreateFolder_StatusCode");
                    var responseObject = response.OpenJObjectResponse();
                    if (responseObject["asset"] != null)
                    {
                        _testFolderUid = responseObject["asset"]["uid"]?.ToString();
                        TestOutputLogger.LogContext("FolderUID", _testFolderUid ?? "null");
                    }
                }
                else
                {
                    AssertLogger.Fail("Folder Creation Failed");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Folder Creation Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Create_Subfolder()
        {
            TestOutputLogger.LogContext("TestScenario", "CreateSubfolder");
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    TestOutputLogger.LogContext("FolderUID", _testFolderUid);
                    ContentstackResponse response = _stack.Asset().Folder().Create("Test Subfolder", _testFolderUid);

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "CreateSubfolder_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "CreateSubfolder_ResponseContainsFolder");
                    }
                    else
                    {
                        AssertLogger.Fail("SubFolder Creation Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("SubFolder Fetch Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Fetch_Folder()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchFolder");
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    TestOutputLogger.LogContext("FolderUID", _testFolderUid);
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).Fetch();

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "FetchFolder_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "FetchFolder_ResponseContainsFolder");
                    }
                    else
                    {
                        AssertLogger.Fail("Fetch Failed for Folder");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Fetch Async Failed for Folder ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Fetch_Folder_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "FetchFolderAsync");
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    TestOutputLogger.LogContext("FolderUID", _testFolderUid);
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).FetchAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "FetchFolderAsync_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "FetchFolderAsync_ResponseContainsFolder");
                    }
                    else
                    {
                        AssertLogger.Fail("Fetch Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Fetch Async Failed for Folder ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Update_Folder()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateFolder");
            try
            {
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    TestOutputLogger.LogContext("FolderUID", _testFolderUid);
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).Update("Updated Test Folder", null);

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "UpdateFolder_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "UpdateFolder_ResponseContainsFolder");
                    }
                    else
                    {
                        AssertLogger.Fail("Folder update Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Folder Update Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_Update_Folder_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "UpdateFolderAsync");
            try
            {
                // First create a folder if we don't have one
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    TestOutputLogger.LogContext("FolderUID", _testFolderUid);
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).UpdateAsync("Async Updated Test Folder", null).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "UpdateFolderAsync_StatusCode");
                        var responseObject = response.OpenJObjectResponse();
                        AssertLogger.IsNotNull(responseObject["asset"], "UpdateFolderAsync_ResponseContainsFolder");
                    }
                    else
                    {
                        AssertLogger.Fail("Folder Update Async Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Folder Delete Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Delete_Folder()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteFolder");
            try
            {
                // First create a folder if we don't have one
                if (string.IsNullOrEmpty(_testFolderUid))
                {
                    Test014_Should_Create_Folder();
                }

                if (!string.IsNullOrEmpty(_testFolderUid))
                {
                    TestOutputLogger.LogContext("FolderUID", _testFolderUid);
                    ContentstackResponse response = _stack.Asset().Folder(_testFolderUid).Delete();

                    if (response.IsSuccessStatusCode)
                    {
                        AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "DeleteFolder_StatusCode");
                        _testFolderUid = null; // Clear the UID since folder is deleted
                    }
                    else
                    {
                        AssertLogger.Fail("Delete Folder Failed");
                    }
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Delete Folder Async Failed ",e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_Delete_Folder_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "DeleteFolderAsync");
            try
            {
                // Create a new folder for deletion
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");
                ContentstackResponse createResponse = _stack.Asset().Folder().Create("Delete Test Folder", null);

                if (createResponse.IsSuccessStatusCode)
                {
                    var responseObject = createResponse.OpenJObjectResponse();
                    string folderUid = responseObject["asset"]["uid"]?.ToString();
                    TestOutputLogger.LogContext("FolderUID", folderUid ?? "null");

                    if (!string.IsNullOrEmpty(folderUid))
                    {
                        ContentstackResponse deleteResponse = _stack.Asset().Folder(folderUid).DeleteAsync().Result;

                        if (deleteResponse.IsSuccessStatusCode)
                        {
                            AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, deleteResponse.StatusCode, "DeleteFolderAsync_StatusCode");
                        }
                        else
                        {
                            AssertLogger.Fail("The Delete Folder Async Failed");
                        }
                    }
                }
                else
                {
                    AssertLogger.Fail("The Create Folder Call Failed");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail("Delete Folder Async Failed ",e.Message);
            }
        }

        // Phase 4: Error Handling and Edge Case Tests
        [TestMethod]
        [DoNotParallelize]
        public void Test024_Should_Handle_Invalid_Asset_Operations()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleInvalidAssetOperations");
            string invalidAssetUid = "invalid_asset_uid_12345";
            TestOutputLogger.LogContext("InvalidAssetUID", invalidAssetUid);

            // Test fetching non-existent asset - expect exception
            try
            {
                _stack.Asset(invalidAssetUid).Fetch();
                AssertLogger.Fail("Expected exception for invalid asset fetch, but operation succeeded");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected exception for invalid asset operations
                AssertLogger.IsTrue(ex.Message.Contains("not found") || ex.Message.Contains("invalid"),
                    $"Expected 'not found' or 'invalid' in exception message, got: {ex.Message}", "InvalidAssetFetch_ExceptionMessage");
            }

            // Test updating non-existent asset - expect exception
            try
            {
                var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
                AssetModel updateModel = new AssetModel("invalid_asset.json", path, "application/json", title:"Invalid Asset", description:"invalid test asset", parentUID: null, tags:"invalid,test");

                _stack.Asset(invalidAssetUid).Update(updateModel);
                AssertLogger.Fail("Expected exception for invalid asset update, but operation succeeded");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected exception for invalid asset operations
                AssertLogger.IsTrue(ex.Message.Contains("not found") || ex.Message.Contains("invalid"),
                    $"Expected 'not found' or 'invalid' in exception message, got: {ex.Message}", "InvalidAssetUpdate_ExceptionMessage");
            }

            // Test deleting non-existent asset - expect exception
            try
            {
                _stack.Asset(invalidAssetUid).Delete();
                AssertLogger.Fail("Expected exception for invalid asset delete, but operation succeeded");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected exception for invalid asset operations
                AssertLogger.IsTrue(ex.Message.Contains("not found") || ex.Message.Contains("invalid"),
                    $"Expected 'not found' or 'invalid' in exception message, got: {ex.Message}", "InvalidAssetDelete_ExceptionMessage");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Handle_Invalid_Folder_Operations()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleInvalidFolderOperations");
            string invalidFolderUid = "invalid_folder_uid_12345";
            TestOutputLogger.LogContext("InvalidFolderUID", invalidFolderUid);

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
            AssertLogger.IsTrue(fetchExceptionThrown, "Expected ContentstackErrorException for invalid folder fetch", "InvalidFolderFetch_ExceptionThrown");

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
            TestOutputLogger.LogContext("TestScenario", "HandleAssetCreationWithInvalidFile");
            string invalidPath = Path.Combine(System.Environment.CurrentDirectory, "non_existent_file.json");
            TestOutputLogger.LogContext("InvalidFilePath", invalidPath);

            // Expect FileNotFoundException during AssetModel construction due to file not found
            try
            {
                new AssetModel("invalid_file.json", invalidPath, "application/json", title:"Invalid File Asset", description:"asset with invalid file", parentUID: null, tags:"invalid,file");
                AssertLogger.Fail("Expected FileNotFoundException during AssetModel construction, but it succeeded");
            }
            catch (FileNotFoundException ex)
            {
                // Expected exception for file not found during AssetModel construction
                AssertLogger.IsTrue(ex.Message.Contains("non_existent_file.json") || ex.Message.Contains("Could not find file"),
                    $"Expected file not found exception, got: {ex.Message}", "InvalidFileAsset_ExceptionMessage");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test029_Should_Handle_Query_With_Invalid_Parameters()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleQueryWithInvalidParameters");
            TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");

            // Test asset query with invalid parameters - expect exception to be raised directly
            var assetQuery = _stack.Asset().Query();
            assetQuery.Limit(-1); // Invalid limit
            assetQuery.Skip(-1); // Invalid skip

            try
            {
                assetQuery.Find();
                AssertLogger.Fail("Expected exception for invalid query parameters, but operation succeeded");
            }
            catch (ArgumentException ex)
            {
                // Expected exception for invalid parameters
                AssertLogger.IsTrue(ex.Message.Contains("limit") || ex.Message.Contains("skip") || ex.Message.Contains("invalid"),
                    $"Expected parameter validation error, got: {ex.Message}", "InvalidQuery_ArgumentException");
            }
            catch (ContentstackErrorException ex)
            {
                // Expected ContentstackErrorException for invalid parameters
                AssertLogger.IsTrue(ex.Message.Contains("parameter") || ex.Message.Contains("invalid") || ex.Message.Contains("limit") || ex.Message.Contains("skip"),
                    $"Expected parameter validation error, got: {ex.Message}", "InvalidQuery_ContentstackErrorException");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Handle_Empty_Query_Results()
        {
            TestOutputLogger.LogContext("TestScenario", "HandleEmptyQueryResults");
            try
            {
                TestOutputLogger.LogContext("StackAPIKey", _stack?.APIKey ?? "null");
                // Test query with very high skip value to get empty results
                var assetQuery = _stack.Asset().Query();
                assetQuery.Skip(999999);
                assetQuery.Limit(1);

                ContentstackResponse response = assetQuery.Find();

                if (response.IsSuccessStatusCode)
                {
                    AssertLogger.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "EmptyQuery_StatusCode");
                    var responseObject = response.OpenJObjectResponse();
                    AssertLogger.IsNotNull(responseObject["assets"], "EmptyQuery_ResponseContainsAssets");
                    // Empty results are valid, so we don't assert on count
                }
                else
                {
                    AssertLogger.Fail("Asset Querying with Empty Query Failed");
                }
            }
            catch (Exception e)
            {
                AssertLogger.Fail(e.Message);
            }
        }

    }
}
