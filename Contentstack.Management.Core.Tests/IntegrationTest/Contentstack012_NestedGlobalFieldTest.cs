using System;
using System.Collections.Generic;
using System.IO;
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
                        FieldMetadata = new FieldMetadata
                        {
                            Default = "true"
                        }
                    },
                    new TextboxField
                    {
                        DisplayName = "Description",
                        Uid = "description",
                        DataType = "text",
                        Mandatory = false,
                        FieldMetadata = new FieldMetadata
                        {
                            Description = "A description field"
                        }
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
                        FieldMetadata = new FieldMetadata
                        {
                            Description = "",
                            DefaultValue = "",
                            Version = 3
                        }
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
                        FieldMetadata = new FieldMetadata
                        {
                            Description = "Reference to another global field"
                        }
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
            var referencedGlobalFieldModel = CreateReferencedGlobalFieldModel();
            ContentstackResponse response = _stack.GlobalField().Create(referencedGlobalFieldModel);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(referencedGlobalFieldModel.Title, globalField.Modelling.Title);
            Assert.AreEqual(referencedGlobalFieldModel.Uid, globalField.Modelling.Uid);
            Assert.AreEqual(referencedGlobalFieldModel.Schema.Count, globalField.Modelling.Schema.Count);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Create_Nested_Global_Field()
        {
            var nestedGlobalFieldModel = CreateNestedGlobalFieldModel();
            ContentstackResponse response = _stack.GlobalField().Create(nestedGlobalFieldModel);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(nestedGlobalFieldModel.Title, globalField.Modelling.Title);
            Assert.AreEqual(nestedGlobalFieldModel.Uid, globalField.Modelling.Uid);
            Assert.AreEqual(nestedGlobalFieldModel.Schema.Count, globalField.Modelling.Schema.Count);
          
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Fetch_Nested_Global_Field()
        {

            ContentstackResponse response = _stack.GlobalField("nested_global_field_test").Fetch();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);

            Assert.IsTrue(globalField.Modelling.Schema.Count >= 2);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test004_Should_Fetch_Async_Nested_Global_Field()
        {

            ContentstackResponse response = await _stack.GlobalField("nested_global_field_test").FetchAsync();
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Update_Nested_Global_Field()
        {
            var updateModel = new ContentModelling
            {
                Title = "Updated Nested Global Field",
                Uid = "nested_global_field_test",
                Description = "Updated description for nested global field",
                Schema = CreateNestedGlobalFieldModel().Schema,
                GlobalFieldRefs = CreateNestedGlobalFieldModel().GlobalFieldRefs
            };

            ContentstackResponse response = _stack.GlobalField("nested_global_field_test").Update(updateModel);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(updateModel.Title, globalField.Modelling.Title);
            Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Test006_Should_Update_Async_Nested_Global_Field()
        {
            var updateModel = new ContentModelling
            {
                Title = "Updated Async Nested Global Field",
                Uid = "nested_global_field_test",
                Description = "Updated async description for nested global field",
                Schema = CreateNestedGlobalFieldModel().Schema,
                GlobalFieldRefs = CreateNestedGlobalFieldModel().GlobalFieldRefs
            };

            ContentstackResponse response = await _stack.GlobalField("nested_global_field_test").UpdateAsync(updateModel);
            GlobalFieldModel globalField = response.OpenTResponse<GlobalFieldModel>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(globalField);
            Assert.IsNotNull(globalField.Modelling);
            Assert.AreEqual(updateModel.Title, globalField.Modelling.Title);
            Assert.AreEqual("nested_global_field_test", globalField.Modelling.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test007_Should_Query_Nested_Global_Fields()
        {

            ContentstackResponse response = _stack.GlobalField().Query().Find();
            GlobalFieldsModel globalFields = response.OpenTResponse<GlobalFieldsModel>();

            Assert.IsNotNull(response);
            Assert.IsNotNull(globalFields);
            Assert.IsNotNull(globalFields.Modellings);
            Assert.IsTrue(globalFields.Modellings.Count >= 1);

            var nestedGlobalField = globalFields.Modellings.Find(gf => gf.Uid == "nested_global_field_test");
            Assert.IsNotNull(nestedGlobalField);
            Assert.AreEqual("nested_global_field_test", nestedGlobalField.Uid);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test009_Should_Delete_Referenced_Global_Field()
        {
            // This has been used to avoid tthe confirmation prompt during deletion in case the global field is referenced
            var parameters = new ParameterCollection();
            parameters.Add("force", "true");
            ContentstackResponse response = _stack.GlobalField("referenced_global_field").Delete(parameters);

            Assert.IsNotNull(response);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test008_Should_Delete_Nested_Global_Field()
        {
            ContentstackResponse response = _stack.GlobalField("nested_global_field_test").Delete();
            Assert.IsNotNull(response);
        }


    }
}