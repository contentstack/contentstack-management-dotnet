using System;
using System.Collections.Generic;
using System.Text;
using Contentstack.Management.Core.Services;

namespace Contentstack.Management.Core.Utils
{
    internal class ContentstackUtilities
    {
        internal static Uri ComposeUrI(Uri baseUri, IContentstackService service)
        {

            Uri requestUri = baseUri;

            var delim = "?";

            var sb = new StringBuilder();

            if (service.QueryResources.Count > 0)
            {
                foreach (var queryResource in service.QueryResources)
                {
                    if (queryResource.Value != null)
                    {
                        sb.AppendFormat("{0}{1}={2}", delim, queryResource.Key, queryResource.Value);
                        delim = "&";
                    }
                }
            }

            //if (service.UseQueryString && service.Parameters.Count > 0)
            //{
            //    var queryString = ""; // TODO: Covert params to query string
            //    sb.AppendFormat("{0}{1}", delim, queryString);
            //}

            var resourcePath = service.ResourcePath;

            if (resourcePath == null)
            {
                resourcePath = string.Empty;
            }
            else
            {
                resourcePath = ResolvePathResource(resourcePath, service.PathResources);
            }

            var parameterizedPath = string.Concat(resourcePath, sb);

            var hasSlash = requestUri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) || parameterizedPath.StartsWith("/", StringComparison.Ordinal);
            requestUri = hasSlash
                ? new Uri(requestUri.AbsoluteUri + parameterizedPath)
                : new Uri(requestUri.AbsoluteUri + "/" + parameterizedPath);

            return requestUri;
        }

        internal static string ResolvePathResource(string resourcePath, IDictionary<string, string> pathResources)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return resourcePath;
            }

            return string.Join(CSConstants.Slash, SplitResourceSegment(resourcePath, pathResources));
        }

        internal static IEnumerable<string> SplitResourceSegment(string resourcePath, IDictionary<string, string> pathResources)
        {
            var splitChars = new char[] { CSConstants.SlashChar };

            var segment = resourcePath.Split(splitChars, options: StringSplitOptions.None);

            if (pathResources.Count > 0)
            {
                var resolvedSegment = new List<string>();

                foreach (var stringSeg in segment)
                {
                    if (pathResources.ContainsKey(stringSeg))
                    {
                        resolvedSegment.Add(pathResources[stringSeg]);
                    }
                    else
                    {
                        resolvedSegment.Add(stringSeg);
                    }
                }

                return resolvedSegment;
            }
            return segment;
        }
    }
}
