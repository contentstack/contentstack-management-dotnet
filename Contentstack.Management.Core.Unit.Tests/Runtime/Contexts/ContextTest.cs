using System;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Contexts
{
    [TestClass]
    public class ContextTest
    {
        private readonly JsonSerializer jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());

        [TestMethod]
        public void Initialize_ExecutionContext()
        {
            ExecutionContext executionContext = new ExecutionContext(new RequestContext(), new ResponseContext());

            Assert.IsNotNull(executionContext.RequestContext);
            Assert.IsNull(executionContext.RequestContext.config);
            Assert.IsNull(executionContext.RequestContext.service);
            Assert.IsNotNull(executionContext.ResponseContext);
            Assert.IsNull(executionContext.ResponseContext.httpResponse);
        }

        [TestMethod]
        public void Initialize_Request_Context()
        {
            RequestContext requestContext = new RequestContext()
            {
                config = new ContentstackClientOptions(),
                service = new ContentstackService(jsonSerializer)
            };

            Assert.IsNotNull(requestContext.config);
            Assert.IsNotNull(requestContext.service);
        }

        [TestMethod]
        public void Initialize_Response_Context()
        {
            ResponseContext responseContext = new ResponseContext()
            {
                httpResponse = new ContentstackResponse(MockResponse.CreateFromResource("LoginResponse.txt"), jsonSerializer)
            };

            Assert.IsNotNull(responseContext.httpResponse);
        }
    }
}