using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Response contract representing a user.
    /// </summary>
    public class UserListItem
    {
        [DataMember(Name = "userId", IsRequired = true)]
        private string __UserId
        {
            get { return _userId.ToJsonString(); }
            set { _userId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid _userId;
        /// <summary>
        /// Gets the user's unique identifier.
        /// </summary>
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private string _displayName = "";
        [DataMember(Name = "displayName", IsRequired = true)]
        /// <summary>
        /// The display name of the currently authenticated user.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value.EmptyIfNullOrTrimmed(); }
        }

        private string _userName = "";
        [DataMember(Name = "userName", IsRequired = true)]
        /// <summary>
        /// The login name of the currently authenticated user.
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value.EmptyIfNullOrTrimmed(); }
        }
    }
}
