using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class CardDeck
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    private string _name = "";
    /// <summary>
    /// 
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value.WsNormalized(); }
    }

    private string? _description;
    /// <summary>
    /// 
    /// </summary>
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }

    private string _previewUrl = "";
    /// <summary>
    /// 
    /// </summary>
    public string PreviewUrl
    {
        get { return _previewUrl; }
        set { _previewUrl = value.EmptyIfNullOrTrimmed(); }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Height { get; set; }
    
    // FIXME: CardDeck needs to be in a many-to-many relationship with CardDefinition
    private Collection<DeckCard> _cards = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<DeckCard> Cards
    {
        get { return _cards; }
        set { _cards = value ?? new Collection<DeckCard>(); }
    }

    private Collection<SheetDefinition> _sheets = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<SheetDefinition> Sheets
    {
        get { return _sheets; }
        set { _sheets = value ?? new Collection<SheetDefinition>(); }
    }
    
    private Collection<PlanningMeeting> _meetings = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<PlanningMeeting> Meetings
    {
        get { return _meetings; }
        set { _meetings = value ?? new Collection<PlanningMeeting>(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<CardDeck> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
