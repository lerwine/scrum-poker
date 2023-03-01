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

    /// <summary>
    /// 
    /// </summary>
    public Guid DeckTypeId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public DeckType DeckType { get; set; }
#pragma warning restore CS8618

    /// <summary>
    /// 
    /// </summary>
    public Guid ColorSchemeId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public ColorSchema ColorScheme { get; set; }
#pragma warning restore CS8618
    
    /// <summary>
    /// 
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public Team Team { get; set; }
#pragma warning restore CS8618

    // TODO: Need to validate that the Initiative belongs to the same team as the current PlanningMeeting before being saved to DB
    /// <summary>
    /// 
    /// </summary>
    public Guid? InitiativeId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Initiative? Initiative { get; set; }

    // TODO: Need to validate that the Epic belongs to the same team as the current PlanningMeeting before being saved to DB
    /// <summary>
    /// 
    /// </summary>
    public Guid? EpicId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Epic? Epic { get; set; }

    // TODO: Need to validate that the Milestone belongs to the same team as the current PlanningMeeting before being saved to DB
    /// <summary>
    /// 
    /// </summary>
    public Guid? MilestoneId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Milestone? Milestone { get; set; }

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
        _ = builder.HasOne(p => p.DeckType).WithMany(d => d.Meetings).HasForeignKey(nameof(DeckTypeId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.ColorScheme).WithMany(d => d.Meetings).HasForeignKey(nameof(ColorSchemeId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(Id));
    }
}