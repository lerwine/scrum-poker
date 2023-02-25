using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    [DataContract]
    public class ParticipantListItem : UserListItem
    {
        private Guid? _selectedCardId;
        public Guid? SelectedCardId
        {
            get { return _selectedCardId; }
            set { _selectedCardId = value.NullIfEmpty(); }
        }

        [DataMember(Name = "selectedCardId", EmitDefaultValue = false)]
        #pragma warning disable IDE0051, IDE1006
        private string __SelectedCardId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _selectedCardId.ToJsonString(); }
            set { _selectedCardId = value.JsonStringToGuidNotEmpty().NullIfEmpty(); }
        }
        
        public Guid CardColorId { get; set; }

        [DataMember(Name = "cardColorId", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __ColorSchemeId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return CardColorId.ToJsonString(); }
            set { CardColorId = value.JsonStringToGuid() ?? Guid.Empty; }
        }
        
        private int _assignedPoints = 0;

        [DataMember(Name = "assignedPoints", IsRequired = true)]
        public int AssignedPoints
        {
            get { return _assignedPoints; } 
            set { _assignedPoints = (value < 0) ? 0 : value; }
        }

        private int? _sprintCapacity;
        [DataMember(Name = "sprintCapacity", EmitDefaultValue = false)]
        public int? SprintCapacity
        {
            get { return _sprintCapacity; }
            set { _sprintCapacity = (value.HasValue && value.Value < 1) ? null : value; }
        }

        // TODO: Add Card Color ID
    }
}