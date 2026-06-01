using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Branches allow you to isolate and manage "in-progress" work from your stable production work.
    /// </summary>
    public class Branch : BaseModel<BranchModel>
    {
        internal Branch(Stack stack, string? uid = null)
            : base(stack, "branch", uid)
        {
            resourcePath = uid == null ? "/stacks/branches" : "/stacks/branches/" + uid;
        }

        /// <summary>
        /// The Query on Branch fetches the list of all branches available in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse contentstackResponse = client.Stack("&lt;API_KEY&gt;").Branch().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create branch call creates a new branch in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// BranchModel model = new BranchModel() { Uid = "my-branch", Source = "main" };
        /// ContentstackResponse contentstackResponse = client.Stack("&lt;API_KEY&gt;").Branch().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">BranchModel for creating Branch.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(BranchModel model, ParameterCollection? collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create branch call creates a new branch in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// BranchModel model = new BranchModel() { Uid = "my-branch", Source = "main" };
        /// ContentstackResponse contentstackResponse = await client.Stack("&lt;API_KEY&gt;").Branch().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">BranchModel for creating Branch.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(BranchModel model, ParameterCollection? collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch branch call returns information about a specific branch of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse contentstackResponse = client.Stack("&lt;API_KEY&gt;").Branch("&lt;BRANCH_UID&gt;").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection? collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch branch call returns information about a specific branch of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse contentstackResponse = await client.Stack("&lt;API_KEY&gt;").Branch("&lt;BRANCH_UID&gt;").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection? collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete branch call deletes a specific branch of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse contentstackResponse = client.Stack("&lt;API_KEY&gt;").Branch("&lt;BRANCH_UID&gt;").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection? collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete branch call deletes a specific branch of a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse contentstackResponse = await client.Stack("&lt;API_KEY&gt;").Branch("&lt;BRANCH_UID&gt;").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection? collection = null)
        {
            return base.DeleteAsync(collection);
        }
    }
}
