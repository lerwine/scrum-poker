using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class Epic
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

    private Collection<Milestone> _milestones = new();
    public Collection<Milestone> Milestones
    {
        get { return _milestones; }
        set { _milestones = value ?? new Collection<Milestone>(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<Epic> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Team).WithMany(t => t.Epics).HasForeignKey(nameof (TeamId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
