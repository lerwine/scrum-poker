using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// Defines a color scheme.
/// </summary>
public class ColorSchema
{
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
    }
}
