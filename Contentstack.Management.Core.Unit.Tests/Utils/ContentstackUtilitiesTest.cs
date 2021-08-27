using System;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Utils
{
    [TestClass]
    public class ContentstackUtilitiesTest
    {
        private readonly Uri baseUri = new Uri("https://localHost.com");

        [TestMethod]
        public void Return_Uri_On_Service()
        {
            var uri = ContentstackUtilities.ComposeUrI(baseUri, new MockService());

            Assert.AreEqual(baseUri, uri);
        }

        [TestMethod]
        public void Return_Uri_On_Service_QueryResource()
        {
            var service = new MockService();
            service.AddQueryResource("limit", "4");
            service.AddQueryResource("count", "true");

            var uri = ContentstackUtilities.ComposeUrI(baseUri, service);

            Assert.AreEqual(string.Format("{0}?limit=4&count=true", baseUri.AbsoluteUri), uri.AbsoluteUri);
        }

        [TestMethod]
        public void Return_Uri_On_Service_Empty_Resource()
        {
            var service = new MockService();
            service.ResourcePath = string.Empty;
            var uri = ContentstackUtilities.ComposeUrI(baseUri, service);

            Assert.AreEqual(baseUri, uri);
        }

        [TestMethod]
        public void Return_Uri_On_Service_With_Resource_Andy_QueryResource()
        {
            var service = new MockService();
            service.ResourcePath = "content_Type";
            service.AddQueryResource("limit", "4");
            service.AddQueryResource("count", "true");

            var uri = ContentstackUtilities.ComposeUrI(baseUri, service);

            Assert.AreEqual(string.Format("{0}{1}?limit=4&count=true", baseUri.AbsoluteUri, service.ResourcePath), uri.AbsoluteUri);
        }

        [TestMethod]
        public void Return_Uri_On_Service_With_ResourcePath()
        {
            var service = new MockService();
            service.ResourcePath = "content_type/{content_type_uid}/entries/{entry_uid}";
            service.AddPathResource("{content_type_uid}", "ContentTypeUid");
            service.AddPathResource("{entry_uid}", "EntryUid");
            var uri = ContentstackUtilities.ComposeUrI(baseUri, service);

            Assert.AreEqual(string.Format("{0}content_type/ContentTypeUid/entries/EntryUid", baseUri.AbsoluteUri), uri.AbsoluteUri);
        }

        [TestMethod]
        public void Return_Uri_On_Service_With_ResourcePath_And_Query()
        {
            var service = new MockService();
            service.ResourcePath = "content_type/{content_type_uid}/entries/{entry_uid}";
            service.AddPathResource("{content_type_uid}", "ContentTypeUid");
            service.AddPathResource("{entry_uid}", "EntryUid");

            service.AddQueryResource("limit", "4");
            service.AddQueryResource("count", "true");

            var uri = ContentstackUtilities.ComposeUrI(baseUri, service);

            Assert.AreEqual(string.Format("{0}content_type/ContentTypeUid/entries/EntryUid?limit=4&count=true", baseUri.AbsoluteUri), uri.AbsoluteUri);
        }

        [TestMethod]
        public void Return_Query_Parameters_On_ParameterCollection()
        {
            var param = new ParameterCollection();
            param.Add("limit", 10);
            param.Add("include", "type");

            var result = ContentstackUtilities.GetQueryParameter(param);

            Assert.AreEqual("include=type&limit=10", result);
        }

        [TestMethod]
        public void Return_Empty_Query_Parameters_On_ParameterCollection()
        {
            var param = new ParameterCollection();

            var result = ContentstackUtilities.GetQueryParameter(param);

            Assert.AreEqual("", result);
        }
    }
}
