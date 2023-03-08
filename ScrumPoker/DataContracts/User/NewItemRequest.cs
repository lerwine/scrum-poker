using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    [DataContract]
    public class NewItemRequest
    {
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

        private Collection<Guid> _teamIds;
        [DataMember(Name = "teamIds", EmitDefaultValue = false)]
        public Collection<Guid> TeamIds
        {
            get { return _teamIds; }
            set { _teamIds = value; }
        }
    }
}
