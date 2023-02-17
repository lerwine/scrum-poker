using System.Collections.ObjectModel;

namespace ScrumPoker.WebApp.Models.DTO;

public class DeckTypeDetailDTO : DeckTypeItemDTO
{
    public Collection<CardDefinitionDTO>? Cards { get; set; }
    public Collection<SheetDefinitionDTO>? Sheets { get; set; }
}
