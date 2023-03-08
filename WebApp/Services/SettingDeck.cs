namespace ScrumPoker.WebApp.Services;

public class SettingDeck
{
    public string? name;

    public string? description;

    public (string url, int width, int Hheight)? preview;

    public string[]? cards;

    public (string url, int maxValue)? printableSheets;
}
