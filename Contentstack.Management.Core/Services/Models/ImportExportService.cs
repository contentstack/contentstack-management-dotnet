using System;
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Models
{
    internal class ImportExportService : ContentstackService
    {
        internal ImportExportService(JsonSerializer serializer, Core.Models.Stack stack, string resourcePath, bool isImport = false, string httpMethod = "GET", ParameterCollection collection = null)
            : base(serializer, stack: stack, collection: collection)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            if (resourcePath == null)
            {
                throw new ArgumentNullException("resourcePath", CSConstants.ResourcePathRequired);
            }
            
            ResourcePath = isImport ? $"{resourcePath}/import" : $"{resourcePath}/export";
            HttpMethod = httpMethod;
        }
    }
}
