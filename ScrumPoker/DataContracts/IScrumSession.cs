using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    [Obsolete("Replace with DTO object(s)")]
    public interface IScrumSession : ITitleAndIdentifier
    {
        string Description { get; set; }

        ISprintGrouping Initiative { get; set; }
        
        ISprintGrouping Epic { get; set; }
        
        ISprintGrouping Milestone { get; set; }
        
        DateTime? PlannedStartDate { get; set; }
        
        DateTime? PlannedEndDate { get; set; }
        
        int CurrentScopePoints { get; set; }
        
        int? SprintCapacity { get; set; }
        
        Guid DeckId { get; set; }
        
        Guid OrganizerId { get; set; }
    }
}
