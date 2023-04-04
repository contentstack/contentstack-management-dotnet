using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Services;
using Contentstack.Management.Core.Utils;

namespace Contentstack.Management.Core.Queryable
{
    public class Query
    {
        private readonly Stack _stack;
        private readonly string _resourcePath;
        private readonly ParameterCollection _collection = new ParameterCollection();
        internal Query(Stack stack, string resourcePath)
        {
            if(stack == null)
            {
                throw new ArgumentNullException("stack", "Stack can not be null");
            }

            if (resourcePath== null)
            {
                throw new ArgumentNullException("resourcePath", "Respource path can not be null");
            }
            _stack = stack;
            _resourcePath = resourcePath;
        }
        #region Public
        /// <summary>
        /// The ‘limit’ parameter will return a specific number of Objects in the output.
        /// </summary>
        /// <param name="value">Number of object in limit</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack().Query().Limit(5).Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/>.</returns>
        public Query Limit(double value)
        {
            _collection.Add("limit", value);
            return this;
        }

        /// <summary>
        /// The ‘skip’ parameter will skip a specific number of Object in the output.
        /// </summary>
        /// <param name="value">Number of object to skip</param>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack().Query().Skip(5).Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/>.</returns>
        public Query Skip(double value)
        {
            _collection.Add("skip", value);
            return this;
        }

        /// <summary>
        /// The ‘include_count’ parameter returns the total number of object related to the user.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack().Query().IncludeCount().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/>.</returns>
        public Query IncludeCount()
        {
            _collection.Add("include_count", true);
            return this;
        }

        /// <summary>
        /// The Find all object call fetches the list of all objects owned by a particular user account.
        /// </summary>
        /// <returns>The <see cref="ContentstackResponse"/></returns>
        public ContentstackResponse Find(ParameterCollection collection = null)
        {
            _stack.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (collection != null)
            {
                foreach (var kvp in collection)
                {
                    _collection.Add(kvp.Key, kvp.Value);
                }
            }
            
            var service = new QueryService(_stack, _collection, _resourcePath);
            return _stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Find all object call fetches the list of all objects owned by a particular user account.
        /// </summary>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> FindAsync(ParameterCollection collection = null)
        {
            _stack.ThrowIfNotLoggedIn();
            this.ThrowIfAPIKeyEmpty();
            if (collection != null)
            {
                foreach (var kvp in collection)
                {
                    _collection.Add(kvp.Key, kvp.Value);
                }
            }
            var service = new QueryService(_stack, _collection, _resourcePath);

            return _stack.client.InvokeAsync<QueryService, ContentstackResponse>(service);
        }
        #endregion
        #region Throw Error

        internal void ThrowIfAPIKeyEmpty()
        {
            if (string.IsNullOrEmpty(_stack.APIKey))
            {
                throw new InvalidOperationException(CSConstants.MissingAPIKey);
            }
        }

        #endregion
    }
}
