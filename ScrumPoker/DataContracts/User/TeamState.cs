using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Response data contract for GET: /api/User/Team/{id}
    /// </summary>
    [DataContract]
    public class TeamState
    {
        public const string SUB_ROUTE = "TeamState";
        public const string FULL_ROUTE = Routings.User_Route + "/" + SUB_ROUTE;
        public const string PARAM_NAME = "id";
        
        private Guid _teamId;
        /// <summary>
        /// The team's unique identifier.
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
        /// <summary>
        /// The team's facilitator.
        /// </summary>
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
            set { _title = value.WsNormalized(); }
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
        /// The planning meetings for the current team.
        /// </summary>
        public Collection<PlanningMeetingListItem> Meetings
        {
            get { return _meetings; }
            set { _meetings = value ?? new Collection<PlanningMeetingListItem>(); }
        }
        
        private Collection<EpicListItem> _epics = new Collection<EpicListItem>();
        [DataMember(Name = "epics", IsRequired = true)]
        /// <summary>
        /// The epics for the current team.
        /// </summary>
        public Collection<EpicListItem> Epics
        {
            get { return _epics; }
            set { _epics = value ?? new Collection<EpicListItem>(); }
        }

        private Collection<MilestoneListItem> _milestones = new Collection<MilestoneListItem>();
        [DataMember(Name = "milestones", IsRequired = true)]
        /// <summary>
        /// The milestones for the current team that do not belong to an epic.
        /// </summary>
        public Collection<MilestoneListItem> Milestones
        {
            get { return _milestones; }
            set { _milestones = value ?? new Collection<MilestoneListItem>(); }
        }

        private Collection<InitiativeListItem> _initiatives = new Collection<InitiativeListItem>();
        [DataMember(Name = "initiatives", IsRequired = true)]
        /// <summary>
        /// The initiatives for the current team.
        /// </summary>
        public Collection<InitiativeListItem> Initiatives
        {
            get { return _initiatives; }
            set { _initiatives = value ?? new Collection<InitiativeListItem>(); }
        }
    }
}