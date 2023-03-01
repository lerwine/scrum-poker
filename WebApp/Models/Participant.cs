using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class Participant
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? DrawnCardId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public CardDefinition DrawnCard { get; set; }

#pragma warning restore CS8618

    /// <summary>
    /// 
    /// </summary>
    public Guid CardColorId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public CardColor CardColor { get; set; }

#pragma warning restore CS8618

    /// <summary>
    /// 
    /// </summary>
    public int PointsAssigned { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? ScrumCapacity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime LastActivity { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public Guid MeetingId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public PlanningMeeting Meeting { get; set; }
#pragma warning restore CS8618

    /// <summary>
    /// 
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public UserProfile User { get; set; }
#pragma warning restore CS8618
    
    internal static void OnBuildEntity(EntityTypeBuilder<Participant> builder)
    {
        _ = builder.HasOne(p => p.Meeting).WithMany(d => d.Participants).HasForeignKey(nameof(MeetingId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.User).WithMany(d => d.Participation).HasForeignKey(nameof(UserId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.CardColor).WithMany(d => d.Participants).HasForeignKey(nameof(CardColorId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.DrawnCard).WithMany(d => d.Participants).HasForeignKey(nameof(DrawnCardId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(MeetingId), nameof(UserId));
    }
}
