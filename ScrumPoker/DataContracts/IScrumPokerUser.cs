using System;

namespace ScrumPoker.DataContracts
{
    public interface IScrumPokerUser : IValidatableObject
    {
        string DisplayName { get; set; }

        string UserName { get; set; }
        
        Guid? ColorId { get; set; }
        
        int? SprintCapacity { get; set; }
        
        bool IsParticipant { get; set; }
    }
}
