using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using contentstack.management.core.Services.App;
using Contentstack.Management.Core.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services.App
{
    [TestClass]
    public class CreateUpdateAppsServiceTest
	{
        private JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly IFixture _fixture = new Fixture()
       .Customize(new AutoMoqCustomization());

        [TestMethod]
        public void Should_Throw_On_Null_Serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CreateUpdateAppsService(
                null,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<JObject>(),
                _fixture.Create<string>(),
                _fixture.Create<ParameterCollection>()));
        }
    }
}

