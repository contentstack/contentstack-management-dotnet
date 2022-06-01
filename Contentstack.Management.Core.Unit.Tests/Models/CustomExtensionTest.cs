using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.CustomExtension;
using Contentstack.Management.Core.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class CustomExtensionTest
    {

        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Filepath_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CustomFieldModel(byteArray: null, "", "", ""));
            Assert.ThrowsException<ArgumentNullException>(() => new CustomWidgetModel(byteArray: null, "", ""));
            Assert.ThrowsException<ArgumentNullException>(() => new DashboardWidgetModel(byteArray: null, "", ""));
        }

        [TestMethod]
        public void Should_Return_CustomFieldModel()
        {
            var title = _fixture.Create<string>();
            var dataType = _fixture.Create<string>();
            var contentType = "application/text";
            CustomFieldModel customFieldModel = new CustomFieldModel("../../../../README.md", contentType, title, dataType);

            Assert.AreEqual(title, customFieldModel.Title);
            Assert.AreEqual(dataType, customFieldModel.DataType);
            Assert.AreEqual(contentType, customFieldModel.ContentType);
        }

        [TestMethod]
        public async Task Should_Return_CustomFieldModel_Multipart()
        {
            var title = _fixture.Create<string>();
            var dataType = _fixture.Create<string>();
            
            var tags = _fixture.Create<string>();
            CustomFieldModel assetModel = new CustomFieldModel("../../../../README.md", "application/text", title, dataType, true, tags);

            var content = assetModel.GetHttpContent();
            var stringContent = await content.ReadAsStringAsync();

            Assert.IsTrue(stringContent.Contains(title));
            Assert.IsTrue(stringContent.Contains(dataType));
            Assert.IsTrue(stringContent.Contains("true"));
            Assert.IsTrue(stringContent.Contains(tags));
            Assert.IsTrue(stringContent.Contains("field"));
            Assert.IsFalse(stringContent.Contains("widget"));
            Assert.IsFalse(stringContent.Contains("dashboard"));

        }

        [TestMethod]
        public void Should_Return_CustomWidgetModel()
        {
            var title = _fixture.Create<string>();
            var dataType = _fixture.Create<string>();
            var contentType = "application/text";
            CustomWidgetModel customFieldModel = new CustomWidgetModel("../../../../README.md", contentType, title);

            Assert.AreEqual(title, customFieldModel.Title);
            Assert.AreEqual(contentType, customFieldModel.ContentType);
        }

        [TestMethod]
        public async Task Should_Return_CustomWidgetModel_Multipart()
        {
            var title = _fixture.Create<string>();
            var tags = _fixture.Create<string>();

            CustomWidgetModel assetModel = new CustomWidgetModel("../../../../README.md", "application/text", title, tags, _fixture.Create<ExtensionScope>());

            var content = assetModel.GetHttpContent();
            var stringContent = await content.ReadAsStringAsync();

            Assert.IsTrue(stringContent.Contains(title));
            Assert.IsTrue(stringContent.Contains(tags));
            Assert.IsTrue(stringContent.Contains("widget"));
            Assert.IsFalse(stringContent.Contains("dashboard"));
            Assert.IsFalse(stringContent.Contains("field"));
        }

        [TestMethod]
        public void Should_Return_DashboardWidgetModel()
        {
            var title = _fixture.Create<string>();
            var dataType = _fixture.Create<string>();
            var contentType = "application/text";
            DashboardWidgetModel customFieldModel = new DashboardWidgetModel("../../../../README.md", contentType, title);

            Assert.AreEqual(title, customFieldModel.Title);
            Assert.AreEqual(contentType, customFieldModel.ContentType);
        }

        [TestMethod]
        public async Task Should_Return_DashboardWidgetModel_Multipart()
        {
            var title = _fixture.Create<string>();
            var tags = _fixture.Create<string>();
            DashboardWidgetModel assetModel = new DashboardWidgetModel("../../../../README.md", "application/text", title, tags);

            var content = assetModel.GetHttpContent();
            var stringContent = await content.ReadAsStringAsync();

            Assert.IsTrue(stringContent.Contains(title));
            Assert.IsTrue(stringContent.Contains(tags));
            Assert.IsTrue(stringContent.Contains("dashboard"));
            Assert.IsFalse(stringContent.Contains("widget"));
            Assert.IsFalse(stringContent.Contains("field"));

        }
    }
}