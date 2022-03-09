using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Models
{
    public class Entry: BaseModel<IEntry>
    {
        internal Entry(Stack stack, string contentTyppe, string uid)
            : base(stack, "entry", uid)
        {
            resourcePath = uid == null ? $"/content_types/{contentTyppe}/entries" : $"/content_types/{contentTyppe}/entries/{uid}";
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
        /// The Version on Entry will allow to fetch all version, delete specific version or naming the asset version.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry().Version().GetAll();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/></returns>
        public Version Version(int? versionNumber = null)
        {
            ThrowIfUidEmpty();
            return new Version(stack, resourcePath, "entry", versionNumber);
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
        /// <param name="model">IEntry for createing Entry.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
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
        /// <param name="model">IEntry for createing Entry.</param>
        /// <returns>The Task.</returns>
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
        /// <param name="model">IEntry for updating entry.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
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
        /// <param name="model">IEntry for updating entry.</param>
        /// <returns>The Task.</returns>
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
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// List&lt;string&gt; locales = new List&lt;string&gt;(); 
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).DeleteMultipleLocal(locales);
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
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// List&lt;string&gt; locales = new List&lt;string&gt;(); 
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).DeleteMultipleLocalAsync(locales);
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
        /// <example>
        /// <pre><code>
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
        /// <example>
        /// <pre><code>
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
        /// <example>
        /// <pre><code>
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
        /// <example>
        /// <pre><code>
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
        /// <example>
        /// <pre><code>
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
        /// <example>
        /// <pre><code>
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
        /// <example>
        /// <pre><code>
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
        /// <example>
        /// <pre><code>
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

        /// <summary>
        /// The Publish an entry request lets you publish an entry either immediately or schedule it for a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Publish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Publish(PublishUnpublishDetails details, string locale = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "entry", locale);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Publish an entry request lets you publish an entry either immediately or schedule it for a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).PublishAsync(new PublishUnpublishDetails(), "en-us");
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The Task</returns>
        public virtual Task<ContentstackResponse> PublishAsync(PublishUnpublishDetails details, string locale = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "entry", locale);
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Unpublish an entry call will unpublish an entry at once, and also, gives you the provision to unpublish an entry automatically at a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Unpublish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Unpublish(PublishUnpublishDetails details, string locale = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "entry", locale);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Unpublish an entry call will unpublish an entry at once, and also, gives you the provision to unpublish an entry automatically at a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).UnpublishAsync(new PublishUnpublishDetails(), "en-us");
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The Task</returns>
        public virtual Task<ContentstackResponse> UnpublishAsync(PublishUnpublishDetails details, string locale = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "entry", locale);
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Import an entry call is used to import an entry.
        /// To import an entry, you need to upload a JSON file that has entry data in the format that fits the schema of the content type it is being imported to.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Import(&quot;PATH/TO/FILE&quot;);
        /// </code></pre>
        /// </example>
        /// <param name="filePath">Path to file you want to import</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Import(string filePath, ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();

            var text = File.ReadAllText(filePath);
            var service = new ImportExportService(stack.client.serializer, stack, resourcePath, true, "POST", collection);
            service.ByteContent = System.Text.Encoding.UTF8.GetBytes(text);

            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Import an entry call is used to import an entry.
        /// To import an entry, you need to upload a JSON file that has entry data in the format that fits the schema of the content type it is being imported to.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).ImportAsync(&quot;PATH/TO/FILE&quot;);
        /// </code></pre>
        /// </example>
        /// <param name="filePath">Path to file you want to import</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ImportAsync(string filePath, ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var text = File.ReadAllText(filePath);
            var service = new ImportExportService(stack.client.serializer, stack, resourcePath, isImport: true, "POST", collection);
            service.ByteContent = System.Text.Encoding.UTF8.GetBytes(text);
            return stack.client.InvokeAsync<ImportExportService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Export an entry call is used to export an entry. The exported entry data is saved in a downloadable JSON file/
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).ContentType(&quot;&lt;CONTENT_TYPE_UID&gt;&quot;).Entry(&quot;&lt;ENTRY_UID&gt;&quot;).Import(&quot;PATH/TO/FILE&quot;);
        /// </code></pre>
        /// </example>
        /// <param name="filePath">Path to file you want to export entry.</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Export(string filePath, ParameterCollection collection = null)
        {
            stack.client.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            try
            {
                var service = new ImportExportService(stack.client.serializer, stack, resourcePath, collection: collection);
                ContentstackResponse response = stack.client.InvokeSync(service);
                if (response.IsSuccessStatusCode)
                {
                    using (StreamWriter file = File.CreateText(filePath))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        JObject json = response.OpenJObjectResponse();
                        json.WriteTo(writer);
                    }
                }
                return response;
            } catch
            {
                throw;
            }
        }
    }
}
