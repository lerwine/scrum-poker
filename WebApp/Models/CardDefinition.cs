using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

// TODO: CardDefinition needs to be in a many-to-many relationship with DeckType
/// <summary>
/// 
/// </summary>
public class CardDefinition
{
    private string _title = "";
    /// <summary>
    /// 
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value.WsNormalized(); }
    }

    private string _symbolText = "";
    /// <summary>
    /// 
    /// </summary>
    public string SymbolText
    {
        get { return _symbolText; }
        set { _symbolText = value.EmptyIfNullOrTrimmed(); }
    }

    private string? _symbolFont;
    /// <summary>
    /// 
    /// </summary>
    public string? SymbolFont
    {
        get { return _symbolFont; }
        set { _symbolFont = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _upperSymbolPath;
    /// <summary>
    /// 
    /// </summary>
    public string? UpperSymbolPath
    {
        get { return _upperSymbolPath; }
        set { _upperSymbolPath = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _middleSymbolPath;
    /// <summary>
    /// 
    /// </summary>
    public string? MiddleSymbolPath
    {
        get { return _middleSymbolPath; }
        set { _middleSymbolPath = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _lowerSymbolPath;
    /// <summary>
    /// 
    /// </summary>
    public string? LowerSymbolPath
    {
        get { return _lowerSymbolPath; }
        set { _lowerSymbolPath = value.TrimmedOrNullIfEmpty(); }
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

    private string? _truncatedDescription;
    /// <summary>
    /// 
    /// </summary>
    public string? TruncatedDescription
    {
        get { return _truncatedDescription; }
        set { _truncatedDescription = value.TrimmedOrNullIfEmpty(); }
    }

    /// <summary>
    /// The unique identifier of the deck type definition that uses this card definition.
    /// </summary>
    public Guid DeckTypeId { get; set; }

    /// <summary>
    /// The deck type definition that uses this card definition.
    /// </summary>
#pragma warning disable CS8618
    public DeckType DeckType { get; set; }
#pragma warning restore CS8618

    /// <summary>
    /// The deck order for the card.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// The points value for the card.
    /// </summary>
    public int? Value { get; set; }

    /// <summary>
    /// The top position, in pixels, for the large middle symbol.
    /// </summary>
    public int? MiddleSymbolTop { get; set; }

    /// <summary>
    /// The font size for small symbol text.
    /// </summary>
    public int? SmallSymbolFontSize { get; set; }

    /// <summary>
    /// The font size for large symbol text.
    /// </summary>
    public int? LargeSymbolFontSize { get; set; }

    /// <summary>
    /// The general card type.
    /// </summary>
    public DataContracts.CardType Type { get; set; }
    
    private Collection<Participant> _participants = new();
    /// <summary>
    /// Participants which have selected this card.
    /// </summary>
    public Collection<Participant> Participants
    {
        get { return _participants; }
        set { _participants = value ?? new Collection<Participant>(); }
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<CardDefinition> builder)
    {
        _ = builder.HasKey(nameof(DeckTypeId), nameof(Order));
        _ = builder.HasOne(i => i.DeckType).WithMany(t => t.Cards).HasForeignKey(nameof (DeckTypeId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
