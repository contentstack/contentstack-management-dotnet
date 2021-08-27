using System;
namespace Contentstack.Management.Core.Tests.Model
{
    public class OrganizationModel
    {
        public string Uid { get; set; }
        public string Name { get; set; }
    }

    public class OrganisationResponse
    {
        public OrganizationModel Organization { get; set; }
    }
}
