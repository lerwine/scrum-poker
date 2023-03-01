using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class DeckCard
{
    private readonly FKNavProperty<CardDeck> _deck = new(e => e.Id);
    public Guid DeckId
    {
        get => _deck.ForeignKey;
        set => _deck.ForeignKey = value;
    }

    public CardDeck? Deck
    {
        get => _deck.Model;
        set => _deck.Model = value;
    }

    private readonly FKNavProperty<CardDefinition> _card = new(e => e.Id);
    public Guid CardId
    {
        get => _card.ForeignKey;
        set => _card.ForeignKey = value;
    }
    
    public CardDefinition? Definition
    {
        get => _card.Model;
        set => _card.Model = value;
    }

    public int Order { get; set; }

    internal static void OnBuildEntity(EntityTypeBuilder<DeckCard> builder)
    {
        _ = builder.HasKey(nameof(DeckId), nameof(CardId));
        _ = builder.HasOne(c => c.Deck).WithMany(d => d.Cards).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(c => c.Definition).WithMany(d => d.Decks).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}