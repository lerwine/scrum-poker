using System.Runtime.Serialization;

namespace ScrumPokerServer.DataContracts
{
    [DataContract]
    public class WebAppUser : IWebAppUser
    {
        [DataMember(IsRequired = false)]
        internal string displayName;
        string IWebAppUser.DisplayName { get { return displayName; } }

        [DataMember(IsRequired = true)]
        internal string userName;
        string IWebAppUser.UserName { get { return userName; } }

        [DataMember(IsRequired = false)]
        internal string password;
        string IWebAppUser.Password { get { return password; } }
    }
}