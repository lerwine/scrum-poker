namespace ScrumPoker.WebApp.Models.DTO;

public class UpdateOrganizerRequestDTO
{
    public Guid ID { get; set; }

    public string Name { get; set; } = "";

    public string EmailAddress { get; set; } = "";
}
