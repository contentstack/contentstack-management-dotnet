using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
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

            var path = Path.Combine(Environment.CurrentDirectory, "../../../Mock/contentTypeSchema.json");

            AssetModel asset = new AssetModel("contentTypeSchema.json", path, "application/json");
            ContentstackResponse response = _stack.Asset().Create(asset);
            
        }

    }
}
