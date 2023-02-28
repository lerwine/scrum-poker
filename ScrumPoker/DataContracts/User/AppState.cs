using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    [DataContract]
    /// <summary>
    /// Response data contract for GET: /api/User
    /// </summary>
    public class AppState : UserListItem
    {
        // public const string FULL_ROUTE = Routings.User_Route;

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

        private Collection<TeamListItem> _teams = new Collection<TeamListItem>();
        [DataMember(Name = "teams", IsRequired = true)]
        /// <summary>
        /// Gets the teams that the current user belongs to.
        /// </summary>
        public Collection<TeamListItem> Teams
        {
            get { return _teams; }
            set { _teams = value ?? new Collection<TeamListItem>(); }
        }

        private Collection<UserListItem> _facilitators = new Collection<UserListItem>();
        [DataMember(Name = "facilitators", IsRequired = true)]
        /// <summary>
        /// Gets the team facilitators that are referenced in one more more <see cref="Teams" />.
        /// </summary>
        public Collection<UserListItem> Facilitators
        {
            get { return _facilitators; }
            set { _facilitators = value ?? new Collection<UserListItem>(); }
        }
    }
}