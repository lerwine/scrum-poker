namespace ScrumPoker.WebApp.Services;

public class DeckSettings
{
    public string Name { get; set; } = "";
    public DeckPreviewSettings? Preview { get; set; }
    public string Description { get; set; } = "";
    public int[]? Cards { get; set; }
    public int[]? Sheets { get; set; }
}
