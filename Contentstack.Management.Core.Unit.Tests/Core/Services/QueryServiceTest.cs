using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Unit.Tests.Core.Services
{
    [TestClass]
    public class QueryServiceTest
    {
        private Management.Core.Models.Stack _stack;

        JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        [TestInitialize]
        public void initialize()
        {
            _stack = new Management.Core.Models.Stack(new ContentstackClient());
        }

        [TestMethod]
        public void Should_Not_Allow_Null_ResourcePath()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryService(_stack, new ParameterCollection(), null));
        }

        [TestMethod]
        public void Should_Not_Allow_Null_API_Key()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryService(_stack, new ParameterCollection(), _fixture.Create<string>()));
        }
    }
}
