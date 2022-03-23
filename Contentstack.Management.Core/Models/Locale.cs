using System;
using System.Threading.Tasks;
using Contentstack.Management.Core.Abstractions;
using Contentstack.Management.Core.Queryable;

namespace Contentstack.Management.Core.Models
{
    public class Locale: BaseModel<ILocale>
    {
        internal Locale(Stack stack, string code = null)
            : base(stack, "locale", code)
        {
            resourcePath = code == null ? $"/locales" : $"/locales/{code}";
        }

        /// <summary>
        /// The Query on locale allow to get the list of all languages (along with the language codes) available for a stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale().Query().Find();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="Query"/></returns>
        public Query Query()
        {
            ThrowIfUidNotEmpty();
            return new Query(stack, resourcePath);
        }

        /// <summary>
        /// This call lets you add a new language to your stack. You can either add a supported language or a custom language of your choice.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// LocaleModel model = new LocaleModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale().Create(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ILocale for createing Locale.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Create(ILocale model, ParameterCollection collection = null)
        {
            return base.Create(model, collection);
        }

        /// <summary>
        /// This call lets you add a new language to your stack. You can either add a supported language or a custom language of your choice.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// LocaleModel model = new LocaleModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale().CreateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ILocale for createing Locale.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> CreateAsync(ILocale model, ParameterCollection collection = null)
        {
            return base.CreateAsync(model);
        }

        /// <summary>
        /// The Update language call will let you update the details (such as display name) and the fallback language of an existing language of your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// LocaleModel model = new LocaleModel(); // Add field values
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale().Update(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ILocale for updating locale.</param>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Update(ILocale model, ParameterCollection collection = null)
        {
            return base.Update(model, collection);
        }

        /// <summary>
        /// The Update language call will let you update the details (such as display name) and the fallback language of an existing language of your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// LocaleModel model = new LocaleModel(); // Add field values
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale().UpdateAsync(model);
        /// </code></pre>
        /// </example>
        /// <param name="model">ILocale for updating locale.</param>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> UpdateAsync(ILocale model, ParameterCollection collection = null)
        {
            return base.UpdateAsync(model, collection);
        }

        /// <summary>
        /// The Get a language call returns information about a specific language available on the stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale(&quot;&lt;LOCALE_CODE&gt;&quot;).Fetch();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Fetch(ParameterCollection collection = null)
        {
            return base.Fetch(collection);
        }

        /// <summary>
        /// The Get a language call returns information about a specific language available on the stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale(&quot;&lt;LOCALE_CODE&gt;&quot;).FetchAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> FetchAsync(ParameterCollection collection = null)
        {
            return base.FetchAsync(collection);
        }

        /// <summary>
        /// The Delete language call deletes an existing language from your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale(&quot;&lt;LOCALE_CODE&gt;&quot;).Delete();
        /// </code></pre>
        /// </example>
        /// <returns>The <see cref="ContentstackResponse"/>.</returns>
        public override ContentstackResponse Delete(ParameterCollection collection = null)
        {
            return base.Delete(collection);
        }

        /// <summary>
        /// The Delete language call deletes an existing language from your stack.
        /// </summary>
        /// <example>
        /// <pre><code>
        /// ContentstackClient client = new ContentstackClient(&quot;&lt;AUTHTOKEN&gt;&quot;, &quot;&lt;API_HOST&gt;&quot;);
        /// ContentstackResponse contentstackResponse = await client.Stack(&quot;&lt;API_KEY&gt;&quot;).Locale(&quot;&lt;LOCALE_CODE&gt;&quot;).DeleteAsync();
        /// </code></pre>
        /// </example>
        /// <returns>The Task.</returns>
        public override Task<ContentstackResponse> DeleteAsync(ParameterCollection collection = null)
        {
            return base.DeleteAsync(collection);
        }
    }
}
