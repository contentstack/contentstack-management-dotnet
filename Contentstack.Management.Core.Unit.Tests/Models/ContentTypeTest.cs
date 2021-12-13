using System;
using System.Collections.Generic;
using AutoFixture;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contentstack.Management.Core.Unit.Tests.Models
{
    [TestClass]
    public class ContentTypeTest
    {
        private Stack _stack;
        private readonly IFixture _fixture = new Fixture();

        [TestInitialize]
        public void initialize()
        {
            _stack = new Stack(new ContentstackClient());
        }

        [TestMethod]
        public void Initialize_ContentType()
        {
            ContentType contentType = new ContentType(_stack, null);

            Assert.IsNull(contentType.Uid);
        }

        [TestMethod]
        public void Initialize_ContentType_With_Uid()
        {
            string uid = _fixture.Create<string>();
            ContentType contentType = new ContentType(_stack, uid);

            Assert.AreEqual(uid, contentType.Uid);
        }
    }
}
