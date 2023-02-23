using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User.ScrumState
{
    [DataContract]
    /// <summary>
    /// Response data contract for GET: /api/User/ScrumMeeting/{meetingId}
    /// </summary>
    public class Response : PlanningMeetingListItem
    {
        private DateTime? _plannedStartDate;
        public DateTime? PlannedStartDate
        {
            get { return _plannedStartDate; }
            set { _plannedStartDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "plannedStartDate", EmitDefaultValue = false)]
        private string __PlannedStartDate
        {
            get { return _plannedStartDate.ToJsonDateString(); }
            set { _plannedStartDate = value.JsonStringToDate().ToLocalDate(); }
        }

        private DateTime? _plannedEndDate;
        public DateTime? PlannedEndDate
        {
            get { return _plannedEndDate; }
            set { _plannedEndDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "plannedEndDate", EmitDefaultValue = false)]
        private string __PlannedEndDate
        {
            get { return _plannedEndDate.ToJsonDateString(); }
            set { _plannedEndDate = value.JsonStringToDate().ToLocalDate(); }
        }

        private SprintGroupingResponse _initiative;
        [DataMember(Name = "initiative", EmitDefaultValue = false)]
        public SprintGroupingResponse Initiative
        {
            get { return _initiative; }
            set { _initiative = value; }
        }

        private SprintGroupingResponse _epic;
        [DataMember(Name = "epic", EmitDefaultValue = false)]
        public SprintGroupingResponse Epic
        {
            get { return _epic; }
            set { _epic = value; }
        }

        private SprintGroupingResponse _milestone;
        [DataMember(Name = "milestone", EmitDefaultValue = false)]
        public SprintGroupingResponse Milestone
        {
            get { return _milestone; }
            set { _milestone = value; }
        }


        private int _currentScopePoints = 0;
        [DataMember(Name = "currentScopePoints", IsRequired = true)]
        public int CurrentScopePoints
        {
            get { return _currentScopePoints; }
            set { _currentScopePoints = (value < 0) ? 0 : value; }
        }

        private int? _sprintCapacity;
        [DataMember(Name = "sprintCapacity", EmitDefaultValue = false)]
        public int? SprintCapacity
        {
            get { return _sprintCapacity; }
            set { _sprintCapacity = (value.HasValue && value.Value < 1) ? null : value; }
        }

        private TeamListItem _team = null;
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

        private Collection<ParticipantListItem> _participants = new Collection<ParticipantListItem>();
        [DataMember(Name = "participants", IsRequired = true)]
        public Collection<ParticipantListItem> Participants
        {
            get { return _participants; }
            set { _participants = value ?? new Collection<ParticipantListItem>(); }
        }

        private Collection<ColorSchemeListItem> _colorSchemes = new Collection<ColorSchemeListItem>();
        [DataMember(Name = "colorSchemes", IsRequired = true)]
        public Collection<ColorSchemeListItem> ColorSchemes
        {
            get { return _colorSchemes; }
            set { _colorSchemes = value ?? new Collection<ColorSchemeListItem>(); }
        }

        // TODO: Add Deck Information
    }
}