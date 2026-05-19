using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using System.Text.Json;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackSettingsService: ContentstackService
    {
        #region Internal

        private readonly StackSettings _settings;

        internal StackSettingsService(JsonSerializerOptions serializerOptions, Core.Models.Stack stack, string method = "GET", StackSettings settings = null) : base(serializerOptions, stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("stack", CSConstants.MissingAPIKey);
            }
            ResourcePath = "stacks/settings";
            HttpMethod = method;
            _settings = settings;
        }

        public override void ContentBody()
        {
            switch (HttpMethod)
            {
                case "POST":
                    string snippet = $"{{\"stack_settings\":{JsonSerializer.Serialize(_settings, SerializerOptions)}}}";
                    ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
                    break;
                default:
                    break;

            }
        }
        #endregion
    }
}
