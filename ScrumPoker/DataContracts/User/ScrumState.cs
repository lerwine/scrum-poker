using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Response data contract for GET: /api/User/ScrumMeeting/{id}
    /// </summary>
    [DataContract]
    public class ScrumState : PlanningMeetingListItem
    {
        public const string SUB_ROUTE = "ScrumMeeting";
        public const string FULL_ROUTE = Routings.User_Route + "/" + SUB_ROUTE;
        public const string PARAM_NAME = "id";

        private DateTime? _plannedStartDate;
        /// <summary>
        /// The sprint's planned start date.
        /// </summary>
        public DateTime? PlannedStartDate
        {
            get { return _plannedStartDate; }
            set { _plannedStartDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "plannedStartDate", EmitDefaultValue = false)]
        #pragma warning disable IDE0051, IDE1006
        private string __PlannedStartDate
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _plannedStartDate.ToJsonDateString(); }
            set { _plannedStartDate = value.JsonStringToDate().ToLocalDate(); }
        }

        private DateTime? _plannedEndDate;
        /// <summary>
        /// The sprint's planned end date.
        /// </summary>
        public DateTime? PlannedEndDate
        {
            get { return _plannedEndDate; }
            set { _plannedEndDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "plannedEndDate", EmitDefaultValue = false)]
        #pragma warning disable IDE0051, IDE1006
        private string __PlannedEndDate
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _plannedEndDate.ToJsonDateString(); }
            set { _plannedEndDate = value.JsonStringToDate().ToLocalDate(); }
        }

        private InitiativeListItem _initiative;
        /// <summary>
        /// The optional initiative for the sprint.
        /// </summary>
        [DataMember(Name = "initiative", EmitDefaultValue = false)]
        public InitiativeListItem Initiative
        {
            get { return _initiative; }
            set { _initiative = value; }
        }

        private EpicListItem _epic;
        /// <summary>
        /// The optional epic of the sprint.
        /// </summary>
        [DataMember(Name = "epic", EmitDefaultValue = false)]
        public EpicListItem Epic
        {
            get { return _epic; }
            set { _epic = value; }
        }

        private MilestoneListItem _milestone;
        /// <summary>
        /// The optional milestone for the sprint.
        /// </summary>
        [DataMember(Name = "milestone", EmitDefaultValue = false)]
        public MilestoneListItem Milestone
        {
            get { return _milestone; }
            set { _milestone = value; }
        }

        private int _currentScopePoints = 0;
        /// <summary>
        /// The number of points assigned to the sprint.
        /// </summary>
        [DataMember(Name = "currentScopePoints", IsRequired = true)]
        public int CurrentScopePoints
        {
            get { return _currentScopePoints; }
            set { _currentScopePoints = (value < 0) ? 0 : value; }
        }

        private int? _sprintCapacity;
        /// <summary>
        /// The optional limit in points for the entire sprint.
        /// </summary>
        [DataMember(Name = "sprintCapacity", EmitDefaultValue = false)]
        public int? SprintCapacity
        {
            get { return _sprintCapacity; }
            set { _sprintCapacity = (value.HasValue && value.Value < 1) ? null : value; }
        }

        private TeamListItem _team = null;
        /// <summary>
        /// The team conducting the sprint planning meeting.
        /// </summary>
        [DataMember(Name = "team", IsRequired = true)]
        public TeamListItem Team
        {
            get { return _team; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _team = value;
            }
        }

        private UserListItem _facilitator = null;
        /// <summary>
        /// The team facilitator.
        /// </summary>
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

        private ColorScheme _colorScheme = null;
        /// <summary>
        /// The team facilitator.
        /// </summary>
        [DataMember(Name = "colorScheme", IsRequired = true)]
        public ColorScheme ColorScheme
        {
            get { return _colorScheme; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _colorScheme = value;
            }
        }

        private Collection<ParticipantListItem> _participants = new Collection<ParticipantListItem>();
        /// <summary>
        /// Participants in the sprint planning meeting.
        /// </summary>
        [DataMember(Name = "participants", IsRequired = true)]
        public Collection<ParticipantListItem> Participants
        {
            get { return _participants; }
            set { _participants = value ?? new Collection<ParticipantListItem>(); }
        }

        // TODO: Add Deck Information
    }
}