using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

public class SessionOrganizer : Participant
{
    public Collection<ScrumSession>? Sessions { get; set; }
    
    internal static void OnBuildEntity(EntityTypeBuilder<SessionOrganizer> builder)
    {
        _ = builder.HasKey(nameof(ID));
        _ = builder.HasIndex(nameof(Token));
    }
}