using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.PlanningMeeting
{
    public class Details : BaseEntry
    {
        private Team.RecordEntry _team = null;
        /// <summary>
        /// The team conducting the sprint planning meeting.
        /// </summary>
        [DataMember(Name = "team", IsRequired = true)]
        public Team.RecordEntry Team
        {
            get { return _team; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _team = value;
            }
        }

        private ColorScheme.Details _colorScheme = null;
        /// <summary>
        /// The team facilitator.
        /// </summary>
        [DataMember(Name = "colorScheme", IsRequired = true)]
        public ColorScheme.Details ColorScheme
        {
            get { return _colorScheme; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _colorScheme = value;
            }
        }

        private Deck.BaseEntry _deck = null;
        [DataMember(Name = "deck", IsRequired = true)]
        public Deck.BaseEntry Deck
        {
            get { return _deck; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _deck = value;
            }
        }

        private Initiative.RecordEntry _initiative;
        /// <summary>
        /// The optional initiative for the sprint.
        /// </summary>
        [DataMember(Name = "initiative", EmitDefaultValue = false)]
        public Initiative.RecordEntry Initiative
        {
            get { return _initiative; }
            set { _initiative = value; }
        }

        private Epic.RecordEntry _epic;
        /// <summary>
        /// The optional epic of the sprint.
        /// </summary>
        [DataMember(Name = "epic", EmitDefaultValue = false)]
        public Epic.RecordEntry Epic
        {
            get { return _epic; }
            set { _epic = value; }
        }

        private Milestone.RecordEntry _milestone;
        /// <summary>
        /// The optional milestone for the sprint.
        /// </summary>
        [DataMember(Name = "milestone", EmitDefaultValue = false)]
        public Milestone.RecordEntry Milestone
        {
            get { return _milestone; }
            set { _milestone = value; }
        }

    }
}