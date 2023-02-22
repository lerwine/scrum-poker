using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.DTO
{
    public interface IPlanningSession : IValidatableObject
    {
        Guid Id { get; set; }

        Guid TeamId { get; set; }

        string Description { get; set; }

        ISprintGrouping Initiative { get; set; }
        
        ISprintGrouping Epic { get; set; }
        
        ISprintGrouping Milestone { get; set; }
        
        DateTime? PlannedStartDate { get; set; }
        
        DateTime? PlannedEndDate { get; set; }
        
        int CurrentScopePoints { get; set; }
        
        int? SprintCapacity { get; set; }
        
        Guid DeckId { get; set; }
    }
}