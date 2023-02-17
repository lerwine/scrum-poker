using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.WebApp.Models.DTO;

public class OrganizerDetailsDTO : OrganizerAdminItemDTO
{
    public Collection<ScrumSessionItemDTO>? Sessions { get; set; }
}
