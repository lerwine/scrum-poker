using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class TeamMember : ScrumPokerUser, ScrumPoker.DataContracts.ITeamMember
    {
        private static readonly PropertyDescriptor _pdSelectedCardId;
        private Guid? _selectedCardId;
        public Guid? SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId : null; }
            set
            {
                if (value.ToNullIfEmpty(SyncRoot, ref _selectedCardId))
                    RaisePropertyChanged(_pdSelectedCardId);
            }
        }

        [DataMember(Name = "selectedCardId", EmitDefaultValue = false)]
        private string __SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId.ToJsonString() : null; }
            set
            {
                if (value.JsonStringToGuidNotEmpty().ToNullIfEmpty(SyncRoot, ref _selectedCardId))
                    RaisePropertyChanged(_pdSelectedCardId);
            }
        }

        private static readonly PropertyDescriptor _pdAssignedPoints;
        private int _assignedPoints = 0;

        [DataMember(Name = "assignedPoints", IsRequired = true)]
        public int AssignedPoints
        {
            get { return _assignedPoints; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _assignedPoints))
                    RaisePropertyChanged(_pdAssignedPoints);
            }
        }

        static TeamMember()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TeamMember));
            _pdSelectedCardId = pdc["SelectedCardId"];
            _pdAssignedPoints = pdc["AssignedPoints"];
        }

        public override string ToString() { return this.ToJSON<TeamMember>(); }

        public static new TeamMember FromJSON(string jsonText) { return jsonText.FromJSON<TeamMember>(); }
    }
}
