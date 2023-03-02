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
    public class ScrumState : PlanningMeeting.Details
    {
        public const string SUB_ROUTE = "ScrumMeeting";
        public const string FULL_ROUTE = Routings.User_Route + "/" + SUB_ROUTE;
        public const string PARAM_NAME = "id";

        private BaseEntry _facilitator = null;
        /// <summary>
        /// The team facilitator.
        /// </summary>
        [DataMember(Name = "facilitator", IsRequired = true)]
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

        private Collection<Deck.CardEntry> _cards = new Collection<Deck.CardEntry>();
        [DataMember(Name = "cards", IsRequired = true)]
        public Collection<Deck.CardEntry> Cards
        {
            get { return _cards; }
            set { _cards = value ?? new Collection<Deck.CardEntry>(); }
        }

        private Collection<PlanningMeeting.ParticipantEntry> _participants = new Collection<PlanningMeeting.ParticipantEntry>();
        /// <summary>
        /// Participants in the sprint planning meeting.
        /// </summary>
        [DataMember(Name = "participants", IsRequired = true)]
        public Collection<PlanningMeeting.ParticipantEntry> Participants
        {
            get { return _participants; }
            set { _participants = value ?? new Collection<PlanningMeeting.ParticipantEntry>(); }
        }
    }
}