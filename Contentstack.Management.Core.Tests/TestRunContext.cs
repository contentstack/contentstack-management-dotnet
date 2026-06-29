using System;

namespace Contentstack.Management.Core.Tests
{
    /// <summary>
    /// Shared in-memory state populated once by [AssemblyInitialize] and read by every test class.
    /// Replaces the file-based stackApiKey.txt handoff.
    /// </summary>
    public static class TestRunContext
    {
        /// <summary>API key of the dynamically created test stack.</summary>
        public static string StackApiKey { get; internal set; }

        /// <summary>True when the stack was created by AssemblyInitialize (not read from file).</summary>
        public static bool StackCreatedDynamically { get; internal set; }

        /// <summary>Organisation UID used to create the test stack.</summary>
        public static string OrganizationUid { get; internal set; }

        /// <summary>
        /// When false, the test stack and other dynamic resources survive the test run so you
        /// can inspect them. Driven by Contentstack:DeleteDynamicResources in appsettings.json.
        /// </summary>
        public static bool DeleteDynamicResources { get; internal set; } = true;
    }
}
