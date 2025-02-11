
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Contentstack.Management.Core;
using System.Net.Http;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds Contentstack services to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="configuration">The ContentstackClientOptions used to retrieve configuration from.</param>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection AddContentstackClien(this IServiceCollection services, ContentstackClientOptions configuration)
        {

            return services;
        }

        /// <summary>
        /// Adds Contentstack services to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="configuration">The IConfiguration used to retrieve configuration from.</param>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection TryAddContentstackClient(this IServiceCollection services, ContentstackClientOptions configuration)
        {

            return services;
        }

        public static IServiceCollection AddContentstackClient(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            services.AddHttpClient<ContentstackClient>(configureClient);

            return services;
        }
    }
}