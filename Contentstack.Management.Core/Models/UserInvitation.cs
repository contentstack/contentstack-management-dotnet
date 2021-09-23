using System;
using System.Collections.Generic;

namespace Contentstack.Management.Core.Models
{
    public class UserInvitation
    {
        /// <summary>
        /// User email-id for invitation
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User uid for invitation
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// List of roles Uid to be assigned to the user.
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
