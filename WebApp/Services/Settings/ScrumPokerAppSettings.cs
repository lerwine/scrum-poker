namespace ScrumPoker.WebApp.Services.Settings;

public class ScrumPokerAppSettings
{
    public string? dbFile;
    public string? adminUserName;

    public string? adminDisplayName;

    public ColorSchemeSetting[] defaultColorSchemes = null!;

    public SettingDeck[] defaultDecks = null!;

    public SettingCard[] defaultCards = null!;
}
