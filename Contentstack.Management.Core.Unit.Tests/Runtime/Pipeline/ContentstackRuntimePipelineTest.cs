using System;
using System.Collections.Generic;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Runtime.Pipeline;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline
{
    [TestClass]
    public class ContentstackRuntimePipelineTest
    {
        private readonly ContentstackResponse response = MockResponse.CreateContentstackResponse("LoginResponse.txt");
        private ExecutionContext context = null;

        [TestInitialize]
        public void Initialize()
        {
            context = new ExecutionContext(new RequestContext(), new ResponseContext());
        }

        [TestMethod]
        public void Should_Throw_ArgumentNullException_On_Null_PipelineHandle()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ContentstackRuntimePipeline(handler: null, null));
        }

        [TestMethod]
        public void Should_Throw_ArgumentNullException_On_Null_PipelineHandlers()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ContentstackRuntimePipeline(handlers: null, null));
        }

        [TestMethod]
        public void Should_Throw_ArgumentNullException_On_Null_Logger()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ContentstackRuntimePipeline(new MockHttpHandler(response), null));
        }

        [TestMethod]
        public void Initialize_ContentstackRuntimePipeline()
        {
            var pipeline = new ContentstackRuntimePipeline(new MockHttpHandler(response), LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));

            Assert.IsNotNull(pipeline.Handler);
        }

        [TestMethod]
        public void Should_Throw_ObjectDisposedException_On_Dispose()
        {
            var pipeline = new ContentstackRuntimePipeline(new MockHttpHandler(response), LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));
            pipeline.Dispose();

            _ = Assert.ThrowsException<ObjectDisposedException>(() => pipeline.InvokeSync(context));
            Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => pipeline.InvokeAsync<ResponseContext>(context));
            pipeline.Dispose();
        }

        [TestMethod]
        public void Should_Dispose_If_Handler_Disposable_On_Dispose()
        {
            var handler = new MockHttpHandlerDisposable(response);
            var pipeline = new ContentstackRuntimePipeline(handler, LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));
            pipeline.Dispose();

            Assert.IsTrue(handler.disposed);
        }

        [TestMethod]
        public void Should_Dispose_If_Handlers_Disposable_On_Dispose()
        {
            var handler = new MockHttpHandlerDisposable(response);
            var handler2 = new MockHttpHandlerDisposable(response);
            var pipeline = new ContentstackRuntimePipeline(new List<IPipelineHandler>()
            {
                handler,
                handler2
            }, LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));
            pipeline.Dispose();
            Assert.IsTrue(handler2.disposed);
            Assert.IsTrue(handler.disposed);
        }

        [TestMethod]
        public void Should_Throw_ArgumentNullException_Replace_Null_Handler()
        {
            var handler = new MockHttpHandlerDisposable(response);
            var pipeline = new ContentstackRuntimePipeline(new MockHttpHandler(response), LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));

            Assert.ThrowsException<ArgumentNullException>(() => pipeline.ReplaceHandler(null));
        }

        [TestMethod]
        public void Should_Allow_To_Add_Logger()
        {
            var pipeline = new ContentstackRuntimePipeline(new MockHttpHandler(response), LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));

            pipeline.AddLogger(new CustomLogger(typeof(ContentstackRuntimePipelineTest)));
        }

        [TestMethod]
        public void Should_Replace_New_Handler()
        {
            var handler = new MockHttpHandlerDisposable(response);
            var pipeline = new ContentstackRuntimePipeline(new MockHttpHandler(response), LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));

            pipeline.ReplaceHandler(handler);

            Assert.AreEqual(handler, pipeline.Handler);
        }

        [TestMethod]
        public void Return_Response_On_InvokeSync()
        {
            var pipeline = new ContentstackRuntimePipeline(new MockHttpHandler(response), LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));
            pipeline.InvokeSync(context);

            Assert.AreEqual(response, context.ResponseContext.httpResponse);
            Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
            Assert.AreEqual(response.ContentType, context.ResponseContext.httpResponse.ContentType);
            Assert.AreEqual(response.ContentLength, context.ResponseContext.httpResponse.ContentLength);
            Assert.AreEqual(response.OpenResponse(), context.ResponseContext.httpResponse.OpenResponse());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Return_Response_On_InvokeAsync()
        {
            var pipeline = new ContentstackRuntimePipeline(new MockHttpHandler(response), LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));
            await pipeline.InvokeAsync<ContentstackResponse>(context);

            Assert.AreEqual(response, context.ResponseContext.httpResponse);
            Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
            Assert.AreEqual(response.ContentType, context.ResponseContext.httpResponse.ContentType);
            Assert.AreEqual(response.ContentLength, context.ResponseContext.httpResponse.ContentLength);
            Assert.AreEqual(response.OpenResponse(), context.ResponseContext.httpResponse.OpenResponse());
        }

        [TestMethod]
        public void Return_Response_For_Multiple_On_InvokeSync()
        {
            var pipeline = new ContentstackRuntimePipeline(new List<IPipelineHandler>() { new MockHttpHandler(response), new MockRetryHadler() }, LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));
            pipeline.InvokeSync(context);

            Assert.AreEqual(response, context.ResponseContext.httpResponse);
            Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
            Assert.AreEqual(response.ContentType, context.ResponseContext.httpResponse.ContentType);
            Assert.AreEqual(response.ContentLength, context.ResponseContext.httpResponse.ContentLength);
            Assert.AreEqual(response.OpenResponse(), context.ResponseContext.httpResponse.OpenResponse());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Return_Response_For_Multiple_On_InvokeAsync()
        {
            var pipeline = new ContentstackRuntimePipeline(new List<IPipelineHandler>() { new MockHttpHandler(response), new MockRetryHadler() }, LogManager.GetLogManager(typeof(ContentstackRuntimePipelineTest)));
            await pipeline.InvokeAsync<ContentstackResponse>(context);

            Assert.AreEqual(response, context.ResponseContext.httpResponse);
            Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
            Assert.AreEqual(response.ContentType, context.ResponseContext.httpResponse.ContentType);
            Assert.AreEqual(response.ContentLength, context.ResponseContext.httpResponse.ContentLength);
            Assert.AreEqual(response.OpenResponse(), context.ResponseContext.httpResponse.OpenResponse());
        }
    }
}
