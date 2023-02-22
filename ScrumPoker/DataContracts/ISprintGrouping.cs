using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    [Obsolete("Replace with DTO object(s)")]
    public interface ISprintGrouping : ITitleAndIdentifier
    {
        string Description { get; set; }
        
        DateTime? StartDate { get; set; }
        
        DateTime? PlannedEndDate { get; set; }
    }
}
