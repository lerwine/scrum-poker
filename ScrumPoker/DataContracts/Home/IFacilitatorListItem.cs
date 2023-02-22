using System;

namespace ScrumPoker.DataContracts.Home
{
    public interface IFacilitatorListItem
    {
        Guid UserId { get; set; }

        string DisplayName { get; set; }

        string UserName { get; set; }   
    }
}
