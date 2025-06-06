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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry().Query().Find();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry().Version().GetAll();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry().Create(model);
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">IEntry for createing Entry.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(IEntry model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model, collection);
        }

        /// <summary>
        /// The Update Entry call is used to update the schema of an existing content type.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Update(model);
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").UpdateAsync(model);
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Fetch();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").FetchAsync();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Delete();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").DeleteAsync();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// List<string> locales = new List<string>(); 
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").DeleteMultipleLocal(locales);
        /// </code></pre>
        /// </example>
        /// <param name="locales">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse DeleteMultipleLocal(List<string> locales)
        {
            stack.ThrowIfNotLoggedIn();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// List<string> locales = new List<string>(); 
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").DeleteMultipleLocalAsync(locales);
        /// </code></pre>
        /// </example>
        /// <param name="locales">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> DeleteMultipleLocalAsync(List<string> locales)
        {
            stack.ThrowIfNotLoggedIn();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Localize(model, "hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="model">Localized IEntry model.</param>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Localize(IEntry model, string locale)
        {
            stack.ThrowIfNotLoggedIn();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryModel model = new EntryModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").LocalizeAsync(model, "hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="model">Localized IEntry model.</param>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> LocalizeAsync(IEntry model, string locale)
        {
            stack.ThrowIfNotLoggedIn();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Unlocalize("hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Unlocalize(string locale)
        {
            stack.ThrowIfNotLoggedIn();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").UnlocalizeAsync("hi-in");
        /// </code></pre>
        /// </example>
        /// <param name="locale">Enter the code of the language to unlocalize the entry of that particular language.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> UnlocalizeAsync(string locale)
        {
            stack.ThrowIfNotLoggedIn();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Locales();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Locales()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new LocaleService(stack.client.serializer, stack, resourcePath);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get languages of an entry call returns the details of all the languages that an entry exists in.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").LocalesAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> LocalesAsync()
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new LocaleService(stack.client.serializer, stack, resourcePath);
            return stack.client.InvokeAsync<LocaleService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Get references of an entry call returns all the entries of content types that are referenced by a particular entry.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").References();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse References(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchReferencesService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Get references of an entry call returns all the entries of content types that are referenced by a particular entry.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").ReferencesAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ReferencesAsync(ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new FetchReferencesService(stack.client.serializer, stack, resourcePath, collection: collection);
            return stack.client.InvokeAsync<FetchReferencesService, ContentstackResponse>(service);
        }

        /// <summary>
        /// The Publish an entry request lets you publish an entry either immediately or schedule it for a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Publish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Publish(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "entry", locale);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Publish an entry request lets you publish an entry either immediately or schedule it for a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").PublishAsync(new PublishUnpublishDetails(), "en-us");
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The Task</returns>
        public virtual Task<ContentstackResponse> PublishAsync(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/publish", "entry", locale);
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service, apiVersion: apiVersion);
        }

        /// <summary>
        /// The Unpublish an entry call will unpublish an entry at once, and also, gives you the provision to unpublish an entry automatically at a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Unpublish(new PublishUnpublishDetails());
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public virtual ContentstackResponse Unpublish(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "entry", locale);
            return stack.client.InvokeSync(service, apiVersion: apiVersion);
        }

        /// <summary>
        /// The Unpublish an entry call will unpublish an entry at once, and also, gives you the provision to unpublish an entry automatically at a later date/time.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").UnpublishAsync(new PublishUnpublishDetails(), "en-us");
        /// </code></pre>
        /// </example>
        /// <param name="details">Publish/Unpublish details.</param>
        /// <param name="locale">Locale for entry to be publish</param>
        /// <returns>The Task</returns>
        public virtual Task<ContentstackResponse> UnpublishAsync(PublishUnpublishDetails details, string locale = null, string apiVersion = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();

            var service = new PublishUnpublishService(stack.client.serializer, stack, details, $"{resourcePath}/unpublish", "entry", locale);
            return stack.client.InvokeAsync<PublishUnpublishService, ContentstackResponse>(service, apiVersion: apiVersion);
        }

        /// <summary>
        /// The Import an entry call is used to import an entry.
        /// To import an entry, you need to upload a JSON file that has entry data in the format that fits the schema of the content type it is being imported to.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Import("PATH/TO/FILE");
        /// </code></pre>
        /// </example>
        /// <param name="filePath">Path to file you want to import</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Import(string filePath, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();

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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").ImportAsync("PATH/TO/FILE");
        /// </code></pre>
        /// </example>
        /// <param name="filePath">Path to file you want to import</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The Task</returns>
        public Task<ContentstackResponse> ImportAsync(string filePath, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
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
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").Export("PATH/TO/FILE");
        /// </code></pre>
        /// </example>
        /// <param name="filePath">Path to file you want to export entry.</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse Export(string filePath, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
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
            } catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// The Set Entry Workflow Stage request allows you to either set a particular workflow stage of an entry or update the workflow stage details of an entry.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryWorkflowStage model = new EntryWorkflowStage();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").SetWorkflow(model);
        /// </code></pre>
        /// </example>
        /// <param name="model"><see cref="EntryWorkflowStage"/> object.</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public ContentstackResponse SetWorkflow(EntryWorkflowStage model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            Dictionary<string, EntryWorkflowStage> dict = new Dictionary<string, EntryWorkflowStage>()
            {
                { "workflow_stage", model}
            };
            var service = new CreateUpdateService<Dictionary<string, EntryWorkflowStage>>(stack.client.serializer, stack, $"{resourcePath}/workflow", dict, "workflow", collection: collection);
            return stack.client.InvokeSync(service);
        }

        /// <summary>
        /// The Set Entry Workflow Stage request allows you to either set a particular workflow stage of an entry or update the workflow stage details of an entry.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryWorkflowStage model = new EntryWorkflowStage();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").SetWorkflowAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model"><see cref="EntryWorkflowStage"/> object.</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns>The Task.</returns>
        public Task<ContentstackResponse> SetWorkflowAsync(EntryWorkflowStage model, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            Dictionary<string, EntryWorkflowStage> dict = new Dictionary<string, EntryWorkflowStage>()
            {
                { "workflow_stage", model}
            };
            var service = new CreateUpdateService<Dictionary<string, EntryWorkflowStage>>(stack.client.serializer, stack, $"{resourcePath}/workflow", dict, "workflow", collection: collection);
            return stack.client.InvokeAsync<CreateUpdateService<Dictionary<string, EntryWorkflowStage>>, ContentstackResponse>(service);
        }

        /// <summary>
        /// This multipurpose request allows you to either send a publish request or accept/reject a received publish request.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryPublishAction model = new EntryPublishAction();
        /// ContentstackResponse contentstackResponse = client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").PublishRequest(model);
        /// </code></pre>
        /// </example>
        /// <param name="publishAction"><see cref="EntryPublishAction"/> object.</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns></returns>
        public ContentstackResponse PublishRequest(EntryPublishAction publishAction, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            Dictionary<string, EntryPublishAction> dict = new Dictionary<string, EntryPublishAction>()
            {
                { "publishing_rule", publishAction}
            };
            var service = new CreateUpdateService<Dictionary<string, EntryPublishAction>>(stack.client.serializer, stack, $"{resourcePath}/workflow", dict, "workflow", collection: collection);
            return stack.client.InvokeSync(service);
        }
        /// <summary>
        /// This multipurpose request allows you to either send a publish request or accept/reject a received publish request.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient("<AUTHTOKEN>", "<API_HOST>");
        /// EntryPublishAction model = new EntryPublishAction();
        /// ContentstackResponse contentstackResponse = await client.Stack("<API_KEY>").ContentType("<CONTENT_TYPE_UID>").Entry("<ENTRY_UID>").PublishRequestAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="publishAction"><see cref="EntryPublishAction"/> object.</param>
        /// <param name="collection">Query parameter.</param>
        /// <returns></returns>
        public Task<ContentstackResponse> PublishRequestAsync(EntryPublishAction publishAction, ParameterCollection collection = null)
        {
            stack.ThrowIfNotLoggedIn();
            ThrowIfUidEmpty();
            Dictionary<string, EntryPublishAction> dict = new Dictionary<string, EntryPublishAction>()
            {
                { "publishing_rule", publishAction}
            };
            var service = new CreateUpdateService<Dictionary<string, EntryPublishAction>>(stack.client.serializer, stack, $"{resourcePath}/workflow", dict, "workflow", collection: collection);
            return stack.client.InvokeAsync<CreateUpdateService<Dictionary<string, EntryPublishAction>>, ContentstackResponse>(service);
        }
    }
}
