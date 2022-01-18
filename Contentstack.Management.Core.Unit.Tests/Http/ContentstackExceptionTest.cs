using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Http
{
    [TestClass]
    public class ContentstackExceptionTest
    {
        private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization());
        [TestMethod]
        public void Test_Contentstack_Exception()
        {

            var message = _fixture.Create<string>();
            var exception = new ContentstackException(message);

            Assert.AreEqual(message, exception.Message);
        }

        [TestMethod]
        public void Test_Contentstack_Exception_with_Inner_Exception()
        {

            var message = _fixture.Create<string>();
            var exception = new ContentstackException(message, new Exception(message));

            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(message, exception.InnerException.Message);
        }
    }
}
