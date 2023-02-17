namespace ScrumPoker.WebApp.Models.DTO;

public class CardDefinitionDTO
{
    public int ID { get; set; }

    public int Value { get; set; }

    public string Symbol { get; set; } = "";
    
    public CardType Type { get; set; }
    
    public string BaseName { get; set; } = "";
}
