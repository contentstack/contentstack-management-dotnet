using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contentstack.Management.Core.Runtime.Pipeline;
using System.Net.Http;
using Contentstack.Management.Core.Internal;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Moq;
using System.Threading.Tasks;
using Moq.Protected;
using System.Threading;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Unit.Tests.Runtime.Pipeline
{
    [TestClass]
    public class HttpHandlerTest
    {
        private Management.Core.Runtime.Contexts.ExecutionContext context = null;

        [TestInitialize]
        public void Initialize()
        {
            context = new Management.Core.Runtime.Contexts.ExecutionContext(
                new RequestContext()
                {
                    config = new ContentstackClientOptions(),
                    service = new MockService()
                }
                , new ResponseContext());
        }

        [TestMethod]
        public void Initialize_HttpHandler()
        {
            HttpHandler httpHandler = new HttpHandler(new HttpClient());

            Assert.IsNotNull(httpHandler);
            Assert.IsNull(httpHandler.LogManager);
        }

        [TestMethod]
        public void Should_Set_LogManger_HttpHandler()
        {
            HttpHandler httpHandler = new HttpHandler(new HttpClient());
            httpHandler.LogManager = LogManager.EmptyLogger;

            Assert.IsNotNull(httpHandler);
            Assert.IsNotNull(httpHandler.LogManager);
        }

        [TestMethod]
        public void Should_Through_On_InvokeSync_Failuer()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("422Response.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            HttpHandler httpHandler = new HttpHandler(httpClient);
            try
            {
                httpHandler.InvokeSync(context);
                Assert.Fail("Should Fail on 422 Response");
            }
            catch (Exception e)
            {
                ContentstackErrorException errorException = e as ContentstackErrorException;
                Assert.AreEqual(response.StatusCode, errorException.StatusCode);
                Assert.AreEqual("Looks like your email or password is invalid. You have 4 login attempt(s) left.", errorException.Message);
                Assert.AreEqual("Looks like your email or password is invalid. You have 4 login attempt(s) left.", errorException.ErrorMessage);
                Assert.AreEqual(104, errorException.ErrorCode);
            }

            handlerMock.Protected().Verify(
                       "SendAsync",
               Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                       ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task Should_Through_On_InvokAsync_Failuer()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("422Response.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            HttpHandler httpHandler = new HttpHandler(httpClient);
            try
            {
                await httpHandler.InvokeAsync<ResponseContext>(context);
                Assert.Fail("Should Fail on 422 Response");
            }
            catch (Exception e)
            {
                ContentstackErrorException errorException = e as ContentstackErrorException;
                Assert.AreEqual(response.StatusCode, errorException.StatusCode);
                Assert.AreEqual("Looks like your email or password is invalid. You have 4 login attempt(s) left.", errorException.Message);
                Assert.AreEqual("Looks like your email or password is invalid. You have 4 login attempt(s) left.", errorException.ErrorMessage);
                Assert.AreEqual(104, errorException.ErrorCode);
            }

            handlerMock.Protected().Verify(
                       "SendAsync",
               Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                       ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task Return_Response_On_InvokeAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("LoginResponse.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            HttpHandler httpHandler = new HttpHandler(httpClient);
            try
            {
                await httpHandler.InvokeAsync<ContentstackResponse>(context);
                Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
                Assert.AreEqual(await response.Content.ReadAsStringAsync(), context.ResponseContext.httpResponse.OpenResponse());
                Assert.IsNotNull(context.ResponseContext.httpResponse);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            handlerMock.Protected().Verify(
                       "SendAsync",
               Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                       ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task Return_Response_On_InvokeSync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("LoginResponse.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            HttpHandler httpHandler = new HttpHandler(httpClient);
            try
            {
                httpHandler.InvokeSync(context);
                Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
                Assert.AreEqual(await response.Content.ReadAsStringAsync(), context.ResponseContext.httpResponse.OpenResponse());
                Assert.IsNotNull(context.ResponseContext.httpResponse);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }


            handlerMock.Protected().Verify(
                       "SendAsync",
               Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                       ItExpr.IsAny<CancellationToken>());
        }


        [TestMethod]
        public async Task Return_Response_On_Post_InvokeAsync()
        {
            context.RequestContext.service.HttpMethod = "POST";
            context.RequestContext.service.Headers["Content-Type"] = "application/json";
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("LoginResponse.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            HttpHandler httpHandler = new HttpHandler(httpClient);
            try
            {
                await httpHandler.InvokeAsync<ContentstackResponse>(context);
                Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
                Assert.AreEqual(await response.Content.ReadAsStringAsync(), context.ResponseContext.httpResponse.OpenResponse());
                Assert.IsNotNull(context.ResponseContext.httpResponse);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            handlerMock.Protected().Verify(
                       "SendAsync",
               Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                       ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task Return_Response_On_Post_InvokeSync()
        {
            context.RequestContext.service.HttpMethod = "POST";
            context.RequestContext.service.Headers["Content-Type"] = "application/json";
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("LoginResponse.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            HttpHandler httpHandler = new HttpHandler(httpClient);
            try
            {
                httpHandler.InvokeSync(context);
                Assert.AreEqual(response.StatusCode, context.ResponseContext.httpResponse.StatusCode);
                Assert.AreEqual(await response.Content.ReadAsStringAsync(), context.ResponseContext.httpResponse.OpenResponse());
                Assert.IsNotNull(context.ResponseContext.httpResponse);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }


            handlerMock.Protected().Verify(
                       "SendAsync",
               Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                       ItExpr.IsAny<CancellationToken>());
        }
    }
}
