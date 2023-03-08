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
        set { _title = value.WsNormalizedOrEmptyIfNull(); }
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

    private readonly FKNavProperty<UserProfile> _facilitator = new(e => e.Id);
    /// <summary>
    ///
    /// </summary>
    public Guid FacilitatorId
    {
        get => _facilitator.ForeignKey;
        set => _facilitator.ForeignKey = value;
    }

    /// <summary>
    ///
    /// </summary>
    public UserProfile? Facilitator
    {
        get => _facilitator.Model;
        set => _facilitator.Model = value;
    }

    private Collection<UserProfile> _members = new();
    /// <summary>
    ///
    /// </summary>
    public Collection<UserProfile> Members
    {
        get { return _members; }
        set { _members = value ?? new Collection<UserProfile>(); }
    }

    private Collection<TeamMember> _memberships = new();
    public Collection<TeamMember> Memberships
    {
        get { return _memberships; }
        set { _memberships = value ?? new Collection<TeamMember>(); }
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
        _ = builder.HasKey(nameof(Id));
        _ = builder.HasOne(ss => ss.Facilitator).WithMany(d => d.Facilitated).HasForeignKey(nameof(FacilitatorId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasMany(t => t.Members).WithMany(u => u.Teams).UsingEntity<TeamMember>(b => b.HasOne(m => m.User).WithMany(u => u.Memberships).HasForeignKey(m => m.UserId),
            b => b.HasOne(m => m.Team).WithMany(t => t.Memberships).HasForeignKey(m => m.TeamId),
            b => b.HasKey(m => new { m.TeamId, m.UserId }));
        _ = builder.Property(c => c.Title).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}
