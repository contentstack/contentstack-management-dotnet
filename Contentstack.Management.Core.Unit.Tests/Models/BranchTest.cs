using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class BranchTest
    {
        private Stack _stack;
        private MockHttpHandler _mockHandler;
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            var client = new ContentstackClient();
            var contentstackResponse = MockResponse.CreateContentstackResponse("MockResponse.txt");
            _mockHandler = new MockHttpHandler(contentstackResponse);
            client.ContentstackPipeline.ReplaceHandler(_mockHandler);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            _stack = new Stack(client, _fixture.Create<string>());
        }

        [TestMethod]
        public void Initialize_Branch()
        {
            Branch branch = new Branch(_stack);

            Assert.IsNull(branch.Uid);
            Assert.AreEqual("/stacks/branches", branch.resourcePath);
            Assert.ThrowsException<ArgumentException>(() => branch.Fetch());
            Assert.ThrowsExceptionAsync<ArgumentException>(() => branch.FetchAsync());
            Assert.ThrowsException<ArgumentException>(() => branch.Delete());
            Assert.ThrowsExceptionAsync<ArgumentException>(() => branch.DeleteAsync());
            Assert.AreEqual(branch.Query().GetType(), typeof(Query));
        }

        [TestMethod]
        public void Initialize_Branch_With_Uid()
        {
            string uid = _fixture.Create<string>();
            Branch branch = new Branch(_stack, uid);

            Assert.AreEqual(uid, branch.Uid);
            Assert.AreEqual($"/stacks/branches/{branch.Uid}", branch.resourcePath);
            Assert.ThrowsException<InvalidOperationException>(() => branch.Create(_fixture.Create<BranchModel>()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => branch.CreateAsync(_fixture.Create<BranchModel>()));
            Assert.ThrowsException<InvalidOperationException>(() => branch.Query());
        }

        [TestMethod]
        public void Should_Query_Branches_With_Params()
        {
            _stack.Branch().Query().Limit(2).Skip(2).IncludeCount().Find();

            Assert.IsNotNull(_mockHandler.LastRequestUri);
            StringAssert.Contains(_mockHandler.LastRequestUri.AbsoluteUri, "/v3/stacks/branches");
            StringAssert.Contains(_mockHandler.LastRequestUri.Query, "include_count=true");
            StringAssert.Contains(_mockHandler.LastRequestUri.Query, "limit=2");
            StringAssert.Contains(_mockHandler.LastRequestUri.Query, "skip=2");
        }

        [TestMethod]
        public void Create_Sends_Environment_Names_As_Given()
        {
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.Created, BranchJson));
            var model = NewBranchWithEnvironments("development", "staging");

            StackWith(handler).Branch().Create(model);

            AssertEnvironmentsSent(handler.LastRequestBody, "development", "staging");
        }

        [TestMethod]
        public async Task CreateAsync_Sends_Environment_Names_As_Given()
        {
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.Created, BranchJson));
            var model = NewBranchWithEnvironments("development", "staging");

            await StackWith(handler).Branch().CreateAsync(model);

            AssertEnvironmentsSent(handler.LastRequestBody, "development", "staging");
        }

        [TestMethod]
        public void Create_Sends_Empty_Environment_List_As_Empty_Array()
        {
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.Created, BranchJson));
            var model = NewBranchWithEnvironments();

            StackWith(handler).Branch().Create(model);

            AssertEnvironmentsSent(handler.LastRequestBody);
        }

        [TestMethod]
        public async Task CreateAsync_Sends_Empty_Environment_List_As_Empty_Array()
        {
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.Created, BranchJson));
            var model = NewBranchWithEnvironments();

            await StackWith(handler).Branch().CreateAsync(model);

            AssertEnvironmentsSent(handler.LastRequestBody);
        }

        [TestMethod]
        public void Create_Without_PublishConfig_Omits_The_Key()
        {
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.Created, BranchJson));
            var model = new BranchModel { Uid = "dev", Source = "main" };

            StackWith(handler).Branch().Create(model);

            AssertNoPublishConfigSent(handler.LastRequestBody);
        }

        [TestMethod]
        public async Task CreateAsync_Without_PublishConfig_Omits_The_Key()
        {
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.Created, BranchJson));
            var model = new BranchModel { Uid = "dev", Source = "main" };

            await StackWith(handler).Branch().CreateAsync(model);

            AssertNoPublishConfigSent(handler.LastRequestBody);
        }

        [TestMethod]
        public void Create_Rejected_With_422_Throws_With_Api_Error_Unchanged()
        {
            var handler = new BranchRequestHandler(ErrorResponse());
            var model = NewBranchWithEnvironments("development", "staging_typo");

            var ex = Assert.ThrowsException<ContentstackErrorException>(() => StackWith(handler).Branch().Create(model));

            AssertRejectedWithApiError(ex, handler.LastRequestBody);
        }

        [TestMethod]
        public async Task CreateAsync_Rejected_With_422_Throws_With_Api_Error_Unchanged()
        {
            var handler = new BranchRequestHandler(ErrorResponse());
            var model = NewBranchWithEnvironments("development", "staging_typo");

            var ex = await Assert.ThrowsExceptionAsync<ContentstackErrorException>(() => StackWith(handler).Branch().CreateAsync(model));

            AssertRejectedWithApiError(ex, handler.LastRequestBody);
        }

        [TestMethod]
        public void Fetch_Deserializes_Environment_List_Into_Model()
        {
            const string json = "{\"branch\":{\"uid\":\"dev\",\"source\":\"main\",\"publish_config\":{\"environments\":[\"development\",\"staging\"]}}}";
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.OK, json));

            ContentstackResponse response = StackWith(handler).Branch("dev").Fetch();
            BranchResponse fetched = response.OpenTResponse<BranchResponse>();

            Assert.IsNotNull(fetched.Branch?.PublishConfig);
            CollectionAssert.AreEqual(new List<string> { "development", "staging" }, fetched.Branch.PublishConfig.Environments);
        }

        [TestMethod]
        public void Fetch_Without_PublishConfig_Leaves_It_Null()
        {
            var handler = new BranchRequestHandler(JsonResponse(HttpStatusCode.OK, BranchJson));

            ContentstackResponse response = StackWith(handler).Branch("dev").Fetch();
            BranchResponse fetched = response.OpenTResponse<BranchResponse>();

            Assert.IsNotNull(fetched.Branch);
            Assert.IsNull(fetched.Branch.PublishConfig);
        }

        // ---- helpers -----------------------------------------------------------

        private const string BranchJson = "{\"branch\":{\"uid\":\"dev\",\"source\":\"main\"}}";

        private Stack StackWith(IPipelineHandler handler)
        {
            var client = new ContentstackClient();
            client.ContentstackPipeline.ReplaceHandler(handler);
            client.contentstackOptions.Authtoken = _fixture.Create<string>();
            return new Stack(client, _fixture.Create<string>());
        }

        private static BranchModel NewBranchWithEnvironments(params string[] environments)
        {
            return new BranchModel
            {
                Uid = "dev",
                Source = "main",
                PublishConfig = new BranchPublishConfigModel { Environments = new List<string>(environments) }
            };
        }

        private static ContentstackResponse JsonResponse(HttpStatusCode statusCode, string json)
        {
            return new ContentstackResponse(MockResponse.Create(statusCode, null, json), new JsonSerializerOptions());
        }

        private static HttpResponseMessage ErrorResponse()
        {
            const string json = "{\"error_message\":\"Environment not found.\",\"error_code\":422," +
                "\"errors\":{\"publish_config.environments\":[\"staging_typo\"]}}";
            return MockResponse.Create((HttpStatusCode)422, null, json);
        }

        private static JsonObject SentBranch(string body)
        {
            Assert.IsNotNull(body, "no request body was captured");
            return JsonNode.Parse(body)!["branch"]!.AsObject();
        }

        private static void AssertEnvironmentsSent(string body, params string[] expected)
        {
            JsonNode publishConfig = SentBranch(body)["publish_config"];
            Assert.IsNotNull(publishConfig, "publish_config was not sent");
            JsonNode environments = publishConfig["environments"];
            Assert.IsInstanceOfType(environments, typeof(JsonArray), "environments must be sent as a JSON array");
            CollectionAssert.AreEqual(expected, environments.AsArray().Select(n => n!.GetValue<string>()).ToList());
        }

        private static void AssertNoPublishConfigSent(string body)
        {
            Assert.IsFalse(SentBranch(body).ContainsKey("publish_config"), "publish_config must be omitted when unset");
        }

        private static void AssertRejectedWithApiError(ContentstackErrorException ex, string body)
        {
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, ex.StatusCode);
            Assert.AreEqual("Environment not found.", ex.Message);
            Assert.AreEqual("Environment not found.", ex.ErrorMessage);
            Assert.AreEqual(422, ex.ErrorCode);
            Assert.IsNotNull(ex.Errors);
            Assert.AreEqual("[\"staging_typo\"]", JsonSerializer.Serialize(ex.Errors["publish_config.environments"]));
            AssertEnvironmentsSent(body, "development", "staging_typo");
        }

        private sealed class BranchResponse
        {
            [JsonPropertyName("branch")]
            public BranchModel Branch { get; set; }
        }

        /// <summary>
        /// Stands in for the HTTP handler. It builds the request exactly as production does, records the
        /// JSON body that would be sent, then either returns the canned response or throws the canned error
        /// through the SDK's own error factory.
        /// </summary>
        private sealed class BranchRequestHandler : IPipelineHandler
        {
            private readonly ContentstackResponse _response;
            private readonly HttpResponseMessage _error;

            public BranchRequestHandler(ContentstackResponse response)
            {
                _response = response;
            }

            public BranchRequestHandler(HttpResponseMessage error)
            {
                _error = error;
            }

            public string LastRequestBody { get; private set; }

            public ILogManager LogManager { get; set; }
            public IPipelineHandler InnerHandler { get; set; }

            public void InvokeSync(IExecutionContext executionContext, bool addAcceptMediaHeader = false, string apiVersion = null)
            {
                Capture(executionContext);
                if (_error != null)
                {
                    throw ContentstackErrorException.CreateException(_error);
                }
                executionContext.ResponseContext.httpResponse = _response;
            }

            public Task<T> InvokeAsync<T>(IExecutionContext executionContext, bool addAcceptMediaHeader = false, string apiVersion = null)
            {
                Capture(executionContext);
                if (_error != null)
                {
                    throw ContentstackErrorException.CreateException(_error);
                }
                executionContext.ResponseContext.httpResponse = _response;
                return Task.FromResult((T)executionContext.ResponseContext.httpResponse);
            }

            private void Capture(IExecutionContext executionContext)
            {
                IContentstackService service = executionContext.RequestContext.service;
                service.CreateHttpRequest(null, executionContext.RequestContext.config);
                LastRequestBody = service.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            }
        }
    }
}
