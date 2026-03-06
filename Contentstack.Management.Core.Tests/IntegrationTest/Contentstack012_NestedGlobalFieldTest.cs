using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack008_NestedGlobalFieldTest
    {
        private Stack _stack;

        private static string _host => Contentstack.Client.contentstackOptions.Host;

        [TestInitialize]
        public void Initialize()
        {
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
        }

        private ContentModelling CreateReferencedGlobalFieldModel()
        {
            return new ContentModelling
            {
                Title = "Referenced Global Field",
                Uid = "referenced_global_field",
                Description = "A global field that will be referenced by another global field",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true,
                        Unique = true,
                        FieldMetadata = new FieldMetadata { Default = "true" }
                    },
                    new TextboxField
                    {
                        DisplayName = "Description",
                        Uid = "description",
                        DataType = "text",
                        Mandatory = false,
                        FieldMetadata = new FieldMetadata { Description = "A description field" }
                    }
                }
            };
        }

        private ContentModelling CreateNestedGlobalFieldModel()
        {
            return new ContentModelling
            {
                Title = "Nested Global Field Test",
                Uid = "nested_global_field_test",
                Description = "Test nested global field for .NET SDK",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Single Line Textbox",
                        Uid = "single_line",
                        DataType = "text",
                        Mandatory = false,
                        Multiple = false,
                        Unique = false,
                        FieldMetadata = new FieldMetadata { Description = "", DefaultValue = "", Version = 3 }
                    },
                    new GlobalFieldReference
                    {
                        DisplayName = "Global Field Reference",
                        Uid = "global_field_reference",
                        DataType = "global_field",
                        ReferenceTo = "referenced_global_field",
                        Mandatory = false,
                        Multiple = false,
                        Unique = false,
                        NonLocalizable = false,
                        FieldMetadata = new FieldMetadata { Description = "Reference to another global field" }
                    }
                },
                GlobalFieldRefs = new List<GlobalFieldRefs>
                {
                    new GlobalFieldRefs
                    {
                        Uid = "referenced_global_field",
                        OccurrenceCount = 1,
                        IsChild = true,
                        Paths = new List<string> { "schema.1" }
                    }
                }
            };
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Create_Referenced_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var referencedGlobalFieldModel = CreateReferencedGlobalFieldModel();
                TestReportHelper.LogRequest("_stack.GlobalField().Create(referenced)", "POST",
                    $"https://{_host}/v3/stacks/global_fields");

                ContentstackResponse response = _stack.GlobalField().Create(referencedGlobalFieldModel);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Title == referencedGlobalFieldModel.Title,
                    "Title matches", expected: referencedGlobalFieldModel.Title, actual: globalField?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(referencedGlobalFieldModel.Title, globalField.Modelling.Title);
                Assert.AreEqual(referencedGlobalFieldModel.Uid, globalField.Modelling.Uid);
                Assert.AreEqual(referencedGlobalFieldModel.Schema.Count, globalField.Modelling.Schema.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_Nested_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var nestedGlobalFieldModel = CreateNestedGlobalFieldModel();
                TestReportHelper.LogRequest("_stack.GlobalField().Create(nested)", "POST",
                    $"https://{_host}/v3/stacks/global_fields");

                ContentstackResponse response = _stack.GlobalField().Create(nestedGlobalFieldModel);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Title == nestedGlobalFieldModel.Title,
                    "Title matches", expected: nestedGlobalFieldModel.Title, actual: globalField?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(nestedGlobalFieldModel.Title, globalField.Modelling.Title);
                Assert.AreEqual(nestedGlobalFieldModel.Uid, globalField.Modelling.Uid);
                Assert.AreEqual(nestedGlobalFieldModel.Schema.Count, globalField.Modelling.Schema.Count);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Nested_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField(nested_uid).Fetch()", "GET",
                    $"https://{_host}/v3/stacks/global_fields/nested_global_field_test");

                ContentstackResponse response = _stack.GlobalField("nested_global_field_test").Fetch();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Uid == "nested_global_field_test",
                    "UID matches", expected: "nested_global_field_test", actual: globalField?.Modelling?.Uid, type: "AreEqual");
                TestReportHelper.LogAssertion(globalField?.Modelling?.Schema?.Count >= 2,
                    "Schema has at least 2 fields", expected: ">=2", actual: globalField?.Modelling?.Schema?.Count.ToString(), type: "IsTrue");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);
                Assert.IsTrue(globalField.Modelling.Schema.Count >= 2);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Async_Nested_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField(nested_uid).FetchAsync()", "GET",
                    $"https://{_host}/v3/stacks/global_fields/nested_global_field_test");

                ContentstackResponse response = await _stack.GlobalField("nested_global_field_test").FetchAsync();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Uid == "nested_global_field_test",
                    "UID matches", expected: "nested_global_field_test", actual: globalField?.Modelling?.Uid, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Nested_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var updateModel = new ContentModelling
                {
                    Title = "Updated Nested Global Field",
                    Uid = "nested_global_field_test",
                    Description = "Updated description for nested global field",
                    Schema = CreateNestedGlobalFieldModel().Schema,
                    GlobalFieldRefs = CreateNestedGlobalFieldModel().GlobalFieldRefs
                };
                TestReportHelper.LogRequest("_stack.GlobalField(nested_uid).Update()", "PUT",
                    $"https://{_host}/v3/stacks/global_fields/nested_global_field_test");

                ContentstackResponse response = _stack.GlobalField("nested_global_field_test").Update(updateModel);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Title == updateModel.Title,
                    "Title matches", expected: updateModel.Title, actual: globalField?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(updateModel.Title, globalField.Modelling.Title);
                Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Async_Nested_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var updateModel = new ContentModelling
                {
                    Title = "Updated Async Nested Global Field",
                    Uid = "nested_global_field_test",
                    Description = "Updated async description for nested global field",
                    Schema = CreateNestedGlobalFieldModel().Schema,
                    GlobalFieldRefs = CreateNestedGlobalFieldModel().GlobalFieldRefs
                };
                TestReportHelper.LogRequest("_stack.GlobalField(nested_uid).UpdateAsync()", "PUT",
                    $"https://{_host}/v3/stacks/global_fields/nested_global_field_test");

                ContentstackResponse response = await _stack.GlobalField("nested_global_field_test").UpdateAsync(updateModel);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

                TestReportHelper.LogAssertion(globalField?.Modelling?.Title == updateModel.Title,
                    "Title matches", expected: updateModel.Title, actual: globalField?.Modelling?.Title, type: "AreEqual");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalField);
                Assert.IsNotNull(globalField.Modelling);
                Assert.AreEqual(updateModel.Title, globalField.Modelling.Title);
                Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_Nested_Global_Fields()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField().Query().Find()", "GET",
                    $"https://{_host}/v3/stacks/global_fields");

                ContentstackResponse response = _stack.GlobalField().Query().Find();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                GlobalFieldsModel globalFields = response.OpenTResponse<GlobalFieldsModel>();

                TestReportHelper.LogAssertion(globalFields?.Modellings?.Count >= 1,
                    "Modellings count >= 1", expected: ">=1", actual: globalFields?.Modellings?.Count.ToString(), type: "IsTrue");
                Assert.IsNotNull(response);
                Assert.IsNotNull(globalFields);
                Assert.IsNotNull(globalFields.Modellings);
                Assert.IsTrue(globalFields.Modellings.Count >= 1);

                var nestedGlobalField = globalFields.Modellings.Find(gf => gf.Uid == "nested_global_field_test");
                Assert.IsNotNull(nestedGlobalField);
                Assert.AreEqual("nested_global_field_test", nestedGlobalField.Uid);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Delete_Nested_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                TestReportHelper.LogRequest("_stack.GlobalField(nested_uid).Delete()", "DELETE",
                    $"https://{_host}/v3/stacks/global_fields/nested_global_field_test");

                ContentstackResponse response = _stack.GlobalField("nested_global_field_test").Delete();
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Delete_Referenced_Global_Field()
        {
            TestReportHelper.Begin();
            var sw = Stopwatch.StartNew();
            try
            {
                var parameters = new ParameterCollection();
                parameters.Add("force", "true");
                TestReportHelper.LogRequest("_stack.GlobalField(ref_uid).Delete(force)", "DELETE",
                    $"https://{_host}/v3/stacks/global_fields/referenced_global_field",
                    queryParams: new Dictionary<string, string> { ["force"] = "true" });

                ContentstackResponse response = _stack.GlobalField("referenced_global_field").Delete(parameters);
                sw.Stop();
                TestReportHelper.LogResponse((int)response.StatusCode,
                    response.StatusCode.ToString(), sw.ElapsedMilliseconds, response.OpenResponse());

                TestReportHelper.LogAssertion(response != null, "Response not null", type: "IsNotNull");
                Assert.IsNotNull(response);
            }
            catch (Exception e)
            {
                sw.Stop();
                TestReportHelper.LogAssertion(false, $"Exception: {e.GetType().Name} — {e.Message}", type: "Fail");
                throw;
            }
            finally
            {
                TestReportHelper.Flush();
            }
        }
    }
}
