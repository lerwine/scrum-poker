using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class CardDefinition
{
    private string _title = "";
    public string Title
    {
        get { return _title; }
        set { _title = value.EmptyIfNullOrTrimmed(); }
    }

    private string _symbolText = "";
    public string SymbolText
    {
        get { return _symbolText; }
        set { _symbolText = value.EmptyIfNullOrTrimmed(); }
    }

    private string? _symbolFont;
    public string? SymbolFont
    {
        get { return _symbolFont; }
        set { _symbolFont = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _upperSymbolPath;
    public string? UpperSymbolPath
    {
        get { return _upperSymbolPath; }
        set { _upperSymbolPath = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _middleSymbolPath;
    public string? MiddleSymbolPath
    {
        get { return _middleSymbolPath; }
        set { _middleSymbolPath = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _lowerSymbolPath;
    public string? LowerSymbolPath
    {
        get { return _lowerSymbolPath; }
        set { _lowerSymbolPath = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _description;
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }

    private string? _truncatedDescription;
    public string? TruncatedDescription
    {
        get { return _truncatedDescription; }
        set { _truncatedDescription = value.TrimmedOrNullIfEmpty(); }
    }

    public Guid DeckTypeId { get; set; }

#pragma warning disable CS8618
    public DeckType DeckType { get; set; }
#pragma warning restore CS8618

    public int Order { get; set; }

    public int? Value { get; set; }

    public int? MiddleSymbolTop { get; set; }

    public int? SmallSymbolFontSize { get; set; }

    public int? LargeSymbolFontSize { get; set; }

    public DataContracts.CardType Type { get; set; }
    
    private Collection<Participant> _participants = new();
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
