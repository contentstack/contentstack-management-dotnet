using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Utils;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;


namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack007_EntryTest
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
        public async System.Threading.Tasks.Task Test001_Should_Create_Entry()
        {
            PageJSONRTE pg = new PageJSONRTE()
            {
                Title = "My First JSON-Rte Entry",
                RteData = new Node
                {
                    type = "doc",
                    children = new List<Node>
                    {
                        new Node
                        {
                            type = "p",
                            attrs = new Dictionary<string, object>
                            {
                                {"style", new Object() },
                                {"redactor-attributes", new Object() },
                                {"dir", "ltr" }
                            },
                            children = new List<Node>
                            {
                                new TextNode
                                {
                                    text = "My new text",
                                    attrs = null,
                                    type = "text"
                                }
                            }
                        }
                    },
                    attrs = null
                }

            };
            //JsonSerializer js = new JsonSerializer();
            //js.Converters.Add(new NodeJsonConverter);
            //js.Serialize();

            try
            {
                ContentstackResponse response = await _stack.ContentType("page_json_rte").Entry().CreateAsync(pg);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            }catch (Exception e)
            {
                Assert.Fail(e.Data+ e.Message);
            }
        }

    }
}
