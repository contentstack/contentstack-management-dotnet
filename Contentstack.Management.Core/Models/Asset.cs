using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class Asset
    {
        internal Stack stack;
        public string Uid;

        internal string resourcePath;

        public Asset(Stack stack, string uid = null)
        {
            stack.ThrowIfAPIKeyEmpty();
            
            this.stack = stack;
            Uid = uid;
            resourcePath = uid == null ? "/assets" : $"/assets/{uid}";
        }

        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        public virtual ContentstackResponse Create(AssetModel model, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> CreateAsync(AssetModel model, ParameterCollection collection = null)
        {
            ThrowIfUidNotEmpty();
            stack.client.ThrowIfNotLoggedIn();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model);
            return stack.client.InvokeAsync<UploadService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Update(AssetModel model, ParameterCollection collection = null)
        {
            ThrowIfUidEmpty();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model, "PUT");
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> UpdateAsync(AssetModel model, ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new UploadService(stack.client.serializer, stack, resourcePath, model, "PUT");
            return stack.client.InvokeAsync<UploadService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        public virtual ContentstackResponse Delete(ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE", collection: collection);
            return stack.client.InvokeSync(service);
        }

        public virtual Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, "DELETE", collection: collection);
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

