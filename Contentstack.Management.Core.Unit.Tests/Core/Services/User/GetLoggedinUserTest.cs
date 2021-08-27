using System;
using Contentstack.Management.Core.Services.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.User
{
    [TestClass]
    public class GetLoggedinUserTest
    {
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GetLoggedInUserService(null, null));
        }

        [TestMethod]
        public void Should_Initialize_With_Proper_ResourcePath_And_Method()
        {
            GetLoggedInUserService getLoggedInUserService = new GetLoggedInUserService(serializer, null);
            Assert.IsNotNull(getLoggedInUserService);

            Assert.AreEqual("GET", getLoggedInUserService.HttpMethod);
            Assert.AreEqual("user", getLoggedInUserService.ResourcePath);
        }
    }
}
