using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class PlanningMeeting
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    private string _title = "";
    /// <summary>
    /// 
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value.WsNormalized(); }
    }

    /// <summary>
    /// 
    /// </summary>
    public DataContracts.SessionStage Stage { get; set; }

    private string? _description = null;
    /// <summary>
    /// 
    /// </summary>
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public bool NoHalfPoint { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool NoZeroPoint { get; set; }
    
    private DateTime _meetingDate;
    /// <summary>
    /// 
    /// </summary>
    public DateTime MeetingDate
    {
        get { return _meetingDate; }
        set { _meetingDate = value.ToLocalDate(); }
    }
    
    private DateTime? _plannedStartDate;
    /// <summary>
    /// 
    /// </summary>
    public DateTime? PlannedStartDate
    {
        get { return _plannedStartDate; }
        set { _plannedStartDate = value.ToLocalDate(); }
    }

    private DateTime? _plannedEndDate;
    /// <summary>
    /// 
    /// </summary>
    public DateTime? PlannedEndDate
    {
        get { return _plannedEndDate; }
        set { _plannedEndDate = value.ToLocalDate(); }
    }

    private int _currentScopePoints = 0;
    /// <summary>
    /// 
    /// </summary>
    public int CurrentScopePoints
    {
        get { return _currentScopePoints; }
        set { _currentScopePoints = (value < 0) ? 0 : value; }
    }

    private int? _sprintCapacity;
    /// <summary>
    /// 
    /// </summary>
    public int? SprintCapacity
    {
        get { return _sprintCapacity; }
        set { _sprintCapacity = (value.HasValue && value.Value < 1) ? null : value; }
    }

    private DateTime _lastActivity;
    /// <summary>
    /// 
    /// </summary>
    public DateTime LastActivity
    {
        get { return _lastActivity; }
        set { _lastActivity = value.ToLocalDate(); }
    }

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

    private readonly FKNavProperty<ColorSchema> _colorScheme = new(e => e.Id);
    /// <summary>
    /// 
    /// </summary>
    public Guid ColorSchemeId
    {
        get => _colorScheme.ForeignKey;
        set => _colorScheme.ForeignKey = value;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public ColorSchema? ColorScheme
    {
        get => _colorScheme.Model;
        set => _colorScheme.Model = value;
    }
    
    private readonly FKNavProperty<Team> _team = new(e => e.Id);
    /// <summary>
    /// The unique identifier for the planning meeting's team.
    /// </summary>
    public Guid TeamId
    {
        get => _team.ForeignKey;
        set => _team.ForeignKey = value;
    }

    /// <summary>
    /// The planning meeting's team.
    /// </summary>
    public Team? Team
    {
        get => _team.Model;
        set => _team.Model = value;
    }

    private readonly FKOptionalNavProperty<Initiative> _initiative = new(e => e.Id);
    // TODO: Need to validate that the Initiative belongs to the same team as the current PlanningMeeting before being saved to DB
    /// <summary>
    /// 
    /// </summary>
    public Guid? InitiativeId
    {
        get => _initiative.ForeignKey;
        set => _initiative.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public Initiative? Initiative
    {
        get => _initiative.Model;
        set => _initiative.Model = value;
    }

    private readonly FKOptionalNavProperty<Epic> _epic = new(e => e.Id);
    // TODO: Need to validate that the Epic belongs to the same team as the current PlanningMeeting before being saved to DB
    /// <summary>
    /// 
    /// </summary>
    public Guid? EpicId
    {
        get => _epic.ForeignKey;
        set => _epic.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public Epic? Epic
    {
        get => _epic.Model;
        set => _epic.Model = value;
    }

    private readonly FKOptionalNavProperty<Milestone> _milestone = new(e => e.Id);
    // TODO: Need to validate that the Milestone belongs to the same team as the current PlanningMeeting before being saved to DB
    /// <summary>
    /// 
    /// </summary>
    public Guid? MilestoneId
    {
        get => _milestone.ForeignKey;
        set => _milestone.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public Milestone? Milestone
    {
        get => _milestone.Model;
        set => _milestone.Model = value;
    }

    private Collection<Participant> _participants = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<Participant> Participants
    {
        get { return _participants; }
        set { _participants = value ?? new Collection<Participant>(); }
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<PlanningMeeting> builder)
    {
        _ = builder.HasOne(p => p.Initiative).WithMany(d => d.Meetings).HasForeignKey(nameof(InitiativeId)).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.Epic).WithMany(d => d.Meetings).HasForeignKey(nameof(EpicId)).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.Milestone).WithMany(d => d.Meetings).HasForeignKey(nameof(MilestoneId)).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.Team).WithMany(d => d.Meetings).HasForeignKey(nameof(TeamId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.Deck).WithMany(d => d.Meetings).HasForeignKey(nameof(DeckId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.ColorScheme).WithMany(d => d.Meetings).HasForeignKey(nameof(ColorSchemeId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.Title).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}