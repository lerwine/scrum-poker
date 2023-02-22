using System;

namespace ScrumPoker.DataContracts.DTO
{
    public interface ITeamMember : IValidatableObject
    {
        Guid TeamId { get; set; }

        Guid UserId { get; set; }
    }
}