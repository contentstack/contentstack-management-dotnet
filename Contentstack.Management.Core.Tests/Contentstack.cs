using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Contentstack.Management.Core.Tests
{
    public class Contentstack
    {
        private static readonly Lazy<IConfigurationRoot>
        config =
        new Lazy<IConfigurationRoot>(() =>
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        });

        private static readonly Lazy<NetworkCredential> credential =
        new Lazy<NetworkCredential>(() =>
        {
            return Config.GetSection("Contentstack:Credentials").Get<NetworkCredential>();
        });

        private static readonly Lazy<OrganizationModel> organization =
        new Lazy<OrganizationModel>(() =>
        {
            return Config.GetSection("Contentstack:Organization").Get<OrganizationModel>();
        });

        private static readonly Lazy<string> mfaSecret =
        new Lazy<string>(() =>
        {
            return Config.GetSection("Contentstack:MfaSecret").Value;
        });

        public static IConfigurationRoot Config{ get { return config.Value; } }
        public static NetworkCredential Credential { get { return credential.Value; } }
        public static OrganizationModel Organization { get { return organization.Value; } }
        public static string MfaSecret { get { return mfaSecret.Value; } }

        public static StackModel Stack { get; set; }

        // TOTP token tracking to prevent reuse
        private static readonly HashSet<string> _usedTotpTokens = new HashSet<string>();
        private static DateTime _lastTotpGeneration = DateTime.MinValue;
        private static readonly object _totpLock = new object();

        /// <summary>
        /// Checks if the exception indicates TOTP token reuse
        /// </summary>
        public static bool IsTotpReuse(Exception exception)
        {
            if (exception is ContentstackErrorException csException)
            {
                return csException.ErrorMessage?.Contains("Totp has already been Used") == true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the exception indicates an account lockout
        /// </summary>
        public static bool IsAccountLockout(Exception exception)
        {
            if (exception is ContentstackErrorException csException)
            {
                return csException.ErrorCode == 104 && 
                       (csException.ErrorMessage?.Contains("locked") == true ||
                        csException.ErrorMessage?.Contains("temporarily") == true);
            }
            return false;
        }

        /// <summary>
        /// Ensures sufficient time has passed for fresh TOTP token generation
        /// </summary>
        public static void EnsureFreshTotpWindow()
        {
            lock (_totpLock)
            {
                var timeSinceLastTotp = DateTime.UtcNow - _lastTotpGeneration;
                if (timeSinceLastTotp.TotalSeconds < 35)
                {
                    int sleepMs = (int)(35000 - timeSinceLastTotp.TotalMilliseconds);
                    System.Threading.Thread.Sleep(sleepMs);
                }
                
                // Clean up old tokens (older than 2 minutes)
                var cutoff = DateTime.UtcNow.AddMinutes(-2);
                if (_lastTotpGeneration < cutoff)
                {
                    _usedTotpTokens.Clear();
                }
                
                _lastTotpGeneration = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Executes login with retry logic for account lockouts
        /// </summary>
        public static ContentstackResponse LoginWithRetry(ContentstackClient client, int maxRetries = 3, int baseDelayMs = 5000)
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return client.Login(Credential, null, MfaSecret);
                }
                catch (Exception ex) when (IsAccountLockout(ex) && attempt < maxRetries)
                {
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt);
                    System.Threading.Thread.Sleep(delay);
                }
            }
            return client.Login(Credential, null, MfaSecret);
        }

        public static async Task<ContentstackResponse> LoginWithRetryAsync(ContentstackClient client, int maxRetries = 3, int baseDelayMs = 5000)
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await client.LoginAsync(Credential, null, MfaSecret);
                }
                catch (Exception ex) when (IsAccountLockout(ex) && attempt < maxRetries)
                {
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt);
                    await Task.Delay(delay);
                }
            }
            return await client.LoginAsync(Credential, null, MfaSecret);
        }

        public static ContentstackResponse LoginWithTotpRetry(ContentstackClient client, int maxRetries = 3)
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    EnsureFreshTotpWindow();
                    return client.Login(Credential, null, MfaSecret);
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    if (IsTotpReuse(ex))
                        System.Threading.Thread.Sleep(35000);
                    else if (IsAccountLockout(ex))
                    {
                        int delay = 5000 * (int)Math.Pow(2, attempt);
                        System.Threading.Thread.Sleep(delay);
                    }
                    else
                        System.Threading.Thread.Sleep(1000);
                }
            }
            EnsureFreshTotpWindow();
            return client.Login(Credential, null, MfaSecret);
        }

        public static async Task<ContentstackResponse> LoginWithTotpRetryAsync(ContentstackClient client, int maxRetries = 3)
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    EnsureFreshTotpWindow();
                    return await client.LoginAsync(Credential, null, MfaSecret);
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    if (IsTotpReuse(ex))
                        await Task.Delay(35000);
                    else if (IsAccountLockout(ex))
                    {
                        int delay = 5000 * (int)Math.Pow(2, attempt);
                        await Task.Delay(delay);
                    }
                    else
                        await Task.Delay(1000);
                }
            }
            EnsureFreshTotpWindow();
            return await client.LoginAsync(Credential, null, MfaSecret);
        }

        /// <summary>
        /// Creates a new ContentstackClient, logs in via the Login API (never from config),
        /// and returns the authenticated client. Callers are responsible for calling Logout()
        /// when done.
        /// </summary>
        public static ContentstackClient CreateAuthenticatedClient()
        {
            ContentstackClientOptions options = Config.GetSection("Contentstack").Get<ContentstackClientOptions>();
            options.Authtoken = null;
            var handler = new LoggingHttpHandler();
            var httpClient = new HttpClient(handler);
            var client = new ContentstackClient(httpClient, options);
            LoginWithTotpRetry(client);
            return client;
        }

        public static T serialize<T>(JsonSerializerOptions options, string filePath)
        {
            string response = GetResourceText(filePath);
            return JsonSerializer.Deserialize<T>(response, options);
        }

        public static T serializeArray<T>(JsonSerializerOptions options, string filePath)
        {
            string response = GetResourceText(filePath);
            return JsonSerializer.Deserialize<T>(response, options);
        }

        public static string GetResourceText(string resourceName)
        {
            using (StreamReader reader = new StreamReader(GetResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream GetResourceStream(string resourceName)
        {
            Assembly assembly = typeof(Contentstack).Assembly;
            var resource = FindResourceName(resourceName);
            Stream stream = assembly.GetManifestResourceStream(resource);
            return stream;
        }

        public static string FindResourceName(string partialName)
        {
            return FindResourceName(s => s.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0).Single();
        }

        public static IEnumerable<string> FindResourceName(Predicate<string> match)
        {
            Assembly assembly = typeof(Contentstack).Assembly;
            var allResources = assembly.GetManifestResourceNames();
            foreach (var resource in allResources)
            {
                if (match(resource))
                    yield return resource;
            }
        }
    }
}
