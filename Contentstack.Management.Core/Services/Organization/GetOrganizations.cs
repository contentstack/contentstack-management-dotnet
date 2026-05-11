
using Contentstack.Management.Core.Queryable;
using System.Text.Json;
namespace Contentstack.Management.Core.Services.Organization
{
    internal class GetOrganizations: ContentstackService
    {
        #region Internal

        internal GetOrganizations(JsonSerializerOptions serializerOptions, ParameterCollection collection, string uid = null) : base(serializerOptions, collection: collection)
        {
            this.ResourcePath = "organizations";

            if (uid != null)
            {
                this.AddPathResource("{organization_uid}", uid);
                this.ResourcePath = "organizations/{organization_uid}";
            }
            if (collection != null && collection.Count > 0)
            {
                this.UseQueryString = true;
            }
        }
        #endregion
    }
}
