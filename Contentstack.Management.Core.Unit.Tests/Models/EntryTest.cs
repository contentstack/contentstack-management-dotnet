using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Models.ContentModel;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class EntryTest
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
        public void Initialize_Entry()
        {
            var contentTypeUID = _fixture.Create<string>();
            Entry entry = new Entry(_stack, contentTypeUID, null);

            Assert.IsNull(entry.Uid);
            Assert.AreEqual($"/content_types/{contentTypeUID}/entries", entry.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => entry.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => entry.Update(_fixture.Create<EntryModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.UpdateAsync(_fixture.Create<EntryModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => entry.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.DeleteAsync());
            Assert.ThrowsException<InvalidOperationException>(() => entry.DeleteMultipleLocal(_fixture.Create<List<string>>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.DeleteMultipleLocalAsync(_fixture.Create<List<string>>()));
            Assert.ThrowsException<InvalidOperationException>(() => entry.Localize(_fixture.Create<EntryModel>(), "fr-fr"));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.LocalizeAsync(_fixture.Create<EntryModel>(), "fr-fr"));
            Assert.ThrowsException<InvalidOperationException>(() => entry.Unlocalize(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.UnlocalizeAsync(_fixture.Create<string>()));
            Assert.ThrowsException<InvalidOperationException>(() => entry.Locales());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.LocalesAsync());
            Assert.ThrowsException<InvalidOperationException>(() => entry.References());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.ReferencesAsync());
            Assert.ThrowsException<InvalidOperationException>(() => entry.Publish(_fixture.Create<PublishUnpublishDetails>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.PublishAsync(_fixture.Create<PublishUnpublishDetails>()));
            Assert.ThrowsException<InvalidOperationException>(() => entry.Unpublish(_fixture.Create<PublishUnpublishDetails>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.UnpublishAsync(_fixture.Create<PublishUnpublishDetails>()));
            Assert.ThrowsException<InvalidOperationException>(() => entry.Export(_fixture.Create<string>()));
            Assert.AreEqual(entry.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Entry_With_Uid()
        {
            string uid = _fixture.Create<string>();
            var contentTypeUID = _fixture.Create<string>();
            Entry entry = new Entry(_stack, contentTypeUID, uid);

            Assert.AreEqual(uid, entry.Uid);
            Assert.AreEqual($"/content_types/{contentTypeUID}/entries/{uid}", entry.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => entry.Create(_fixture.Create<EntryModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => entry.CreateAsync(_fixture.Create<EntryModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => entry.Query());
        }

        [TestMethod]
        public void Should_Create_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry().Create(_fixture.Create<EntryModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry().CreateAsync(_fixture.Create<EntryModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Query_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry().Query().Find();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Query_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry().Query().FindAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Fetch_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Fetch();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Find_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).FetchAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Update(_fixture.Create<EntryModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).UpdateAsync(_fixture.Create<EntryModel>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Delete();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).DeleteAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Locale_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).DeleteMultipleLocal(_fixture.Create<List<string>>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Locale_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).DeleteMultipleLocalAsync(_fixture.Create<List<string>>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Localize_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Localize(_fixture.Create<EntryModel>(), _fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Localize_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).LocalizeAsync(_fixture.Create<EntryModel>(), _fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Unlocalize_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Unlocalize(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Unlocalize_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).UnlocalizeAsync(_fixture.Create<string>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Get_Locale_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Locales();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Get_Locale_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).LocalesAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Get_References_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).References();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Get_References_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).ReferencesAsync();

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Publish_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Publish(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Publish_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).PublishAsync(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
        [TestMethod]
        public void Should_Unpublish_Entry()
        {
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Unpublish(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Unpublish_Entry_Async()
        {
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).UnpublishAsync(_fixture.Create<PublishUnpublishDetails>());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Export_Entry()
        {
            var filePath = "entry.json";
            _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Export(filePath, new ParameterCollection());
            var text = File.ReadAllText(filePath);
            Assert.AreEqual(_contentstackResponse.OpenResponse(), text);
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), text);
        }

        [TestMethod]
        public void Should_Export_Throw_OnError()
        {
            var client = new ContentstackClient();
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ContentstackErrorException>(() => stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Export(_fixture.Create<string>(), new ParameterCollection()));
        }

        [TestMethod]
        public void Should_Import_Entry()
        {
            var filePath = "entry.json";
            ContentstackResponse response = _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).Import(filePath, new ParameterCollection());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());

        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_ImportAsync_Entry()
        {
            var filePath = "entry.json";
            ContentstackResponse response = await _stack.ContentType(_fixture.Create<string>()).Entry(_fixture.Create<string>()).ImportAsync(filePath, new ParameterCollection());

            Assert.AreEqual(_contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(_contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
