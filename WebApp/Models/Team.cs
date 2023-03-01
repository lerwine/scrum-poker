using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class Team
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    private string _title = "";
    /// <summary>
    /// The title of the current team.
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value.WsNormalized(); }
    }

    private string? _description;
    /// <summary>
    /// The description of the current tem.
    /// </summary>
    public string? Description
    {
        get { return _description; }
        set { _description = value.TrimmedOrNullIfEmpty(); }
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid FacilitatorId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public UserProfile Facilitator { get; set; }
#pragma warning restore CS8618

    private Collection<TeamMember> _members = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<TeamMember> Members
    {
        get { return _members; }
        set { _members = value ?? new Collection<TeamMember>(); }
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

    private Collection<Epic> _epics = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<Epic> Epics
    {
        get { return _epics; }
        set { _epics = value ?? new Collection<Epic>(); }
    }
    
    private Collection<Initiative> _initiatives = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<Initiative> Initiatives
    {
        get { return _initiatives; }
        set { _initiatives = value ?? new Collection<Initiative>(); }
    }
    
    private Collection<Milestone> _milestones = new();
    /// <summary>
    /// 
    /// </summary>
    public Collection<Milestone> Milestones
    {
        get { return _milestones; }
        set { _milestones = value ?? new Collection<Milestone>(); }
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<Team> builder)
    {
        _ = builder.HasOne(ss => ss.Facilitator).WithMany(d => d.Facilitated).HasForeignKey(nameof(FacilitatorId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(Id));
    }
}