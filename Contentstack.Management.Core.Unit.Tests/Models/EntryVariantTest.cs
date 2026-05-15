using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class EntryVariantTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();
        private ContentstackResponse _contentstackResponse;

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(_contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Initialize_EntryVariant()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();

            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid);

            Assert.IsNull(variant.Uid);
            Assert.AreEqual($"/content_types/{ctUid}/entries/{entryUid}/variants", variant.resourcePath);
        }

        [TestMethod]
        public void Initialize_EntryVariant_With_Uid()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();

            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);

            Assert.AreEqual(uid, variant.Uid);
            Assert.AreEqual($"/content_types/{ctUid}/entries/{entryUid}/variants/{uid}", variant.resourcePath);
        }

        [TestMethod]
        public void Should_Throw_ArgumentNullException_On_Null_Stack()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            
            Assert.ThrowsException<ArgumentNullException>(() => new EntryVariant(null, ctUid, entryUid));
        }

        [TestMethod]
        public void Should_Find_EntryVariants()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid);

            ContentstackResponse response = variant.Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_EntryVariants_Async()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid);

            ContentstackResponse response = await variant.FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(
                _contentstackResponse.OpenJObjectResponse().ToString(),
                response.OpenJObjectResponse().ToString()
            );
        }

        [TestMethod]
        public void Should_Create_EntryVariant()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);

            var model = new { entry = new { banner_color = "Navy Blue" } };

            ContentstackResponse response = variant.Create(model);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_EntryVariant_Async()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);

            var model = new { entry = new { banner_color = "Navy Blue" } };

            ContentstackResponse response = await variant.CreateAsync(model);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
        }
        
        [TestMethod]
        public void Should_Update_EntryVariant()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);

            var model = new { entry = new { banner_color = "Red" } };

            ContentstackResponse response = variant.Update(model);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
        }

        [TestMethod]
        public void Should_Delete_EntryVariant()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);

            ContentstackResponse response = variant.Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
        }
    }
}