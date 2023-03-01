using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class SheetDefinition
{
    /// <summary>
    /// 
    /// </summary>
    public int SheetNumber { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string URL { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public int MaxValue { get; set; }

    private readonly FKNavProperty<CardDeck> _deck = new(e => e.Id);
    /// <summary>
    /// 
    /// </summary>
    public Guid DeckId
    {
        get => _deck.ForeignKey;
        set => _deck.ForeignKey = value;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public CardDeck? Deck
    {
        get => _deck.Model;
        set => _deck.Model = value;
    }

    internal static void OnBuildEntity(EntityTypeBuilder<SheetDefinition> builder)
    {
        _ = builder.HasKey(nameof(SheetNumber), nameof(DeckId));
        _ = builder.HasOne(i => i.Deck).WithMany(t => t.Sheets).HasForeignKey(nameof (DeckId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.Property(c => c.URL).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
