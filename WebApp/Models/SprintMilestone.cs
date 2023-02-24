using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class SprintMilestone
{
    public Guid Id { get; set; }

    private string _title = "";
    public string Title
    {
        get { return _title; }
        set { _title = value.EmptyIfNullOrTrimmed(); }
    }

    private string? _description;
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }

    private DateTime? _startDate;
    public DateTime? StartDate
    {
        get { return _startDate; }
        set { _startDate = value.ToLocalDate(); }
    }

    private DateTime? _plannedEndDate;
    public DateTime? PlannedEndDate
    {
        get { return _plannedEndDate; }
        set { _plannedEndDate = value.ToLocalDate(); }
    }

    // TODO: Need to validate that the Epic belongs to the same team as the current Milestone before being saved to DB
    public Guid? EpicId { get; set; }

    public SprintEpic? Epic { get; set; }

    public Guid TeamId { get; set; }

#pragma warning disable CS8618
    public Team Team { get; set; }
#pragma warning restore CS8618

    private Collection<PlanningMeeting> _meetings = new();
    public Collection<PlanningMeeting> Meetings
    {
        get { return _meetings; }
        set { _meetings = value ?? new Collection<PlanningMeeting>(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<SprintMilestone> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Team).WithMany(t => t.Milestones).HasForeignKey(nameof (TeamId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(i => i.Epic).WithMany(t => t.Milestones).HasForeignKey(nameof (TeamId)).OnDelete(DeleteBehavior.Restrict);
    }
}