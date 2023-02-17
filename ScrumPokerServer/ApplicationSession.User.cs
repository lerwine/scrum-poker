using System;
using System.Linq;

namespace ScrumPokerServer
{
    public partial class ApplicationSession
    {
        // TODO: Make this obsolete
        [Obsolete("Use DataContracts.DeveloperEntity, instead")]
        public class User
        {
            private readonly string _userName;
            public string UserName { get { return _userName; } }

            private string _displayName;
            public string DisplayName
            {
                get { return _displayName; }
                set
                {
                    string displayName = value;
                    _displayName = (displayName != null && (displayName = displayName.Trim()).Length > 0) ? displayName : _userName;
                }
            }
            

            private User(string userName, string displayName)
            {
                _userName = userName;
                if (displayName == null || (displayName = displayName.Trim()).Length == 0)
                {
                    int index = userName.IndexOf('/');
                    _displayName = (index > -1 && (displayName = userName.Substring(index + 1)).Trim().Length > 0) ? displayName : userName;
                }
                else
                    _displayName = displayName;
            }

            internal static User Create(DataContracts.IWebAppUser source, out string message)
            {
                if (source == null)
                    message = "Source is null";
                else
                {
                    string userName = source.UserName;
                    if (userName == null || (userName = userName.Trim()).Length == 0)
                        message = "User name not specified.";
                    else
                    {
                        message = "";
                        return new User(userName, source.DisplayName);;
                    }
                }
                return null;
            }

            internal static bool TryFindFromTokenString(ApplicationSession appSession, string value, out User user)
            {
                throw new NotImplementedException();
            }
        }
    }
}
