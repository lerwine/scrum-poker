using System;

namespace ScrumPoker.DataContracts
{
    public interface ITeam
    {
        Guid Id { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        Guid FacilitatorId { get; set; }
    }
}