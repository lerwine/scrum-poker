namespace ScrumPoker.WebApp.Models.DTO;

public class SessionAdminItemDTO : SessionItemDTO
{
    public string Token { get; set; } = "";
    
    public OrganizerAdminItemDTO? Organizer { get; set; }
}
