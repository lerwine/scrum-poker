using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface ITeamMember : IScrumPokerUser
    {
        Guid? SelectedCardId { get; set; }
        
        int AssignedPoints { get; set; }
    }
    // TODO: Move to ScrumPoker.StandaloneServer
    [DataContract]
    public class TeamMember : ScrumPokerUser, ITeamMember
    {
        private Guid? _selectedCardId;
        public Guid? SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId : null; }
            set { _selectedCardId = (value.HasValue && !value.Value.Equals(Guid.Empty)) ? value : null; }
        }

        [DataMember(Name = "selectedCardId", EmitDefaultValue = false)]
        private string __SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId.ToJsonString() : null; }
            set { _selectedCardId = JsonStringToGuidNotEmpty(value); }
        }

        private int _assignedPoints = 0;
        [DataMember(Name = "assignedPoints", IsRequired = true)]
        public int AssignedPoints
        {
            get { return _assignedPoints; }
            set { _assignedPoints = (value < 0) ? 0 : value; }
        }

        public override string ToString() { return this.ToJSON<TeamMember>(); }

        public static new TeamMember FromJSON(string jsonText) { return jsonText.FromJSON<TeamMember>(); }
    }
}
