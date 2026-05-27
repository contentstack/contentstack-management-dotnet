using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models.Versioning;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    public class Version
    {
        internal Stack stack = null!;
        internal string resourcePath = null!;
        internal string fieldName = null!;

        public int? Number { get; set; }

        public Version(Stack stack, string resourcePath, string fieldName, int? number)
        {
            this.resourcePath = number == null ? $"{resourcePath}/versions": $"{resourcePath}/versions/{number}/name";
            Number = number;
            this.stack = stack;
            this.fieldName = fieldName;
        }

        public virtual ContentstackResponse GetAll(ParameterCollection? collection = null)
        {
            ThrowIfVersionNumberNotEmpty();

            var service = new VersionService(stack, this.resourcePath, "GET", fieldName, collection, stjOptions: stack.client.SerializerOptions);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> GetAllAsync(ParameterCollection? collection = null)
        {
            ThrowIfVersionNumberNotEmpty();

            var service = new VersionService(stack, this.resourcePath, "GET", fieldName, collection, stjOptions: stack.client.SerializerOptions);
            return stack.client.InvokeAsync<VersionService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Delete(string? locale = null)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack, this.resourcePath, "DELETE", fieldName, stjOptions: stack.client.SerializerOptions);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> DeleteAsync(string? locale = null)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack, this.resourcePath, "DELETE", fieldName, stjOptions: stack.client.SerializerOptions);
            return stack.client.InvokeAsync<VersionService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse SetName(string name, string? locale = null, bool force = false)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack, this.resourcePath, "POST", fieldName, stjOptions: stack.client.SerializerOptions);
            service.name = name;
            service.locale = locale;
            service.force = force;

            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> SetNameAsync(string name, string? locale = null, bool force = false)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack, this.resourcePath, "POST", fieldName, stjOptions: stack.client.SerializerOptions);
            service.name = name;
            service.locale = locale;
            service.force = force;

            return stack.client.InvokeAsync<VersionService, ContentstackResponse>(service);
        }

        #region Throw Error

        internal void ThrowIfVersionNumberNotEmpty()
        {
            if (Number != null)
            {
                throw new InvalidOperationException(CSConstants.OperationNotAllowedForVersion);
            }
        }

        internal void ThrowIfVersionNumberEmpty()
        {
            if (Number == null)
            {
                throw new InvalidOperationException(CSConstants.VersionUIDRequired);
            }
        }
        #endregion
    }
}
