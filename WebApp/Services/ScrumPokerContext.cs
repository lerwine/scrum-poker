using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ScrumPoker.WebApp.Models;

namespace ScrumPoker.WebApp.Services;

public class ScrumPokerContext : DbContext
{
    private readonly ILogger<ScrumPokerContext> _logger;

    public ScrumPokerContext(DbContextOptions<ScrumPokerContext> options)
        : base(options)
    {
        _logger = Program.Services.GetRequiredService<ILogger<ScrumPokerContext>>();
    }

    public DbSet<UserProfile> Profles { get; set; } = null!;
    
    public DbSet<Team> Teams { get; set; } = null!;
    
    public DbSet<TeamMember> TeamMembers { get; set; } = null!;
    
    public DbSet<PlanningMeeting> Meetings { get; set; } = null!;
    
    public DbSet<Participant> Participants { get; set; } = null!;
    
    public DbSet<SprintInitiative> SprintInitiatives { get; set; } = null!;
    
    public DbSet<SprintEpic> SprintEpics { get; set; } = null!;
    
    public DbSet<SprintMilestone> SprintMilestones { get; set; } = null!;
    
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Inherited class will have called SuppressFinalize if necessary.")]
    public override void Dispose()
    {
        _logger.LogInformation($"Disposing {nameof(ScrumPokerContext)}: {nameof(DbContextId.InstanceId)}={{{nameof(DbContextId.InstanceId)}}}, {nameof(DbContextId.Lease)}={{{nameof(DbContextId.Lease)}}}",
            ContextId.InstanceId, ContextId.Lease);
        base.Dispose();
    }

    private UserProfile? _userProfile;

    public async Task<UserProfile?> GetUserProfileAsync(CancellationToken cancellationToken)
    {
        IIdentity? identity = ClaimsPrincipal.Current?.Identity;
        if (identity is not null && identity.IsAuthenticated)
        {
            string? userName = identity.Name.NullIfEmpty();
            if (userName is not null)
            {
                userName = userName.ToLower();
                return await Profles.FirstOrDefaultAsync(p => p.UserName == userName, cancellationToken);
            }
        }
        return null;
    }
    
    public bool TryGetUserProfile([MaybeNullWhen(false)] out UserProfile result)
    {
        IIdentity? identity = ClaimsPrincipal.Current?.Identity;
        if (identity is not null && identity.IsAuthenticated)
        {
            string? userName = identity.Name.NullIfEmpty();
            if (userName is not null)
            {
                StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
                if (_userProfile is not null && comparer.Equals(_userProfile.UserName, userName))
                {
                    result = _userProfile;
                    return true;
                }
                foreach (UserProfile u in Profles)
                    if (comparer.Equals(u.UserName, userName))
                    {
                        _userProfile = result = u;
                        return true;
                    }
            }
        }
        _userProfile = result = null;
        return false;
    }
    
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder"> The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserProfile>(UserProfile.OnBuildEntity)
            .Entity<Team>(Team.OnBuildEntity)
            .Entity<TeamMember>(TeamMember.OnBuildEntity)
            .Entity<PlanningMeeting>(PlanningMeeting.OnBuildEntity)
            .Entity<Participant>(Participant.OnBuildEntity)
            .Entity<SprintInitiative>(SprintInitiative.OnBuildEntity)
            .Entity<SprintEpic>(SprintEpic.OnBuildEntity)
            .Entity<SprintMilestone>(SprintMilestone.OnBuildEntity);
    }
}