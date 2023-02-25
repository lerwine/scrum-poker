using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class CardDefinition
{
    public Guid DeckTypeId { get; set; }

#pragma warning disable CS8618
    public DeckType DeckType { get; set; }
#pragma warning restore CS8618

    public int Order { get; set; }

    public int Value { get; set; }

    public string Symbol { get; set; } = "";
    
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
