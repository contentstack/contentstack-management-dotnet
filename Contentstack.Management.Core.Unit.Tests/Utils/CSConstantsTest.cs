using System;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Utils
{
    [TestClass]
    public class CSConstantsTest
    {
        [TestMethod]
        public void Test_HeadersKey_Constants()
        {
            Assert.AreEqual("User-Agent", HeadersKey.UserAgentHeader);
            Assert.AreEqual("X-User-Agent", HeadersKey.XUserAgentHeader);
            Assert.AreEqual("Content-Type", HeadersKey.ContentTypeHeader);
            Assert.AreEqual("x-header-ea", HeadersKey.EarlyAccessHeader);
        }

        [TestMethod]
        public void Test_CSConstants_InternalConstants()
        {
            Assert.AreEqual(1024 * 1024 * 1024, CSConstants.ContentBufferSize);
            Assert.AreEqual(TimeSpan.FromSeconds(30), CSConstants.Timeout);
            Assert.AreEqual(TimeSpan.FromMilliseconds(300), CSConstants.Delay);
            Assert.AreEqual("/", CSConstants.Slash);
            Assert.AreEqual('/', CSConstants.SlashChar);
        }

        [TestMethod]
        public void Test_CSConstants_InternalMessages()
        {
            Assert.AreEqual("You are already logged in.", CSConstants.YouAreLoggedIn);
            Assert.AreEqual("You are need to login.", CSConstants.YouAreNotLoggedIn);
            Assert.AreEqual("Uid should not be empty.", CSConstants.MissingUID);
            Assert.AreEqual("API Key should not be empty.", CSConstants.MissingAPIKey);
            Assert.AreEqual("API Key should be empty.", CSConstants.APIKey);
            Assert.AreEqual("Please enter email id to remove from org.", CSConstants.RemoveUserEmailError);
            Assert.AreEqual("Please enter share uid to resend invitation.", CSConstants.OrgShareUIDMissing);
        }
    }
}
