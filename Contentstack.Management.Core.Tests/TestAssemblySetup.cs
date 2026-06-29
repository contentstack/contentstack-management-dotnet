using System;
using System.Net.Http;
using System.Threading.Tasks;
using Contentstack.Management.Core;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Tests.Helpers;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests
{
    /// <summary>
    /// Runs once per test assembly.
    /// [AssemblyInitialize] creates a fresh test stack and stores its API key in TestRunContext
    /// so every test class can use it without reading from a file.
    /// [AssemblyCleanup] logs the stack for manual teardown (SDK does not expose Stack.Delete).
    /// </summary>
    [TestClass]
    public class TestAssemblySetup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Console.WriteLine("[AssemblyInit] Initialising integration test run...");

            // ── Config ─────────────────────────────────────────────────────────
            var orgUid = Contentstack.Organization?.Uid;
            if (string.IsNullOrWhiteSpace(orgUid))
                throw new InvalidOperationException(
                    "[AssemblyInit] Contentstack:Organization:Uid is missing in appsettings.json. " +
                    "Cannot create a test stack without an organisation UID.");

            TestRunContext.OrganizationUid = orgUid;
            TestRunContext.DeleteDynamicResources = Contentstack.DeleteDynamicResources;

            // ── Login ───────────────────────────────────────────────────────────
            Console.WriteLine("[AssemblyInit] Logging in...");
            var httpClient = new System.Net.Http.HttpClient(new LoggingHttpHandler());
            var options = Contentstack.Config
                .GetSection("Contentstack")
                .Get<ContentstackClientOptions>();
            options.Authtoken = null;

            var client = new ContentstackClient(httpClient, options);
            Contentstack.LoginWithTotpRetry(client);
            Console.WriteLine("[AssemblyInit] Login successful.");

            // ── Create test stack ───────────────────────────────────────────────
            var stackName = $"dotnet_sdk_test_{DateTime.UtcNow:yyyyMMddHHmmss}";
            Console.WriteLine($"[AssemblyInit] Creating test stack '{stackName}' in org '{orgUid}'...");

            try
            {
                ContentstackResponse createResp = client.Stack()
                    .Create(stackName, "en-us", orgUid,
                            $"Automated integration test stack – created {DateTime.UtcNow:O}");

                StackResponse model = createResp.OpenTResponse<StackResponse>();

                if (string.IsNullOrWhiteSpace(model?.Stack?.APIKey))
                    throw new InvalidOperationException(
                        "[AssemblyInit] Stack created but response contained no api_key.");

                TestRunContext.StackApiKey = model.Stack.APIKey;
                TestRunContext.StackCreatedDynamically = true;
                Contentstack.Stack = model.Stack;

                // Persist for legacy tests still using StackResponse.getStack(file)
                System.IO.File.WriteAllText("./stackApiKey.txt", createResp.OpenResponse());

                Console.WriteLine($"[AssemblyInit] Test stack created: {model.Stack.APIKey}");

                // Brief pause to let the stack provision fully on the server
                System.Threading.Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"[AssemblyInit] Failed to create test stack: {ex.Message}", ex);
            }
            finally
            {
                // Release the bootstrap client – each test class creates its own
                try { client.Logout(); } catch { }
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (!TestRunContext.StackCreatedDynamically)
                return;

            if (TestRunContext.DeleteDynamicResources)
            {
                // The .NET CMA SDK does not expose a Stack.Delete() method at the
                // Stack model level. Log the key so CI pipelines / developers can
                // clean up manually or via the Contentstack UI.
                Console.WriteLine(
                    $"[AssemblyCleanup] Test stack '{TestRunContext.StackApiKey}' was created " +
                    "dynamically but cannot be auto-deleted (SDK does not expose Stack.Delete). " +
                    "Delete it manually via the Contentstack UI or Management API.");
            }
            else
            {
                Console.WriteLine(
                    $"[AssemblyCleanup] DeleteDynamicResources=false. " +
                    $"Stack '{TestRunContext.StackApiKey}' preserved for debugging.");
            }
        }
    }
}
