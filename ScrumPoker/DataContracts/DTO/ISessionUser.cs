using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.DTO
{
    public interface ISessionUser : IValidatableObject
    {
        Guid SessionId { get; set; }

        Guid UserId { get; set; }

        Guid ColorId { get; set; }
        
        int? SprintCapacity { get; set; }

        Guid? SelectedCardId { get; set; }
        
        int AssignedPoints { get; set; }
    }
}