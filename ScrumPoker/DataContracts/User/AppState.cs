using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Response data contract for GET: /api/User/AppState
    /// </summary>
    [DataContract]
    public class AppState : BaseEntry
    {
        public const string SUB_ROUTE = "AppState";
        public const string FULL_ROUTE = Routings.User_Route + "/" + SUB_ROUTE;

        private Collection<Team.RecordEntry> _teams = new Collection<Team.RecordEntry>();
        [DataMember(Name = "teams", IsRequired = true)]
        /// <summary>
        /// The teams that the current user belongs to.
        /// </summary>
        public Collection<Team.RecordEntry> Teams
        {
            get { return _teams; }
            set { _teams = value ?? new Collection<Team.RecordEntry>(); }
        }

        private Collection<BaseEntry> _facilitators = new Collection<BaseEntry>();
        [DataMember(Name = "facilitators", IsRequired = true)]
        /// <summary>
        /// The team facilitators that are referenced in one more more <see cref="Teams" />.
        /// </summary>
        public Collection<BaseEntry> Facilitators
        {
            get { return _facilitators; }
            set { _facilitators = value ?? new Collection<BaseEntry>(); }
        }
    }
}