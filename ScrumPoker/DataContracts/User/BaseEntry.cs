using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Response contract representing a user.
    /// </summary>
    [DataContract]
    public class BaseEntry
    {
        [DataMember(Name = "userId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __UserId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _userId.ToJsonString(); }
            set { _userId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid _userId;
        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private string _displayName = "";
        [DataMember(Name = "displayName", IsRequired = true)]
        /// <summary>
        /// The display name of the current user.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value.WsNormalizedOrEmptyIfNull(); }
        }

        private string _userName = "";
        [DataMember(Name = "userName", IsRequired = true)]
        /// <summary>
        /// The login name of the currentl user.
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value.WsNormalizedOrEmptyIfNull(); }
        }

        private bool _isAdmin = false;
        [DataMember(Name = "isAdmin", IsRequired = true)]
        /// <summary>
        /// Indicates whether the user is registered as an administrative user.
        /// </summary>
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }
    }
}
