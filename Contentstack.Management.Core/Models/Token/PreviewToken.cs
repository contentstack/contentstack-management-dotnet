using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models.Token
{
    /// <summary>
    /// Preview Tokens provide access to retrieve website details within the Live Preview panel.
    /// They are compatible only with the rest-preview.contentstack.com endpoint
    /// and are scoped to a specific Delivery Token.
    /// </summary>
    public class PreviewToken : BaseModel<PreviewTokenModel>
    {
        internal PreviewToken(Stack stack, string deliveryTokenUid)
            : base(stack, "token")
        {
            resourcePath = $"stacks/delivery_tokens/{deliveryTokenUid}/preview_token";
        }

        /// <summary>
        /// Creates a Preview Token for a specific Delivery Token in a stack.
        /// Preview Tokens are compatible only with the rest-preview.contentstack.com endpoint.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// PreviewTokenModel model = new PreviewTokenModel() { Name = "My Preview Token" };
        /// ContentstackResponse response = client.Stack("&lt;API_KEY&gt;").PreviewToken("&lt;DELIVERY_TOKEN_UID&gt;").Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">PreviewTokenModel containing the details for the new preview token.</param>
        /// <param name="collection">Optional query parameters.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(PreviewTokenModel model, ParameterCollection? collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// Creates a Preview Token for a specific Delivery Token in a stack asynchronously.
        /// Preview Tokens are compatible only with the rest-preview.contentstack.com endpoint.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// PreviewTokenModel model = new PreviewTokenModel() { Name = "My Preview Token" };
        /// ContentstackResponse response = await client.Stack("&lt;API_KEY&gt;").PreviewToken("&lt;DELIVERY_TOKEN_UID&gt;").CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">PreviewTokenModel containing the details for the new preview token.</param>
        /// <param name="collection">Optional query parameters.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(PreviewTokenModel model, ParameterCollection? collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// Deletes the Preview Token associated with a specific Delivery Token.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse response = client.Stack("&lt;API_KEY&gt;").PreviewToken("&lt;DELIVERY_TOKEN_UID&gt;").Delete();
        /// </code></pre>
        /// </example>
        /// <param name="collection">Optional query parameters.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection? collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            var service = new FetchDeleteService(stack, resourcePath, "DELETE", collection: collection, stjOptions: stack.client.SerializerOptions);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// Deletes the Preview Token associated with a specific Delivery Token asynchronously.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("&lt;AUTHTOKEN&gt;", "&lt;API_HOST&gt;");
        /// ContentstackResponse response = await client.Stack("&lt;API_KEY&gt;").PreviewToken("&lt;DELIVERY_TOKEN_UID&gt;").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <param name="collection">Optional query parameters.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection? collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            var service = new FetchDeleteService(stack, resourcePath, "DELETE", collection: collection, stjOptions: stack.client.SerializerOptions);
            return stack.client.InvokeAsync<FetchDeleteService, ContentstackResponse>(service);
        }
    }
}
