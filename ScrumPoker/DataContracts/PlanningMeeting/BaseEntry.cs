using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.PlanningMeeting
{
    [DataContract]
    public class BaseEntry
    {
        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember(Name = "id", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __Id
#pragma warning restore IDE0051, IDE1006
        {
            get { return _id.ToJsonString(); }
            set { _id = value.JsonStringToGuid() ?? Guid.Empty; }
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

        private DateTime _meetingDate;
        /// <summary>
        /// The date for the sprint planning meeting.
        /// </summary>
        public DateTime MeetingDate
        {
            get { return _meetingDate; }
            set { _meetingDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "meetingDate", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __MeetingDate
#pragma warning restore IDE0051, IDE1006
        {
            get { return _meetingDate.ToJsonDateString(); }
            set { _meetingDate = value.JsonStringToDate().ToLocalDate() ?? DateTime.Now; }
        }

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
    }
}
