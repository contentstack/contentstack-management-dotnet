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
            StackResponse response = StackResponse.getStack(Contentstack.Client.serializer);
            _stack = Contentstack.Client.Stack(response.Stack.APIKey);
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test001_Should_Create_Asset()
        {
            
            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");
            try
            {
                AssetModel asset = new AssetModel("contentTypeSchema.json", path, "application/json", title:"New.json", description:"new test desc", parentUID: "bltcbc90d17c326ae8a", tags:"one,two");
                ContentstackResponse response = _stack.Asset().Create(asset);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            }catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test004_Should_Create_Dashboard()
        {

            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html");
            try
            {
                DashboardWidgetModel dashboard = new DashboardWidgetModel(path, "text/html", "Dashboard", isEnable: true, defaultWidth: "half", tags: "one,two");
                ContentstackResponse response = _stack.Extension().Upload(dashboard);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test002_Should_Create_Custom_Widget()
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
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test003_Should_Create_Custom_field()
        {

            var path = Path.Combine(System.Environment.CurrentDirectory, "../../../Mock/customUpload.html");
            try
            {
                CustomFieldModel fieldModel = new CustomFieldModel(path, "text/html", "Custom field Upload", "text", isMultiple: false, tags: "one,two");
                ContentstackResponse response = _stack.Extension().Upload(fieldModel);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

    }
}
