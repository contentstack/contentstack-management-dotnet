using System;
using System.Net;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Contentstack.Management.Core.Unit.Tests.Mokes.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Http
{
    [TestClass]
    public class ContentstackHttpResponseTest
    {
        [TestMethod]
        public void Initialize_Response_On_Success()
        {
            ContentstackResponse contentstackHttpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            Assert.IsNotNull(contentstackHttpResponse.ResponseBody);
            Assert.AreEqual(HttpStatusCode.OK, contentstackHttpResponse.StatusCode);
            Assert.AreEqual("application/json", contentstackHttpResponse.ContentType);
            Assert.AreEqual(true, contentstackHttpResponse.IsSuccessStatusCode);
        }

        [TestMethod]
        public void Initialize_Response_On_Failuer()
        {
            ContentstackResponse contentstackHttpResponse = MockResponse.CreateContentstackResponse("422Response.txt");

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, contentstackHttpResponse.StatusCode);
            Assert.AreEqual(118, contentstackHttpResponse.ContentLength);
            Assert.AreEqual("application/json", contentstackHttpResponse.ContentType);
            Assert.AreEqual(false, contentstackHttpResponse.IsSuccessStatusCode);
        }

        [TestMethod]
        public void Get_Header_Name()
        {
            ContentstackResponse contentstackHttpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            CollectionAssert.AreEqual(new string[]{"Date", "Connection", "Content-Type", "Content-Length" }, contentstackHttpResponse.GetHeaderNames());
        }

        [TestMethod]
        public void Get_Typeed_Response()
        {
            ContentstackResponse contentstackHttpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            Response response = contentstackHttpResponse.OpenTResponse<Response>();
            Assert.AreEqual("Login Successful.", response.Notice);
        }

        [TestMethod]
        public void Get_Header_Value()
        {
            ContentstackResponse contentstackHttpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            Assert.AreEqual("Tue, 27 Apr 2021 13:28:24 GMT", contentstackHttpResponse.GetHeaderValue("Date"));
            Assert.AreEqual(string.Empty, contentstackHttpResponse.GetHeaderValue("Unknown"));
        }

        [TestMethod]
        public void Check_Header_Present()
        {
            ContentstackResponse contentstackHttpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            Assert.IsTrue(contentstackHttpResponse.IsHeaderPresent("Date"));
            Assert.IsFalse(contentstackHttpResponse.IsHeaderPresent("Unknown"));
        }

        [TestMethod]
        public void Should_Throw_Object_Disposed_Exception_On_Object_Dispose()
        {
            ContentstackResponse contentstackHttpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            contentstackHttpResponse.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => contentstackHttpResponse.OpenResponse());
            Assert.ThrowsException<ObjectDisposedException>(() => contentstackHttpResponse.OpenJObjectResponse());
            Assert.ThrowsException<ObjectDisposedException>(() => contentstackHttpResponse.OpenTResponse<StackModel>());

            contentstackHttpResponse.Dispose();
        }
    }
}
