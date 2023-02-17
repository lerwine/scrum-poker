using System.Collections.ObjectModel;

namespace ScrumPoker.WebApp.Models.DTO;

public class GetSessionResponseDTO : SessionItemDTO
{
    public string Token { get; set; } = "";
    
    public DeckTypeDetailDTO? DeckType { get; set; }
    
    public Collection<TeamMemberSessionItemDTO>? TeamMembers { get; set; }
}
