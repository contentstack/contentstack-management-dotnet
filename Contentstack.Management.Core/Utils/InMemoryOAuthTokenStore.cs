using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Contentstack.Management.Core.Models;

namespace Contentstack.Management.Core.Utils
{
    /// <summary>
    /// Thread-safe in-memory storage for OAuth tokens that can be accessed across SDK instances.
    /// This enables sharing OAuth tokens between the Management SDK and other SDKs like Model Generator.
    /// </summary>
    public static class InMemoryOAuthTokenStore
    {
        private static readonly ConcurrentDictionary<string, OAuthTokens> _tokens = new ConcurrentDictionary<string, OAuthTokens>();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _refreshLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        /// <summary>
        /// Gets OAuth tokens for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <returns>The OAuth tokens if found, null otherwise.</returns>
        public static OAuthTokens GetTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                return null;

            _tokens.TryGetValue(clientId, out var tokens);
            return tokens;
        }

        /// <summary>
        /// Stores OAuth tokens for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <param name="tokens">The OAuth tokens to store.</param>
        public static void SetTokens(string clientId, OAuthTokens tokens)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            _tokens.AddOrUpdate(clientId, tokens, (key, oldValue) => tokens);
        }

        /// <summary>
        /// Removes OAuth tokens for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        public static void ClearTokens(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                return;

            _tokens.TryRemove(clientId, out _);
            
            // Clean up refresh lock
            if (_refreshLocks.TryRemove(clientId, out var semaphore))
            {
                semaphore?.Dispose();
            }
        }

        /// <summary>
        /// Gets or creates a semaphore for token refresh operations to prevent race conditions.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <returns>A semaphore for coordinating refresh operations.</returns>
        public static SemaphoreSlim GetRefreshLock(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

            return _refreshLocks.GetOrAdd(clientId, _ => new SemaphoreSlim(1, 1));
        }

        /// <summary>
        /// Checks if valid OAuth tokens exist for the specified client ID.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <returns>True if valid tokens exist, false otherwise.</returns>
        public static bool HasValidTokens(string clientId)
        {
            var tokens = GetTokens(clientId);
            return tokens?.IsValid == true;
        }

        /// <summary>
        /// Checks if OAuth tokens exist for the specified client ID (regardless of validity).
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <returns>True if tokens exist, false otherwise.</returns>
        public static bool HasTokens(string clientId)
        {
            return !string.IsNullOrEmpty(clientId) && _tokens.ContainsKey(clientId);
        }

        /// <summary>
        /// Gets the number of stored token sets.
        /// </summary>
        /// <returns>The number of stored token sets.</returns>
        public static int TokenCount => _tokens.Count;

        /// <summary>
        /// Clears all stored OAuth tokens and disposes of all refresh locks.
        /// </summary>
        public static void ClearAllTokens()
        {
            _tokens.Clear();
            
            // Dispose all semaphores
            foreach (var kvp in _refreshLocks)
            {
                kvp.Value?.Dispose();
            }
            _refreshLocks.Clear();
        }

        /// <summary>
        /// Gets a snapshot of all stored client IDs.
        /// </summary>
        /// <returns>An array of all stored client IDs.</returns>
        public static string[] GetAllClientIds()
        {
            var keys = new string[_tokens.Count];
            _tokens.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// Waits for a refresh lock to be available and returns a disposable lock.
        /// </summary>
        /// <param name="clientId">The OAuth client ID.</param>
        /// <returns>A task that resolves to a disposable refresh lock.</returns>
        public static async Task<IDisposable> WaitForRefreshLockAsync(string clientId)
        {
            var semaphore = GetRefreshLock(clientId);
            await semaphore.WaitAsync();
            return new RefreshLock(semaphore);
        }

        /// <summary>
        /// Disposable wrapper for refresh locks to ensure proper cleanup.
        /// </summary>
        private sealed class RefreshLock : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private bool _disposed = false;

            public RefreshLock(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _semaphore?.Release();
                    _disposed = true;
                }
            }
        }
    }
}
