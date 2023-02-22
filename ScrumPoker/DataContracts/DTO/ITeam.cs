using System;

namespace ScrumPoker.DataContracts.DTO
{
    public interface ITeam : IValidatableObject
    {
        Guid Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        Guid FacilitatorId { get; set; }
    }
}