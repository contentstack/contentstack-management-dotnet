using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Contentstack.Management.Core.Exceptions;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Management.Core.Tests
{
    /// <summary>Holds OAuth credentials from appsettings.json Contentstack:OAuth section.</summary>
    public class OAuthConfig
    {
        public string ClientId { get; set; }
        public string AppId { get; set; }
        public string RedirectUri { get; set; }
    }

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

        // ── New optional config keys ─────────────────────────────────────────

        private static readonly Lazy<string> memberEmail =
            new Lazy<string>(() => Config.GetSection("Contentstack:MemberEmail").Value);

        private static readonly Lazy<string> tfaEmail =
            new Lazy<string>(() => Config.GetSection("Contentstack:TfaEmail").Value);

        private static readonly Lazy<string> tfaPassword =
            new Lazy<string>(() => Config.GetSection("Contentstack:TfaPassword").Value);

        private static readonly Lazy<OAuthConfig> oAuthConfig =
            new Lazy<OAuthConfig>(() =>
                Config.GetSection("Contentstack:OAuth").Get<OAuthConfig>() ?? new OAuthConfig());

        private static readonly Lazy<string> personalizeHost =
            new Lazy<string>(() =>
                Config.GetSection("Contentstack:PersonalizeHost").Value
                ?? "personalize-api.contentstack.com");

        private static readonly Lazy<bool> deleteDynamicResources =
            new Lazy<bool>(() =>
                !string.Equals(
                    Config.GetSection("Contentstack:DeleteDynamicResources").Value,
                    "false", StringComparison.OrdinalIgnoreCase));

        private static readonly Lazy<bool> damV2Enabled =
            new Lazy<bool>(() =>
                string.Equals(
                    Config.GetSection("Contentstack:DamV2Enabled").Value,
                    "true", StringComparison.OrdinalIgnoreCase));

        private static readonly Lazy<string> amOrgUid =
            new Lazy<string>(() => Config.GetSection("Contentstack:AmOrgUid").Value);

        // ── Public accessors ─────────────────────────────────────────────────

        public static IConfigurationRoot Config { get { return config.Value; } }
        public static NetworkCredential Credential { get { return credential.Value; } }
        public static OrganizationModel Organization { get { return organization.Value; } }
        public static string MfaSecret { get { return mfaSecret.Value; } }

        /// <summary>Secondary user email for team / stack-sharing tests.</summary>
        public static string MemberEmail => memberEmail.Value;

        /// <summary>Email of a 2FA-enabled account for testing the TFA login flow.</summary>
        public static string TfaEmail => tfaEmail.Value;

        /// <summary>Password matching TfaEmail.</summary>
        public static string TfaPassword => tfaPassword.Value;

        /// <summary>OAuth app credentials (ClientId, AppId, RedirectUri).</summary>
        public static OAuthConfig OAuth => oAuthConfig.Value;

        /// <summary>Personalize API host; defaults to personalize-api.contentstack.com.</summary>
        public static string PersonalizeHost => personalizeHost.Value;

        /// <summary>
        /// When true (default) the dynamically created test stack is deleted after the run.
        /// Set Contentstack:DeleteDynamicResources=false in appsettings.json to preserve it.
        /// </summary>
        public static bool DeleteDynamicResources => deleteDynamicResources.Value;

        /// <summary>Enables DAM 2.0 / asset-scan-status tests.</summary>
        public static bool DamV2Enabled => damV2Enabled.Value;

        /// <summary>Org UID for AM (Advanced Managed) org tests.</summary>
        public static string AmOrgUid => amOrgUid.Value;

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
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt); // Exponential backoff
                    System.Threading.Thread.Sleep(delay);
                }
            }
            // Final attempt without catching lockout
            return client.Login(Credential, null, MfaSecret);
        }

        /// <summary>
        /// Executes async login with retry logic for account lockouts
        /// </summary>
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
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt); // Exponential backoff
                    await Task.Delay(delay);
                }
            }
            // Final attempt without catching lockout
            return await client.LoginAsync(Credential, null, MfaSecret);
        }

        /// <summary>
        /// Executes login with TOTP-aware retry logic for token reuse and account lockouts
        /// </summary>
        public static ContentstackResponse LoginWithTotpRetry(ContentstackClient client, int maxRetries = 3)
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Ensure fresh TOTP window before each attempt
                    EnsureFreshTotpWindow();
                    return client.Login(Credential, null, MfaSecret);
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    if (IsTotpReuse(ex))
                    {
                        // Wait for fresh TOTP window (35+ seconds)
                        System.Threading.Thread.Sleep(35000);
                    }
                    else if (IsAccountLockout(ex))
                    {
                        // Exponential backoff for account lockout
                        int delay = 5000 * (int)Math.Pow(2, attempt);
                        System.Threading.Thread.Sleep(delay);
                    }
                    else
                    {
                        // For other errors, short delay before retry
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
            
            // Final attempt without catching errors
            EnsureFreshTotpWindow();
            return client.Login(Credential, null, MfaSecret);
        }

        /// <summary>
        /// Executes async login with TOTP-aware retry logic for token reuse and account lockouts
        /// </summary>
        public static async Task<ContentstackResponse> LoginWithTotpRetryAsync(ContentstackClient client, int maxRetries = 3)
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Ensure fresh TOTP window before each attempt
                    EnsureFreshTotpWindow();
                    return await client.LoginAsync(Credential, null, MfaSecret);
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    if (IsTotpReuse(ex))
                    {
                        // Wait for fresh TOTP window (35+ seconds)
                        await Task.Delay(35000);
                    }
                    else if (IsAccountLockout(ex))
                    {
                        // Exponential backoff for account lockout
                        int delay = 5000 * (int)Math.Pow(2, attempt);
                        await Task.Delay(delay);
                    }
                    else
                    {
                        // For other errors, short delay before retry
                        await Task.Delay(1000);
                    }
                }
            }
            
            // Final attempt without catching errors
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

        public static T serialize<T>(JsonSerializer serializer, string filePath)
        {
            string response = GetResourceText(filePath);
            JObject jObject = JObject.Parse(response);
            return jObject.ToObject<T>(serializer);
        }
        public static T serializeArray<T>(JsonSerializer serializer, string filePath)
        {
            string response = GetResourceText(filePath);
            JArray jObject = JArray.Parse(response);
            return jObject.ToObject<T>(serializer);
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
