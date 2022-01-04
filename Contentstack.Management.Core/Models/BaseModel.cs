using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class BaseModel<T>
    {
        internal Stack stack;
        internal string fieldName;
        public string Uid;

        internal string resourcePath;

        public BaseModel(Stack stack, string fieldName, string uid)
        {
            this.stack = stack;
            this.fieldName = fieldName;
            Uid = uid;
        }

        public virtual ContentstackResponse Create(T model)
        {
            ThrowIfUidNotEmpty();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, this.fieldName);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> CreateAsync(T model)
        {
            ThrowIfUidNotEmpty();
            stack.client.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, this.fieldName);

            return stack.client.InvokeAsync<CreateUpdateService<T>, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Update(T model)
        {
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, "PUT");
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> UpdateAsync(T model)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<T>(stack.client.serializer, stack, resourcePath, model, "PUT");

            return stack.client.InvokeAsync<CreateUpdateService<T>, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Fetch()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> FetchAsync()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath);

            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Delete()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> DeleteAsync()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE");

            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        #region Throw Error

        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Operation not allowed.");
            }
        }

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Uid can not be empty.");
            }
        }
        #endregion
    }
}
