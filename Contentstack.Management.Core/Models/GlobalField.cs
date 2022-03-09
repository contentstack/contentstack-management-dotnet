using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    public class GlobalField : BaseModel<ContentModelling>
    {
        internal GlobalField(Stack stack, string uid)
            : base(stack, "global_field", uid)
        {
            resourcePath = uid == null ? "/global_fields" : $"/global_fields/{uid}";
        }

        /// <summary>
        /// The Query on Global Field will allow to fetch details of all or specific Content Type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create global field with JSON RTE request shows you how to add a JSON RTE field while creating a global field.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentModeling model = new ContentModeling() // Add global field schema or field rules 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IGlobalField for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Create(ContentModelling model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create global fieldwith JSON RTE request shows you how to add a JSON RTE field while creating a global field.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentModeling model = new ContentModeling() // Add global field schema or field rules
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IGlobalField for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> CreateAsync(ContentModelling model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update Content Type call is used to update the schema of an existing global field.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentModeling model = new ContentModeling() // Add global field schema or field rules
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IGlobalField for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Update(ContentModelling model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update Content Type call is used to update the schema of an existing global field.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentModeling model = new ContentModeling() // Add global field schema or field rules
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IGlobalField for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> UpdateAsync(ContentModelling model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch a single global fieldcall returns information of a specific global field.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch a single global fieldcall returns information of a specific global field.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete Content Type call deletes an existing global fieldand all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete Content Type call deletes an existing global fieldand all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).GlobalField(&quot;&lt;GLOBAL_FIELD_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }
    }
}
