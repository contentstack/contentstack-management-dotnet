using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.CustomExtension;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    /// <summary>
    /// Expanded content-type API coverage: disposable UIDs, complex fixtures, errors, taxonomy, delete/cleanup.
    /// </summary>
    [TestClass]
    public class Contentstack012b_ContentTypeExpandedIntegrationTest
    {
        private static ContentstackClient _client;
        private Stack _stack;

        private static string NewSuffix() => Guid.NewGuid().ToString("N").Substring(0, 12);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = Contentstack.CreateAuthenticatedClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try { _client?.Logout(); } catch { /* ignore */ }
            _client = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var response = StackResponse.getStack(_client.serializer);
            _stack = _client.Stack(response.Stack.APIKey);
        }

        #region Simple disposable — sync/async lifecycle

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_DisposableSimple_FullLifecycle_Sync()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            try
            {
                TestOutputLogger.LogContext("TestScenario", "DisposableSimple_Sync");
                TestOutputLogger.LogContext("ContentType", model.Uid);

                var createRes = _stack.ContentType().Create(model);
                var created = createRes.OpenTResponse<ContentTypeModel>();
                AssertLogger.IsNotNull(created?.Modelling, "created");
                AssertLogger.AreEqual(model.Uid, created.Modelling.Uid, "uid");

                var fetchRes = _stack.ContentType(model.Uid).Fetch();
                var fetched = fetchRes.OpenTResponse<ContentTypeModel>();
                AssertLogger.AreEqual(model.Uid, fetched.Modelling.Uid, "fetch uid");

                model.Description = "Updated " + sfx;
                var updateRes = _stack.ContentType(model.Uid).Update(model);
                var updated = updateRes.OpenTResponse<ContentTypeModel>();
                AssertLogger.AreEqual(model.Description, updated.Modelling.Description, "description");

                var queryRes = _stack.ContentType().Query().Find();
                var list = queryRes.OpenTResponse<ContentTypesModel>();
                AssertLogger.IsTrue(list.Modellings.Any(m => m.Uid == model.Uid), "query contains uid");

                var limited = _stack.ContentType().Query().Limit(5).Find().OpenTResponse<ContentTypesModel>();
                AssertLogger.IsTrue(limited.Modellings.Count <= 5, "limit");

                var skipped = _stack.ContentType().Query().Skip(0).Limit(20).Find().OpenTResponse<ContentTypesModel>();
                AssertLogger.IsTrue(skipped.Modellings.Count <= 20, "skip/limit");

                var delRes = _stack.ContentType(model.Uid).Delete();
                AssertLogger.IsTrue(delRes.IsSuccessStatusCode, "delete success");

                AssertLogger.ThrowsContentstackError(
                    () => _stack.ContentType(model.Uid).Fetch(),
                    "FetchAfterDelete",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test002_Should_DisposableSimple_FullLifecycle_Async()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            try
            {
                TestOutputLogger.LogContext("TestScenario", "DisposableSimple_Async");
                TestOutputLogger.LogContext("ContentType", model.Uid);

                var createRes = await _stack.ContentType().CreateAsync(model);
                var created = createRes.OpenTResponse<ContentTypeModel>();
                AssertLogger.IsNotNull(created?.Modelling, "created");
                AssertLogger.AreEqual(model.Uid, created.Modelling.Uid, "uid");

                var fetchRes = await _stack.ContentType(model.Uid).FetchAsync();
                AssertLogger.AreEqual(model.Uid, fetchRes.OpenTResponse<ContentTypeModel>().Modelling.Uid, "fetch");

                model.Description = "Updated async " + sfx;
                var updateRes = await _stack.ContentType(model.Uid).UpdateAsync(model);
                AssertLogger.AreEqual(model.Description, updateRes.OpenTResponse<ContentTypeModel>().Modelling.Description, "desc");

                var queryRes = await _stack.ContentType().Query().FindAsync();
                var list = queryRes.OpenTResponse<ContentTypesModel>();
                AssertLogger.IsTrue(list.Modellings.Any(m => m.Uid == model.Uid), "query async");

                var limited = (await _stack.ContentType().Query().Limit(5).FindAsync()).OpenTResponse<ContentTypesModel>();
                AssertLogger.IsTrue(limited.Modellings.Count <= 5, "limit async");

                var delRes = await _stack.ContentType(model.Uid).DeleteAsync();
                AssertLogger.IsTrue(delRes.IsSuccessStatusCode, "delete async");

                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.ContentType(model.Uid).FetchAsync(),
                    "FetchAfterDeleteAsync",
                    HttpStatusCode.NotFound,
                    (HttpStatusCode)422);
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_DisposableSimple_Delete_Sync()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            _stack.ContentType().Create(model);
            try
            {
                var del = _stack.ContentType(model.Uid).Delete();
                AssertLogger.IsTrue(del.IsSuccessStatusCode, "delete");
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_DisposableSimple_Delete_Async()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            await _stack.ContentType().CreateAsync(model);
            try
            {
                var del = await _stack.ContentType(model.Uid).DeleteAsync();
                AssertLogger.IsTrue(del.IsSuccessStatusCode, "delete async");
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        #endregion

        #region Error cases

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Error_Create_DuplicateUid_Sync()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            _stack.ContentType().Create(model);
            try
            {
                AssertLogger.ThrowsContentstackError(
                    () => _stack.ContentType().Create(model),
                    "DuplicateUid",
                    HttpStatusCode.Conflict,
                    (HttpStatusCode)422);
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Error_Create_DuplicateUid_Async()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            await _stack.ContentType().CreateAsync(model);
            try
            {
                await AssertLogger.ThrowsContentstackErrorAsync(
                    async () => await _stack.ContentType().CreateAsync(model),
                    "DuplicateUidAsync",
                    HttpStatusCode.Conflict,
                    (HttpStatusCode)422);
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Error_Create_InvalidUid_Sync()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            model.Uid = "Invalid-UID-Caps!";
            AssertLogger.ThrowsContentstackError(
                () => _stack.ContentType().Create(model),
                "InvalidUid",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test008_Should_Error_Create_InvalidUid_Async()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
            model.Uid = "Invalid-UID-Caps!";
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.ContentType().CreateAsync(model),
                "InvalidUidAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Error_Create_MissingTitle_Sync()
        {
            var model = new ContentModelling
            {
                Uid = "no_title_" + NewSuffix(),
                Schema = new List<Field>()
            };
            AssertLogger.ThrowsContentstackError(
                () => _stack.ContentType().Create(model),
                "MissingTitle",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test010_Should_Error_Create_MissingTitle_Async()
        {
            var model = new ContentModelling
            {
                Uid = "no_title_" + NewSuffix(),
                Schema = new List<Field>()
            };
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.ContentType().CreateAsync(model),
                "MissingTitleAsync",
                HttpStatusCode.BadRequest,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test011_Should_Error_Fetch_NonExistent_Sync()
        {
            AssertLogger.ThrowsContentstackError(
                () => _stack.ContentType("non_existent_ct_" + NewSuffix()).Fetch(),
                "FetchMissing",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test012_Should_Error_Fetch_NonExistent_Async()
        {
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.ContentType("non_existent_ct_" + NewSuffix()).FetchAsync(),
                "FetchMissingAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test013_Should_Error_Update_NonExistent_Sync()
        {
            var m = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", NewSuffix());
            AssertLogger.ThrowsContentstackError(
                () => _stack.ContentType("non_existent_ct_" + NewSuffix()).Update(m),
                "UpdateMissing",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test014_Should_Error_Update_NonExistent_Async()
        {
            var m = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", NewSuffix());
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.ContentType("non_existent_ct_" + NewSuffix()).UpdateAsync(m),
                "UpdateMissingAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test015_Should_Error_Delete_NonExistent_Sync()
        {
            AssertLogger.ThrowsContentstackError(
                () => _stack.ContentType("non_existent_ct_" + NewSuffix()).Delete(),
                "DeleteMissing",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test016_Should_Error_Delete_NonExistent_Async()
        {
            await AssertLogger.ThrowsContentstackErrorAsync(
                async () => await _stack.ContentType("non_existent_ct_" + NewSuffix()).DeleteAsync(),
                "DeleteMissingAsync",
                HttpStatusCode.NotFound,
                (HttpStatusCode)422);
        }

        #endregion

        #region Complex / medium fixtures

        [TestMethod]
        [DoNotParallelize]
        public void Test017_Should_ComplexFixture_CreateFetch_AssertStructure_Sync()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeComplex.json", sfx);
            try
            {
                _stack.ContentType().Create(model);
                var fetched = _stack.ContentType(model.Uid).Fetch().OpenTResponse<ContentTypeModel>().Modelling;

                var bodyHtml = fetched.Schema.OfType<TextboxField>().FirstOrDefault(f => f.Uid == "body_html");
                AssertLogger.IsNotNull(bodyHtml, "body_html");
                AssertLogger.IsTrue(bodyHtml.FieldMetadata?.AllowRichText == true, "RTE allow_rich_text");

                var jsonRte = fetched.Schema.OfType<JsonField>().FirstOrDefault(f => f.Uid == "content_json_rte");
                AssertLogger.IsNotNull(jsonRte, "json rte field");
                AssertLogger.IsTrue(jsonRte.FieldMetadata?.AllowJsonRte == true, "allow_json_rte");

                var seo = fetched.Schema.OfType<GroupField>().FirstOrDefault(f => f.Uid == "seo");
                AssertLogger.IsNotNull(seo, "seo group");
                AssertLogger.IsTrue(seo.Schema.Count >= 2, "nested seo fields");

                var links = fetched.Schema.OfType<GroupField>().FirstOrDefault(f => f.Uid == "links");
                AssertLogger.IsNotNull(links, "links group");
                AssertLogger.IsTrue(links.Multiple, "repeatable group");

                var sections = fetched.Schema.OfType<ModularBlockField>().FirstOrDefault(f => f.Uid == "sections");
                AssertLogger.IsNotNull(sections, "modular blocks");
                AssertLogger.IsTrue(sections.blocks.Count >= 2, "block definitions");
                var hero = sections.blocks.FirstOrDefault(b => b.Uid == "hero_section");
                AssertLogger.IsNotNull(hero, "hero block");
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test018_Should_ComplexFixture_CreateFetch_AssertStructure_Async()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeComplex.json", sfx);
            try
            {
                await _stack.ContentType().CreateAsync(model);
                var fetched = (await _stack.ContentType(model.Uid).FetchAsync()).OpenTResponse<ContentTypeModel>().Modelling;
                AssertLogger.IsNotNull(fetched.Schema.OfType<ModularBlockField>().FirstOrDefault(f => f.Uid == "sections"), "sections async");
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test019_Should_MediumFixture_CreateFetch_AssertFieldTypes_Sync()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeMedium.json", sfx);
            try
            {
                _stack.ContentType().Create(model);
                var fetched = _stack.ContentType(model.Uid).Fetch().OpenTResponse<ContentTypeModel>().Modelling;

                var num = fetched.Schema.OfType<NumberField>().FirstOrDefault(f => f.Uid == "view_count");
                AssertLogger.IsNotNull(num, "number");
                AssertLogger.IsTrue(num.Min.HasValue && num.Min.Value == 0, "min");

                var status = fetched.Schema.OfType<SelectField>().FirstOrDefault(f => f.Uid == "status");
                AssertLogger.IsNotNull(status, "dropdown");
                AssertLogger.IsNotNull(status.Enum?.Choices, "choices");

                var hero = fetched.Schema.OfType<ImageField>().FirstOrDefault(f => f.Uid == "hero_image");
                AssertLogger.IsNotNull(hero, "image file");

                var pub = fetched.Schema.OfType<DateField>().FirstOrDefault(f => f.Uid == "publish_date");
                AssertLogger.IsNotNull(pub, "date");
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test020_Should_MediumFixture_CreateFetch_AssertFieldTypes_Async()
        {
            var sfx = NewSuffix();
            var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeMedium.json", sfx);
            try
            {
                await _stack.ContentType().CreateAsync(model);
                var fetched = (await _stack.ContentType(model.Uid).FetchAsync()).OpenTResponse<ContentTypeModel>().Modelling;
                AssertLogger.IsNotNull(fetched.Schema.OfType<NumberField>().FirstOrDefault(f => f.Uid == "view_count"), "number async");
            }
            finally
            {
                TryDeleteContentType(model.Uid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test021_Should_ExtensionField_CreateFetch_AfterUpload_Sync()
        {
            var sfx = NewSuffix();
            string extUid = null;
            string ctUid = null;
            try
            {
                extUid = UploadDisposableCustomFieldExtensionAndGetUid(sfx);
                TestOutputLogger.LogContext("ExtensionUid", extUid ?? "");

                var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
                ctUid = model.Uid;
                model.Schema.Add(new ExtensionField
                {
                    DisplayName = "Custom Extension",
                    Uid = "ext_widget_" + sfx,
                    DataType = "extension",
                    extension_uid = extUid,
                    Mandatory = false
                });

                _stack.ContentType().Create(model);
                var fetched = _stack.ContentType(model.Uid).Fetch().OpenTResponse<ContentTypeModel>().Modelling;
                var ext = fetched.Schema.OfType<ExtensionField>().FirstOrDefault(f => f.Uid.StartsWith("ext_widget_", StringComparison.Ordinal));
                AssertLogger.IsNotNull(ext, "extension field");
                AssertLogger.AreEqual(extUid, ext.extension_uid, "extension_uid");
            }
            finally
            {
                TryDeleteContentType(ctUid);
                TryDeleteExtension(extUid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test022_Should_ExtensionField_CreateFetch_AfterUpload_Async()
        {
            var sfx = NewSuffix();
            string extUid = null;
            string ctUid = null;
            try
            {
                extUid = await UploadDisposableCustomFieldExtensionAndGetUidAsync(sfx);
                TestOutputLogger.LogContext("ExtensionUid", extUid ?? "");

                var model = ContentTypeFixtureLoader.LoadFromMock(_client.serializer, "contentTypeSimple.json", sfx);
                ctUid = model.Uid;
                model.Schema.Add(new ExtensionField
                {
                    DisplayName = "Custom Extension",
                    Uid = "ext_widget_a_" + sfx,
                    DataType = "extension",
                    extension_uid = extUid,
                    Mandatory = false
                });

                await _stack.ContentType().CreateAsync(model);
                var fetched = (await _stack.ContentType(model.Uid).FetchAsync()).OpenTResponse<ContentTypeModel>().Modelling;
                var ext = fetched.Schema.OfType<ExtensionField>().FirstOrDefault(f => f.Uid.StartsWith("ext_widget_a_", StringComparison.Ordinal));
                AssertLogger.IsNotNull(ext, "extension async");
                AssertLogger.AreEqual(extUid, ext.extension_uid, "extension_uid");
            }
            finally
            {
                TryDeleteContentType(ctUid);
                await TryDeleteExtensionAsync(extUid);
            }
        }

        #endregion

        #region Taxonomy + content type

        [TestMethod]
        [DoNotParallelize]
        public void Test023_Should_TaxonomyField_OnContentType_RoundTrip_Sync()
        {
            var sfx = NewSuffix();
            var taxUid = "tax_ct_" + sfx;
            var ctUid = "ct_with_tax_" + sfx;

            _stack.Taxonomy().Create(new TaxonomyModel
            {
                Uid = taxUid,
                Name = "Taxonomy for CT test " + sfx,
                Description = "integration"
            });

            try
            {
                var modelling = BuildContentTypeWithTaxonomyField(ctUid, taxUid, sfx);
                _stack.ContentType().Create(modelling);

                var fetched = _stack.ContentType(ctUid).Fetch().OpenTResponse<ContentTypeModel>().Modelling;
                var taxField = fetched.Schema.OfType<TaxonomyField>().FirstOrDefault(f => f.Uid == "taxonomies");
                AssertLogger.IsNotNull(taxField, "taxonomy field");
                AssertLogger.IsTrue(taxField.Taxonomies.Any(t => t.TaxonomyUid == taxUid), "binding uid");
            }
            finally
            {
                TryDeleteContentType(ctUid);
                TryDeleteTaxonomy(taxUid);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test024_Should_TaxonomyField_OnContentType_RoundTrip_Async()
        {
            var sfx = NewSuffix();
            var taxUid = "tax_ct_a_" + sfx;
            var ctUid = "ct_with_tax_a_" + sfx;

            await _stack.Taxonomy().CreateAsync(new TaxonomyModel
            {
                Uid = taxUid,
                Name = "Taxonomy async CT " + sfx,
                Description = "integration"
            });

            try
            {
                var modelling = BuildContentTypeWithTaxonomyField(ctUid, taxUid, sfx);
                await _stack.ContentType().CreateAsync(modelling);

                var fetched = (await _stack.ContentType(ctUid).FetchAsync()).OpenTResponse<ContentTypeModel>().Modelling;
                var taxField = fetched.Schema.OfType<TaxonomyField>().FirstOrDefault(f => f.Uid == "taxonomies");
                AssertLogger.IsNotNull(taxField, "taxonomy field async");
            }
            finally
            {
                TryDeleteContentType(ctUid);
                await TryDeleteTaxonomyAsync(taxUid);
            }
        }

        #endregion

        private static ContentModelling BuildContentTypeWithTaxonomyField(string ctUid, string taxUid, string sfx)
        {
            return new ContentModelling
            {
                Title = "Article With Taxonomy " + sfx,
                Uid = ctUid,
                Description = "CT taxonomy integration",
                Options = new Option
                {
                    IsPage = false,
                    Singleton = false,
                    Title = "title",
                    SubTitle = new List<string>()
                },
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true,
                        Unique = true,
                        FieldMetadata = new FieldMetadata { Description = "title" }
                    },
                    new TaxonomyField
                    {
                        DisplayName = "Topics",
                        Uid = "taxonomies",
                        DataType = "taxonomy",
                        Mandatory = false,
                        Multiple = true,
                        Taxonomies = new List<TaxonomyFieldBinding>
                        {
                            new TaxonomyFieldBinding
                            {
                                TaxonomyUid = taxUid,
                                MaxTerms = 5,
                                Mandatory = false,
                                Multiple = true,
                                NonLocalizable = false
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Resolves Mock/customUpload.html from typical test output / working directories.
        /// </summary>
        private static string ResolveCustomUploadHtmlPath()
        {
            var candidates = new[]
            {
                Path.Combine(AppContext.BaseDirectory ?? ".", "Mock", "customUpload.html"),
                Path.Combine(Directory.GetCurrentDirectory(), "Mock", "customUpload.html"),
                Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html"),
            };
            foreach (var relative in candidates)
            {
                try
                {
                    var full = Path.GetFullPath(relative);
                    if (File.Exists(full))
                        return full;
                }
                catch
                {
                    /* try next */
                }
            }

            AssertLogger.Fail("Could not find Mock/customUpload.html for extension upload. Ensure the file exists next to other Mock assets.");
            throw new InvalidOperationException("Unreachable: AssertLogger.Fail should throw.");
        }

        private static string ParseExtensionUidFromUploadResponse(ContentstackResponse response)
        {
            var jo = response.OpenJObjectResponse();
            var token = jo["extension"]?["uid"] ?? jo["uid"];
            return token?.ToString();
        }

        private string UploadDisposableCustomFieldExtensionAndGetUid(string sfx)
        {
            var path = ResolveCustomUploadHtmlPath();
            var title = "CT integration ext " + sfx;
            var fieldModel = new CustomFieldModel(path, "text/html", title, "text", isMultiple: false, tags: "ct_integration," + sfx);
            var response = _stack.Extension().Upload(fieldModel);
            if (!response.IsSuccessStatusCode)
            {
                AssertLogger.Fail($"Extension upload failed: {(int)response.StatusCode} {response.OpenResponse()}");
            }

            var uid = ParseExtensionUidFromUploadResponse(response);
            if (string.IsNullOrEmpty(uid))
            {
                AssertLogger.Fail("Extension upload succeeded but response contained no extension.uid.");
            }

            return uid;
        }

        private async Task<string> UploadDisposableCustomFieldExtensionAndGetUidAsync(string sfx)
        {
            var path = ResolveCustomUploadHtmlPath();
            var title = "CT integration ext async " + sfx;
            var fieldModel = new CustomFieldModel(path, "text/html", title, "text", isMultiple: false, tags: "ct_integration_async," + sfx);
            var response = await _stack.Extension().UploadAsync(fieldModel);
            if (!response.IsSuccessStatusCode)
            {
                AssertLogger.Fail($"Extension upload failed: {(int)response.StatusCode} {response.OpenResponse()}");
            }

            var uid = ParseExtensionUidFromUploadResponse(response);
            if (string.IsNullOrEmpty(uid))
            {
                AssertLogger.Fail("Extension upload succeeded but response contained no extension.uid.");
            }

            return uid;
        }

        private void TryDeleteExtension(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            try
            {
                _stack.Extension(uid).Delete();
            }
            catch
            {
                /* best-effort cleanup */
            }
        }

        private async Task TryDeleteExtensionAsync(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            try
            {
                await _stack.Extension(uid).DeleteAsync();
            }
            catch
            {
                /* best-effort cleanup */
            }
        }

        private void TryDeleteContentType(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            try
            {
                _stack.ContentType(uid).Delete();
            }
            catch
            {
                /* best-effort cleanup */
            }
        }

        private void TryDeleteTaxonomy(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            try
            {
                _stack.Taxonomy(uid).Delete();
            }
            catch
            {
                /* ignore */
            }
        }

        private async Task TryDeleteTaxonomyAsync(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;
            try
            {
                await _stack.Taxonomy(uid).DeleteAsync();
            }
            catch
            {
                /* ignore */
            }
        }
    }
}
