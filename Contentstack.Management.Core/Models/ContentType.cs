using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    public class ContentType: BaseModel<ContentModelling>
    {
        internal ContentType(Stack stack, string uid) 
            : base(stack, "content_type", uid)
        {
            resourcePath = uid == null ? "/content_types" : $"/content_types/{uid}";
        }

        /// <summary>
        /// The Query on Content Type will allow to fetch details of all or specific Content Type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create content type with JSON RTE request shows you how to add a JSON RTE field while creating a content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentTypeModel model = new ContentTypeModel(); // Add content type schema or fieldrules 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Create(ContentModelling model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create content type with JSON RTE request shows you how to add a JSON RTE field while creating a content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentTypeModel model = new ContentTypeModel(); // Add content type schema or fieldrules
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> CreateAsync(ContentModelling model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update Content Type call is used to update the schema of an existing content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentTypeModel model = new ContentTypeModel(); // Add content type schema or fieldrules
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Update(ContentModelling model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update Content Type call is used to update the schema of an existing content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentTypeModel model = new ContentTypeModel(); // Add content type schema or fieldrules
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> UpdateAsync(ContentModelling model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch a single content type call returns information of a specific content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch a single content type call returns information of a specific content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete Content Type call deletes an existing content type and all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete Content Type call deletes an existing content type and all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }

        /// <summary>
        /// <see cref="Models.Entry" /> is the actual piece of content created using one of the defined content types.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry().Query().Find();
        /// </code></pre>
        /// 
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <param name="uid">Optional entry uid for performing entry specific operation</param>
        /// <returns></returns>
        public Entry Entry(string uid = null)
        {
            ThrowIfUidEmpty();
            return new Entry(stack, Uid, uid);
        }
    }
}
