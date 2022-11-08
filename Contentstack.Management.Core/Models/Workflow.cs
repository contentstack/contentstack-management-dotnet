using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class Workflow: BaseModel<WorkflowModel>
    {
        internal Workflow(Stack stack, string uid)
            : base(stack, "workflows", uid)
        {
            resourcePath = uid == null ? "/workflows" : $"/workflows/{uid}";
        }

        /// <summary>
        /// The Get all Workflows request retrieves the details of all the Workflows of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().FindAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse FindAll(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get all Workflows request retrieves the details of all the Workflows of a stack.
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().FindAllAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> FindAllAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidNotEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Create Workflow request allows you to create a Workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// WorkflowModel model = new WorkflowModel(); 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Workflow Model for creating workflow.</param>
        /// <returns></returns>
        public override ContentstackResponse Create(WorkflowModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create Workflow request allows you to create a Workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// WorkflowModel model = new WorkflowModel();
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Workflow Model for creating workflow.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> CreateAsync(WorkflowModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update Workflow request allows you to add a workflow stage or update the details of the existing stages of a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// WorkflowModel model = new WorkflowModel(); 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Workflow Model for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Update(WorkflowModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update Workflow request allows you to add a workflow stage or update the details of the existing stages of a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// WorkflowModel model = new WorkflowModel(); 
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Workflow Model for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> UpdateAsync(WorkflowModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The fetch Workflow request retrieves the comprehensive details of a specific Workflow of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The fetch Workflow request retrieves the comprehensive details of a specific Workflow of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete Workflow request allows you to delete a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete Workflow request allows you to delete a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }

        /// <summary>
        /// The Disable Workflow request allows you to disable a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).Disable();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Disable()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/disable");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Disable Workflow request allows you to disable a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).DisableAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> DisableAsync()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/disable");
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Enable Workflow request allows you to enable a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).Enable();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Enable()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/disable");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Enable Workflow request allows you to enable a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).EnableAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> EnableAsync()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchDeleteService(stack.client.serializer, stack, $"{resourcePath}/disable");
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }

        /// <summary>
        /// <see cref="Models.PublishRule" /> is a tool that allows you to streamline the process of content creation and publishing, and lets you manage the content lifecycle of your project smoothly.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule(&quot;&lt;ENTRY_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <param name="uid">Optional Publish rule uid for performing rule specific operation</param>
        /// <returns></returns>
        public PublishRule PublishRule(string uid = null)
        {
            return new PublishRule(stack, uid);
        }

        /// <summary>
        /// The Get Publish Rules by Content Types request allows you to retrieve details of a Publish Rule applied to a specific content type of your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).GetPublishRule(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse GetPublishRule(string contentType, ParameterCollection collection)
        {
            stack.ThrowIfNotLoggedIn();
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException("Content Type can not be empty.");
            }

            var service = new FetchDeleteService(stack.client.serializer, stack, $"/workflows/content_type/{contentType}", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get Publish Rules by Content Types request allows you to retrieve details of a Publish Rule applied to a specific content type of your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow(&quot;&lt;WORKFLOW_UID&gt;&quot;).GetPublishRuleAsync(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;);
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual Task<ContentstackResponse> GetPublishRuleAsync(string contentType, ParameterCollection collection)
        {
            stack.ThrowIfNotLoggedIn();
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException("Content Type can not be empty.");
            }
            var service = new FetchDeleteService(stack.client.serializer, stack, $"/workflows/content_type/{contentType}", collection: collection);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }
    }
}
