using System;

namespace ScrumPoker.DataContracts
{
    public interface ISprintGrouping : ITitleAndIdentifier
    {
        string Description { get; set; }
        
        DateTime? StartDate { get; set; }
        
        DateTime? PlannedEndDate { get; set; }
    }
}
