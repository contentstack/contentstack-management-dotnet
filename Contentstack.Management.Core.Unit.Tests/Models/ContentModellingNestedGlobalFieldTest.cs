using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class ContentModellingNestedGlobalFieldTest
    {
        private readonly IFixture _fixture = new Fixture();

        [TestMethod]
        public void Should_Create_ContentModelling_With_Nested_Global_Fields()
        {
            var contentModelling = new ContentModelling
            {
                Title = "nested_global_field_test",
                Uid = "nested_global_field_test",
                Description = "Test nested global field functionality",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true
                    },
                    new GlobalFieldReference
                    {
                        DisplayName = "Product Information",
                        Uid = "product_info",
                        DataType = "global_field",
                        ReferenceTo = "referenced_global_field",
                        Mandatory = true,
                        Multiple = false,
                        Unique = false,
                        NonLocalizable = false
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

            Assert.IsNotNull(contentModelling);
            Assert.AreEqual("nested_global_field_test", contentModelling.Title);
            Assert.AreEqual("nested_global_field_test", contentModelling.Uid);
            Assert.AreEqual("Test nested global field functionality", contentModelling.Description);
            Assert.IsNotNull(contentModelling.Schema);
            Assert.AreEqual(2, contentModelling.Schema.Count);
            Assert.IsNotNull(contentModelling.GlobalFieldRefs);
            Assert.AreEqual(1, contentModelling.GlobalFieldRefs.Count);

            var globalFieldRef = contentModelling.Schema[1] as GlobalFieldReference;
            Assert.IsNotNull(globalFieldRef);
            Assert.AreEqual("Product Information", globalFieldRef.DisplayName);
            Assert.AreEqual("product_info", globalFieldRef.Uid);
            Assert.AreEqual("global_field", globalFieldRef.DataType);
            Assert.AreEqual("referenced_global_field", globalFieldRef.ReferenceTo);

            var globalFieldRefs = contentModelling.GlobalFieldRefs[0];
            Assert.AreEqual("referenced_global_field", globalFieldRefs.Uid);
            Assert.AreEqual(1, globalFieldRefs.OccurrenceCount);
            Assert.IsTrue(globalFieldRefs.IsChild);
            Assert.AreEqual(1, globalFieldRefs.Paths.Count);
            Assert.AreEqual("schema.1", globalFieldRefs.Paths[0]);
        }

        [TestMethod]
        public void Should_Serialize_ContentModelling_With_Nested_Global_Fields()
        {
            var contentModelling = new ContentModelling
            {
                Title = "nested_global_field_test",
                Uid = "nested_global_field_test",
                Description = "Test nested global field functionality",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true
                    },
                    new GlobalFieldReference
                    {
                        DisplayName = "Product Information",
                        Uid = "product_info",
                        DataType = "global_field",
                        ReferenceTo = "referenced_global_field",
                        Mandatory = true,
                        Multiple = false,
                        Unique = false,
                        NonLocalizable = false
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

            var json = JsonConvert.SerializeObject(contentModelling);

            Assert.IsTrue(json.Contains("\"title\":\"nested_global_field_test\""));
            Assert.IsTrue(json.Contains("\"uid\":\"nested_global_field_test\""));
            Assert.IsTrue(json.Contains("\"description\":\"Test nested global field functionality\""));
            Assert.IsTrue(json.Contains("\"schema\""));
            Assert.IsTrue(json.Contains("\"global_field_refs\""));
            Assert.IsTrue(json.Contains("\"data_type\":\"global_field\""));
            Assert.IsTrue(json.Contains("\"reference_to\":\"referenced_global_field\""));
        }



        [TestMethod]
        public void Should_Create_ContentModelling_With_Multiple_Nested_Global_Fields()
        {
            var contentModelling = new ContentModelling
            {
                Title = "Complex Nested Global Field",
                Uid = "complex_nested_global_field",
                Description = "Complex nested global field with multiple references",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true
                    },
                    new GlobalFieldReference
                    {
                        DisplayName = "Product Info",
                        Uid = "product_info",
                        DataType = "global_field",
                        ReferenceTo = "product_information",
                        Mandatory = true,
                        Multiple = false
                    },
                    new GlobalFieldReference
                    {
                        DisplayName = "Category Info",
                        Uid = "category_info",
                        DataType = "global_field",
                        ReferenceTo = "category_information",
                        Mandatory = false,
                        Multiple = true
                    },
                    new GroupField
                    {
                        DisplayName = "Additional Info",
                        Uid = "additional_info",
                        DataType = "group",
                        Schema = new List<Field>
                        {
                            new GlobalFieldReference
                            {
                                DisplayName = "Nested Product",
                                Uid = "nested_product",
                                DataType = "global_field",
                                ReferenceTo = "product_information",
                                Mandatory = true,
                                Multiple = false
                            }
                        }
                    }
                },
                GlobalFieldRefs = new List<GlobalFieldRefs>
                {
                    new GlobalFieldRefs
                    {
                        Uid = "product_information",
                        OccurrenceCount = 2,
                        IsChild = true,
                        Paths = new List<string> { "schema.1", "schema.3.schema.0" }
                    },
                    new GlobalFieldRefs
                    {
                        Uid = "category_information",
                        OccurrenceCount = 1,
                        IsChild = true,
                        Paths = new List<string> { "schema.2" }
                    }
                }
            };

            Assert.IsNotNull(contentModelling);
            Assert.AreEqual("Complex Nested Global Field", contentModelling.Title);
            Assert.AreEqual(4, contentModelling.Schema.Count);
            Assert.AreEqual(2, contentModelling.GlobalFieldRefs.Count);

            var productInfoRef = contentModelling.Schema[1] as GlobalFieldReference;
            Assert.IsNotNull(productInfoRef);
            Assert.AreEqual("product_information", productInfoRef.ReferenceTo);

            var categoryInfoRef = contentModelling.Schema[2] as GlobalFieldReference;
            Assert.IsNotNull(categoryInfoRef);
            Assert.AreEqual("category_information", categoryInfoRef.ReferenceTo);

            var groupField = contentModelling.Schema[3] as GroupField;
            Assert.IsNotNull(groupField);
            Assert.AreEqual(1, groupField.Schema.Count);
            
            var nestedProductRef = groupField.Schema[0] as GlobalFieldReference;
            Assert.IsNotNull(nestedProductRef);
            Assert.AreEqual("product_information", nestedProductRef.ReferenceTo);

            var productRefs = contentModelling.GlobalFieldRefs[0];
            Assert.AreEqual("product_information", productRefs.Uid);
            Assert.AreEqual(2, productRefs.OccurrenceCount);
            Assert.AreEqual(2, productRefs.Paths.Count);
            Assert.AreEqual("schema.1", productRefs.Paths[0]);
            Assert.AreEqual("schema.3.schema.0", productRefs.Paths[1]);

            var categoryRefs = contentModelling.GlobalFieldRefs[1];
            Assert.AreEqual("category_information", categoryRefs.Uid);
            Assert.AreEqual(1, categoryRefs.OccurrenceCount);
            Assert.AreEqual(1, categoryRefs.Paths.Count);
            Assert.AreEqual("schema.2", categoryRefs.Paths[0]);
        }

        [TestMethod]
        public void Should_Handle_ContentModelling_With_Null_GlobalFieldRefs()
        {
            var contentModelling = new ContentModelling
            {
                Title = "Simple Global Field",
                Uid = "simple_global_field",
                Description = "Simple global field without nested references",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true
                    }
                },
                GlobalFieldRefs = null
            };

            var json = JsonConvert.SerializeObject(contentModelling);
            Assert.IsTrue(json.Contains("\"title\":\"Simple Global Field\""));
            Assert.IsFalse(json.Contains("\"global_field_refs\""));
        }

        [TestMethod]
        public void Should_Handle_ContentModelling_With_Empty_GlobalFieldRefs()
        {
            var contentModelling = new ContentModelling
            {
                Title = "Empty Global Field",
                Uid = "empty_global_field",
                Description = "Global field with empty references",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true
                    }
                },
                GlobalFieldRefs = new List<GlobalFieldRefs>()
            };

            var json = JsonConvert.SerializeObject(contentModelling);

            Assert.IsTrue(json.Contains("\"title\":\"Empty Global Field\""));
            Assert.IsTrue(json.Contains("\"global_field_refs\":[]"));
        }

        [TestMethod]
        public void Should_Serialize_Complex_ContentModelling_With_Nested_Global_Fields()
        {
            var contentModelling = new ContentModelling
            {
                Title = "Complex Nested Global Field",
                Uid = "complex_nested_global_field",
                Description = "Complex nested global field with multiple references",
                Schema = new List<Field>
                {
                    new TextboxField
                    {
                        DisplayName = "Title",
                        Uid = "title",
                        DataType = "text",
                        Mandatory = true
                    },
                    new GlobalFieldReference
                    {
                        DisplayName = "Product Info",
                        Uid = "product_info",
                        DataType = "global_field",
                        ReferenceTo = "product_information",
                        Mandatory = true,
                        Multiple = false
                    },
                    new GlobalFieldReference
                    {
                        DisplayName = "Category Info",
                        Uid = "category_info",
                        DataType = "global_field",
                        ReferenceTo = "category_information",
                        Mandatory = false,
                        Multiple = true
                    }
                },
                GlobalFieldRefs = new List<GlobalFieldRefs>
                {
                    new GlobalFieldRefs
                    {
                        Uid = "product_information",
                        OccurrenceCount = 1,
                        IsChild = true,
                        Paths = new List<string> { "schema.1" }
                    },
                    new GlobalFieldRefs
                    {
                        Uid = "category_information",
                        OccurrenceCount = 1,
                        IsChild = true,
                        Paths = new List<string> { "schema.2" }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(contentModelling);

            Assert.IsTrue(json.Contains("\"title\":\"Complex Nested Global Field\""));
            Assert.IsTrue(json.Contains("\"global_field_refs\""));
            Assert.IsTrue(json.Contains("\"product_information\""));
            Assert.IsTrue(json.Contains("\"category_information\""));
            Assert.IsTrue(json.Contains("\"occurrence_count\":1"));
            Assert.IsTrue(json.Contains("\"isChild\":true"));
        }


    }
} 