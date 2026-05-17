using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Contentstack.Management.Core.Utils;
using System.Text.Json;
using Contentstack.Management.Core.Enums;

namespace Contentstack.Management.Core.Services.Stack
{
    internal class StackCreateUpdateService : ContentstackService
    {
        private readonly string _name;
        private readonly string _masterLocale;
        private readonly string _description;

        #region Internal
        internal StackCreateUpdateService(
            Newtonsoft.Json.JsonSerializer serializer,
            Core.Models.Stack stack,
            string name,
            string masterLocale = null,
            string description = null,
            string organizationUid = null,
            JsonSerializerOptions stjOptions = null,
            SerializationMode serializationMode = SerializationMode.Newtonsoft)
            : base(serializer, stack, stjOptions: stjOptions, serializationMode: serializationMode)
        {
            this.ResourcePath = "/stacks";

            _name = name;
            _masterLocale = masterLocale;
            _description = description;

            if (stack.APIKey != null)
            {
                this.HttpMethod = "PUT";
            }
            else if (!string.IsNullOrEmpty(organizationUid))
            {
                if (masterLocale == null)
                {
                    throw new ArgumentNullException("masterLocale", CSConstants.MasterLocaleRequired);
                }
                if (_name == null)
                {
                    throw new ArgumentNullException("name", CSConstants.StackNameRequired);
                }
                this.Headers.Add("organization_uid", organizationUid);
                this.HttpMethod = "POST";
            }
            else 
            {
                throw new ArgumentNullException("stack", CSConstants.APIKeyOrOrgUIDRequired);
            }
        }

        public override void ContentBody()
        {
            // Build the stack object dynamically based on what's provided
            var stackObj = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(_name))
                stackObj["name"] = _name;

            if (!string.IsNullOrEmpty(_description))
                stackObj["description"] = _description;

            if (HttpMethod == "POST" && !string.IsNullOrEmpty(_masterLocale))
                stackObj["master_locale"] = _masterLocale;

            var stackRequest = new { stack = stackObj };
            var mode = GetSerializationMode();
            WriteObjectWithBothEngines(stackRequest, mode, GetSerializerSettings(), GetStjOptions(), out byte[] content);
            ByteContent = content;
        }
        #endregion
    }
}
