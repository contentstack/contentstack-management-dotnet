using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

namespace Contentstack.Management.Core.Models
{
    public class Release : BaseModel<ReleaseModel>
    {
        internal Release(Stack stack, string uid = null)
           : base(stack, "release", uid)
        {
            resourcePath = uid == null ? "/releases" : $"/releases/{uid}";
        }

        /// <summary>
        /// The Query on ReleaseModel request retrieves a list of all Releases of a stack along with details of each Release.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Release().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Queryable.Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// The Create request allows you to create a new Release in your stack.
        /// To add entries/assets to a Release, you need to provide the UIDs of the entries/assets in ‘items’ in the request body.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ReleaseModel model = new ReleaseModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Release().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Release Model for creating ReleaseModel.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(ReleaseModel model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create request allows you to create a new Release in your stack.
        /// To add entries/assets to a Release, you need to provide the UIDs of the entries/assets in ‘items’ in the request body.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ReleaseModel model = new ReleaseModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Release().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Release Model Model for creating ReleaseModel.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(ReleaseModel model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update call allows you to update the details of a Release, i.e., the ‘name’ and ‘description’.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ReleaseModel model = new ReleaseModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Release("<RELEASE_UID>").Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Release Model for creating ReleaseModel.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Update(ReleaseModel model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update call allows you to update the details of a Release, i.e., the ‘name’ and ‘description’.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ReleaseModel model = new ReleaseModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Release("<RELEASE_UID>").UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">Release Model for creating ReleaseModel.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> UpdateAsync(ReleaseModel model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch rrequest gets the details of a specific Release in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Release("<RELEASE_UID>").Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch request gets the details of a specific Release in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Release("<RELEASE_UID>").FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete request allows you to delete a specific Release from a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Release("<RELEASE_UID>").Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete request allows you to delete a specific Release from a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Release("<RELEASE_UID>").DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }

        /// <summary>
        /// The list of all items (entries and assets) that are part of a specific Release.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Release("<RELEASE_UID>").Item().GetAll();
        /// </code></pre>
        /// </example>
        /// <param name="uid">Release Item UID</param>
        /// <returns>The <see cref="ReleaseItem"/>.</returns>
        public ReleaseItem Item()
        {
            ThrowIfUidEmpty();

            return new ReleaseItem(stack, this.Uid);
        }

        /// <summary>
        /// The Deploy a Release request deploys a specific Release to specific environment(s) and locale(s).
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// DeployModel model = new DeployModel();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Relase("<RELEASE_UID>").Deploy(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Deploy(DeployModel model)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new CreateUpdateService<DeployModel>(stack.client.serializer, stack, $"{resourcePath}/deploy", model, "release");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Deploy a Release request deploys a specific Release to specific environment(s) and locale(s).
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// DeployModel model = new DeployModel();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Relase("<RELEASE_UID>").DeployAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> DeployAsync(DeployModel model)
        {
            ThrowIfUidEmpty();
            stack.ThrowIfNotLoggedIn();

            var service = new CreateUpdateService<DeployModel>(stack.client.serializer, stack, $"{resourcePath}/deploy", model, "release");

            return stack.client.InvokeAsync<CreateUpdateService<DeployModel>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Clone request allows you to clone (make a copy of) a specific Release in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").Relase("<RELEASE_UID>").Clone("<NAME>", "<DESCRIPTION>");
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Clone(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Invalide name.");
            }
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            Dictionary<string, string> model = new Dictionary<string, string>()
            {
                { "name", name }
            };
            if (!string.IsNullOrEmpty(description))
            {
                model.Add("desctiption", description);
            }
            var service = new CreateUpdateService<Dictionary<string, string>>(stack.client.serializer, stack, $"{resourcePath}/clone", model, "release");
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Clone request allows you to clone (make a copy of) a specific Release in a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").Relase("<RELEASE_UID>").CloneAsync("<NAME>", "<DESCRIPTION>");
        /// </code></pre>
        /// </example>
        /// <param name="model">ReleaseItem Model for creating ReleaseItem.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> CloneAsync(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Invalide name.");
            }
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            Dictionary<string, string> model = new Dictionary<string, string>()
            {
                { "name", name }
            };
            if (!string.IsNullOrEmpty(description))
            {
                model.Add("desctiption", description);
            }
            var service = new CreateUpdateService<Dictionary<string, string>>(stack.client.serializer, stack, $"{resourcePath}/clone", model, "release");

            return stack.client.InvokeAsync<CreateUpdateService<Dictionary<string, string>>, ContentstackResponse>(service);
        }
    }
}
