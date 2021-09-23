using System;
using System.Collections.Generic;
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
            Assert.ThrowsException<InvalidOperationException>(() => stack.UpdateUserRole(new List<UserInvitation>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.UpdateUserRoleAsync(new List<UserInvitation>()));
            Assert.ThrowsException<InvalidOperationException>(() => stack.Settings());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.SettingsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.ResetSettings());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.ResetSettingsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.AddSettings(new StackSettings()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.AddSettingsAsync(new StackSettings()));
            Assert.ThrowsException<InvalidOperationException>(() => stack.Share(new List<UserInvitation>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.ShareAsync(new List<UserInvitation>()));
            Assert.ThrowsException<InvalidOperationException>(() => stack.UnShare(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.UnShareAsync(_fixture.Create<string>()));
        }

        [TestMethod]
        public void Initialize_Stack_With_Authtoken()
        {
            Stack stack = new Stack(client);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            var name = _fixture.Create<string>();

            Assert.IsNull(stack.APIKey);
            Assert.IsNotNull(stack);
            Assert.ThrowsException<InvalidOperationException>(() => stack.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.TransferOwnership(null));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.TransferOwnershipAsync(null));
            Assert.ThrowsException<InvalidOperationException>(() => stack.Update(name));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.UpdateAsync(name));
            Assert.ThrowsException<InvalidOperationException>(() => stack.UpdateUserRole(new List<UserInvitation>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.UpdateUserRoleAsync(new List<UserInvitation>()));
            Assert.ThrowsException<InvalidOperationException>(() => stack.Settings());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.SettingsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.ResetSettings());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.ResetSettingsAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.AddSettings(new StackSettings()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.AddSettingsAsync(new StackSettings()));
            Assert.ThrowsException<InvalidOperationException>(() => stack.Share(new List<UserInvitation>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.ShareAsync(new List<UserInvitation>()));
            Assert.ThrowsException<InvalidOperationException>(() => stack.UnShare(_fixture.Create<string>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.UnShareAsync(_fixture.Create<string>()));
        }

        [TestMethod]
        public void Initialize_Stack_With_API_Key()
        {
            var apiKey = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var locale = _fixture.Create<string>();
            var orgID = _fixture.Create<string>();
            Stack stack = new Stack(client, apiKey);

            Assert.AreEqual(apiKey, stack.APIKey);
            Assert.IsNotNull(stack);
            Assert.ThrowsException<InvalidOperationException>(() => stack.GetAll());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.GetAllAsync());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Create(name, locale, orgID));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => stack.CreateAsync(name, locale, orgID));
        }

        [TestMethod]
        public void Initialize_Stack_With_API_Key_And_Authtoken()
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
        public void Should_Throw_Update_User_Role_With_Null_List()
        {
            var apiKey = _fixture.Create<string>();
            Stack stack = new Stack(client, apiKey);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.UpdateUserRole(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.UpdateUserRoleAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Add_Settings_With_Null_Settings_Object()
        {
            var apiKey = _fixture.Create<string>();
            Stack stack = new Stack(client, apiKey);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.AddSettings(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.AddSettingsAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Share_With_Null_List()
        {
            var apiKey = _fixture.Create<string>();
            Stack stack = new Stack(client, apiKey);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.Share(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.ShareAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Un_Share_With_Null_Email()
        {
            var apiKey = _fixture.Create<string>();
            Stack stack = new Stack(client, apiKey);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Assert.IsNotNull(stack);
            Assert.ThrowsException<ArgumentNullException>(() => stack.UnShare(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.UnShareAsync(null));
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

        [TestMethod]
        public void Should_Transfer_Stack_Update_Users_Role()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.UpdateUserRole(_fixture.Create<List<UserInvitation>>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Transfer_Stack_Update_Users_Role_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.UpdateUserRoleAsync(_fixture.Create<List<UserInvitation>>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }


        [TestMethod]
        public void Should_Transfer_Stack_Settings()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.Settings();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Transfer_Stack_Settings_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.SettingsAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Transfer_Stack_Reset_Settings()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.ResetSettings();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Transfer_Stack_Reset_Settings_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.ResetSettingsAsync();

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Transfer_Stack_Add_Settings()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.AddSettings(_fixture.Create<StackSettings>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Transfer_Stack_Add_Settings_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.AddSettingsAsync(_fixture.Create<StackSettings>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Invite_User_to_Stack()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.Share(_fixture.Create<List<UserInvitation>>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Invite_User_to_Stack_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.ShareAsync(_fixture.Create<List<UserInvitation>>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Remove_User_From_Stack()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.UnShare(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Remove_User_From_Stack_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.UnShareAsync(_fixture.Create<string>());

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }
    }
}
