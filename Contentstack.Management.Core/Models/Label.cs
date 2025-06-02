using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    public class Label : BaseModel<LabelModel>
    {
        internal Label(Stack stack, string uid = null)
           : base(stack, "label", uid)
        {
            resourcePath = uid == null ? "/labels" : $"/labels/{uid}";
        }

        /// <summary>
        /// The Query on Label This call fetches all the existing labels of the stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Label().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create used to create a label.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// LabelMode model = new LabelMode();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Label().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">LabelModel for creating Label.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(LabelModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create used to create a label.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// LabelMode model = new LabelMode();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Label().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">LabelModel for creating Label.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(LabelModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update label call is used to update an existing label.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// LabelMode model = new LabelMode();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Label("<LABEL_UID>").Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">LabelModel for creating Label.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Update(LabelModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update label call is used to update an existing label.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// LabelMode model = new LabelMode();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Label("<LABEL_UID>").UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">LabelModel for creating Label.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> UpdateAsync(LabelModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch a single label call returns information about a particular label of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Label("<LABEL_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch a single label call returns information about a particular label of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Label("<LABEL_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete label call is used to delete a specific label.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Label("<LABEL_UID>").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete label call is used to delete a specific label.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Label("<LABEL_UID>").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }
    }
}
