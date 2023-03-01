using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class TeamMember
{
    /// <summary>
    /// 
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8618
    public Team Team { get; set; }
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
    
    internal static void OnBuildEntity(EntityTypeBuilder<TeamMember> builder)
    {
        _ = builder.HasOne(p => p.Team).WithMany(d => d.Members).HasForeignKey(nameof(TeamId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(p => p.User).WithMany(d => d.Memberships).HasForeignKey(nameof(UserId)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(TeamId), nameof(UserId));
    }
}