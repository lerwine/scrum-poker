using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    public class ParticipantListItem : UserListItem
    {
        private Guid? _selectedCardId;
        public Guid? SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId : null; }
            set { _selectedCardId = value.ToNullIfEmpty(); }
        }

        [DataMember(Name = "selectedCardId", EmitDefaultValue = false)]
        private string __SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId.ToJsonString() : null; }
            set { _selectedCardId = value.JsonStringToGuidNotEmpty().ToNullIfEmpty(); }
        }
        
        private int _assignedPoints = 0;

        [DataMember(Name = "assignedPoints", IsRequired = true)]
        public int AssignedPoints
        {
            get { return _assignedPoints; } 
            set { _assignedPoints = (value < 0) ? 0 : value; }
        }

        // TODO: Add Card Color ID
    }
}