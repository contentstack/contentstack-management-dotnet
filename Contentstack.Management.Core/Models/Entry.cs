using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;

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
        /// The Query on Entry will allow to fetch details of all or specific Content Type.
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
        /// The Create entry with JSON RTE request shows you how to add a JSON RTE field while creating a content type.
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
        public override ContentstackResponse Create(IEntry model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// The Create entry with JSON RTE request shows you how to add a JSON RTE field while creating a content type.
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
        public override Task<ContentstackResponse> CreateAsync(IEntry model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model);
        }

        /// <summary>
        /// The Update Entry call is used to update the schema of an existing content type.
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
        public override ContentstackResponse Update(IEntry model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update Entry call is used to update the schema of an existing content type.
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
        public override Task<ContentstackResponse> UpdateAsync(IEntry model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Fetch a single entry call returns information of a specific content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Fetch a single entry call returns information of a specific content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete Entry call deletes an existing entry and all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete Entry call deletes an existing entry and all the entries within it.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }

        /// <summary>
        /// The Delete Locale will delete specific localized entries by passing the locale codes.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// List<string> locales = new List<string>() { "hi-in", "mr-in", "es" }
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).DeleteMultipleLocal();
        /// </code></pre>
        /// </example>
        /// <param name="locales">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse DeleteMultipleLocal(List<string> locales)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new DeleteService<Dictionary<string, List<string>>>(stack.client.serializer, stack, resourcePath, "entry", new Dictionary<string, List<string>>()
            {
                {"locales", locales }
            });
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Delete Locale will delete specific localized entries by passing the locale codes.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// List<string> locales = new List<string>() { "hi-in", "mr-in", "es" }
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).DeleteMultipleLocalAsync();
        /// </code></pre>
        /// </example>
        /// <param name="locales">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> DeleteMultipleLocalAsync(List<string> locales)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new DeleteService<Dictionary<string, List<string>>>(stack.client.serializer, stack, resourcePath, "entry", new Dictionary<string, List<string>>()
            {
                {"locales", locales }
            });
            return stack.client.InvokeAsync<DeleteService<Dictionary<string, List<string>>>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Localize an entry request allows you to localize an entry i.e., the entry will cease to fetch data from its fallback language and possess independent content specific to the selected locale.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Localize(model, "hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="model">Localized IEntry model.</param>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Localize(IEntry model, string locale)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            ParameterCollection collection = new ParameterCollection();

            collection.Add("locale", locale);

            var service = new LocalizationService<IEntry>(stack.client.serializer, stack, resourcePath, model, "entry", collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Localize an entry request allows you to localize an entry i.e., the entry will cease to fetch data from its fallback language and possess independent content specific to the selected locale.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).LocalizeAsync(model, "hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="model">Localized IEntry model.</param>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> LocalizeAsync(IEntry model, string locale)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            ParameterCollection collection = new ParameterCollection();

            collection.Add("locale", locale);

            var service = new LocalizationService<IEntry>(stack.client.serializer, stack, resourcePath, model, "entry", collection);
            return stack.client.InvokeAsync<LocalizationService<IEntry>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Unlocalize an entry request is used to unlocalize an existing entry. 
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Unlocalize("hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Unlocalize(string locale)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            ParameterCollection collection = new ParameterCollection();

            collection.Add("locale", locale);

            var service = new LocalizationService<IEntry>(stack.client.serializer, stack, resourcePath, null, "entry", collection, true);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Unlocalize an entry request is used to unlocalize an existing entry. 
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).UnlocalizeAsync("hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UnlocalizeAsync(string locale)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            ParameterCollection collection = new ParameterCollection();

            collection.Add("locale", locale);

            var service = new LocalizationService<IEntry>(stack.client.serializer, stack, resourcePath, null, "entry", collection, true);
            return stack.client.InvokeAsync<LocalizationService<IEntry>, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Get languages of an entry call returns the details of all the languages that an entry exists in.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Locales();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Locales()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new LocaleService(stack.client.serializer, stack, resourcePath);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get languages of an entry call returns the details of all the languages that an entry exists in.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).LocalesAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> LocalesAsync()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new LocaleService(stack.client.serializer, stack, resourcePath);
            return stack.client.InvokeAsync<LocaleService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Get references of an entry call returns all the entries of content types that are referenced by a particular entry.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).References();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse References()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchReferencesService(stack.client.serializer, stack, resourcePath);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get references of an entry call returns all the entries of content types that are referenced by a particular entry.
        /// </summary>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).ReferencesAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ReferencesAsync()
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchReferencesService(stack.client.serializer, stack, resourcePath);
            return stack.client.InvokeAsync<FetchReferencesService, ContentstackResponse>(service);
        }
    }
}
