namespace ScrumPoker.WebApp.Services;

public class SettingDeck
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public (string Url, int Width, int Height)? Preview { get; set; }

    public string[]? Cards { get; set; }

    public (string URL, int MaxValue)? PrintableSheets { get; set; }
}
