using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User.TeamState
{
    /// <summary>
    /// Response data contract for GET: /api/User/TeamState/{teamId}
    /// </summary>
    public class Response
    {
        private Guid _teamId;
        /// <summary>
        /// Gets the team's unique identifier.
        /// </summary>
        public Guid TeamId
        {
            get { return _teamId; }
            set { _teamId = value; }
        }

        [DataMember(Name = "teamId", IsRequired = true)]
        private string __TeamId
        {
            get { return _teamId.ToJsonString(); }
            set { _teamId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private UserListItem _facilitator = null;
        [DataMember(Name = "facilitator", IsRequired = true)]
        public UserListItem Facilitator
        {
            get { return _facilitator; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _facilitator = value;
            }
        }

        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        /// <summary>
        /// The title of the current team.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value.EmptyIfNullOrTrimmed(); }
        }

        private string _description = null;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        /// <summary>
        /// The description of the current tem.
        /// </summary>
        public string _description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }
        
        private Collection<ScrumMeetingListItem> _meetings = new Collection<TeamListItem>();
        [DataMember(Name = "meetings", IsRequired = true)]
        /// <summary>
        /// Gets the teams that the current user belongs to.
        /// </summary>
        public Collection<ScrumMeetingListItem> Meetings
        {
            get { return _meetings; }
            set { _meetings = value ?? new Collection<TeamListItem>(); }
        }
    }
}