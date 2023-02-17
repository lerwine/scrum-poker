namespace ScrumPoker.WebApp.Models.DTO;

public class DeckTypeItemDTO
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ImageFileDimensionsDTO? Preview { get; set; }
}