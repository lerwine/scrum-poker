using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.DTO
{
    public interface IAppUser : IValidatableObject
    {
        Guid Id { get; set; }
        
        string DisplayName { get; set; }

        string UserName { get; set; }
    }
}