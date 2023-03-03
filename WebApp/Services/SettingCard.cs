namespace ScrumPoker.WebApp.Services;

public class SettingCard
{
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public string? TruncatedDescription { get; set; }
    
    public string? SymbolText { get; set; }
    
    public string? SymbolFont { get; set; }
    
    public string? UpperSymbolPath { get; set; }
    
    public string? MiddleSymbolPath { get; set; }
    
    public string? LowerSymbolPath { get; set; }
    
    public int? Value { get; set; }
    
    public int? MiddleSymbolTop { get; set; }

    public int? SmallSymbolFontSize { get; set; }

    public int? LargeSymbolFontSize { get; set; }

    public DataContracts.CardType Type { get; set; }
}
