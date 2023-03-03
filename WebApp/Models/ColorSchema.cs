using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// Defines a color scheme.
/// </summary>
public class ColorSchema
{
    private static readonly (string Name, ColorModel.NamedColors Fill, ColorModel.NamedColors Stroke, ColorModel.NamedColors Text) DEFAULT_COLOR_SCHEMA = ("Default4", ColorModel.NamedColors.Gainsboro, ColorModel.NamedColors.Silver, ColorModel.NamedColors.Gray);
    private static readonly (string Name, ColorModel.NamedColors Fill, ColorModel.NamedColors Stroke, ColorModel.NamedColors Text)[] DEFAULT_CARD_COLORS = {
        ("Blue", ColorModel.NamedColors.LightCyan, ColorModel.NamedColors.Blue, ColorModel.NamedColors.Black),
        ("Green", ColorModel.NamedColors.PaleGreen, ColorModel.NamedColors.Green, ColorModel.NamedColors.Black),
        ("Red", ColorModel.NamedColors.MistyRose, ColorModel.NamedColors.DarkRed, ColorModel.NamedColors.Black),
        ("Yellow", ColorModel.NamedColors.LemonChiffon, ColorModel.NamedColors.DarkGoldenRod, ColorModel.NamedColors.Black)
    };
    
    /// <summary>
    /// The unique identifier of the color scheme.
    /// </summary>
    public Guid Id { get; set; }

    private string _name = "";
    /// <summary>
    /// The name of the color scheme.
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value.WsNormalized(); }
    }

    private ColorModel.CssColor _votingFill;
    /// <summary>
    /// The fill color for the "voting" card.
    /// </summary>
    public ColorModel.CssColor VotingFill
    {
        get { return _votingFill; }
        set { _votingFill = value; }
    }

    private ColorModel.CssColor _votingStroke;
    /// <summary>
    /// The stroke color for the "voting" card.
    /// </summary>
    public ColorModel.CssColor VotingStroke
    {
        get { return _votingStroke; }
        set { _votingStroke = value; }
    }

    private ColorModel.CssColor _votingText;
    /// <summary>
    /// The color for the text for the "voting" card.
    /// </summary>
    public ColorModel.CssColor VotingText
    {
        get { return _votingText; }
        set { _votingText = value; }
    }

    private Collection<CardColor> _cardColors = new();
    /// <summary>
    /// Card deck colors.
    /// </summary>
    public Collection<CardColor> CardColors
    {
        get { return _cardColors; }
        set { _cardColors = value ?? new Collection<CardColor>(); }
    }

    private Collection<PlanningMeeting> _meetings = new();
    /// <summary>
    /// The sprint planning meetings that use this color schema.
    /// </summary>
    public Collection<PlanningMeeting> Meetings
    {
        get { return _meetings; }
        set { _meetings = value ?? new Collection<PlanningMeeting>(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<ColorSchema> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Name).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
