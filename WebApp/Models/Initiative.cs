using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class Initiative
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

    private string? _description;
    /// <summary>
    /// 
    /// </summary>
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }


    private DateTime? _startDate;
    /// <summary>
    /// 
    /// </summary>
    public DateTime? StartDate
    {
        get { return _startDate; }
        set { _startDate = value.ToLocalDate(); }
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

    private readonly FKNavProperty<Team> _team = new(e => e.Id);
    /// <summary>
    /// The unique identifier for the initiative's team.
    /// </summary>
    public Guid TeamId
    {
        get => _team.ForeignKey;
        set => _team.ForeignKey = value;
    }

    /// <summary>
    /// The initiative's team.
    /// </summary>
    public Team? Team
    {
        get => _team.Model;
        set => _team.Model = value;
    }

    private Collection<PlanningMeeting> _meetings = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<PlanningMeeting> Meetings
    {
        get { return _meetings; }
        set { _meetings = value ?? new Collection<PlanningMeeting>(); }
    }

    internal static void OnBuildEntity(EntityTypeBuilder<Initiative> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(i => i.Team).WithMany(t => t.Initiatives).HasForeignKey(nameof (TeamId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.Property(c => c.Title).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
