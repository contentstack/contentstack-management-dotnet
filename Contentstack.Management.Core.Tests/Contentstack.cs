using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests
{
    public class Contentstack
    {
        private static readonly Lazy<ContentstackClient>
        client =
        new Lazy<ContentstackClient>(() =>
        {
            ContentstackClientOptions options = Config.GetSection("Contentstack").Get<ContentstackClientOptions>();
            return new ContentstackClient(new OptionsWrapper<ContentstackClientOptions>(options));
        });


        private static readonly Lazy<IConfigurationRoot>
        config =
        new Lazy<IConfigurationRoot>(() =>
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        });

        private static readonly Lazy<NetworkCredential> credential =
        new Lazy<NetworkCredential>(() =>
        {
            return Config.GetSection("Contentstack:Credentials").Get<NetworkCredential>();
        });

        private static readonly Lazy<OrganizationModel> organization =
        new Lazy<OrganizationModel>(() =>
        {
            return Config.GetSection("Contentstack:Organization").Get<OrganizationModel>();
        });

        public static ContentstackClient Client { get { return client.Value; } }
        public static IConfigurationRoot Config{ get { return config.Value; } }
        public static NetworkCredential Credential { get { return credential.Value; } }
        public static OrganizationModel Organization { get { return organization.Value; } }

        public static StackModel Stack { get; set; }

        public static T serialize<T>(JsonSerializer serializer, string filePath)
        {
            string response = GetResourceText(filePath);
            JObject jObject = JObject.Parse(response);
            return jObject.ToObject<T>(serializer);
        }
        public static T serializeArray<T>(JsonSerializer serializer, string filePath)
        {
            string response = GetResourceText(filePath);
            JArray jObject = JArray.Parse(response);
            return jObject.ToObject<T>(serializer);
        }

        public static string GetResourceText(string resourceName)
        {
            using (StreamReader reader = new StreamReader(GetResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream GetResourceStream(string resourceName)
        {
            Assembly assembly = typeof(Contentstack).Assembly;
            var resource = FindResourceName(resourceName);
            Stream stream = assembly.GetManifestResourceStream(resource);
            return stream;
        }

        public static string FindResourceName(string partialName)
        {
            return FindResourceName(s => s.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0).Single();
        }

        public static IEnumerable<string> FindResourceName(Predicate<string> match)
        {
            Assembly assembly = typeof(Contentstack).Assembly;
            var allResources = assembly.GetManifestResourceNames();
            foreach (var resource in allResources)
            {
                if (match(resource))
                    yield return resource;
            }
        }
    }
}
