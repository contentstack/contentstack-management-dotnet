using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class PublishRule: BaseModel<PublishRuleModel>
    {
        internal PublishRule(Stack stack, string uid)
            : base(stack, "publishing_rule", uid)
        {
            resourcePath = uid == null ? "/workflows/publishing_rules" : $"/workflows/publishing_rules/{uid}";
        }

        /// <summary>
        /// The Get all Publish Rules request retrieves the details of all the Publish rules of a workflow. 
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule().FindAll();
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
        /// The Get all Publish Rules request retrieves the details of all the Publish rules of a workflow. 
        /// </summary>
        /// <param name="collection">Query parameter</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule().FindAllAsync();
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
        /// The Create Publish Rules request allows you to create publish rules for the workflow of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// PublishRuleModel model = new PublishRuleModel(); 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">PublishRule Model for creating workflow.</param>
        /// <returns></returns>
        public override ContentstackResponse Create(PublishRuleModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create Publish Rules request allows you to create publish rules for the workflow of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// PublishRuleModel model = new PublishRuleModel();
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">PublishRule Model for creating workflow.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> CreateAsync(PublishRuleModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update Publish Rules request allows you to add a publish rule or update the details of the existing publish rules of a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// PublishRuleModel model = new PublishRuleModel(); 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule(&quot;&lt;PUBLISH_RULE_UID&gt;&quot;).Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">PublishRule Model for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Update(PublishRuleModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update Publish Rules request allows you to add a publish rule or update the details of the existing publish rules of a workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// PublishRuleModel model = new PublishRuleModel(); 
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule(&quot;&lt;PUBLISH_RULE_UID&gt;&quot;).UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">PublishRule Model for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> UpdateAsync(PublishRuleModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The fetch Publish Rule request retrieves the comprehensive details of a specific publish rule of a Workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule(&quot;&lt;PUBLISH_RULE_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The fetch Publish Rule request retrieves the comprehensive details of a specific publish rule of a Workflow.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule(&quot;&lt;PUBLISH_RULE_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete Publish Rules request allows you to delete an existing publish rule. 
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule(&quot;&lt;PUBLISH_RULE_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete Publish Rules request allows you to delete an existing publish rule. 
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Workflow().PublishRule(&quot;&lt;PUBLISH_RULE_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }
    }
}
