using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Models;

public class ScrumPokerContext : DbContext
{
    public ScrumPokerContext(DbContextOptions<ScrumPokerContext> options)
        : base(options) { }

    public DbSet<ScrumSession> Sessions { get; set; } = null!;
    
    public DbSet<TeamMember> TeamMembers { get; set; } = null!;
    
    public DbSet<SessionOrganizer> Organizers { get; set; } = null!;
    
    internal async Task<ScrumSession?> FindSessionAsync(string? token, SessionTokenService tokenService, bool includeMembers = false, bool includeOrganizer = false)
    {
        if (tokenService.TryUnrotectToken(token, out string? decryptedToken))
        {
            if (includeMembers)
                return includeOrganizer ? await Sessions.Include(o => o.Organizer).Include(o => o.TeamMembers).FirstOrDefaultAsync(o => o.Token == decryptedToken) :
                    await Sessions.Include(o => o.TeamMembers).FirstOrDefaultAsync(o => o.Token == decryptedToken);
            return includeOrganizer ? await Sessions.Include(o => o.Organizer).FirstOrDefaultAsync(o => o.Token == decryptedToken) :
                await Sessions.FirstOrDefaultAsync(o => o.Token == decryptedToken);
        }
        return null;
    }
    
    internal async Task<SessionOrganizer?> FindOrganizerAsync(string? token, SessionTokenService tokenService, bool includeSessions = false)
    {
        if (tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return includeSessions ? await Organizers.Include(o => o.Sessions).FirstOrDefaultAsync(o => o.Token == decryptedToken) :
                await Organizers.FirstOrDefaultAsync(o => o.Token == decryptedToken);
        return null;
    }
    
    internal async Task<TeamMember?> FindTeamMemberAsync(string? token, SessionTokenService tokenService, bool includeSession = false)
    {
        if (tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return includeSession ? await TeamMembers.Include(o => o.Session).FirstOrDefaultAsync(o => o.Token == decryptedToken) :
                await TeamMembers.FirstOrDefaultAsync(o => o.Token == decryptedToken);
        return null;
    }
    
    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder"> The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScrumSession>(ScrumSession.OnBuildEntity)
        .Entity<TeamMember>(TeamMember.OnBuildEntity)
        .Entity<SessionOrganizer>(SessionOrganizer.OnBuildEntity);
    }
}