using System;

namespace ScrumPoker.DataContracts
{
    [Obsolete("Replace with DTO object(s)")]
    public interface ITeamMember : IScrumPokerUser
    {
        Guid? SelectedCardId { get; set; }
        
        int AssignedPoints { get; set; }
    }
}
