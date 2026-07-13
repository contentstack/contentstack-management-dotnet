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
        private MockHttpHandler _mockHttpHandler;

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            _contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            _mockHttpHandler = new MockHttpHandler(_contentstackResponse);
            client.ContentstackPipeline.ReplaceHandler(_mockHttpHandler);
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
                _contentstackResponse.OpenJsonObjectResponse().ToJsonString(),
                response.OpenJsonObjectResponse().ToJsonString()
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
                _contentstackResponse.OpenJsonObjectResponse().ToJsonString(),
                response.OpenJsonObjectResponse().ToJsonString()
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

        [TestMethod]
        public void Initialize_EntryVariant_With_BranchUid()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var branchUid = _fixture.Create<string>();

            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, branchUid: branchUid);

            Assert.AreEqual(branchUid, variant.branchUid);
        }

        [TestMethod]
        public void Should_Override_Branch_Header_On_Find_When_BranchUid_Provided()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var branchUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, branchUid: branchUid);

            variant.Find();

            Assert.AreEqual(branchUid, _mockHttpHandler.LastRequestHeaders["branch"]);
        }

        [TestMethod]
        public void Should_Override_Branch_Header_On_Create_When_BranchUid_Provided()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            var branchUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid, branchUid);
            var model = new { entry = new { banner_color = "Navy Blue" } };

            variant.Create(model);

            Assert.AreEqual(branchUid, _mockHttpHandler.LastRequestHeaders["branch"]);
        }

        [TestMethod]
        public void Should_Override_Branch_Header_On_Fetch_When_BranchUid_Provided()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            var branchUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid, branchUid);

            variant.Fetch();

            Assert.AreEqual(branchUid, _mockHttpHandler.LastRequestHeaders["branch"]);
        }

        [TestMethod]
        public void Should_Override_Branch_Header_On_Delete_When_BranchUid_Provided()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            var branchUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid, branchUid);

            variant.Delete();

            Assert.AreEqual(branchUid, _mockHttpHandler.LastRequestHeaders["branch"]);
        }

        [TestMethod]
        public void Should_Fallback_To_Stack_Branch_When_BranchUid_Is_Empty()
        {
            var stackBranchUid = _fixture.Create<string>();
            var client = new ContentstackClient();
            var mockHttpHandler = new MockHttpHandler(_contentstackResponse);
            client.ContentstackPipeline.ReplaceHandler(mockHttpHandler);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            var stack = new Stack(client, _fixture.Create<string>(), branchUid: stackBranchUid);

            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(stack, ctUid, entryUid, branchUid: "   ");

            variant.Find();

            Assert.AreEqual(stackBranchUid, mockHttpHandler.LastRequestHeaders["branch"]);
        }

        [TestMethod]
        public void Should_Publish_EntryVariant()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);
            var details = new PublishUnpublishDetails { Locales = new System.Collections.Generic.List<string> { "en-us" }, Version = 1 };

            ContentstackResponse response = variant.Publish(details);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.IsTrue(details.Variants.Exists(v => v.Uid == uid && v.Version == 1));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Publish_EntryVariant_Async()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);
            var details = new PublishUnpublishDetails();

            ContentstackResponse response = await variant.PublishAsync(details);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.IsTrue(details.Variants.Exists(v => v.Uid == uid));
        }

        [TestMethod]
        public void Should_Unpublish_EntryVariant()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);
            var details = new PublishUnpublishDetails();

            ContentstackResponse response = variant.Unpublish(details);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.IsTrue(details.Variants.Exists(v => v.Uid == uid));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Unpublish_EntryVariant_Async()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid);
            var details = new PublishUnpublishDetails();

            ContentstackResponse response = await variant.UnpublishAsync(details);

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.IsTrue(details.Variants.Exists(v => v.Uid == uid));
        }

        [TestMethod]
        public void Should_Override_Branch_Header_On_Publish_When_BranchUid_Provided()
        {
            var ctUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            var uid = _fixture.Create<string>();
            var branchUid = _fixture.Create<string>();
            EntryVariant variant = new EntryVariant(_stack, ctUid, entryUid, uid, branchUid);

            variant.Publish(new PublishUnpublishDetails());

            Assert.AreEqual(branchUid, _mockHttpHandler.LastRequestHeaders["branch"]);
        }
    }
}