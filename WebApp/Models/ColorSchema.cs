using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class ColorSchema
{
    public Guid Id { get; set; }

    private string _name = "";
    public string Name
    {
        get { return _name; }
        set { _name = value.EmptyIfNullOrTrimmed(); }
    }

    private ColorModel.CssColor _votingFill;
    public ColorModel.CssColor VotingFill
    {
        get { return _votingFill; }
        set { _votingFill = value; }
    }

    private ColorModel.CssColor _votingStroke;
    public ColorModel.CssColor VotingStroke
    {
        get { return _votingStroke; }
        set { _votingStroke = value; }
    }

    private ColorModel.CssColor _votingText;
    public ColorModel.CssColor VotingText
    {
        get { return _votingText; }
        set { _votingText = value; }
    }

    private Collection<CardColor> _cardColors = new();
    public Collection<CardColor> CardColors
    {
        get { return _cardColors; }
        set { _cardColors = value ?? new Collection<CardColor>(); }
    }

    private Collection<PlanningMeeting> _meetings = new();
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
