using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;
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
    
    public DbSet<PlanningMeeting> Meetings { get; set; } = null!;
    
    public DbSet<Participant> Participants { get; set; } = null!;
    
    public DbSet<Initiative> Initiatives { get; set; } = null!;
    
    public DbSet<Epic> Epics { get; set; } = null!;
    
    public DbSet<Milestone> Milestones { get; set; } = null!;
    
    // FIXME: Rename to Decks
    public DbSet<CardDeck> DeckTypes { get; set; } = null!;

    // FIXME: Add entity for many-to-many relationship with CardDeck
    public DbSet<CardDefinition> Cards { get; set; } = null!;
    
    public DbSet<ColorSchema> ColorSchemas { get; set; } = null!;
    
    public DbSet<CardColor> CardColors { get; set; } = null!;
    
    public DbSet<SheetDefinition> PrintableSheets { get; set; } = null!;
    
    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Inherited class will have called SuppressFinalize if necessary.")]
    public override void Dispose()
    {
        _logger.LogInformation($"Disposing {nameof(ScrumPokerContext)}: {nameof(DbContextId.InstanceId)}={{{nameof(DbContextId.InstanceId)}}}, {nameof(DbContextId.Lease)}={{{nameof(DbContextId.Lease)}}}",
            ContextId.InstanceId, ContextId.Lease);
        base.Dispose();
    }

    private UserProfile? _userProfile;

    public bool TryGetCurrentIdentityName([MaybeNullWhen(false)] out string result)
    {
        IIdentity? identity = ClaimsPrincipal.Current?.Identity;
        if (identity is not null && identity.IsAuthenticated)
        {
            if ((result = identity.Name.NullIfEmpty()) is not null)
            {
                result = result.ToLower();
                return true;
            }
        }
        else
            result = null;
        return false;   
    }

    public bool TryFindUserProfile(IEnumerable<UserProfile> source, [MaybeNullWhen(false)] out UserProfile userProfile)
    {
        if (TryGetCurrentIdentityName(out string? userName))
            return (userProfile = source.FirstOrDefault(p => p.UserName == userName)) is not null;
        userProfile = null;
        return false;
    }

    public bool TryFindUserProfile(UserProfile? userProfile, IEnumerable<UserProfile> source, [MaybeNullWhen(false)] out UserProfile result)
    {
        if (TryGetCurrentIdentityName(out string? userName))
        {
            if (userProfile is not null && userProfile.UserName == userName)
            {
                result = userProfile;
                return true;
            }
            return (result = source.FirstOrDefault(p => p.UserName == userName)) is not null;
        }
        result = null;
        return false;
    }

    public async Task<UserProfile?> GetUserProfileAsync(CancellationToken cancellationToken)
    {
        if (TryGetCurrentIdentityName(out string? userName))
        {
            if (_userProfile is null || _userProfile.UserName != userName)
                _userProfile = await Profles.FirstOrDefaultAsync(p => p.UserName == userName, cancellationToken);
        }
        else
            _userProfile = null;
        return _userProfile;
    }

    public async Task<UserProfile?> GetUserProfileAsync(Func<IQueryable<UserProfile>, IQueryable<UserProfile>> onQuery, CancellationToken cancellationToken)
    {
        if (TryGetCurrentIdentityName(out string? userName))
            _userProfile = await onQuery(Profles).FirstOrDefaultAsync(p => p.UserName == userName, cancellationToken);
        else
            _userProfile = null;
        return _userProfile;
    }
    
    public bool TryGetUserProfile([MaybeNullWhen(false)] out UserProfile result)
    {
        if (TryGetCurrentIdentityName(out string? userName))
        {
            if ((_userProfile is not null && _userProfile.UserName == userName) || (_userProfile = Profles.FirstOrDefault(u => u.UserName == userName)) is not null)
            {
                result = _userProfile;
                return true;
            }
        }
        else
            _userProfile = null;
        result = null;
        return false;
    }
    
    /// <summary>
    /// Configures the data model.
    /// </summary>
    /// <param name="modelBuilder"> The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserProfile>(UserProfile.OnBuildEntity)
            .Entity<Team>(Team.OnBuildEntity)
            .Entity<PlanningMeeting>(PlanningMeeting.OnBuildEntity)
            .Entity<Participant>(Participant.OnBuildEntity)
            .Entity<Initiative>(Initiative.OnBuildEntity)
            .Entity<Epic>(Epic.OnBuildEntity)
            .Entity<Milestone>(Milestone.OnBuildEntity)
            .Entity<CardDeck>(CardDeck.OnBuildEntity)
            .Entity<CardDefinition>(CardDefinition.OnBuildEntity)
            .Entity<DeckCard>(DeckCard.OnBuildEntity)
            .Entity<ColorSchema>(ColorSchema.OnBuildEntity)
            .Entity<CardColor>(CardColor.OnBuildEntity)
            .Entity<SheetDefinition>(SheetDefinition.OnBuildEntity);
    }
}