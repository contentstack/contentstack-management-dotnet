using Moq;
using System;
using System.Net;
using System.Text;
using AutoFixture;
using System.Net.Http;
using AutoFixture.AutoMoq;
using System.Collections.Generic;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq.Protected;
using System.Threading;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using System.IO;
using Contentstack.Management.Core.Exceptions;

namespace Contentstack.Management.Core.Unit.Tests.Http
{
    [TestClass]
    public class ContentstackHttpRequestTest
    {
        private readonly IFixture _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        private ContentstackHttpRequest _httpRequest = null;

        [TestInitialize]
        public void Initialize()
        {
            _httpRequest = new ContentstackHttpRequest(new System.Net.Http.HttpClient(), Utils.Utilities.GetJsonSerializer());
        }


        [TestMethod]
        public void Returns_Object_On_Initilize_Contentstack_Http_Request()
        {
            Assert.IsNotNull(_httpRequest);
            Assert.IsNotNull(_httpRequest.HttpClient);
            Assert.IsNotNull(_httpRequest.Request);
            Assert.IsNotNull(_httpRequest.Method);
            Assert.AreEqual(HttpMethod.Get, _httpRequest.Method);
        }

        [TestMethod]
        public void Should_Allow_To_Set_Http_Method()
        {
            _httpRequest.Method = HttpMethod.Post;

            Assert.AreEqual(HttpMethod.Post, _httpRequest.Method);
        }

        [TestMethod]
        public void Should_Allow_To_Set_Request_Uri()
        {
            _httpRequest.RequestUri = new Uri("https://localhost");

            Assert.AreEqual("localhost", _httpRequest.RequestUri.Host);
        }

        [TestMethod]
        public void Should_Throw_Object_Disposed_Exception_On_Object_Dispose()
        {
            _httpRequest.Dispose();

            var testContent = _fixture.Create<string>();
            byte[] bytes = Encoding.ASCII.GetBytes(testContent);

            Assert.ThrowsException<ObjectDisposedException>(() => _httpRequest.GetResponse());
            Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => _httpRequest.GetResponseAsync());
            Assert.ThrowsException<ObjectDisposedException>(() => _httpRequest.GetRequestContent());
            Assert.ThrowsException<ObjectDisposedException>(() => _httpRequest.WriteToRequestBody(bytes, new Dictionary<string, string>()));

            _httpRequest.Dispose();
        }

        [TestMethod]
        public void Should_Allow_To_Set_Headers()
        {
            var acceptHeader = _fixture.Create<string>();
            var userAgentHeader = _fixture.Create<string>();
            var date = DateTime.Now.ToUniversalTime();
            var rangeHeader = _fixture.Create<string>();

            _httpRequest.SetRequestHeaders(new Dictionary<string, string>
            {
                {"Accept", acceptHeader},
                {"User-Agent", userAgentHeader},
                {"Date", date.ToString("r")},
                {"If-Modified-Since", date.ToString("r")},
                {"Expires", date.ToString("r")},
            });

            Assert.AreEqual(acceptHeader, _httpRequest.Request.Headers.Accept.ToString());
            Assert.AreEqual(userAgentHeader, _httpRequest.Request.Headers.UserAgent.ToString());
            Assert.AreEqual(DateTime.Parse(date.ToString("r")), _httpRequest.Request.Headers.Date);
            Assert.AreEqual(DateTime.Parse(date.ToString("r")), _httpRequest.Request.Headers.IfModifiedSince);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Should_Allow_Write_Content()
        {
            var testContent = _fixture.Create<string>();
            byte[] bytes = Encoding.ASCII.GetBytes(testContent);

            _httpRequest.WriteToRequestBody(bytes, new Dictionary<string, string>() { { HeadersKey.ContentTypeHeader, "application/json" } });

            Assert.AreEqual(testContent, await _httpRequest.GetRequestContent().ReadAsStringAsync());
        }

        [TestMethod]
        public void Should_Throw_Exception_On_Null_Uri()
        {
            //Assert.ThrowsException<InvalidOperationException>(() => _httpRequest.GetResponse());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _httpRequest.GetResponseAsync());
        }

        [TestMethod]
        public async Task Return_OK_Response_On_Request_SuccessAsync()
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
            var HttpRequest = new ContentstackHttpRequest(httpClient, Utils.Utilities.GetJsonSerializer());
            HttpRequest.RequestUri = new Uri("https://localhost.com");
            var httpResponse = HttpRequest.GetResponse();

            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual(await response.Content.ReadAsStringAsync(), httpResponse.OpenResponse());
            Assert.IsNotNull(httpResponse);
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public void Return_UnprocessableEntity_Response_On_Request_FailuerAsync()
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
            var HttpRequest = new ContentstackHttpRequest(httpClient, Utils.Utilities.GetJsonSerializer());
            HttpRequest.RequestUri = new Uri("https://localhost.com");
            try
            {
                var httpResponse = HttpRequest.GetResponse();
            } catch (Exception e)
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
        public void Return_UnprocessableEntity_Response_On_Request_Fail_400()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("400Response.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object);
            var HttpRequest = new ContentstackHttpRequest(httpClient, Utils.Utilities.GetJsonSerializer());
            HttpRequest.RequestUri = new Uri("https://localhost.com");
            try
            {
                var httpResponse = HttpRequest.GetResponse();
            }
            catch (Exception e)
            {
                ContentstackErrorException errorException = e as ContentstackErrorException;
                Assert.AreEqual(response.StatusCode, errorException.StatusCode);
                Assert.AreEqual("Please set a valid 'Content-Type' header", errorException.Message);
                Assert.AreEqual("Please set a valid 'Content-Type' header", errorException.ErrorMessage);
            }

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public void Return_UnprocessableEntity_Response_On_Request_Fail_304()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = MockResponse.CreateFromResource("304Response.txt");
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object);
            var HttpRequest = new ContentstackHttpRequest(httpClient, Utils.Utilities.GetJsonSerializer());
            HttpRequest.RequestUri = new Uri("https://localhost.com");
            try
            {
                var httpResponse = HttpRequest.GetResponse();
            }
            catch (Exception e)
            {
                ContentstackErrorException errorException = e as ContentstackErrorException;
                Assert.AreEqual(response.StatusCode, errorException.StatusCode);
            }

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public void Return_Exception_Response_On_Request_Exception()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = "Something went Wrong";
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .Throws(new HttpRequestException(response));

            var httpClient = new HttpClient(handlerMock.Object);
            var HttpRequest = new ContentstackHttpRequest(httpClient, Utils.Utilities.GetJsonSerializer());
            HttpRequest.RequestUri = new Uri("https://localhost.com");
            try
            {
                var httpResponse = HttpRequest.GetResponse();
            }
            catch (Exception e)
            {
                HttpRequestException errorException = e as HttpRequestException;
                Assert.AreEqual(response, errorException.Message);
            }

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
        }


        [TestMethod]
        public void Return_Inner_Exception_Response_On_Request_Exception()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = "Something went Wrong";
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .Throws(new HttpRequestException("Outer Exception" ,new IOException(response)));

            var httpClient = new HttpClient(handlerMock.Object);
            var HttpRequest = new ContentstackHttpRequest(httpClient, Utils.Utilities.GetJsonSerializer());
            HttpRequest.RequestUri = new Uri("https://localhost.com");
            try
            {
                var httpResponse = HttpRequest.GetResponse();
            }
            catch (Exception e)
            {
                IOException errorException = e as IOException;
                Assert.AreEqual(response, errorException.Message);
            }

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
        }
    }
}
