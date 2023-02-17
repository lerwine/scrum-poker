using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class DeveloperEntity : Developer
    {
        private int? _selectedCardId;
        [DataMember(Name = "selectedCardId", EmitDefaultValue = false)]
        public int? SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId : null; }
            set { _selectedCardId = value; }
        }

        private int _assignedPoints = 0;
        [DataMember(Name = "assignedPoints", IsRequired = true)]
        public int AssignedPoints
        {
            get { return _assignedPoints; }
            set { _assignedPoints = (value < 0) ? 0 : value; }
        }

        public override string ToString() { return this.ToJSON<DeveloperEntity>(); }

        public static new DeveloperEntity FromJSON(string jsonText) { return jsonText.FromJSON<DeveloperEntity>(); }
    }
}
