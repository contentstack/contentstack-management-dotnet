using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models.Token
{
    public class DeliveryToken : BaseModel<DeliveryTokenModel>
    {
        internal DeliveryToken(Stack stack, string uid = null)
           : base(stack, "token", uid)
        {
            resourcePath = uid == null ? "/delivery_tokens" : $"/delivery_tokens/{uid}";
        }

        /// <summary>
        /// The Query on DeliveryToken returns the details of all the delivery tokens created in a stack..
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").DeliveryToken().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create request is used to create a delivery token in the stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// DeliveryTokenModel model = new DeliveryTokenModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").DeliveryToken().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">DeliveryToken Model for creating DeliveryToken.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(DeliveryTokenModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create request is used to create a delivery token in the stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// DeliveryTokenModel model = new DeliveryTokenModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").DeliveryToken().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">DeliveryToken Model for creating DeliveryToken.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(DeliveryTokenModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update request lets you update the details of a delivery token.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// DeliveryTokenModel model = new DeliveryTokenModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").DeliveryToken("<DELIVERY_TOKEN_UID>").Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">DeliveryToken Model for creating DeliveryToken.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Update(DeliveryTokenModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update request lets you update the details of a delivery token.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// DeliveryTokenModel model = new DeliveryTokenModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").DeliveryToken("<DELIVERY_TOKEN_UID>").UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">DeliveryToken Model for creating DeliveryToken.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> UpdateAsync(DeliveryTokenModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch function returns the details of all the delivery tokens created in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").DeliveryToken("<DELIVERY_TOKEN_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch function returns the details of all the delivery tokens created in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").DeliveryToken("<DELIVERY_TOKEN_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete request deletes a specific delivery token.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").DeliveryToken("<DELIVERY_TOKEN_UID>").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete request deletes a specific delivery token.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").DeliveryToken("<DELIVERY_TOKEN_UID>").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }
    }
}
