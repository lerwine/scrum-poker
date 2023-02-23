using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class Participant
{
    public Guid Id { get; set; }

    public Guid? DrawnCardId { get; set; }

    public int PointsAssigned { get; set; }

    public int? ScrumCapacity { get; set; }

    public DateTime LastActivity { get; set; }
    
    public Guid MeetingId { get; set; }

#pragma warning disable CS8618
    public PlanningMeeting Meeting { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }

#pragma warning disable CS8618
    public UserProfile User { get; set; }
#pragma warning restore CS8618
    
    internal static void OnBuildEntity(EntityTypeBuilder<Participant> builder)
    {
        _ = builder.HasOne(p => p.Meeting).WithMany(d => d.Participants).HasForeignKey(nameof(MeetingId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.User).WithMany(d => d.Participation).HasForeignKey(nameof(UserId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(MeetingId), nameof(UserId));
    }
}
