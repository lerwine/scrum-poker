using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class TeamMember
{
    public Guid TeamId { get; set; }

#pragma warning disable CS8618
    public Team Team { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }

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