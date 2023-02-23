using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

[Obsolete("Use PlanningMeeting")]
public class ScrumSession : IScrumSession
{
    public Guid ID { get; set; }

    public string Token { get; set; } = "";

    public Guid OrganizerID { get; set; }

    public SessionOrganizer? Organizer { get; set; }
    
    public string Title { get; set; } = "";

    public SessionStage Stage { get; set; }

    public string? Instructions { get; set; }
    
    public int DeckTypeID { get; set; }
    
    public bool NoHalfPoint { get; set; }
    
    public bool NoZeroPoint { get; set; }
    
    public DateTime LastActivity { get; set; }

    public Collection<TeamMember>? TeamMembers { get; set; }

    // public DateTime GetLastActivity()
    // {
    //     SessionOrganizer? organizer = Organizer;
    //     DateTime lastActivity = (organizer is null || organizer.LastActivity < LastActivity) ? LastActivity : organizer.LastActivity;
    //     Collection<TeamMember>? teamMembers = TeamMembers;
    //     if (teamMembers != null)
    //         foreach (TeamMember member in teamMembers)
    //         {
    //             if (member.LastActivity > lastActivity)
    //                 lastActivity = member.LastActivity;
    //         }
    //     return lastActivity;
    // }

    internal static void OnBuildEntity(EntityTypeBuilder<ScrumSession> builder)
    {
        _ = builder.HasOne(ss => ss.Organizer).WithMany(d => d.Sessions).HasForeignKey(nameof(OrganizerID)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(ID));
        _ = builder.HasIndex(nameof(Token));
    }
}
