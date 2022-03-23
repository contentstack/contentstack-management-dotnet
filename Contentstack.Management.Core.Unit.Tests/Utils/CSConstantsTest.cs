using System;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Utils
{
    [TestClass]
    public class CSConstantsTest
    {
        [TestMethod]
        public void Test_Constants()
        {
            Assert.AreEqual("User-Agent", HeadersKey.UserAgentHeader);
            Assert.AreEqual("X-User-Agent", HeadersKey.XUserAgentHeader);
        }
    }
}
