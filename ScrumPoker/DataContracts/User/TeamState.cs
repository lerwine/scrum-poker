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
    /// Response data contract for GET: /api/User/Team/{teamId}
    /// </summary>
    public class TeamState
    {
        public const string SUB_ROUTE = "TeamState";
        public const string FULL_ROUTE = Routings.User_Route + "/" + SUB_ROUTE;
        public const string PARAM_NAME = "id";
        
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
        #pragma warning disable IDE0051, IDE1006
        private string __TeamId
        #pragma warning restore IDE0051, IDE1006
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
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }
        
        private Collection<PlanningMeetingListItem> _meetings = new Collection<PlanningMeetingListItem>();
        [DataMember(Name = "meetings", IsRequired = true)]
        /// <summary>
        /// Gets the teams that the current user belongs to.
        /// </summary>
        public Collection<PlanningMeetingListItem> Meetings
        {
            get { return _meetings; }
            set { _meetings = value ?? new Collection<PlanningMeetingListItem>(); }
        }

        // TODO: Add Epics

        // TODO: Add Milestones without Epics

        // TODO: Add Initiatives
    }
}