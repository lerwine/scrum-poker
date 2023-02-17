using System.Collections.ObjectModel;

namespace ScrumPoker.WebApp.Models.DTO;

public class GetAllSessionsResponseDTO
{
    public Collection<SessionAdminItemDTO>? Organizers { get; set; }
}
