using System.Collections.ObjectModel;
using ScrumPoker.WebApp.Models;

namespace ScrumPoker.WebApp.Services;

public class ScrumPokerAppSettings
{
    public string? DbFile { get; set; }

    public string? AdminUserName { get; set; }

    public string? AdminDisplayName { get; set; }

    public (string Name, string VotingFill, string VotingStroke, string VotingText, (string Name, string Fill, string Stroke, string Text)[] CardColors)[]? DefaultColorSchemes { get; set; }

    public SettingDeck[]? DefaultDecks { get; set; }

    public SettingCard[]? DefaultCards { get; set; }
}
