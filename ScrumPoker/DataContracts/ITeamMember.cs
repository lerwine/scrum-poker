using System;

namespace ScrumPoker.DataContracts
{
    public interface ITeamMember : IScrumPokerUser
    {
        Guid? SelectedCardId { get; set; }
        
        int AssignedPoints { get; set; }
    }
}
