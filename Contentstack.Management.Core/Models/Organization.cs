using System;
namespace Contentstack.Management.Core.Models
{
    public class Organization
    {
        private ContentstackClient _client;

        #region Constructor
        public Organization(ContentstackClient contentstackClient)
        {
            _client = contentstackClient;
        }
        #endregion

    }
}
