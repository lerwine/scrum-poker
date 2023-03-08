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

        private BaseEntry _facilitator = null;
        [DataMember(Name = "facilitator", IsRequired = true)]
        /// <summary>
        /// The team's facilitator.
        /// </summary>
        public BaseEntry Facilitator
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
            set { _title = value.WsNormalizedOrEmptyIfNull(); }
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

        private Collection<PlanningMeeting.RecordEntry> _meetings = new Collection<PlanningMeeting.RecordEntry>();
        [DataMember(Name = "meetings", IsRequired = true)]
        /// <summary>
        /// The planning meetings for the current team.
        /// </summary>
        public Collection<PlanningMeeting.RecordEntry> Meetings
        {
            get { return _meetings; }
            set { _meetings = value ?? new Collection<PlanningMeeting.RecordEntry>(); }
        }

        private Collection<Epic.RecordEntry> _epics = new Collection<Epic.RecordEntry>();
        [DataMember(Name = "epics", IsRequired = true)]
        /// <summary>
        /// The epics for the current team.
        /// </summary>
        public Collection<Epic.RecordEntry> Epics
        {
            get { return _epics; }
            set { _epics = value ?? new Collection<Epic.RecordEntry>(); }
        }

        private Collection<Milestone.RecordEntry> _milestones = new Collection<Milestone.RecordEntry>();
        [DataMember(Name = "milestones", IsRequired = true)]
        /// <summary>
        /// The milestones for the current team that do not belong to an epic.
        /// </summary>
        public Collection<Milestone.RecordEntry> Milestones
        {
            get { return _milestones; }
            set { _milestones = value ?? new Collection<Milestone.RecordEntry>(); }
        }

        private Collection<Initiative.RecordEntry> _initiatives = new Collection<Initiative.RecordEntry>();
        [DataMember(Name = "initiatives", IsRequired = true)]
        /// <summary>
        /// The initiatives for the current team.
        /// </summary>
        public Collection<Initiative.RecordEntry> Initiatives
        {
            get { return _initiatives; }
            set { _initiatives = value ?? new Collection<Initiative.RecordEntry>(); }
        }
    }
}
