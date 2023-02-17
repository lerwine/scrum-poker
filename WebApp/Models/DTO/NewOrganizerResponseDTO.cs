namespace ScrumPoker.WebApp.Models.DTO;

public class NewOrganizerResponseDTO
{
    public Guid ID { get; set; }

    public string Name { get; set; } = "";

    public string EmailAddress { get; set; } = "";

    public string Token { get; set; } = "";

    public DateTime LastActivity { get; set; }
}
