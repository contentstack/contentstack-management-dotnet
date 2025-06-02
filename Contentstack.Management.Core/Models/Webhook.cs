using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class Webhook : BaseModel<WebhookModel>
    {
        internal Webhook(Stack stack, string uid = null)
           : base(stack, "webhook", uid)
        {
            resourcePath = uid == null ? "/webhooks" : $"/webhooks/{uid}";
        }

        /// <summary>
        /// The Query Webhooks request returns comprehensive information on all the available webhooks in the specified stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create a webhook request allows you to create a new webhook in a specific stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// WebhookModel model = new WebhookModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Webhook Model for creating Webhook.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(WebhookModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create a webhook request allows you to create a new webhook in a specific stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// WebhookModel model = new WebhookModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Webhook().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Webhook Model for creating Webhook.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(WebhookModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update webhook request allows you to update the details of an existing webhook in the stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// WebhookModel model = new WebhookModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Webhook Model for creating Webhook.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Update(WebhookModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update webhook request allows you to update the details of an existing webhook in the stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// WebhookModel model = new WebhookModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Webhook Model for creating Webhook.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> UpdateAsync(WebhookModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch webhook request returns comprehensive information on a specific webhook.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch webhook request returns comprehensive information on a specific webhook.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete webhook call deletes an existing webhook from a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete webhook call deletes an existing webhook from a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }

        /// <summary>
        /// The Get executions of a webhook request allows you to fetch the execution details of a specific webhook, which includes the execution UID.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").Executions();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Executions(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/executions", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get executions of a webhook request allows you to fetch the execution details of a specific webhook, which includes the execution UID.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").ExecutionsAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> ExecutionsAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/executions", collection: collection);

            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// This call makes a manual attempt to execute a webhook after the webhook has finished executing its automatic attempts.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").Retry("<EXECUTION_UID>");
        /// </code></pre>
        /// </example>
        /// <param name="executionUid">execution UID that you receive when you execute the 'Get executions of webhooks' call.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Retry(string executionUid)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"/webhooks/{executionUid}/retry", httpMethod: "POST");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// This call makes a manual attempt to execute a webhook after the webhook has finished executing its automatic attempts.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").RetryAsync("<EXECUTION_UID>");
        /// </code></pre>
        /// </example>
        /// <param name="executionUid">execution UID that you receive when you execute the 'Get executions of webhooks' call.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> RetryAsync(string executionUid)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"/webhooks/{executionUid}/retry", httpMethod: "POST");

            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// This call will return a comprehensive detail of all the webhooks that were executed at a particular execution cycle.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").Logs("<EXECUTION_UID>");
        /// </code></pre>
        /// </example>
        /// <param name="executionUid">execution UID that you receive when you execute the 'Get executions of webhooks' call.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Logs(string executionUid)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"/webhooks/{executionUid}/logs");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// This call will return a comprehensive detail of all the webhooks that were executed at a particular execution cycle.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Webhook("<WEBHOOK_UID>").LogsAsync("<EXECUTION_UID>");
        /// </code></pre>
        /// </example>
        /// <param name="executionUid">execution UID that you receive when you execute the 'Get executions of webhooks' call.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> LogsAsync(string executionUid)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"/webhooks/{executionUid}/logs");

            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }
    }
}
