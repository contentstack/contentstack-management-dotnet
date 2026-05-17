
using Contentstack.Management.Core.Queryable;
using Newtonsoft.Json;
using System.Text.Json;
using Contentstack.Management.Core.Enums;
namespace Contentstack.Management.Core.Services.Organization
{
    internal class GetOrganizations: ContentstackService
    {
        #region Internal

        internal GetOrganizations(Newtonsoft.Json.JsonSerializer serializer, ParameterCollection collection, string uid = null, JsonSerializerOptions stjOptions = null, SerializationMode serializationMode = SerializationMode.Newtonsoft) : base(serializer, collection: collection, stjOptions: stjOptions, serializationMode: serializationMode)
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
