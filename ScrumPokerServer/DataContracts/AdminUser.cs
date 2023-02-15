namespace ScrumPokerServer.DataContracts
{
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
