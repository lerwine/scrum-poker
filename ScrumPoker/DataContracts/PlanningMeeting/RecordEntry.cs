using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.PlanningMeeting
{
    [DataContract]
    public class RecordEntry : BaseEntry
    {
        private Guid _teamId;
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

        private Guid _schemeId;
        public Guid SchemeId
        {
            get { return _schemeId; }
            set { _schemeId = value; }
        }

        [DataMember(Name = "schemeId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __SchemeId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _schemeId.ToJsonString(); }
            set { _schemeId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid _deckId;
        public Guid DeckId
        {
            get { return _deckId; }
            set { _deckId = value; }
        }

        [DataMember(Name = "deckId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __DeckId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _deckId.ToJsonString(); }
            set { _deckId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid? _initiativeId;
        public Guid? InitiativeId
        {
            get { return _initiativeId; }
            set { _initiativeId = value; }
        }

        [DataMember(Name = "initiativeId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __InitiativeId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _initiativeId.ToJsonString(); }
            set { _initiativeId = value.JsonStringToGuid(); }
        }

        private Guid? _epicId;
        public Guid? EpicId
        {
            get { return _epicId; }
            set { _epicId = value; }
        }

        [DataMember(Name = "epicId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __EpicId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _epicId.ToJsonString(); }
            set { _epicId = value.JsonStringToGuid(); }
        }

        private Guid? _milestoneId;
        public Guid? MilestoneId
        {
            get { return _milestoneId; }
            set { _milestoneId = value; }
        }

        [DataMember(Name = "milestoneId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __MilestoneId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _milestoneId.ToJsonString(); }
            set { _milestoneId = value.JsonStringToGuid(); }
        }
    }
}
