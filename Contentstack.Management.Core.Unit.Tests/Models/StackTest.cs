using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class StackTest
    {
        private ContentstackClient client;
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            client = new ContentstackClient();
        }

        [TestMethod]
        public void Initialize_Stack()
        {
            Stack stack = new Stack(client);
            var name = _fixture.Create<string>();

            Assert.IsNull(stack.APIKey);
            Assert.IsNotNull(stack);
            Assert.ThrowsException<InvalidOperationException>(() => stack.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.TransferOwnership(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.TransferOwnershipAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => stack.Update(name));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.UpdateAsync(name));
        }

        [TestMethod]
        public void Initialize_Stack_With_API_Key()
        {
            var apiKey = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var locale= _fixture.Create<string>();
            var orgID = _fixture.Create<string>();
            Stack stack = new Stack(client, apiKey);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.AreEqual(apiKey, stack.APIKey);
            Assert.IsNotNull(stack);
            Assert.ThrowsException<InvalidOperationException>(() => stack.GetAll());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.GetAllAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Create(name, locale, orgID));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.CreateAsync(name, locale, orgID));
        }

        [TestMethod]
        public void Should_Throw_Name_Is_Null()
        {
            var locale = _fixture.Create<string>();
            var orgID = _fixture.Create<string>();
            Stack stack = new Stack(client);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.Create(null, locale, orgID));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.CreateAsync(null, locale, orgID));
        }

        [TestMethod]
        public void Should_Throw_Name_Is_Null_On_Update()
        {
            var apiKey = _fixture.Create<string>();
            var locale = _fixture.Create<string>();
            var orgID = _fixture.Create<string>();
            Stack stack = new Stack(client, apiKey);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.Update(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.UpdateAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Master_Locale_Is_Null()
        {
            var locale = _fixture.Create<string>();
            var orgID = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            Stack stack = new Stack(client);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.Create(name, null, orgID));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.CreateAsync(name, null, orgID));
        }

        [TestMethod]
        public void Should_Throw_Organization_Uid_Is_Null()
        {
            var locale = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            Stack stack = new Stack(client);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.Create(name, locale, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.CreateAsync(name, locale, null));
        }

        [TestMethod]
        public void Should_Return_All_Stack_For_Loggedin_User()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client);

            ContentstackResponse response = stack.GetAll();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_All_Stack_Async_For_Loggedin_User()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client);

            ContentstackResponse response = await stack.GetAllAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }


        [TestMethod]
        public void Should_Return_Stack_Details_for_API_Key()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, "key");

            ContentstackResponse response = stack.Fetch();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Return_Stack_Async_Details_for_API_Key()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, "key");

            ContentstackResponse response = await stack.FetchAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Create_Stack()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client);

            ContentstackResponse response = stack.Create(_fixture.Create<string>(), "en-us", _fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Create_Stack_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client);

            ContentstackResponse response = await stack.CreateAsync(_fixture.Create<string>(), "en-us", _fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Stack()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.Update(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Stack_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.UpdateAsync(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Transfer_Stack_Owner()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.TransferOwnership(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Transfer_Stack_Owner_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.TransferOwnershipAsync(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
