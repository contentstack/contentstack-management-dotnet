using System;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    public class ContentType: BaseModel<Query>
    {
       
        internal ContentType(Stack stack, string uid)
            : base(stack, uid)
        {
            resourcePath = "/content_type";
        }

        /// <summary>
        /// The Query on Content Type will allow to fetch details of all or specific Content Type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack().ContentType().Query().Find();
        /// //Or
        /// ContentstackResponse contentstackResponse = await client.Stack().ContentType().Query().FindAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/></returns>
        public Query Query()
        {
            return new Query(_stack, resourcePath);
        }

    }

    public class BaseModel<T>
    {
        internal Stack _stack;

        public string Uid;

        internal string resourcePath;

        public BaseModel(Stack stack, string uid)
        {
            _stack = stack;
            Uid = uid;
        } 
            
        public void Create(T model)
        {
            ThrowIfUidNotEmpty();
        }

        public void Fetch()
        {
            ThrowIfUidEmpty();
        }

        public void Update(T model)
        {
            ThrowIfUidEmpty();
        }

        public void Delete()
        {
            ThrowIfUidEmpty();
        }

        #region Throw Error

        internal void ThrowIfUidNotEmpty()
        {
            if (!string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Operation not allowed on Uid.");
            }
        }

        internal void ThrowIfUidEmpty()
        {
            if (string.IsNullOrEmpty(this.Uid))
            {
                throw new InvalidOperationException("Uid can not be empty.");
            }
        }
        #endregion
    }
}
