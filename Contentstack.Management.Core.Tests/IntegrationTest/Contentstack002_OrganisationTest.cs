using System;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Queryable;
using Contentstack.Management.Core.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentstack.Management.Core.Tests.IntegrationTest
{
    [TestClass]
    public class Contentstack002_OrganisationTest
    {
        private Double Count;

        [TestMethod]
        [DoNotParallelize]
        public void Test001_Should_Return_All_Organizations()
        {
            try
            {
                Organization organization = Contentstack.Client.Organization();

                ContentstackResponse contentstackResponse = organization.GetOrganizations();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;
                
            } catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            
        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test001_Should_Return_All_OrganizationsAsync()
        {
            try
            {
                Organization organization = Contentstack.Client.Organization();

                ContentstackResponse contentstackResponse = await organization.GetOrganizationsAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test002_Should_Return_With_Skipping_Organizations()
        {
            try
            {
                Organization organization = Contentstack.Client.Organization();
                ParameterCollection collection = new ParameterCollection();
                collection.Add("skip", 4);
                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                var count = (response["organizations"] as Newtonsoft.Json.Linq.JArray).Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test003_Should_Return_Organization_With_UID()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.GetOrganizations();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["organization"]);

                OrganisationResponse model = contentstackResponse.OpenTResponse<OrganisationResponse>();
                Assert.AreEqual(org.Name, model.Organization.Name);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public void Test004_Should_Return_Organization_With_UID_Include_Plan()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);
                ParameterCollection collection = new ParameterCollection();
                collection.Add("include_plan", true);

                ContentstackResponse contentstackResponse = organization.GetOrganizations(collection);

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["organization"]);
                Assert.IsNotNull(response["organization"]["plan"]);
                
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Test005_Should_Return_Organization_Roles()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = organization.Roles();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["roles"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        [DoNotParallelize]
        public async System.Threading.Tasks.Task Test006_Should_Return_Organization_RolesAsync()
        {
            try
            {
                var org = Contentstack.Organization;
                Organization organization = Contentstack.Client.Organization(org.Uid);

                ContentstackResponse contentstackResponse = await organization.RolesAsync();

                var response = contentstackResponse.OpenJObjectResponse();
                Assert.IsNotNull(response);
                Assert.IsNotNull(response["roles"]);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }
    }
}
