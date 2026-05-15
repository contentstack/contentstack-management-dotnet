using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Collections.Generic;

namespace Contentstack.Management.Core.Unit.Tests.Utils
{
    public class Utilities
    {
        public static JsonSerializer GetJsonSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings());
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
            Assembly assembly = typeof(Utilities).Assembly;
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
            Assembly assembly = typeof(Utilities).Assembly;
            var allResources = assembly.GetManifestResourceNames();
            foreach (var resource in allResources)
            {
                if (match(resource))
                    yield return resource;
            }
        }

        public static HttpContent CreateStreamFromString(string s)
        {
            return new StringContent(s, Encoding.UTF8, "application/json");
        }

        public static Stream CreateStreamFromString(string s, Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
