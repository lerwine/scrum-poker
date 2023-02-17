using System;
using System.Runtime.Serialization;

namespace ScrumPokerServer.DataContracts
{
    // TODO: Make this obsolete
    [Obsolete("Use DataContracts.DeveloperEntity, instead")]
    [DataContract]
    public class AdminUser : IWebAppUser
    {
        [DataMember(IsRequired = false)]
        internal string displayName;
        string IWebAppUser.DisplayName { get { return displayName; } }

        [DataMember(IsRequired = false)]
        internal string userName;
        string IWebAppUser.UserName { get { return userName; } }

        [DataMember(IsRequired = false)]
        internal string password;
        string IWebAppUser.Password { get { return password; } }

        [DataMember(IsRequired = false)]
        internal bool isParticipant = false;
    }
}
