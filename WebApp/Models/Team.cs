using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;
public class Team
{
    public Guid Id { get; set; }

    private string _title = "";
    /// <summary>
    /// The title of the current team.
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value.EmptyIfNullOrTrimmed(); }
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

    public Guid FacilitatorId { get; set; }

#pragma warning disable CS8618
    public UserProfile Facilitator { get; set; }
#pragma warning restore CS8618

    private Collection<TeamMember> _members = new();
    public Collection<TeamMember> Members
    {
        get { return _members; }
        set { _members = value ?? new Collection<TeamMember>(); }
    }
    
    private Collection<PlanningMeeting> _meetings = new();
    public Collection<PlanningMeeting> Meetings
    {
        get { return _meetings; }
        set { _meetings = value ?? new Collection<PlanningMeeting>(); }
    }
    
    internal static void OnBuildEntity(EntityTypeBuilder<Team> builder)
    {
        _ = builder.HasOne(ss => ss.Facilitator).WithMany(d => d.Facilitated).HasForeignKey(nameof(FacilitatorId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(Id));
    }
}