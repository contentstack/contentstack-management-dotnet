using System;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Runtime.Contexts;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Core
{
    [TestClass]
    public class ContentstackClientTest
    {
        [TestMethod]
        public void Initialize_Contentstack()
        {
            var contentstackClient = new ContentstackClient();

            Assert.IsNotNull(contentstackClient.LogManager);
            Assert.IsNotNull(contentstackClient.SerializerSettings);
            Assert.AreEqual(contentstackClient.User().GetType(), typeof(User));
            Assert.AreEqual(contentstackClient.Organization().GetType(), typeof(Organization));
            Assert.AreEqual(contentstackClient.Stack().GetType(), typeof(Stack));
            Assert.IsNotNull(contentstackClient.contentstackOptions);
        }

        [TestMethod]
        public void Initialize_Contentstack_With_Authtoken()
        {
            var contentstackClient = new ContentstackClient(authtoken: "token");
            Assert.AreEqual("token", contentstackClient.contentstackOptions.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackClient.contentstackOptions.Host);
            Assert.AreEqual(443, contentstackClient.contentstackOptions.Port);
            Assert.AreEqual("v3", contentstackClient.contentstackOptions.Version);
            Assert.IsNull(contentstackClient.contentstackOptions.ProxyHost);
            Assert.AreEqual(-1, contentstackClient.contentstackOptions.ProxyPort);
            Assert.IsNull(contentstackClient.contentstackOptions.ProxyCredentials);
            Assert.IsNull(contentstackClient.contentstackOptions.GetWebProxy());
            Assert.IsTrue(contentstackClient.contentstackOptions.RetryOnError);
            Assert.IsFalse(contentstackClient.contentstackOptions.DisableLogging);
            Assert.AreEqual(1073741824, contentstackClient.contentstackOptions.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackClient.contentstackOptions.Timeout.Seconds);
        }

        [TestMethod]
        public void Initialize_Contentstack_With_All_Options()
        {
            var contentstackClient = new ContentstackClient(authtoken: "token", host: "host", 445, "v4", true, 1234, 20, false, "proxyHost", 22);

            Assert.AreEqual("token", contentstackClient.contentstackOptions.Authtoken);
            Assert.AreEqual("host", contentstackClient.contentstackOptions.Host);
            Assert.AreEqual(445, contentstackClient.contentstackOptions.Port);
            Assert.AreEqual("v4", contentstackClient.contentstackOptions.Version);
            Assert.AreEqual("proxyHost", contentstackClient.contentstackOptions.ProxyHost);
            Assert.AreEqual(22, contentstackClient.contentstackOptions.ProxyPort);
            Assert.IsNull(contentstackClient.contentstackOptions.ProxyCredentials);
            Assert.IsNotNull(contentstackClient.contentstackOptions.GetWebProxy());
            Assert.IsFalse(contentstackClient.contentstackOptions.RetryOnError);
            Assert.IsTrue(contentstackClient.contentstackOptions.DisableLogging);
            Assert.AreEqual(1234, contentstackClient.contentstackOptions.MaxResponseContentBufferSize);
            Assert.AreEqual(20, contentstackClient.contentstackOptions.Timeout.Seconds);
        }

        [TestMethod]
        public void Initialize_Contentstack_With_Clientptions()
        {
            var contentstackClient = new ContentstackClient(new ContentstackClientOptions()
            {
                Authtoken = "token",
                Host= "host",
                Port= 445,
                Version= "v4",
                DisableLogging= true,
                MaxResponseContentBufferSize= 1234,
                Timeout= TimeSpan.FromSeconds(20),
                RetryOnError= false,
                ProxyHost= "proxyHost",
                ProxyPort= 22,
                EarlyAccess = new string[] { "ea1", "ea2" }
            });

            Assert.AreEqual("token", contentstackClient.contentstackOptions.Authtoken);
            Assert.AreEqual("host", contentstackClient.contentstackOptions.Host);
            Assert.AreEqual(445, contentstackClient.contentstackOptions.Port);
            Assert.AreEqual("v4", contentstackClient.contentstackOptions.Version);
            Assert.AreEqual("proxyHost", contentstackClient.contentstackOptions.ProxyHost);
            Assert.AreEqual(22, contentstackClient.contentstackOptions.ProxyPort);
            Assert.IsNull(contentstackClient.contentstackOptions.ProxyCredentials);
            Assert.IsNotNull(contentstackClient.contentstackOptions.GetWebProxy());
            Assert.IsFalse(contentstackClient.contentstackOptions.RetryOnError);
            Assert.IsTrue(contentstackClient.contentstackOptions.DisableLogging);
            Assert.AreEqual(1234, contentstackClient.contentstackOptions.MaxResponseContentBufferSize);
            Assert.AreEqual(20, contentstackClient.contentstackOptions.Timeout.Seconds);
            CollectionAssert.AreEqual(new string[] {"ea1", "ea2"}, contentstackClient.contentstackOptions.EarlyAccess);
            Assert.AreEqual("ea1,ea2", contentstackClient.DefaultRequestHeaders[HeadersKey.EarlyAccessHeader]);
        }

        [TestMethod]
        public void Should_Dispose_ContentstackClientAsync()
        {
            var contentstackClient = new ContentstackClient();

            contentstackClient.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => contentstackClient.InvokeSync(new MockService()));
            Assert.ThrowsException<ObjectDisposedException>(() =>  contentstackClient.InvokeAsync<MockService, ContentstackResponse>(new MockService()));

            contentstackClient.Dispose();
        }

        [TestMethod]
        public void Should_Invoke_ContentstackClientAsync()
        {
            var contentstackClient = new ContentstackClient(host: "https://MockHost");
            contentstackClient.ContentstackPipeline.ReplaceHandler(new MockHttpHandler(MockResponse.CreateContentstackResponse("LoginResponse.txt")));

            Assert.IsNotNull(contentstackClient.InvokeSync(new MockService()));
            Assert.IsNotNull(contentstackClient.InvokeAsync<MockService, ContentstackResponse>(new MockService()));

            contentstackClient.Dispose();
        }
    }
}
