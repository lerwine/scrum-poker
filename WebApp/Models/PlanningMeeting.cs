using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class PlanningMeeting
{
    public Guid Id { get; set; }

    private string _title = "";
    public string Title
    {
        get { return _title; }
        set { _title = value.EmptyIfNullOrTrimmed(); }
    }

    public DataContracts.SessionStage Stage { get; set; }

    private string? _description = null;
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }
    
    public bool NoHalfPoint { get; set; }
    
    public bool NoZeroPoint { get; set; }
    
    private DateTime _meetingDate;
    public DateTime MeetingDate
    {
        get { return _meetingDate; }
        set { _meetingDate = value.ToLocalDate(); }
    }
    
    private DateTime? _plannedStartDate;
    public DateTime? PlannedStartDate
    {
        get { return _plannedStartDate; }
        set { _plannedStartDate = value.ToLocalDate(); }
    }

    private DateTime? _plannedEndDate;
    public DateTime? PlannedEndDate
    {
        get { return _plannedEndDate; }
        set { _plannedEndDate = value.ToLocalDate(); }
    }

    private int _currentScopePoints = 0;
    public int CurrentScopePoints
    {
        get { return _currentScopePoints; }
        set { _currentScopePoints = (value < 0) ? 0 : value; }
    }

    private int? _sprintCapacity;
    public int? SprintCapacity
    {
        get { return _sprintCapacity; }
        set { _sprintCapacity = (value.HasValue && value.Value < 1) ? null : value; }
    }

    private DateTime _lastActivity;
    public DateTime LastActivity
    {
        get { return _lastActivity; }
        set { _lastActivity = value.ToLocalDate(); }
    }

    public Guid DeckTypeId { get; set; }
    
    public Guid TeamId { get; set; }

#pragma warning disable CS8618
    public Team Team { get; set; }
#pragma warning restore CS8618

    // TODO: Need to validate that the Initiative belongs to the same team as the current PlanningMeeting before being saved to DB
    public Guid? InitiativeId { get; set; }

    public SprintInitiative? Initiative { get; set; }

    // TODO: Need to validate that the Epic belongs to the same team as the current PlanningMeeting before being saved to DB
    public Guid? EpicId { get; set; }

    public SprintEpic? Epic { get; set; }

    // TODO: Need to validate that the Milestone belongs to the same team as the current PlanningMeeting before being saved to DB
    public Guid? MilestoneId { get; set; }

    public SprintMilestone? Milestone { get; set; }

    private Collection<Participant> _participants = new();
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
        _ = builder.HasKey(nameof(Id));
    }
}