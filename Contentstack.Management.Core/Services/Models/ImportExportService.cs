using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Models
{
    internal class ImportExportService : ContentstackService
    {
        internal ImportExportService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, bool isImport = false, string httpMethod = "GET", ParameterCollection collection = null)
            : base(serializer, stack: stack, collection: collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", "Should have API Key to perform this operation.");
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", "Should have resource path for service.");
            }
            
            ResourcePath = isImport ? $"{resourcePath}/import" : $"{resourcePath}/export";
            HttpMethod = httpMethod;
        }
    }
}
