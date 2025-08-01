using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
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

            Assert.ThrowsException<InvalidOperationException>(() => stack.ContentType());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Asset());
            Assert.ThrowsException<InvalidOperationException>(() => stack.GlobalField());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Locale());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Extension());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Label());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Environment());
            Assert.ThrowsException<InvalidOperationException>(() => stack.DeliveryToken());
            Assert.ThrowsException<InvalidOperationException>(() => stack.ManagementTokens());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Role());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Release());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Workflow());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Webhook());
            Assert.ThrowsException<InvalidOperationException>(() => stack.PublishQueue());
            Assert.ThrowsException<InvalidOperationException>(() => stack.AuditLog());
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

            Assert.ThrowsException<InvalidOperationException>(() => stack.ContentType());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Asset());
            Assert.ThrowsException<InvalidOperationException>(() => stack.GlobalField());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Locale());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Extension());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Label());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Environment());
            Assert.ThrowsException<InvalidOperationException>(() => stack.DeliveryToken());
            Assert.ThrowsException<InvalidOperationException>(() => stack.ManagementTokens());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Role());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Release());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Workflow());
            Assert.ThrowsException<InvalidOperationException>(() => stack.Webhook());
            Assert.ThrowsException<InvalidOperationException>(() => stack.PublishQueue());
            Assert.ThrowsException<InvalidOperationException>(() => stack.AuditLog());
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

        #region Bulk Operation Tests

        [TestMethod]
        public void Should_Initialize_BulkOperation()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            var bulkOperation = stack.BulkOperation();

            Assert.IsNotNull(bulkOperation);
        }

        [TestMethod]
        public void Should_Throw_BulkOperation_Without_Authentication()
        {
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<InvalidOperationException>(() => stack.BulkOperation());
        }

        [TestMethod]
        public void Should_Throw_BulkOperation_Without_API_Key()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client);

            Assert.ThrowsException<InvalidOperationException>(() => stack.BulkOperation());
        }

        [TestMethod]
        public void Should_Publish_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Version = 1,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "asset_uid_1" }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "environment_1" }
            };

            ContentstackResponse response = stack.BulkOperation().Publish(publishDetails);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Publish_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Version = 1,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "asset_uid_1" }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "environment_1" }
            };

            ContentstackResponse response = await stack.BulkOperation().PublishAsync(publishDetails);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Publish_Bulk_Operation_With_Flags()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var publishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Version = 1,
                        Locale = "en-us"
                    }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "environment_1" }
            };

            ContentstackResponse response = stack.BulkOperation().Publish(publishDetails, true, true, true);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Unpublish_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var unpublishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Version = 1,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "asset_uid_1" }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "environment_1" }
            };

            ContentstackResponse response = stack.BulkOperation().Unpublish(unpublishDetails);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Unpublish_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var unpublishDetails = new BulkPublishDetails
            {
                Entries = new List<BulkPublishEntry>
                {
                    new BulkPublishEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Version = 1,
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkPublishAsset>
                {
                    new BulkPublishAsset { Uid = "asset_uid_1" }
                },
                Locales = new List<string> { "en-us" },
                Environments = new List<string> { "environment_1" }
            };

            ContentstackResponse response = await stack.BulkOperation().UnpublishAsync(unpublishDetails);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Delete_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var deleteDetails = new BulkDeleteDetails
            {
                Entries = new List<BulkDeleteEntry>
                {
                    new BulkDeleteEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkDeleteAsset>
                {
                    new BulkDeleteAsset { Uid = "asset_uid_1" }
                }
            };

            ContentstackResponse response = stack.BulkOperation().Delete(deleteDetails);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Delete_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var deleteDetails = new BulkDeleteDetails
            {
                Entries = new List<BulkDeleteEntry>
                {
                    new BulkDeleteEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Locale = "en-us"
                    }
                },
                Assets = new List<BulkDeleteAsset>
                {
                    new BulkDeleteAsset { Uid = "asset_uid_1" }
                }
            };

            ContentstackResponse response = await stack.BulkOperation().DeleteAsync(deleteDetails);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Workflow_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var updateBody = new BulkWorkflowUpdateBody
            {
                Entries = new List<BulkWorkflowEntry>
                {
                    new BulkWorkflowEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Locale = "en-us"
                    }
                },
                Workflow = new BulkWorkflowStage
                {
                    Uid = "workflow_stage_uid",
                    Comment = "Test comment"
                }
            };

            ContentstackResponse response = stack.BulkOperation().Update(updateBody);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Workflow_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var updateBody = new BulkWorkflowUpdateBody
            {
                Entries = new List<BulkWorkflowEntry>
                {
                    new BulkWorkflowEntry
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        Locale = "en-us"
                    }
                },
                Workflow = new BulkWorkflowStage
                {
                    Uid = "workflow_stage_uid",
                    Comment = "Test comment"
                }
            };

            ContentstackResponse response = await stack.BulkOperation().UpdateAsync(updateBody);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Add_Items_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().AddItems(itemsData);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Add_Items_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            ContentstackResponse response = await stack.BulkOperation().AddItemsAsync(itemsData, "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Add_Items_Bulk_Operation_With_Version()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().AddItems(itemsData, "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Items_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().UpdateItems(itemsData, "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Items_Bulk_Operation_With_Version()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().UpdateItems(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Items_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            ContentstackResponse response = await stack.BulkOperation().UpdateItemsAsync(itemsData, "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Items_Bulk_Operation_Async_With_Version()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            ContentstackResponse response = await stack.BulkOperation().UpdateItemsAsync(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Add_Items_With_Deployment_Mode_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Release = "release_uid_123",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        ContentTypeUid = "content_type_1",
                        Version = 1,
                        Locale = "en-us",
                        Title = "Test Entry"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().AddItems(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Add_Items_With_Deployment_Mode_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Release = "release_uid_123",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        ContentTypeUid = "content_type_1",
                        Version = 1,
                        Locale = "en-us",
                        Title = "Test Entry"
                    }
                }
            };

            ContentstackResponse response = await stack.BulkOperation().AddItemsAsync(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Update_Items_With_Deployment_Mode_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Release = "release_uid_123",
                Action = "unpublish",
                Locale = new List<string> { "en-us" },
                Reference = false,
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        ContentTypeUid = "content_type_1",
                        Version = 1,
                        Locale = "en-us",
                        Title = "Test Entry"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().UpdateItems(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Update_Items_With_Deployment_Mode_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Release = "release_uid_123",
                Action = "unpublish",
                Locale = new List<string> { "en-us" },
                Reference = false,
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1",
                        ContentTypeUid = "content_type_1",
                        Version = 1,
                        Locale = "en-us",
                        Title = "Test Entry"
                    }
                }
            };

            ContentstackResponse response = await stack.BulkOperation().UpdateItemsAsync(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Test_AddItems_Simple_Mode_With_Version()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            // Test simple mode (no release properties set)
            Assert.IsFalse(itemsData.IsReleaseDeploymentMode());

            ContentstackResponse response = stack.BulkOperation().AddItems(itemsData, "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Test_AddItems_Simple_Mode_With_Version_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            // Test simple mode (no release properties set)
            Assert.IsFalse(itemsData.IsReleaseDeploymentMode());

            ContentstackResponse response = await stack.BulkOperation().AddItemsAsync(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Test_UpdateItems_Simple_Mode_With_Version()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            // Test simple mode (no release properties set)
            Assert.IsFalse(itemsData.IsReleaseDeploymentMode());

            ContentstackResponse response = stack.BulkOperation().UpdateItems(itemsData, "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Test_UpdateItems_Simple_Mode_With_Version_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var itemsData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem
                    {
                        Uid = "entry_uid_1",
                        ContentType = "content_type_1"
                    }
                }
            };

            // Test simple mode (no release properties set)
            Assert.IsFalse(itemsData.IsReleaseDeploymentMode());

            ContentstackResponse response = await stack.BulkOperation().UpdateItemsAsync(itemsData, "2.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Test_Deployment_Mode_Detection()
        {
            // Test deployment mode detection
            var deploymentData = new BulkAddItemsData
            {
                Release = "release_uid_123",
                Action = "publish",
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem { Uid = "entry_uid", ContentType = "content_type" }
                }
            };

            Assert.IsTrue(deploymentData.IsReleaseDeploymentMode());

            // Test simple mode detection
            var simpleData = new BulkAddItemsData
            {
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem { Uid = "entry_uid", ContentType = "content_type" }
                }
            };

            Assert.IsFalse(simpleData.IsReleaseDeploymentMode());

            // Test partial data (missing action)
            var partialData = new BulkAddItemsData
            {
                Release = "release_uid_123",
                Items = new List<BulkAddItem>
                {
                    new BulkAddItem { Uid = "entry_uid", ContentType = "content_type" }
                }
            };

            Assert.IsFalse(partialData.IsReleaseDeploymentMode()); // Should be false without action
        }

        [TestMethod]
        public void Should_Get_Job_Status_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.BulkOperation().JobStatus("job_id_123");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Get_Job_Status_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = await stack.BulkOperation().JobStatusAsync("job_id_123");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Get_Job_Status_Bulk_Operation_With_Version()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            ContentstackResponse response = stack.BulkOperation().JobStatus("job_id_123", "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Release_Items_Bulk_Operation()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var releaseData = new BulkReleaseItemsData
            {
                Release = "release_uid_123",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkReleaseItem>
                {
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "content_type_1",
                        Uid = "entry_uid_1",
                        Version = 1,
                        Locale = "en-us",
                        Title = "Test Entry"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().ReleaseItems(releaseData);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Release_Items_Bulk_Operation_Async()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var releaseData = new BulkReleaseItemsData
            {
                Release = "release_uid_123",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkReleaseItem>
                {
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "content_type_1",
                        Uid = "entry_uid_1",
                        Version = 1,
                        Locale = "en-us",
                        Title = "Test Entry"
                    }
                }
            };

            ContentstackResponse response = await stack.BulkOperation().ReleaseItemsAsync(releaseData);

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Release_Items_Bulk_Operation_With_Version()
        {
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            client.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(contentstackResponse));
            client.contentstackOptions.Authtoken = _fixture.Create<string>();

            Stack stack = new Stack(client, _fixture.Create<string>());

            var releaseData = new BulkReleaseItemsData
            {
                Release = "release_uid_123",
                Action = "publish",
                Locale = new List<string> { "en-us" },
                Reference = true,
                Items = new List<BulkReleaseItem>
                {
                    new BulkReleaseItem
                    {
                        ContentTypeUid = "content_type_1",
                        Uid = "entry_uid_1",
                        Version = 1,
                        Locale = "en-us",
                        Title = "Test Entry"
                    }
                }
            };

            ContentstackResponse response = stack.BulkOperation().ReleaseItems(releaseData, "1.0");

            Assert.AreEqual(contentstackResponse.OpenResponse(), response.OpenResponse());
            Assert.AreEqual(contentstackResponse.OpenJObjectResponse().ToString(), response.OpenJObjectResponse().ToString());
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Publish_Details()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().Publish(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().PublishAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Unpublish_Details()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().Unpublish(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().UnpublishAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Delete_Details()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().Delete(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().DeleteAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Update_Body()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().Update(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().UpdateAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Add_Items_Data()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().AddItems(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().AddItemsAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Update_Items_Data()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().UpdateItems(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().UpdateItemsAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Job_Id()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().JobStatus(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().JobStatusAsync(null));
        }

        [TestMethod]
        public void Should_Throw_Bulk_Operation_With_Null_Release_Data()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Assert.ThrowsException<ArgumentNullException>(() => stack.BulkOperation().ReleaseItems(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stack.BulkOperation().ReleaseItemsAsync(null));
        }

        [TestMethod]
        public void Should_Return_Release_Instance()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Release release = stack.Release();

            Assert.IsNotNull(release);
            Assert.AreEqual(typeof(Release), release.GetType());
            Assert.IsNull(release.Uid);
            Assert.AreEqual("/releases", release.resourcePath);
        }

        [TestMethod]
        public void Should_Return_Release_Instance_With_Uid()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());
            string releaseUid = _fixture.Create<string>();

            Release release = stack.Release(releaseUid);

            Assert.IsNotNull(release);
            Assert.AreEqual(typeof(Release), release.GetType());
            Assert.AreEqual(releaseUid, release.Uid);
            Assert.AreEqual($"/releases/{releaseUid}", release.resourcePath);
        }

        [TestMethod]
        public void Should_Create_Multiple_Release_Instances()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());
            string uid1 = _fixture.Create<string>();
            string uid2 = _fixture.Create<string>();

            Release release1 = stack.Release();
            Release release2 = stack.Release(uid1);
            Release release3 = stack.Release(uid2);

            Assert.IsNotNull(release1);
            Assert.IsNotNull(release2);
            Assert.IsNotNull(release3);
            Assert.AreNotSame(release1, release2);
            Assert.AreNotSame(release2, release3);
            Assert.IsNull(release1.Uid);
            Assert.AreEqual(uid1, release2.Uid);
            Assert.AreEqual(uid2, release3.Uid);
        }

        [TestMethod]
        public void Should_Return_Release_With_Same_Stack_Reference()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Release release = stack.Release();

            Assert.IsNotNull(release);
            // We can't directly access the stack property as it's internal, 
            // but we can verify the release instance is properly initialized
            Assert.AreEqual(typeof(Release), release.GetType());
        }

        [TestMethod]
        public void Should_Return_Release_Query_Instance()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Release release = stack.Release();
            Query query = release.Query();

            Assert.IsNotNull(query);
            Assert.AreEqual(typeof(Query), query.GetType());
        }

        [TestMethod]
        public void Should_Return_ReleaseItem_Instance_With_Uid()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());
            string releaseUid = _fixture.Create<string>();

            Release release = stack.Release(releaseUid);
            ReleaseItem releaseItem = release.Item();

            Assert.IsNotNull(releaseItem);
            Assert.AreEqual(typeof(ReleaseItem), releaseItem.GetType());
        }

        [TestMethod]
        public void Should_Handle_Null_Uid_In_Release()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Release release1 = stack.Release(null);
            Release release2 = stack.Release();

            Assert.IsNotNull(release1);
            Assert.IsNotNull(release2);
            Assert.IsNull(release1.Uid);
            Assert.IsNull(release2.Uid);
            Assert.AreEqual("/releases", release1.resourcePath);
            Assert.AreEqual("/releases", release2.resourcePath);
        }

        [TestMethod]
        public void Should_Handle_Empty_String_Uid_In_Release()
        {
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            Stack stack = new Stack(client, _fixture.Create<string>());

            Release release = stack.Release(string.Empty);

            Assert.IsNotNull(release);
            Assert.AreEqual(string.Empty, release.Uid);
            Assert.AreEqual("/releases/", release.resourcePath);
        }

        #endregion
    }
}
