using System.Collections.ObjectModel;

namespace ScrumPoker.WebApp.Models;

public class DeckType
{
    public int ID { get; set; }

    public string Name { get; set; } = "";
    
    public string Description { get; set; } = "";
    
    public ImageFileDimensions? Preview { get; set; }
    
    public Collection<CardDefinition>? Cards { get; set; }
    
    public Collection<OrderedSheetDefinition>? Sheets { get; set; }
}
