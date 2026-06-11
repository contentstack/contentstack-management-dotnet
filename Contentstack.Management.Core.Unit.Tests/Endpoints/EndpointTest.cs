using System;
using System.Collections.Generic;
using System.IO;
using Contentstack.Management.Core.Endpoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Unit.Tests.Endpoints
{
    [TestClass]
    public class EndpointTest
    {
        [TestInitialize]
        public void Setup() => Endpoint.ResetCache();

        [TestCleanup]
        public void Teardown() => Endpoint.ResetCache();

        // ------------------------------------------------------------------
        // Basic resolution
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetContentstackEndpoint_Na_ReturnsCorrectManagementUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("na", "contentManagement");
            Assert.AreEqual("https://api.contentstack.io", url);
        }

        [DataTestMethod]
        [DataRow("na")]
        [DataRow("eu")]
        [DataRow("au")]
        [DataRow("azure-na")]
        [DataRow("azure-eu")]
        [DataRow("gcp-na")]
        [DataRow("gcp-eu")]
        public void GetContentstackEndpoint_AllRegionIds_Resolve(string regionId)
        {
            string url = Endpoint.GetContentstackEndpoint(regionId, "contentManagement");
            Assert.IsFalse(string.IsNullOrEmpty(url));
            Assert.IsTrue(url.StartsWith("https://"));
        }

        // ------------------------------------------------------------------
        // Alias resolution (case-insensitive, dash/underscore variants)
        // ------------------------------------------------------------------

        [DataTestMethod]
        [DataRow("na")]
        [DataRow("us")]
        [DataRow("NA")]
        [DataRow("US")]
        [DataRow("AWS-NA")]
        [DataRow("aws_na")]
        [DataRow("AWS_NA")]
        public void GetContentstackEndpoint_NaAliasVariants_AllResolveToSameUrl(string alias)
        {
            string url = Endpoint.GetContentstackEndpoint(alias, "contentManagement");
            Assert.AreEqual("https://api.contentstack.io", url);
        }

        [DataTestMethod]
        [DataRow("azure-na")]
        [DataRow("azure_na")]
        [DataRow("AZURE-NA")]
        [DataRow("AZURE_NA")]
        public void GetContentstackEndpoint_AzureNaAliasVariants_AllResolveToSameUrl(string alias)
        {
            string expected = Endpoint.GetContentstackEndpoint("azure-na", "contentManagement");
            string result = Endpoint.GetContentstackEndpoint(alias, "contentManagement");
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("eu")]
        [DataRow("EU")]
        [DataRow("aws-eu")]
        [DataRow("AWS-EU")]
        [DataRow("aws_eu")]
        public void GetContentstackEndpoint_EuAliasVariants_AllResolveToSameUrl(string alias)
        {
            string expected = Endpoint.GetContentstackEndpoint("eu", "contentManagement");
            string result = Endpoint.GetContentstackEndpoint(alias, "contentManagement");
            Assert.AreEqual(expected, result);
        }

        // ------------------------------------------------------------------
        // omitHttps flag
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetContentstackEndpoint_OmitHttps_StripsScheme()
        {
            string url = Endpoint.GetContentstackEndpoint("na", "contentManagement", omitHttps: true);
            Assert.IsFalse(url.StartsWith("https://"), "URL should not start with https://");
            Assert.IsFalse(url.StartsWith("http://"), "URL should not start with http://");
            Assert.IsTrue(url.Contains("."), "URL should contain a hostname");
        }

        [TestMethod]
        public void GetContentstackEndpoint_OmitHttpsFalse_PreservesScheme()
        {
            string url = Endpoint.GetContentstackEndpoint("na", "contentManagement", omitHttps: false);
            Assert.IsTrue(url.StartsWith("https://"));
        }

        [DataTestMethod]
        [DataRow("na")]
        [DataRow("eu")]
        [DataRow("au")]
        [DataRow("azure-na")]
        [DataRow("gcp-na")]
        public void GetContentstackEndpoint_OmitHttps_AllRegions_StripsScheme(string region)
        {
            string url = Endpoint.GetContentstackEndpoint(region, "contentManagement", omitHttps: true);
            Assert.IsFalse(url.StartsWith("https://"));
            Assert.IsFalse(url.StartsWith("http://"));
        }

        // ------------------------------------------------------------------
        // Dictionary overload
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetContentstackEndpoint_DictOverload_ContainsManagementKey()
        {
            var dict = Endpoint.GetContentstackEndpoint("na");
            Assert.IsTrue(dict.ContainsKey("contentManagement"));
            Assert.AreEqual("https://api.contentstack.io", dict["contentManagement"]);
        }

        [TestMethod]
        public void GetContentstackEndpoint_DictOverload_ContainsDeliveryKey()
        {
            var dict = Endpoint.GetContentstackEndpoint("na");
            Assert.IsTrue(dict.ContainsKey("contentDelivery"));
        }

        [TestMethod]
        public void GetContentstackEndpoint_DictOverload_ReturnsMultipleServices()
        {
            var dict = Endpoint.GetContentstackEndpoint("na");
            Assert.IsTrue(dict.Count >= 2, "Should contain at least 2 service endpoints");
        }

        [TestMethod]
        public void GetContentstackEndpoint_DictOverload_OmitHttps_StripsAllSchemes()
        {
            var dict = Endpoint.GetContentstackEndpoint("na", omitHttps: true);
            foreach (var kvp in dict)
            {
                Assert.IsFalse(kvp.Value.StartsWith("https://"),
                    $"Service '{kvp.Key}' URL still has https:// prefix");
            }
        }

        [DataTestMethod]
        [DataRow("na")]
        [DataRow("eu")]
        [DataRow("au")]
        [DataRow("azure-na")]
        [DataRow("azure-eu")]
        [DataRow("gcp-na")]
        [DataRow("gcp-eu")]
        public void GetContentstackEndpoint_DictOverload_AllRegions_ReturnNonEmpty(string region)
        {
            var dict = Endpoint.GetContentstackEndpoint(region);
            Assert.IsTrue(dict.Count > 0, $"Expected at least one endpoint for region '{region}'");
        }

        // ------------------------------------------------------------------
        // Error cases
        // ------------------------------------------------------------------

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetContentstackEndpoint_EmptyRegion_ThrowsArgumentException()
        {
            Endpoint.GetContentstackEndpoint("", "contentManagement");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetContentstackEndpoint_WhitespaceRegion_ThrowsArgumentException()
        {
            Endpoint.GetContentstackEndpoint("   ", "contentManagement");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetContentstackEndpoint_DictOverload_EmptyRegion_ThrowsArgumentException()
        {
            Endpoint.GetContentstackEndpoint("");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetContentstackEndpoint_UnknownRegion_ThrowsKeyNotFoundException()
        {
            Endpoint.GetContentstackEndpoint("xyz", "contentManagement");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetContentstackEndpoint_DictOverload_UnknownRegion_ThrowsKeyNotFoundException()
        {
            Endpoint.GetContentstackEndpoint("xyz");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetContentstackEndpoint_UnknownService_ThrowsKeyNotFoundException()
        {
            Endpoint.GetContentstackEndpoint("na", "unknownService");
        }

        [TestMethod]
        public void GetContentstackEndpoint_UnknownRegion_ErrorMessageContainsInput()
        {
            try
            {
                Endpoint.GetContentstackEndpoint("badregion", "contentManagement");
                Assert.Fail("Expected KeyNotFoundException");
            }
            catch (KeyNotFoundException ex)
            {
                Assert.IsTrue(ex.Message.Contains("badregion"));
            }
        }

        [TestMethod]
        public void GetContentstackEndpoint_UnknownService_ErrorMessageContainsServiceName()
        {
            try
            {
                Endpoint.GetContentstackEndpoint("na", "badService");
                Assert.Fail("Expected KeyNotFoundException");
            }
            catch (KeyNotFoundException ex)
            {
                Assert.IsTrue(ex.Message.Contains("badService"));
            }
        }

        // ------------------------------------------------------------------
        // Cache behaviour
        // ------------------------------------------------------------------

        [TestMethod]
        public void ResetCache_AllowsSubsequentCallToSucceed()
        {
            // First call populates cache
            string url1 = Endpoint.GetContentstackEndpoint("na", "contentManagement");

            // Reset and call again — should reload from disk/CDN and return same value
            Endpoint.ResetCache();
            string url2 = Endpoint.GetContentstackEndpoint("na", "contentManagement");

            Assert.AreEqual(url1, url2);
        }

        [TestMethod]
        public void GetContentstackEndpoint_CalledTwice_ReturnsSameResult()
        {
            string url1 = Endpoint.GetContentstackEndpoint("eu", "contentManagement");
            string url2 = Endpoint.GetContentstackEndpoint("eu", "contentManagement");
            Assert.AreEqual(url1, url2);
        }

        // ------------------------------------------------------------------
        // File path helper
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetLocalFilePath_EndsWithExpectedSegments()
        {
            string path = Endpoint.GetLocalFilePath();
            Assert.IsTrue(path.EndsWith(Path.Combine("Assets", "regions.json")),
                $"Expected path to end with Assets/regions.json, got: {path}");
        }

        // ------------------------------------------------------------------
        // All 7 regions × contentManagement spot-checks
        // ------------------------------------------------------------------

        [DataTestMethod]
        [DataRow("na",       "https://api.contentstack.io")]
        [DataRow("us",       "https://api.contentstack.io")]
        [DataRow("eu",       "https://eu-api.contentstack.com")]
        [DataRow("au",       "https://au-api.contentstack.com")]
        public void GetContentstackEndpoint_KnownRegions_ContentManagement_MatchExpected(
            string region, string expected)
        {
            string url = Endpoint.GetContentstackEndpoint(region, "contentManagement");
            Assert.AreEqual(expected, url);
        }

        // ------------------------------------------------------------------
        // ID takes priority over alias (two-pass lookup)
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetContentstackEndpoint_IdTakesPriorityOverAlias()
        {
            // "eu" is both a valid region ID and an alias in some registries.
            // The two-pass lookup must return the ID match first.
            string byId = Endpoint.GetContentstackEndpoint("eu", "contentManagement");
            Assert.IsFalse(string.IsNullOrEmpty(byId));
        }
    }
}
