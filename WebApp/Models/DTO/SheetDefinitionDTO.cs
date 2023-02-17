namespace ScrumPoker.WebApp.Models.DTO;

public class SheetDefinitionDTO
{
    public int ID { get; set; }
    
    public int SheetNumber { get; set; }

    public string URL { get; set; } = "";
    
    public int MaxValue { get; set; }
}
