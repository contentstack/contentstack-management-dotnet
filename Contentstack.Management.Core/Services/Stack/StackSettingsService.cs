using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Newtonsoft.Json;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackSettingsService: ContentstackService
    {
        #region Internal

        private StackSettings _settings;

        internal StackSettingsService(JsonSerializer serializer, Core.Models.Stack stack, string method = "GET", StackSettings settings = null) : base(serializer, stack)
        {
            if (stack.APIKey == null)
            {
                throw new ArgumentNullException("API Key should be present.");
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
                    string snippet = $"{{\"stack_settings\":{JsonConvert.SerializeObject(_settings)}}}";
                    ByteContent = System.Text.Encoding.UTF8.GetBytes(snippet);
                    break;
                default:
                    break;

            }
        }
        #endregion
    }
}
