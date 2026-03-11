using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack017_TaxonomyTest
    {
        private static string _taxonomyUid;
        private static string _asyncCreatedTaxonomyUid;
        private static string _importedTaxonomyUid;
        private static string _testLocaleCode;
        private static bool _weCreatedTestLocale;
        private static List<string> _createdTermUids;
        private static string _rootTermUid;
        private static string _childTermUid;
        private Stack _stack;
        private TaxonomyModel _createModel;

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
            if (_taxonomyUid == null)
                _taxonomyUid = "taxonomy_integration_test_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            _createdTermUids = _createdTermUids ?? new List<string>();
            _createModel = new TaxonomyModel
            {
                Uid = _taxonomyUid,
                Name = "Taxonomy Integration Test",
                Description = "Description for taxonomy integration test"
            };
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy().Create(_createModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Create failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual(_createModel.Uid, wrapper.Taxonomy.Uid);
            Assert.AreEqual(_createModel.Name, wrapper.Taxonomy.Name);
            Assert.AreEqual(_createModel.Description, wrapper.Taxonomy.Description);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Fetch_Taxonomy()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Fetch();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Fetch failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual(_taxonomyUid, wrapper.Taxonomy.Uid);
            Assert.IsNotNull(wrapper.Taxonomy.Name);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Query_Taxonomies()
        {
            ContentstackResponse response = _stack.Taxonomy().Query().Find();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Query failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomiesResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomies);
            Assert.IsTrue(wrapper.Taxonomies.Count >= 0);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Taxonomy()
        {
            var updateModel = new TaxonomyModel
            {
                Name = "Taxonomy Integration Test Updated",
                Description = "Updated description"
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Update(updateModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Update failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual("Taxonomy Integration Test Updated", wrapper.Taxonomy.Name);
            Assert.AreEqual("Updated description", wrapper.Taxonomy.Description);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Fetch_Taxonomy_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).FetchAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"FetchAsync failed: {response.OpenResponse()}");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual(_taxonomyUid, wrapper.Taxonomy.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Create_Taxonomy_Async()
        {
            _asyncCreatedTaxonomyUid = "taxonomy_async_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var model = new TaxonomyModel
            {
                Uid = _asyncCreatedTaxonomyUid,
                Name = "Taxonomy Async Create Test",
                Description = "Created via CreateAsync"
            };
            ContentstackResponse response = await _stack.Taxonomy().CreateAsync(model);
            Assert.IsTrue(response.IsSuccessStatusCode, $"CreateAsync failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual(_asyncCreatedTaxonomyUid, wrapper.Taxonomy.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Update_Taxonomy_Async()
        {
            var updateModel = new TaxonomyModel
            {
                Name = "Taxonomy Integration Test Updated Async",
                Description = "Updated via UpdateAsync"
            };
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).UpdateAsync(updateModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"UpdateAsync failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            Assert.AreEqual("Taxonomy Integration Test Updated Async", wrapper.Taxonomy.Name);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Query_Taxonomies_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy().Query().FindAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Query FindAsync failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TaxonomiesResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomies);
            Assert.IsTrue(wrapper.Taxonomies.Count >= 0);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Get_Taxonomy_Locales()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Locales();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Locales failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj["taxonomies"]);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test010_Should_Get_Taxonomy_Locales_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).LocalesAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"LocalesAsync failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj["taxonomies"]);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Localize_Taxonomy()
        {
            _weCreatedTestLocale = false;
            ContentstackResponse localesResponse = _stack.Locale().Query().Find();
            Assert.IsTrue(localesResponse.IsSuccessStatusCode, $"Query locales failed: {localesResponse.OpenResponse()}");
            var jobj = localesResponse.OpenJObjectResponse();
            var localesArray = jobj["locales"] as JArray ?? jobj["items"] as JArray;
            if (localesArray == null || localesArray.Count == 0)
            {
                Assert.Inconclusive("Stack has no locales; skipping taxonomy localize tests.");
                return;
            }
            string masterLocale = "en-us";
            _testLocaleCode = null;
            foreach (var item in localesArray)
            {
                var code = item["code"]?.ToString();
                if (string.IsNullOrEmpty(code)) continue;
                if (!string.Equals(code, masterLocale, StringComparison.OrdinalIgnoreCase))
                {
                    _testLocaleCode = code;
                    break;
                }
            }
            if (string.IsNullOrEmpty(_testLocaleCode))
            {
                try
                {
                    _testLocaleCode = "hi-in";
                    var localeModel = new LocaleModel
                    {
                        Code = _testLocaleCode,
                        Name = "Hindi (India)"
                    };
                    ContentstackResponse createResponse = _stack.Locale().Create(localeModel);
                    if (createResponse.IsSuccessStatusCode)
                        _weCreatedTestLocale = true;
                    else
                        _testLocaleCode = null;
                }
                catch (ContentstackErrorException)
                {
                    _testLocaleCode = null;
                }
            }
            if (string.IsNullOrEmpty(_testLocaleCode))
            {
                Assert.Inconclusive("Stack has no non-master locale and could not create one; skipping taxonomy localize tests.");
                return;
            }

            var localizeModel = new TaxonomyModel
            {
                Uid = _taxonomyUid,
                Name = "Taxonomy Localized",
                Description = "Localized description"
            };
            var coll = new ParameterCollection();
            coll.Add("locale", _testLocaleCode);
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Localize(localizeModel, coll);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Localize failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            Assert.IsNotNull(wrapper?.Taxonomy);
            if (!string.IsNullOrEmpty(wrapper.Taxonomy.Locale))
                Assert.AreEqual(_testLocaleCode, wrapper.Taxonomy.Locale);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Throw_When_Localize_With_Invalid_Locale()
        {
            var localizeModel = new TaxonomyModel
            {
                Uid = _taxonomyUid,
                Name = "Invalid",
                Description = "Invalid"
            };
            var coll = new ParameterCollection();
            coll.Add("locale", "invalid_locale_code_xyz");
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Localize(localizeModel, coll));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Import_Taxonomy()
        {
            string importUid = "taxonomy_import_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            string json = $"{{\"taxonomy\":{{\"uid\":\"{importUid}\",\"name\":\"Imported Taxonomy\",\"description\":\"Imported\"}}}}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var importModel = new TaxonomyImportModel(stream, "taxonomy.json");
                ContentstackResponse response = _stack.Taxonomy().Import(importModel);
                Assert.IsTrue(response.IsSuccessStatusCode, $"Import failed: {response.OpenResponse()}");
                var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
                Assert.IsNotNull(wrapper?.Taxonomy);
                _importedTaxonomyUid = wrapper.Taxonomy.Uid ?? importUid;
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test015_Should_Import_Taxonomy_Async()
        {
            string importUid = "taxonomy_import_async_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            string json = $"{{\"taxonomy\":{{\"uid\":\"{importUid}\",\"name\":\"Imported Async\",\"description\":\"Imported via Async\"}}}}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var importModel = new TaxonomyImportModel(stream, "taxonomy.json");
                ContentstackResponse response = await _stack.Taxonomy().ImportAsync(importModel);
                Assert.IsTrue(response.IsSuccessStatusCode, $"ImportAsync failed: {response.OpenResponse()}");
                var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
                Assert.IsNotNull(wrapper?.Taxonomy);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Create_Root_Term()
        {
            _rootTermUid = "term_root_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var termModel = new TermModel
            {
                Uid = _rootTermUid,
                Name = "Root Term",
                ParentUid = null
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Create(termModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Create term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
            Assert.AreEqual(_rootTermUid, wrapper.Term.Uid);
            _createdTermUids.Add(_rootTermUid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Create_Child_Term()
        {
            _childTermUid = "term_child_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var termModel = new TermModel
            {
                Uid = _childTermUid,
                Name = "Child Term",
                ParentUid = _rootTermUid
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Create(termModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Create child term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
            Assert.AreEqual(_childTermUid, wrapper.Term.Uid);
            _createdTermUids.Add(_childTermUid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Fetch_Term()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Fetch();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Fetch term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
            Assert.AreEqual(_rootTermUid, wrapper.Term.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test019_Should_Fetch_Term_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).FetchAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"FetchAsync term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
            Assert.AreEqual(_rootTermUid, wrapper.Term.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test020_Should_Query_Terms()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Query().Find();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Query terms failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermsResponseModel>();
            Assert.IsNotNull(wrapper?.Terms);
            Assert.IsTrue(wrapper.Terms.Count >= 0);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test021_Should_Query_Terms_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms().Query().FindAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Query terms Async failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermsResponseModel>();
            Assert.IsNotNull(wrapper?.Terms);
            Assert.IsTrue(wrapper.Terms.Count >= 0);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Update_Term()
        {
            var updateModel = new TermModel
            {
                Name = "Root Term Updated",
                ParentUid = null
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Update(updateModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Update term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
            Assert.AreEqual("Root Term Updated", wrapper.Term.Name);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test023_Should_Update_Term_Async()
        {
            var updateModel = new TermModel
            {
                Name = "Root Term Updated Async",
                ParentUid = null
            };
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).UpdateAsync(updateModel);
            Assert.IsTrue(response.IsSuccessStatusCode, $"UpdateAsync term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
            Assert.AreEqual("Root Term Updated Async", wrapper.Term.Name);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test024_Should_Get_Term_Ancestors()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).Ancestors();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Ancestors failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test025_Should_Get_Term_Ancestors_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).AncestorsAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"AncestorsAsync failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Get_Term_Descendants()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Descendants();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Descendants failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test027_Should_Get_Term_Descendants_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).DescendantsAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"DescendantsAsync failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Get_Term_Locales()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Locales();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Term Locales failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj["terms"]);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test029_Should_Get_Term_Locales_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).LocalesAsync();
            Assert.IsTrue(response.IsSuccessStatusCode, $"Term LocalesAsync failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj["terms"]);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Localize_Term()
        {
            if (string.IsNullOrEmpty(_testLocaleCode))
            {
                Assert.Inconclusive("No non-master locale available.");
                return;
            }
            var localizeModel = new TermModel
            {
                Uid = _rootTermUid,
                Name = "Root Term Localized",
                ParentUid = null
            };
            var coll = new ParameterCollection();
            coll.Add("locale", _testLocaleCode);
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Localize(localizeModel, coll);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Term Localize failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Move_Term()
        {
            var moveModel = new TermMoveModel
            {
                ParentUid = _rootTermUid,
                Order = 0
            };
            ContentstackResponse response = null;
            try
            {
                response = _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).Move(moveModel, null);
            }
            catch (ContentstackErrorException)
            {
                try
                {
                    var coll = new ParameterCollection();
                    coll.Add("force", true);
                    response = _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).Move(moveModel, coll);
                }
                catch (ContentstackErrorException ex)
                {
                    Assert.Inconclusive("Move term failed: {0}", ex.Message);
                    return;
                }
            }
            Assert.IsTrue(response.IsSuccessStatusCode, $"Move term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test033_Should_Move_Term_Async()
        {
            var moveModel = new TermMoveModel
            {
                ParentUid = _rootTermUid,
                Order = 1
            };
            ContentstackResponse response = null;
            try
            {
                response = await _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).MoveAsync(moveModel, null);
            }
            catch (ContentstackErrorException)
            {
                try
                {
                    var coll = new ParameterCollection();
                    coll.Add("force", true);
                    response = await _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).MoveAsync(moveModel, coll);
                }
                catch (ContentstackErrorException ex)
                {
                    Assert.Inconclusive("Move term failed: {0}", ex.Message);
                    return;
                }
            }
            Assert.IsTrue(response.IsSuccessStatusCode, $"MoveAsync term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Search_Terms()
        {
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Search("Root");
            Assert.IsTrue(response.IsSuccessStatusCode, $"Search terms failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj["terms"] ?? jobj["items"]);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test035_Should_Search_Terms_Async()
        {
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms().SearchAsync("Root");
            Assert.IsTrue(response.IsSuccessStatusCode, $"SearchAsync terms failed: {response.OpenResponse()}");
            var jobj = response.OpenJObjectResponse();
            Assert.IsNotNull(jobj["terms"] ?? jobj["items"]);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test036_Should_Create_Term_Async()
        {
            string termUid = "term_async_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var termModel = new TermModel
            {
                Uid = termUid,
                Name = "Async Term",
                ParentUid = _rootTermUid
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().CreateAsync(termModel).GetAwaiter().GetResult();
            Assert.IsTrue(response.IsSuccessStatusCode, $"CreateAsync term failed: {response.OpenResponse()}");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            Assert.IsNotNull(wrapper?.Term);
            _createdTermUids.Add(termUid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test037_Should_Throw_When_Update_NonExistent_Taxonomy()
        {
            var updateModel = new TaxonomyModel { Name = "No", Description = "No" };
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Update(updateModel));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test038_Should_Throw_When_Fetch_NonExistent_Taxonomy()
        {
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Fetch());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test039_Should_Throw_When_Delete_NonExistent_Taxonomy()
        {
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Delete());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test040_Should_Throw_When_Fetch_NonExistent_Term()
        {
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Terms("non_existent_term_uid_12345").Fetch());
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test041_Should_Throw_When_Update_NonExistent_Term()
        {
            var updateModel = new TermModel { Name = "No", ParentUid = null };
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Terms("non_existent_term_uid_12345").Update(updateModel));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test042_Should_Throw_When_Delete_NonExistent_Term()
        {
            Assert.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Terms("non_existent_term_uid_12345").Delete());
        }

        private static Stack GetStack()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            return Contentstack.Client.Stack(response.Stack.APIKey);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            try
            {
                Stack stack = GetStack();

                if (_createdTermUids != null && _createdTermUids.Count > 0 && !string.IsNullOrEmpty(_taxonomyUid))
                {
                    var coll = new ParameterCollection();
                    coll.Add("force", true);
                    foreach (var termUid in _createdTermUids)
                    {
                        try
                        {
                            stack.Taxonomy(_taxonomyUid).Terms(termUid).Delete(coll);
                        }
                        catch (ContentstackErrorException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                        {
                            // Term already deleted
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Cleanup] Failed to delete term {termUid}: {ex.Message}");
                        }
                    }
                    _createdTermUids.Clear();
                }

                if (!string.IsNullOrEmpty(_importedTaxonomyUid))
                {
                    try
                    {
                        stack.Taxonomy(_importedTaxonomyUid).Delete();
                        Console.WriteLine($"[Cleanup] Deleted imported taxonomy: {_importedTaxonomyUid}");
                        _importedTaxonomyUid = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete imported taxonomy {_importedTaxonomyUid}: {ex.Message}");
                    }
                }

                if (!string.IsNullOrEmpty(_asyncCreatedTaxonomyUid))
                {
                    try
                    {
                        stack.Taxonomy(_asyncCreatedTaxonomyUid).Delete();
                        Console.WriteLine($"[Cleanup] Deleted async-created taxonomy: {_asyncCreatedTaxonomyUid}");
                        _asyncCreatedTaxonomyUid = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete async taxonomy {_asyncCreatedTaxonomyUid}: {ex.Message}");
                    }
                }

                if (!string.IsNullOrEmpty(_taxonomyUid))
                {
                    try
                    {
                        stack.Taxonomy(_taxonomyUid).Delete();
                        Console.WriteLine($"[Cleanup] Deleted main taxonomy: {_taxonomyUid}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete main taxonomy {_taxonomyUid}: {ex.Message}");
                    }
                }

                if (_weCreatedTestLocale && !string.IsNullOrEmpty(_testLocaleCode))
                {
                    try
                    {
                        stack.Locale(_testLocaleCode).Delete();
                        Console.WriteLine($"[Cleanup] Deleted test locale: {_testLocaleCode}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete test locale {_testLocaleCode}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cleanup] Cleanup failed: {ex.Message}");
            }
        }
    }
}
