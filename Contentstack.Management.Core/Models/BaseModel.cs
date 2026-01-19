using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Models
{
    public class BaseModel<T>
    {
        internal Stack stack;
        internal string fieldName;
        internal string resourcePath;

        public string Uid { get; set; }


        public BaseModel(Stack stack, string fieldName, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName", CSConstants.FieldNameRequired);
            }
            this.stack = stack;
            this.fieldName = fieldName;
            Uid = uid;
        }

        public virtual ContentstackResponse Create(T model, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, this.fieldName, collection: collection);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> CreateAsync(T model, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, this.fieldName, collection: collection);

            return stack.client.InvokeAsync<CreateUpdateService<T>, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Update(T model, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, this.fieldName, "PUT", collection: collection);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> UpdateAsync(T model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, this.fieldName, "PUT", collection: collection);

            return stack.client.InvokeAsync<CreateUpdateService<T>, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Delete(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE", collection: collection);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE", collection: collection);

            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        #region Throw Error

        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException(CSConstants.OperationNotAllowedOnModel);
            }
        }

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException(CSConstants.MissingUID);
            }
        }
        #endregion
    }
}
