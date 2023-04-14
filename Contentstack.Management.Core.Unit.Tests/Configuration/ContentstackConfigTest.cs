using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests
{
    [TestClass]
    public class ContentstackConfigTest
    {
        readonly string Host = "10.0.0.1";
        readonly int Port = 20;
        readonly string UserName= "test_user_name";
        readonly string Password = "password";
        readonly string Authtoken = "Authtoken";
        readonly long MaxContentSize = 8589934592;
        readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);


        [TestMethod]
        public void Should_Have_Default_Values_On_Initialization()
        {
            var contentstackConfig = new ContentstackClientOptions();

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
            Assert.AreEqual("https://api.contentstack.io/v3", contentstackConfig.GetUri(contentstack.management.core.Http.VersionStrategy.URLPath).AbsoluteUri);

        }

        [TestMethod]
        public void Should_Allow_To_Set_Authtoken()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.Authtoken = Authtoken;

            Assert.AreEqual(Authtoken, contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
        }

        [TestMethod]
        public void Should_Allow_To_Set_Host()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.Host = Host;

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual(Host, contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
            Assert.AreEqual($"https://{Host}/v3", contentstackConfig.GetUri(contentstack.management.core.Http.VersionStrategy.URLPath).AbsoluteUri);

        }

        [TestMethod]
        public void Should_Allow_To_Set_RetryOnError()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.RetryOnError = false;

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsFalse(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
        }

        [TestMethod]
        public void Should_Disable_Logging()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.DisableLogging = true;

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsTrue(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
        }

        [TestMethod]
        public void Should_Set_MaxResponse_Content_Buffer_Size()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.MaxResponseContentBufferSize = MaxContentSize;

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(MaxContentSize, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
        }

        [TestMethod]
        public void Should_Set_Timeout()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.Timeout = Timeout;

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(Timeout.Seconds, contentstackConfig.Timeout.Seconds);
        }

        [TestMethod]
        public void Should_Set_Port()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.Port = 445;

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(445, contentstackConfig.Port);
            Assert.AreEqual("v3", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
        }

        [TestMethod]
        public void Should_Set_Version()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.Version = "v4";

            Assert.IsNull(contentstackConfig.Authtoken);
            Assert.AreEqual("api.contentstack.io", contentstackConfig.Host);
            Assert.AreEqual(443, contentstackConfig.Port);
            Assert.AreEqual("v4", contentstackConfig.Version);
            Assert.IsNull(contentstackConfig.ProxyHost);
            Assert.AreEqual(-1, contentstackConfig.ProxyPort);
            Assert.IsNull(contentstackConfig.ProxyCredentials);
            Assert.IsNull(contentstackConfig.GetWebProxy());
            Assert.IsTrue(contentstackConfig.RetryOnError);
            Assert.IsFalse(contentstackConfig.DisableLogging);
            Assert.AreEqual(1073741824, contentstackConfig.MaxResponseContentBufferSize);
            Assert.AreEqual(30, contentstackConfig.Timeout.Seconds);
            Assert.AreEqual("https://api.contentstack.io/v4", contentstackConfig.GetUri(contentstack.management.core.Http.VersionStrategy.URLPath).AbsoluteUri);
        }

        [TestMethod]
        public void Should_Allow_To_Set_Proxy()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.ProxyHost = Host;
            contentstackConfig.ProxyPort = Port;

            Assert.AreEqual(Host, contentstackConfig.ProxyHost);
            Assert.AreEqual(Port, contentstackConfig.ProxyPort);

        }

        [TestMethod]
        public void Should_Allow_To_Set_Proxy_With_Http()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.ProxyHost = string.Format("http://{0}", Host);
            contentstackConfig.ProxyPort = Port;

            Assert.AreEqual(string.Format("http://{0}", Host), contentstackConfig.ProxyHost);
            Assert.AreEqual(Port, contentstackConfig.ProxyPort);

        }

        [TestMethod]
        public void Should_Return_WebProxy()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.ProxyHost = Host;
            contentstackConfig.ProxyPort = Port;

            var proxy = contentstackConfig.GetWebProxy() as WebProxy;
            Assert.AreEqual(Host, proxy.Address.Host);
            Assert.AreEqual(Port, proxy.Address.Port);
            Assert.IsNull(proxy.Credentials);
        }

        [TestMethod]
        public void Should_Return_WebProxy_with_Http()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.ProxyHost = string.Format("http://{0}", Host);
            contentstackConfig.ProxyPort = Port;

            var proxy = contentstackConfig.GetWebProxy() as WebProxy;
            Assert.AreEqual(Host, proxy.Address.Host);
            Assert.AreEqual(Port, proxy.Address.Port);
            Assert.IsNull(proxy.Credentials);
        }

        public void Should_Allow_To_Set_Proxy_Credentials()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.ProxyHost = Host;
            contentstackConfig.ProxyPort = Port;
            contentstackConfig.ProxyCredentials = new NetworkCredential(userName: UserName, password: Password);

            var credentials = contentstackConfig.ProxyCredentials as NetworkCredential;
            Assert.AreEqual(Host, contentstackConfig.ProxyHost);
            Assert.AreEqual(Port, contentstackConfig.ProxyPort);
            Assert.AreEqual(UserName, credentials.UserName);
            Assert.AreEqual(Password, credentials.Password);
        }

        [TestMethod]
        public void Should_Return_WebProx_With_Credentials()
        {
            var contentstackConfig = new ContentstackClientOptions();

            contentstackConfig.ProxyHost = Host;
            contentstackConfig.ProxyPort = Port;
            contentstackConfig.ProxyCredentials = new NetworkCredential(userName: UserName, password: Password);

            var proxy = contentstackConfig.GetWebProxy() as WebProxy;
            var credentials = proxy.Credentials as NetworkCredential;
            Assert.AreEqual(Host, contentstackConfig.ProxyHost);
            Assert.AreEqual(Port, contentstackConfig.ProxyPort);
            Assert.AreEqual(UserName, credentials.UserName);
            Assert.AreEqual(Password, credentials.Password);
        }
    }
}
