using AutoFixture;
using Contentstack.Management.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Models
{

    [TestClass]
    public class GlobalFieldTest
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
            GlobalField contentType = new GlobalField(_stack, null);

            Assert.IsNull(contentType.Uid);
        }

        [TestMethod]
        public void Initialize_ContentType_With_Uid()
        {
            string uid = _fixture.Create<string>();
            GlobalField contentType = new GlobalField(_stack, uid);

            Assert.AreEqual(uid, contentType.Uid);
        }
    }
}
