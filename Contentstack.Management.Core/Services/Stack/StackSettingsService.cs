using System;
using System.Globalization;
using System.IO;
using Contentstack.Management.Core.Models;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackSettingsService: ContentstackService
    {
        #region Internal

        private readonly StackSettings _settings;

        internal StackSettingsService(Newtonsoft.Json.JsonSerializer serializer, Core.Models.Stack stack, string method = "GET", StackSettings settings = null, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft) : base(serializer, stack, stjOptions: stjOptions, serializationMode: serializationMode)
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
                    var wrapper = new { stack_settings = _settings };
                    var mode = GetSerializationMode();
                    WriteObjectWithBothEngines(wrapper, mode, GetSerializerSettings(), GetStjOptions(), out byte[] content);
                    ByteContent = content;
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
