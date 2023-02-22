using System;

namespace ScrumPoker.DataContracts.Home
{
    public interface ITeamListItem
    {
        Guid TeamId { get; set; }

        string TeamName { get; set; }

        string Description { get; set; }
        
        Guid FacilitatorId { get; set; }
    }
}
