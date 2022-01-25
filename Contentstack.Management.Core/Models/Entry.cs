using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    public class Entry: BaseModel<IEntry>
    {
        internal Entry(Stack stack, string contentTyppe, string uid)
            : base(stack, "entry", uid)
        {
            resourcePath = uid != null ? $"/content_types/{contentTyppe}/entries" : $"/content_types/{contentTyppe}/entries/{uid}";
        }

        /// <summary>
        /// The Query on Content Type will allow to fetch details of all or specific Content Type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry().Query().Find();
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
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Create(IEntry model)
        {
            return base.Create(model);
        }

        /// <summary>
        /// The Create content type with JSON RTE request shows you how to add a JSON RTE field while creating a content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> CreateAsync(IEntry model)
        {
            return base.CreateAsync(model);
        }

        /// <summary>
        /// The Update Content Type call is used to update the schema of an existing content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override ContentstackResponse Update(IEntry model)
        {
            return base.Update(model);
        }

        /// <summary>
        /// The Update Content Type call is used to update the schema of an existing content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IContentType for updating Content Type.</param>
        /// <returns></returns>
        public override Task<ContentstackResponse> UpdateAsync(IEntry model)
        {
            return base.UpdateAsync(model);
        }

        /// <summary>
        /// The Fetch a single content type call returns information of a specific content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch()
        {
            return base.Fetch();
        }

        /// <summary>
        /// The Fetch a single content type call returns information of a specific content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync()
        {
            return base.FetchAsync();
        }

        /// <summary>
        /// The Delete Content Type call deletes an existing content type and all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete()
        {
            return base.Delete();
        }

        /// <summary>
        /// The Delete Content Type call deletes an existing content type and all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync()
        {
            return base.DeleteAsync();
        }
    }
}
