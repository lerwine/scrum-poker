using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Response contract representing a scrum meeting.
    /// </summary>
    public class PlanningMeetingListItem
    {
        private Guid _meetingId;
        /// <summary>
        /// Gets the team's unique identifier.
        /// </summary>
        public Guid MeetingId
        {
            get { return _meetingId; }
            set { _meetingId = value; }
        }

        [DataMember(Name = "meetingId", IsRequired = true)]
        private string __MeetingId
        {
            get { return _meetingId.ToJsonString(); }
            set { _meetingId = value.JsonStringToGuid() ?? Guid.Empty; }
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

        private DateTime _meetingDate;
        public DateTime MeetingDate
        {
            get { return _meetingDate; }
            set { _meetingDate = value.ToLocalDate(); }
        }

        [DataMember(Name = "meetingDate", EmitDefaultValue = false)]
        private string __MeetingDate
        {
            get { return _meetingDate.ToJsonDateString(); }
            set { _meetingDate = value.JsonStringToDate().ToLocalDate() ?? DateTime.Now; }
        }
    }
}