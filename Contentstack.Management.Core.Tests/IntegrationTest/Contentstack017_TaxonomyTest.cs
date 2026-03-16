using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack017_TaxonomyTest
    {
        private static ContentstackClient _client;
        private static string _taxonomyUid;
        private static string _asyncCreatedTaxonomyUid;
        private static string _importedTaxonomyUid;
        private static string _testLocaleCode;
        private static string _asyncTestLocaleCode;
        private static bool _weCreatedTestLocale;
        private static bool _weCreatedAsyncTestLocale;
        private static List<string> _createdTermUids;
        private static string _rootTermUid;
        private static string _childTermUid;
        private Stack _stack;
        private TaxonomyModel _createModel;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
        }

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
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
            TestOutputLogger.LogContext("TestScenario", "Test001_Should_Create_Taxonomy");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = _stack.Taxonomy().Create(_createModel);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Create failed: {response.OpenResponse()}", "CreateSuccess");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            AssertLogger.AreEqual(_createModel.Uid, wrapper.Taxonomy.Uid, "TaxonomyUid");
            AssertLogger.AreEqual(_createModel.Name, wrapper.Taxonomy.Name, "TaxonomyName");
            AssertLogger.AreEqual(_createModel.Description, wrapper.Taxonomy.Description, "TaxonomyDescription");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Fetch_Taxonomy()
        {
            TestOutputLogger.LogContext("TestScenario", "Test002_Should_Fetch_Taxonomy");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Fetch();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Fetch failed: {response.OpenResponse()}", "FetchSuccess");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            AssertLogger.AreEqual(_taxonomyUid, wrapper.Taxonomy.Uid, "TaxonomyUid");
            AssertLogger.IsNotNull(wrapper.Taxonomy.Name, "Taxonomy name");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Query_Taxonomies()
        {
            TestOutputLogger.LogContext("TestScenario", "Test003_Should_Query_Taxonomies");
            ContentstackResponse response = _stack.Taxonomy().Query().Find();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Query failed: {response.OpenResponse()}", "QuerySuccess");

            var wrapper = response.OpenTResponse<TaxonomiesResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomies, "Wrapper taxonomies");
            AssertLogger.IsTrue(wrapper.Taxonomies.Count >= 0, "Taxonomies count", "TaxonomiesCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Update_Taxonomy()
        {
            TestOutputLogger.LogContext("TestScenario", "Test004_Should_Update_Taxonomy");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            var updateModel = new TaxonomyModel
            {
                Name = "Taxonomy Integration Test Updated",
                Description = "Updated description"
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Update(updateModel);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Update failed: {response.OpenResponse()}", "UpdateSuccess");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            AssertLogger.AreEqual("Taxonomy Integration Test Updated", wrapper.Taxonomy.Name, "UpdatedName");
            AssertLogger.AreEqual("Updated description", wrapper.Taxonomy.Description, "UpdatedDescription");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test005_Should_Fetch_Taxonomy_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test005_Should_Fetch_Taxonomy_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).FetchAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"FetchAsync failed: {response.OpenResponse()}", "FetchAsyncSuccess");

            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            AssertLogger.AreEqual(_taxonomyUid, wrapper.Taxonomy.Uid, "TaxonomyUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Create_Taxonomy_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test006_Should_Create_Taxonomy_Async");
            _asyncCreatedTaxonomyUid = "taxonomy_async_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            TestOutputLogger.LogContext("AsyncCreatedTaxonomyUid", _asyncCreatedTaxonomyUid);
            var model = new TaxonomyModel
            {
                Uid = _asyncCreatedTaxonomyUid,
                Name = "Taxonomy Async Create Test",
                Description = "Created via CreateAsync"
            };
            ContentstackResponse response = await _stack.Taxonomy().CreateAsync(model);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"CreateAsync failed: {response.OpenResponse()}", "CreateAsyncSuccess");
            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            AssertLogger.AreEqual(_asyncCreatedTaxonomyUid, wrapper.Taxonomy.Uid, "AsyncTaxonomyUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test007_Should_Update_Taxonomy_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test007_Should_Update_Taxonomy_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            var updateModel = new TaxonomyModel
            {
                Name = "Taxonomy Integration Test Updated Async",
                Description = "Updated via UpdateAsync"
            };
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).UpdateAsync(updateModel);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"UpdateAsync failed: {response.OpenResponse()}", "UpdateAsyncSuccess");
            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            AssertLogger.AreEqual("Taxonomy Integration Test Updated Async", wrapper.Taxonomy.Name, "UpdatedAsyncName");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Query_Taxonomies_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test008_Should_Query_Taxonomies_Async");
            ContentstackResponse response = await _stack.Taxonomy().Query().FindAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Query FindAsync failed: {response.OpenResponse()}", "QueryFindAsyncSuccess");
            var wrapper = response.OpenTResponse<TaxonomiesResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomies, "Wrapper taxonomies");
            AssertLogger.IsTrue(wrapper.Taxonomies.Count >= 0, "Taxonomies count", "TaxonomiesCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Get_Taxonomy_Locales()
        {
            TestOutputLogger.LogContext("TestScenario", "Test009_Should_Get_Taxonomy_Locales");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Locales();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Locales failed: {response.OpenResponse()}", "LocalesSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj["taxonomies"], "Taxonomies in locales response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test010_Should_Get_Taxonomy_Locales_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test010_Should_Get_Taxonomy_Locales_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).LocalesAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"LocalesAsync failed: {response.OpenResponse()}", "LocalesAsyncSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj["taxonomies"], "Taxonomies in locales response");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Localize_Taxonomy()
        {
            TestOutputLogger.LogContext("TestScenario", "Test011_Should_Localize_Taxonomy");
            _weCreatedTestLocale = false;
            ContentstackResponse localesResponse = _stack.Locale().Query().Find();
            AssertLogger.IsTrue(localesResponse.IsSuccessStatusCode, $"Query locales failed: {localesResponse.OpenResponse()}", "QueryLocalesSuccess");
            var jobj = localesResponse.OpenJObjectResponse();
            var localesArray = jobj["locales"] as JArray ?? jobj["items"] as JArray;
            if (localesArray == null || localesArray.Count == 0)
            {
                AssertLogger.Inconclusive("Stack has no locales; skipping taxonomy localize tests.");
                return;
            }
            string masterLocale = "en-us";
            _testLocaleCode = null;
            _asyncTestLocaleCode = null;
            foreach (var item in localesArray)
            {
                var code = item["code"]?.ToString();
                if (string.IsNullOrEmpty(code)) continue;
                if (string.Equals(code, masterLocale, StringComparison.OrdinalIgnoreCase)) continue;
                if (_testLocaleCode == null)
                    _testLocaleCode = code;
                else if (_asyncTestLocaleCode == null)
                {
                    _asyncTestLocaleCode = code;
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
                AssertLogger.Inconclusive("Stack has no non-master locale and could not create one; skipping taxonomy localize tests.");
                return;
            }
            if (string.IsNullOrEmpty(_asyncTestLocaleCode))
            {
                try
                {
                    _asyncTestLocaleCode = "mr-in";
                    var localeModel = new LocaleModel
                    {
                        Code = _asyncTestLocaleCode,
                        Name = "Marathi (India)"
                    };
                    ContentstackResponse createResponse = _stack.Locale().Create(localeModel);
                    if (createResponse.IsSuccessStatusCode)
                        _weCreatedAsyncTestLocale = true;
                    else
                        _asyncTestLocaleCode = null;
                }
                catch (ContentstackErrorException)
                {
                    _asyncTestLocaleCode = null;
                }
            }

            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("TestLocaleCode", _testLocaleCode ?? "");
            var localizeModel = new TaxonomyModel
            {
                Uid = _taxonomyUid,
                Name = "Taxonomy Localized",
                Description = "Localized description"
            };
            var coll = new ParameterCollection();
            coll.Add("locale", _testLocaleCode);
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Localize(localizeModel, coll);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Localize failed: {response.OpenResponse()}", "LocalizeSuccess");
            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            if (!string.IsNullOrEmpty(wrapper.Taxonomy.Locale))
                AssertLogger.AreEqual(_testLocaleCode, wrapper.Taxonomy.Locale, "LocalizedLocale");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test012_Should_Localize_Taxonomy_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test012_Should_Localize_Taxonomy_Async");
            if (string.IsNullOrEmpty(_asyncTestLocaleCode))
            {
                AssertLogger.Inconclusive("No second non-master locale available; skipping async taxonomy localize test.");
                return;
            }
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("AsyncTestLocaleCode", _asyncTestLocaleCode ?? "");
            var localizeModel = new TaxonomyModel
            {
                Uid = _taxonomyUid,
                Name = "Taxonomy Localized Async",
                Description = "Localized description async"
            };
            var coll = new ParameterCollection();
            coll.Add("locale", _asyncTestLocaleCode);
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).LocalizeAsync(localizeModel, coll);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"LocalizeAsync failed: {response.OpenResponse()}", "LocalizeAsyncSuccess");
            var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Taxonomy, "Wrapper taxonomy");
            if (!string.IsNullOrEmpty(wrapper.Taxonomy.Locale))
                AssertLogger.AreEqual(_asyncTestLocaleCode, wrapper.Taxonomy.Locale, "LocalizedAsyncLocale");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Throw_When_Localize_With_Invalid_Locale()
        {
            TestOutputLogger.LogContext("TestScenario", "Test013_Should_Throw_When_Localize_With_Invalid_Locale");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            var localizeModel = new TaxonomyModel
            {
                Uid = _taxonomyUid,
                Name = "Invalid",
                Description = "Invalid"
            };
            var coll = new ParameterCollection();
            coll.Add("locale", "invalid_locale_code_xyz");
            AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Localize(localizeModel, coll), "LocalizeInvalidLocale");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test014_Should_Import_Taxonomy()
        {
            TestOutputLogger.LogContext("TestScenario", "Test014_Should_Import_Taxonomy");
            string importUid = "taxonomy_import_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            TestOutputLogger.LogContext("ImportUid", importUid);
            string json = $"{{\"taxonomy\":{{\"uid\":\"{importUid}\",\"name\":\"Imported Taxonomy\",\"description\":\"Imported\"}}}}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var importModel = new TaxonomyImportModel(stream, "taxonomy.json");
                ContentstackResponse response = _stack.Taxonomy().Import(importModel);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Import failed: {response.OpenResponse()}", "ImportSuccess");
                var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
                AssertLogger.IsNotNull(wrapper?.Taxonomy, "Imported taxonomy");
                _importedTaxonomyUid = wrapper.Taxonomy.Uid ?? importUid;
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test015_Should_Import_Taxonomy_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test015_Should_Import_Taxonomy_Async");
            string importUid = "taxonomy_import_async_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            TestOutputLogger.LogContext("ImportUid", importUid);
            string json = $"{{\"taxonomy\":{{\"uid\":\"{importUid}\",\"name\":\"Imported Async\",\"description\":\"Imported via Async\"}}}}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var importModel = new TaxonomyImportModel(stream, "taxonomy.json");
                ContentstackResponse response = await _stack.Taxonomy().ImportAsync(importModel);
                AssertLogger.IsTrue(response.IsSuccessStatusCode, $"ImportAsync failed: {response.OpenResponse()}", "ImportAsyncSuccess");
                var wrapper = response.OpenTResponse<TaxonomyResponseModel>();
                AssertLogger.IsNotNull(wrapper?.Taxonomy, "Imported taxonomy");
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test016_Should_Create_Root_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test016_Should_Create_Root_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            _rootTermUid = "term_root_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid);
            var termModel = new TermModel
            {
                Uid = _rootTermUid,
                Name = "Root Term",
                ParentUid = null
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Create(termModel);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Create term failed: {response.OpenResponse()}", "CreateTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
            AssertLogger.AreEqual(_rootTermUid, wrapper.Term.Uid, "RootTermUid");
            _createdTermUids.Add(_rootTermUid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_Create_Child_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test017_Should_Create_Child_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            _childTermUid = "term_child_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            TestOutputLogger.LogContext("ChildTermUid", _childTermUid);
            var termModel = new TermModel
            {
                Uid = _childTermUid,
                Name = "Child Term",
                ParentUid = _rootTermUid
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Create(termModel);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Create child term failed: {response.OpenResponse()}", "CreateChildTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
            AssertLogger.AreEqual(_childTermUid, wrapper.Term.Uid, "ChildTermUid");
            _createdTermUids.Add(_childTermUid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test018_Should_Fetch_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test018_Should_Fetch_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Fetch();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Fetch term failed: {response.OpenResponse()}", "FetchTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
            AssertLogger.AreEqual(_rootTermUid, wrapper.Term.Uid, "RootTermUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test019_Should_Fetch_Term_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test019_Should_Fetch_Term_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).FetchAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"FetchAsync term failed: {response.OpenResponse()}", "FetchAsyncTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
            AssertLogger.AreEqual(_rootTermUid, wrapper.Term.Uid, "RootTermUid");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test020_Should_Query_Terms()
        {
            TestOutputLogger.LogContext("TestScenario", "Test020_Should_Query_Terms");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Query().Find();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Query terms failed: {response.OpenResponse()}", "QueryTermsSuccess");
            var wrapper = response.OpenTResponse<TermsResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Terms, "Terms in response");
            AssertLogger.IsTrue(wrapper.Terms.Count >= 0, "Terms count", "TermsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test021_Should_Query_Terms_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test021_Should_Query_Terms_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms().Query().FindAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Query terms Async failed: {response.OpenResponse()}", "QueryTermsAsyncSuccess");
            var wrapper = response.OpenTResponse<TermsResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Terms, "Terms in response");
            AssertLogger.IsTrue(wrapper.Terms.Count >= 0, "Terms count", "TermsCount");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test022_Should_Update_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test022_Should_Update_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            var updateModel = new TermModel
            {
                Name = "Root Term Updated",
                ParentUid = null
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Update(updateModel);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Update term failed: {response.OpenResponse()}", "UpdateTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
            AssertLogger.AreEqual("Root Term Updated", wrapper.Term.Name, "UpdatedTermName");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test023_Should_Update_Term_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test023_Should_Update_Term_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            var updateModel = new TermModel
            {
                Name = "Root Term Updated Async",
                ParentUid = null
            };
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).UpdateAsync(updateModel);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"UpdateAsync term failed: {response.OpenResponse()}", "UpdateAsyncTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
            AssertLogger.AreEqual("Root Term Updated Async", wrapper.Term.Name, "UpdatedAsyncTermName");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test024_Should_Get_Term_Ancestors()
        {
            TestOutputLogger.LogContext("TestScenario", "Test024_Should_Get_Term_Ancestors");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("ChildTermUid", _childTermUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).Ancestors();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Ancestors failed: {response.OpenResponse()}", "AncestorsSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj, "Ancestors response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test025_Should_Get_Term_Ancestors_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test025_Should_Get_Term_Ancestors_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("ChildTermUid", _childTermUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).AncestorsAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"AncestorsAsync failed: {response.OpenResponse()}", "AncestorsAsyncSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj, "Ancestors async response");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test026_Should_Get_Term_Descendants()
        {
            TestOutputLogger.LogContext("TestScenario", "Test026_Should_Get_Term_Descendants");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Descendants();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Descendants failed: {response.OpenResponse()}", "DescendantsSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj, "Descendants response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test027_Should_Get_Term_Descendants_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test027_Should_Get_Term_Descendants_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).DescendantsAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"DescendantsAsync failed: {response.OpenResponse()}", "DescendantsAsyncSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj, "Descendants async response");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test028_Should_Get_Term_Locales()
        {
            TestOutputLogger.LogContext("TestScenario", "Test028_Should_Get_Term_Locales");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Locales();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Term Locales failed: {response.OpenResponse()}", "TermLocalesSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj["terms"], "Terms in locales response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test029_Should_Get_Term_Locales_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test029_Should_Get_Term_Locales_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).LocalesAsync();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Term LocalesAsync failed: {response.OpenResponse()}", "TermLocalesAsyncSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj["terms"], "Terms in locales async response");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test030_Should_Localize_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test030_Should_Localize_Term");
            if (string.IsNullOrEmpty(_testLocaleCode))
            {
                AssertLogger.Inconclusive("No non-master locale available.");
                return;
            }
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            TestOutputLogger.LogContext("TestLocaleCode", _testLocaleCode ?? "");
            var localizeModel = new TermModel
            {
                Uid = _rootTermUid,
                Name = "Root Term Localized",
                ParentUid = null
            };
            var coll = new ParameterCollection();
            coll.Add("locale", _testLocaleCode);
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).Localize(localizeModel, coll);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Term Localize failed: {response.OpenResponse()}", "TermLocalizeSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test031_Should_Localize_Term_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test031_Should_Localize_Term_Async");
            if (string.IsNullOrEmpty(_asyncTestLocaleCode))
            {
                AssertLogger.Inconclusive("No second non-master locale available.");
                return;
            }
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            TestOutputLogger.LogContext("AsyncTestLocaleCode", _asyncTestLocaleCode ?? "");
            var localizeModel = new TermModel
            {
                Uid = _rootTermUid,
                Name = "Root Term Localized Async",
                ParentUid = null
            };
            var coll = new ParameterCollection();
            coll.Add("locale", _asyncTestLocaleCode);
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_rootTermUid).LocalizeAsync(localizeModel, coll);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Term LocalizeAsync failed: {response.OpenResponse()}", "TermLocalizeAsyncSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test032_Should_Move_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test032_Should_Move_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("ChildTermUid", _childTermUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            var moveModel = new TermMoveModel
            {
                ParentUid = _rootTermUid,
                Order = 1
            };
            var coll = new ParameterCollection();
            coll.Add("force", true);
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).Move(moveModel, coll);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Move term failed: {response.OpenResponse()}", "MoveTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test033_Should_Move_Term_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test033_Should_Move_Term_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("ChildTermUid", _childTermUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            var moveModel = new TermMoveModel
            {
                ParentUid = _rootTermUid,
                Order = 1
            };
            var coll = new ParameterCollection();
            coll.Add("force", true);
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms(_childTermUid).MoveAsync(moveModel, coll);
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"MoveAsync term failed: {response.OpenResponse()}", "MoveAsyncTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test034_Should_Search_Terms()
        {
            TestOutputLogger.LogContext("TestScenario", "Test034_Should_Search_Terms");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().Search("Root");
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"Search terms failed: {response.OpenResponse()}", "SearchTermsSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj["terms"] ?? jobj["items"], "Terms or items in search response");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test035_Should_Search_Terms_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test035_Should_Search_Terms_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            ContentstackResponse response = await _stack.Taxonomy(_taxonomyUid).Terms().SearchAsync("Root");
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"SearchAsync terms failed: {response.OpenResponse()}", "SearchAsyncTermsSuccess");
            var jobj = response.OpenJObjectResponse();
            AssertLogger.IsNotNull(jobj["terms"] ?? jobj["items"], "Terms or items in search async response");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test036_Should_Create_Term_Async()
        {
            TestOutputLogger.LogContext("TestScenario", "Test036_Should_Create_Term_Async");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            TestOutputLogger.LogContext("RootTermUid", _rootTermUid ?? "");
            string termUid = "term_async_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            TestOutputLogger.LogContext("AsyncTermUid", termUid);
            var termModel = new TermModel
            {
                Uid = termUid,
                Name = "Async Term",
                ParentUid = _rootTermUid
            };
            ContentstackResponse response = _stack.Taxonomy(_taxonomyUid).Terms().CreateAsync(termModel).GetAwaiter().GetResult();
            AssertLogger.IsTrue(response.IsSuccessStatusCode, $"CreateAsync term failed: {response.OpenResponse()}", "CreateAsyncTermSuccess");
            var wrapper = response.OpenTResponse<TermResponseModel>();
            AssertLogger.IsNotNull(wrapper?.Term, "Term in response");
            _createdTermUids.Add(termUid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test037_Should_Throw_When_Update_NonExistent_Taxonomy()
        {
            TestOutputLogger.LogContext("TestScenario", "Test037_Should_Throw_When_Update_NonExistent_Taxonomy");
            var updateModel = new TaxonomyModel { Name = "No", Description = "No" };
            AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Update(updateModel), "UpdateNonExistentTaxonomy");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test038_Should_Throw_When_Fetch_NonExistent_Taxonomy()
        {
            TestOutputLogger.LogContext("TestScenario", "Test038_Should_Throw_When_Fetch_NonExistent_Taxonomy");
            AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Fetch(), "FetchNonExistentTaxonomy");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test039_Should_Throw_When_Delete_NonExistent_Taxonomy()
        {
            TestOutputLogger.LogContext("TestScenario", "Test039_Should_Throw_When_Delete_NonExistent_Taxonomy");
            AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy("non_existent_taxonomy_uid_12345").Delete(), "DeleteNonExistentTaxonomy");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test040_Should_Throw_When_Fetch_NonExistent_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test040_Should_Throw_When_Fetch_NonExistent_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Terms("non_existent_term_uid_12345").Fetch(), "FetchNonExistentTerm");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test041_Should_Throw_When_Update_NonExistent_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test041_Should_Throw_When_Update_NonExistent_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            var updateModel = new TermModel { Name = "No", ParentUid = null };
            AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Terms("non_existent_term_uid_12345").Update(updateModel), "UpdateNonExistentTerm");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test042_Should_Throw_When_Delete_NonExistent_Term()
        {
            TestOutputLogger.LogContext("TestScenario", "Test042_Should_Throw_When_Delete_NonExistent_Term");
            TestOutputLogger.LogContext("TaxonomyUid", _taxonomyUid ?? "");
            AssertLogger.ThrowsException<ContentstackErrorException>(() =>
                _stack.Taxonomy(_taxonomyUid).Terms("non_existent_term_uid_12345").Delete(), "DeleteNonExistentTerm");
        }

        private static Stack GetStack()
        {
            StackResponse response = StackResponse.getStack(_client.serializer);
            return _client.Stack(response.Stack.APIKey);
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

                if (_weCreatedAsyncTestLocale && !string.IsNullOrEmpty(_asyncTestLocaleCode))
                {
                    try
                    {
                        stack.Locale(_asyncTestLocaleCode).Delete();
                        Console.WriteLine($"[Cleanup] Deleted async test locale: {_asyncTestLocaleCode}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cleanup] Failed to delete async test locale {_asyncTestLocaleCode}: {ex.Message}");
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

            try { _client?.Logout(); } catch { }
            _client = null;
        }
    }
}
