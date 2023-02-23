using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class SprintInitiative
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

#pragma warning disable CS8618
    public PlanningMeeting Meeting { get; set; }
#pragma warning restore CS8618

    internal static void OnBuildEntity(EntityTypeBuilder<SprintInitiative> builder)
    {
        _ = builder.HasKey(nameof(Id));
    }
}
