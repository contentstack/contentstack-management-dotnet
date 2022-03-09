using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models.Versioning;

namespace Contentstack.Management.Core.Models
{
    public class Version
    {
        public int? Number;
        internal Stack stack;
        internal string resourcePath;
        internal string fieldName;
        public Version(Stack stack, string resourcePath, string fieldName, int? number)
        {
            this.resourcePath = number == null ? $"{resourcePath}/versions": $"{resourcePath}/versions/{Number}/name";
            this.stack = stack;
            this.fieldName = fieldName;
        }

        public virtual ContentstackResponse GetAll(ParameterCollection collection = null)
        {
            ThrowIfVersionNumberNotEmpty();

            var service = new VersionService(stack.client.serializer, stack, this.resourcePath, "GET", fieldName, collection);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> GetAllAsync(ParameterCollection collection = null)
        {
            ThrowIfVersionNumberNotEmpty();

            var service = new VersionService(stack.client.serializer, stack, this.resourcePath, "GET", fieldName, collection);
            return stack.client.InvokeAsync<VersionService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Delete(string locale = null)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack.client.serializer, stack, this.resourcePath, "DELETE", fieldName);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> DeleteAsync(string locale = null)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack.client.serializer, stack, this.resourcePath, "DELETE", fieldName);
            return stack.client.InvokeAsync<VersionService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse SetName(string name, string locale = null, bool force = false)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack.client.serializer, stack, this.resourcePath, "POST", fieldName);
            service.name = name;
            service.locale = locale;
            service.force = force;

            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> SetNameAsync(string name, string locale = null, bool force = false)
        {
            ThrowIfVersionNumberEmpty();

            var service = new VersionService(stack.client.serializer, stack, this.resourcePath, "POST", fieldName);
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
                throw new InvalidOperationException("Operation not allowed.");
            }
        }

        internal void ThrowIfVersionNumberEmpty()
        {
            if (Number == null)
            {
                throw new InvalidOperationException("Uid can not be empty.");
            }
        }
        #endregion
    }
}
