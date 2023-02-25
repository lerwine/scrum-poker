using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class CardColor
{
    public Guid Id { get; set; }

    private string _name = "";
    public string Name
    {
        get { return _name; }
        set { _name = value.EmptyIfNullOrTrimmed(); }
    }

    private ColorModel.CssColor _fill;
    public ColorModel.CssColor VotingFill
    {
        get { return _fill; }
        set { _fill = value; }
    }

    private ColorModel.CssColor _stroke;
    public ColorModel.CssColor VotingStroke
    {
        get { return _stroke; }
        set { _stroke = value; }
    }

    private ColorModel.CssColor _text;
    public ColorModel.CssColor VotingText
    {
        get { return _text; }
        set { _text = value; }
    }

    public Guid SchemaId { get; set; }

#pragma warning disable CS8618
    public ColorSchema Schema { get; set; }
#pragma warning restore CS8618

    private Collection<Participant> _participants = new();
    public Collection<Participant> Participants
    {
        get { return _participants; }
        set { _participants = value ?? new Collection<Participant>(); }
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<CardColor> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Schema).WithMany(t => t.CardColors).HasForeignKey(nameof (SchemaId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}