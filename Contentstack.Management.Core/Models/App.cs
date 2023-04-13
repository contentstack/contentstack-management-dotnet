using System;
namespace Contentstack.Management.Core.Models
{
	public class App
	{
        #region Internal
        internal string orgUid;
        internal string uid;
        internal ContentstackClient client;
        #endregion

        #region Constructor
        internal App(ContentstackClient contentstackClient, string orgUid, string uid = null)
		{
			this.orgUid = orgUid;
            this.uid = uid;
            this.client = contentstackClient;
		}
        #endregion

        #region Public

        #endregion
    }
}

