using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    public class Environment : BaseModel<EnvironmentModel>
    {
        internal Environment(Stack stack, string uid = null)
           : base(stack, "environment", uid)
        {
            resourcePath = uid == null ? "/environments" : $"/environments/{uid}";
        }

        /// <summary>
        /// The Query on Environment function fetches the list of all environments available in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Environment().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create function will add a publishing environment for a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EnvironmentModel model = new EnvironmentModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Environment().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Environment Model for creating Environment.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(EnvironmentModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create function will add a publishing environment for a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EnvironmentModel model = new EnvironmentModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Environment().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Environment Model for creating Environment.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(EnvironmentModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update function will update the details of an existing publishing environment for a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EnvironmentModel model = new EnvironmentModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Environment("<ENVIRONMENT_UID>").Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Environment Model for creating Environment.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Update(EnvironmentModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update function will update the details of an existing publishing environment for a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EnvironmentModel model = new EnvironmentModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Environment("<ENVIRONMENT_UID>").UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Environment Model for creating Environment.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> UpdateAsync(EnvironmentModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch function returns more details about the specified environment of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Environment("<ENVIRONMENT_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch function returns more details about the specified environment of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Environment("<ENVIRONMENT_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete function will delete an existing publishing environment from your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Environment("<ENVIRONMENT_UID>").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete function will delete an existing publishing environment from your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Environment("<ENVIRONMENT_UID>").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }
    }
}
