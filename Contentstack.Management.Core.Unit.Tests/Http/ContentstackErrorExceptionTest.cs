using System;
using System.Net;
using System.Net.Http;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Http
{
    [TestClass]
    public class ContentstackErrorExceptionTest
    {

        [TestMethod]
        public void Test_Exception_422()
        {
            HttpResponseMessage httpResponse = MockResponse.CreateFromResource("422Response.txt");

            var exception = ContentstackErrorException.CreateException(httpResponse);

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            Assert.AreEqual("Unprocessable Entity", exception.ReasonPhrase);
            Assert.AreEqual("Looks like your email or password is invalid. You have 4 login attempt(s) left.", exception.Message);
            Assert.AreEqual("Looks like your email or password is invalid. You have 4 login attempt(s) left.", exception.ErrorMessage);
            Assert.AreEqual(104, exception.ErrorCode);
            Assert.IsNull(exception.Errors);
        }

        [TestMethod]
        public void Test_Exception_422_With_Errors()
        {
            HttpResponseMessage httpResponse = MockResponse.CreateFromResource("422DetailErrorResponse.txt");

            var exception = ContentstackErrorException.CreateException(httpResponse);

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            Assert.AreEqual("Unprocessable Entity", exception.ReasonPhrase);
            Assert.AreEqual("The Content Type 'soure' was not found. Please try again.", exception.Message);
            Assert.AreEqual("The Content Type 'soure' was not found. Please try again.", exception.ErrorMessage);
            Assert.AreEqual(118, exception.ErrorCode);
            Assert.IsNotNull(exception.Errors);
        }

        [TestMethod]
        public void Test_Exception_304()
        {
            HttpResponseMessage httpResponse = MockResponse.CreateFromResource("304Response.txt");

            var exception = ContentstackErrorException.CreateException(httpResponse);

            Assert.AreEqual(HttpStatusCode.NotModified, exception.StatusCode);
            Assert.AreEqual("Not Modified", exception.ReasonPhrase);
            Assert.IsNull(exception.Message);
            Assert.AreEqual(string.Empty, exception.ErrorMessage);
            Assert.AreEqual(0, exception.ErrorCode);
            Assert.IsNull(exception.Errors);
        }

        [TestMethod]
        public void Test_Exception_400()
        {
            HttpResponseMessage httpResponse = MockResponse.CreateFromResource("400Response.txt");

            var exception = ContentstackErrorException.CreateException(httpResponse);

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.AreEqual("Bad Request", exception.ReasonPhrase);
            Assert.AreEqual("Please set a valid 'Content-Type' header", exception.Message);
            Assert.AreEqual("Please set a valid 'Content-Type' header", exception.ErrorMessage);
            Assert.AreEqual(0, exception.ErrorCode);
            Assert.IsNull(exception.Errors);
        }
    }
}
