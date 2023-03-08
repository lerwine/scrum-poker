using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// Represents a color scheme.
/// </summary>
public class CardColor
{
    /// <summary>
    /// The unique identifier of the color scheme.
    /// </summary>
    public Guid Id { get; set; }

    private string _name = "";
    /// <summary>
    /// The name of the color.
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value.WsNormalizedOrEmptyIfNull(); }
    }

    private ColorModel.CssColor _fill;
    /// <summary>
    /// The fill color.
    /// </summary>
    public ColorModel.CssColor Fill
    {
        get { return _fill; }
        set { _fill = value; }
    }

    private ColorModel.CssColor _stroke;
    /// <summary>
    /// The stroke color.
    /// </summary>
    public ColorModel.CssColor Stroke
    {
        get { return _stroke; }
        set { _stroke = value; }
    }

    private ColorModel.CssColor _text;
    /// <summary>
    /// The color for the text.
    /// </summary>
    public ColorModel.CssColor Text
    {
        get { return _text; }
        set { _text = value; }
    }

    private readonly FKNavProperty<ColorSchema> _schema = new(e => e.Id);
    /// <summary>
    /// The unique identifier of the schema containing this color definition.
    /// </summary>
    public Guid SchemaId
    {
        get => _schema.ForeignKey;
        set => _schema.ForeignKey = value;
    }

    /// <summary>
    /// The schema containing this color definition.
    /// </summary>
    public ColorSchema? Schema
    {
        get => _schema.Model;
        set => _schema.Model = value;
    }

    private Collection<Participant> _participants = new();
    /// <summary>
    /// Participants which use this card color definition.
    /// </summary>
    public Collection<Participant> Participants
    {
        get { return _participants; }
        set { _participants = value ?? new Collection<Participant>(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<CardColor> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Schema).WithMany(t => t.CardColors).HasForeignKey(nameof (SchemaId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
