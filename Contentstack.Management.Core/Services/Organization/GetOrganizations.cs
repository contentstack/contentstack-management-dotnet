
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
namespace Contentstack.Management.Core.Services.Organization
{
    internal class GetOrganizations: ContentstackService
    {
        #region Internal

        internal GetOrganizations(JsonSerializer serializer, ParameterCollection collection, string uid = null) : base(serializer, collection: collection)
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
