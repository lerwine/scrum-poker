using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public interface ITeamMember : IParticipant
{
    int? DrawnCardID { get; set; }
}

public class TeamMember : Participant
{
    public int? DrawnCardID { get; set; }

    public string? Explanation { get; set; }

    public Guid SessionID { get; set; }

    public ScrumSession? Session { get; set; }
    
    internal static void OnBuildEntity(EntityTypeBuilder<TeamMember> builder)
    {
        _ = builder.HasOne(p => p.Session).WithMany(d => d.TeamMembers).HasForeignKey(nameof(SessionID)).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasKey(nameof(ID));
        _ = builder.HasIndex(nameof(Token));
    }
}