using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Contentstack.Management.Core.Http;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Unit.Tests.Mokes;
using Contentstack.Management.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Unit.Tests.Core.Services
{
    [TestClass]
    public class ContentstackServiceTest
    {
        JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());

        [TestMethod]
        public void Should_Not_Allow_Null_serializer()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ContentstackService(null));
        }

        [TestMethod]
        public void Should_Throw_Object_Disposed_Exception_On_Dispose()
        {
            var contentstackService = new ContentstackService(serializer);
            contentstackService.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => contentstackService.CreateHttpRequest(new HttpClient(), new ContentstackClientOptions()));
            Assert.ThrowsException<ObjectDisposedException>(() => contentstackService.AddPathResource("resourcePath", "val"));
            Assert.ThrowsException<ObjectDisposedException>(() => contentstackService.AddQueryResource("resourcePath", "val"));
            Assert.ThrowsException<ObjectDisposedException>(() => contentstackService.GetHeaderValue("header"));
            contentstackService.Dispose();
        }

        [TestMethod]
        public void Returns_Null_Content()
        {
            var contentstackService = new ContentstackService(serializer);
            contentstackService.ContentBody();

            Assert.IsNotNull(contentstackService);
            Assert.IsNull(contentstackService.Content);
            Assert.IsNotNull(contentstackService.Serializer);
        }

        [TestMethod]
        public void Return_Null_On_Response()
        {
            var contentstackService = new ContentstackService(serializer);
            var config = new ContentstackClientOptions();
            ContentstackResponse httpResponse = MockResponse.CreateContentstackResponse("LoginResponse.txt");

            Assert.IsNull(config.Authtoken);

            contentstackService.OnResponse(httpResponse, config);

            Assert.IsNull(config.Authtoken);
        }

        [TestMethod]
        public void Return_True_On_Get_Method()
        {
            var contentstackService = new ContentstackService(serializer);

            Assert.AreEqual(HttpMethod.Get.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);
        }

        [TestMethod]
        public void Return_False_Other_Than_Get_Method()
        {
            var contentstackService = new ContentstackService(serializer);
            contentstackService.HttpMethod = HttpMethod.Post.ToString();

            Assert.AreEqual(HttpMethod.Post.ToString(), contentstackService.HttpMethod);
            Assert.IsFalse(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Patch.ToString();

            Assert.AreEqual(HttpMethod.Patch.ToString(), contentstackService.HttpMethod);
            Assert.IsFalse(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Delete.ToString();

            Assert.AreEqual(HttpMethod.Delete.ToString(), contentstackService.HttpMethod);
            Assert.IsFalse(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Head.ToString();

            Assert.AreEqual(HttpMethod.Head.ToString(), contentstackService.HttpMethod);
            Assert.IsFalse(contentstackService.UseQueryString);


            contentstackService.HttpMethod = HttpMethod.Options.ToString();

            Assert.AreEqual(HttpMethod.Options.ToString(), contentstackService.HttpMethod);
            Assert.IsFalse(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Put.ToString();

            Assert.AreEqual(HttpMethod.Put.ToString(), contentstackService.HttpMethod);
            Assert.IsFalse(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Trace.ToString();

            Assert.AreEqual(HttpMethod.Trace.ToString(), contentstackService.HttpMethod);
            Assert.IsFalse(contentstackService.UseQueryString);
        }

        [TestMethod]
        public void Return_True_On_Setting_UseQueryString_To_True_For_All_Methods()
        {
            var contentstackService = new ContentstackService(serializer);
            contentstackService.UseQueryString = true;

            contentstackService.HttpMethod = HttpMethod.Post.ToString();

            Assert.AreEqual(HttpMethod.Post.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Patch.ToString();

            Assert.AreEqual(HttpMethod.Patch.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Delete.ToString();

            Assert.AreEqual(HttpMethod.Delete.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Head.ToString();

            Assert.AreEqual(HttpMethod.Head.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);


            contentstackService.HttpMethod = HttpMethod.Options.ToString();

            Assert.AreEqual(HttpMethod.Options.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Put.ToString();

            Assert.AreEqual(HttpMethod.Put.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);

            contentstackService.HttpMethod = HttpMethod.Trace.ToString();

            Assert.AreEqual(HttpMethod.Trace.ToString(), contentstackService.HttpMethod);
            Assert.IsTrue(contentstackService.UseQueryString);
        }

        [TestMethod]
        public void Return_ResourcePath_On_Setting_ResourcePath()
        {
            var contentstackService = new ContentstackService(serializer);
            contentstackService.ResourcePath = "resourcePath";

            Assert.AreEqual("resourcePath", contentstackService.ResourcePath);
        }

        [TestMethod]
        public void Return_PathResources_On_Adding_Path_Resource()
        {
            var contentstackService = new ContentstackService(serializer);

            contentstackService.AddPathResource("content_type_uid", "contentTypeUid");
            contentstackService.AddPathResource("entry_uid", "entryUid");

            Assert.AreEqual(2, contentstackService.PathResources.Count);
            Assert.AreEqual("contentTypeUid", contentstackService.PathResources["content_type_uid"]);
            Assert.AreEqual("entryUid", contentstackService.PathResources["entry_uid"]);
        }

        [TestMethod]
        public void Return_PathResources_On_Adding_Query_Resource()
        {
            var contentstackService = new ContentstackService(serializer);

            contentstackService.AddQueryResource("content_type_uid", "contentTypeUid");
            contentstackService.AddQueryResource("entry_uid", "entryUid");

            Assert.AreEqual(2, contentstackService.QueryResources.Count);
            Assert.AreEqual("contentTypeUid", contentstackService.QueryResources["content_type_uid"]);
            Assert.AreEqual("entryUid", contentstackService.QueryResources["entry_uid"]);
        }

        [TestMethod]
        public void Return_True_HttpBody_On_PUT_POST_PATCH_Methods()
        {
            var contentstackService = new ContentstackService(serializer);

            contentstackService.HttpMethod = HttpMethod.Put.ToString();

            Assert.IsTrue(contentstackService.HasRequestBody());

            contentstackService.HttpMethod = HttpMethod.Post.ToString();

            Assert.IsTrue(contentstackService.HasRequestBody());

            contentstackService.HttpMethod = HttpMethod.Patch.ToString();

            Assert.IsTrue(contentstackService.HasRequestBody());

            contentstackService.HttpMethod = HttpMethod.Delete.ToString();

            Assert.IsTrue(contentstackService.HasRequestBody());
        }

        [TestMethod]
        public void Return_False_HttpBody_On_Other_Then_PUT_POST_PATCH_Methods()
        {
            var contentstackService = new ContentstackService(serializer);

            contentstackService.HttpMethod = HttpMethod.Get.ToString();

            Assert.IsFalse(contentstackService.HasRequestBody());

            contentstackService.HttpMethod = HttpMethod.Head.ToString();

            Assert.IsFalse(contentstackService.HasRequestBody());

            contentstackService.HttpMethod = HttpMethod.Options.ToString();

            Assert.IsFalse(contentstackService.HasRequestBody());

            contentstackService.HttpMethod = HttpMethod.Trace.ToString();

            Assert.IsFalse(contentstackService.HasRequestBody());
        }

        [TestMethod]
        public void Return_Empty_String_On_Non_Exist_HeaderKey()
        {
            var contentstackService = new ContentstackService(serializer);

            Assert.AreEqual(string.Empty, contentstackService.GetHeaderValue("unknown"));
        }

        [TestMethod]
        public void Return_Value_For_HeaderKey()
        {
            var contentstackService = new ContentstackService(serializer);

            contentstackService.Headers[HeadersKey.ContentTypeHeader] = "application/json";

            Assert.AreEqual("application/json", contentstackService.GetHeaderValue(HeadersKey.ContentTypeHeader));
        }

        [TestMethod]
        public void Return_HttpRequest_On_Create_HttpRequest()
        {
            var contentstackService = new ContentstackService(serializer);
            contentstackService.Headers["authtoken"] = "application/json";
            ContentstackHttpRequest httpClient = (ContentstackHttpRequest)contentstackService.CreateHttpRequest(new HttpClient(), new ContentstackClientOptions());
            Assert.IsNotNull(httpClient);
            IEnumerable<string> headerValues = httpClient.Request.Headers.GetValues("authtoken");
            Assert.AreEqual("application/json", headerValues.FirstOrDefault());
        }

        [TestMethod]
        public void Should_Add_query_Parameter()
        {
            var parameter = new ParameterCollection();
            parameter.Add("limit", 10);
            parameter.Add("include", new List<string>() { "1", "2", "3" });
            var contentstackService = new ContentstackService(serializer, parameter);
            contentstackService.HttpMethod = HttpMethod.Post.ToString();
            contentstackService.UseQueryString = true;

            ContentstackHttpRequest httpClient = (ContentstackHttpRequest)contentstackService.CreateHttpRequest(new HttpClient(), new ContentstackClientOptions());

            Assert.IsNotNull(httpClient);
            Assert.AreEqual("?include[]=1&include[]=2&include[]=3&limit=10", httpClient.RequestUri.Query);
        }
    }
}
