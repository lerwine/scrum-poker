using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Linq;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using ScrumPoker.WebApp.Models;

namespace ScrumPoker.WebApp.Services;

public class ScrumPokerContext : DbContext
{
    private const string DEFAULT_ADMIN_LOGIN = "admin";

    private readonly ILogger<ScrumPokerContext> _logger;
    
    private bool EnsureColorSchemas(ScrumPokerAppSettings settings)
    {
        if (ColorSchemas.Any())
            return true;
        if (!settings.DefaultColorSchemes.HasValue)
        {
            _logger.LogCritical("Database contains no color schemas and no color schemas are defined in settings.");
            return true;
        }
        var (n, f, s, t, cc) = settings.DefaultColorSchemes.Value;
        if ((n = n.TrimmedOrNullIfEmpty()) is null)
        {
            _logger.LogCritical("Database contains no color schemas and color scheme name is empty.");
            return true;
        }
        if (!(ColorModel.CssColor.TryParse(f, out ColorModel.CssColor fill) && ColorModel.CssColor.TryParse(s, out ColorModel.CssColor stroke) && ColorModel.CssColor.TryParse(t, out ColorModel.CssColor text)))
        {
            _logger.LogCritical("Database contains no color schemas and voting card color values couldn't be parsed.");
            return true;
        }
        Guid id = Guid.NewGuid();
        ColorSchemas.Add(new()
        {
            Id = id,
            Name = n,
            VotingFill = fill,
            VotingStroke = stroke,
            VotingText = text
        });
        return cc is not null && !cc.Any(c =>
        {
            (n, f, s, t) = c;
            if ((n = n.TrimmedOrNullIfEmpty()) is null)
            {
                _logger.LogCritical("Database contains no color schemas and card color name is empty.");
                return true;
            }
            if (ColorModel.CssColor.TryParse(f, out fill) && ColorModel.CssColor.TryParse(s, out stroke) && ColorModel.CssColor.TryParse(t, out ColorModel.CssColor text))
            {
                CardColors.Add(new()
                {
                    Id = Guid.NewGuid(),
                    SchemaId = id,
                    Name = n,
                    Fill = fill,
                    Stroke = stroke,
                    Text = text
                });
                return false;
            }
            _logger.LogCritical("Database contains no color schemas and voting card color values couldn't be parsed.");
            return true;
        });
    }

    private bool EnsureDeckTypes(ScrumPokerAppSettings settings)
    {
        if (DeckTypes.Any())
            return true;
        
        return false;
    }

    public ScrumPokerContext(DbContextOptions<ScrumPokerContext> options)
        : base(options)
    {
        _logger = Program.CurrentApp.Services.GetRequiredService<ILogger<ScrumPokerContext>>();
        ScrumPokerAppSettings settings = Program.CurrentApp.Configuration.Get<ScrumPokerAppSettings>();
        string? userName = settings.AdminUserName.TrimmedOrNullIfEmpty();
        UserProfile? adminUser;
        if (Profiles.Any(p => p.IsAdmin))
        {
            if (userName is null)
                return;
            if ((adminUser = Profiles.FirstOrDefault(p => p.UserName == userName)) is null)
                _logger.LogWarning("Default administrative user not found.");
        }
        else
        {
            userName ??= DEFAULT_ADMIN_LOGIN;
            if ((adminUser = Profiles.FirstOrDefault(p => p.UserName == userName)) is null && Profiles.Any())
                _logger.LogWarning("No administrative user found.");
        }
        if ((adminUser = Profiles.FirstOrDefault(p => p.UserName == userName)) is null)
        {
            string displayName = settings.AdminDisplayName.TrimmedOrNullIfEmpty();
            if (displayName is null)
            {
                int index = userName.IndexOfAny(new[] { '@', '/', '\\', ':' });
                displayName = (index < 0) ? userName : userName[(index + 1)..].TrimmedOrNullIfEmpty() ?? userName;
            }
            Guid id = Guid.NewGuid();
            _logger.LogInformation("Adding administrative user: {{ Id = '{Id}', UserName = '{UserName}', DisplayName = {DisplayName} }}", id, userName, displayName);
            Profiles.Add(new()
            {
                Id = id,
                UserName = userName,
                DisplayName = displayName,
                IsAdmin = true
            });
            EnsureColorSchemas(settings);
            EnsureDeckTypes(settings);
        }
        else if (adminUser.IsAdmin)
        {
            if (EnsureColorSchemas(settings))
            {
                if (EnsureDeckTypes(settings))
                    return;
            }
            else
                EnsureDeckTypes(settings);
        }
        else
        {
            _logger.LogInformation("Changing UserProfile to administator: {{ Id = '{Id}', UserName = '{UserName}', DisplayName = {DisplayName} }}", adminUser.Id, adminUser.UserName, adminUser.DisplayName);
            adminUser.IsAdmin = true;
            Profiles.Update(adminUser);
            EnsureColorSchemas(settings);
            EnsureDeckTypes(settings);
        }
        SaveChanges(true);
    }

    public DbSet<UserProfile> Profiles { get; set; } = null!;
    
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
                _userProfile = await Profiles.FirstOrDefaultAsync(p => p.UserName == userName, cancellationToken);
        }
        else
            _userProfile = null;
        return _userProfile;
    }

    public async Task<UserProfile?> GetUserProfileAsync(Func<IQueryable<UserProfile>, IQueryable<UserProfile>> onQuery, CancellationToken cancellationToken)
    {
        if (TryGetCurrentIdentityName(out string? userName))
            _userProfile = await onQuery(Profiles).FirstOrDefaultAsync(p => p.UserName == userName, cancellationToken);
        else
            _userProfile = null;
        return _userProfile;
    }
    
    public bool TryGetUserProfile([MaybeNullWhen(false)] out UserProfile result)
    {
        if (TryGetCurrentIdentityName(out string? userName))
        {
            if ((_userProfile is not null && _userProfile.UserName == userName) || (_userProfile = Profiles.FirstOrDefault(u => u.UserName == userName)) is not null)
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