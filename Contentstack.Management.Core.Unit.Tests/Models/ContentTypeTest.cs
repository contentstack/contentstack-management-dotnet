using System;
using AutoFixture;
using Contentstack.Management.Core.Models;
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
            _stack = new Stack(new ContentstackClient(), _fixture.Create<string>());
        }

        [TestMethod]
        public void Initialize_ContentType()
        {
            ContentType contentType = new ContentType(_stack, null);

            Assert.IsNull(contentType.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Fetch());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.FetchAsync());
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Update(new ContentModeling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.UpdateAsync(new ContentModeling()));
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Delete());
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.DeleteAsync());
        }

        [TestMethod]
        public void Initialize_ContentType_With_Uid()
        {
            string uid = _fixture.Create<string>();
            ContentType contentType = new ContentType(_stack, uid);

            Assert.AreEqual(uid, contentType.Uid);
            Assert.ThrowsException<InvalidOperationException>(() => contentType.Create(new ContentModeling()));
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => contentType.CreateAsync(new ContentModeling()));
        }
    }
}
