using System;

namespace Contentstack.Management.Core.Utils
{
    public static class CSConstants
    {
        #region Internal Constants
        internal const long ContentBufferSize = 1024 * 1024 * 1024;
        internal static TimeSpan Timeout = TimeSpan.FromSeconds(30);
        internal const string Slash = "/";
        internal const char SlashChar = '/';
        #endregion

        #region Internal Message
        internal const string YouAreLoggedIn = "You are already logged in.";
        internal const string YouAreNotLoggedIn = "You are need to login.";
        #endregion
    }
}
