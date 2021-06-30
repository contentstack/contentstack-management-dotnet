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
            Assert.AreEqual(HeadersKey.UserAgentHeader, "User-Agent");
            Assert.AreEqual(HeadersKey.XUserAgentHeader, "X-User-Agent");
        }
    }
}
