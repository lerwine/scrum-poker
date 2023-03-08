using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Linq;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using ScrumPoker.WebApp.Models;
using Microsoft.Extensions.Options;

namespace ScrumPoker.WebApp.Services;

public class ScrumPokerContext : DbContext
{
    private const string DEFAULT_ADMIN_LOGIN = "admin";

    private readonly ILogger<ScrumPokerContext> _logger;
    private readonly string? _adminUserName;
    private readonly string? _adminDisplayName;

    private bool EnsureColorSchemas(ColorSchemeSetting[] defaultColorSchemes)
    {
        if (ColorSchemas.Any())
            return true;
        if (defaultColorSchemes is null || defaultColorSchemes.Length == 0)
        {
            _logger.LogCritical("Database contains no color schemas and no color schemas are defined in settings.");
            return true;
        }
        foreach (ColorSchemeSetting colorSchemeSetting in defaultColorSchemes)
        {
            ColorValuesSetting votingCard = colorSchemeSetting.votingCard;
            if (!(ColorModel.CssColor.TryParse(votingCard.fill, out ColorModel.CssColor fill) && ColorModel.CssColor.TryParse(votingCard.stroke, out ColorModel.CssColor stroke) && ColorModel.CssColor.TryParse(votingCard.text, out ColorModel.CssColor text)))
            {
                _logger.LogCritical("Database contains no color schemas and voting card color values couldn't be parsed.");
                return true;
            }
            string schemaName = colorSchemeSetting.name.WsNormalizedOrNullIfEmpty();
            if (schemaName is null)
            {
                _logger.LogCritical("Database contains no color schemas and color schema name is empty.");
                return true;
            }
            if (colorSchemeSetting.cardColors is null || colorSchemeSetting.cardColors.Length == 0)
            {
                _logger.LogCritical("Database contains no color schemas and card color scheme {name} has no card color definitions.", schemaName);
                return true;
            }
            LinkedList<CardColor> cc = new();
            Guid id = Guid.NewGuid();
            foreach (CardColorSetting cardColorSetting in colorSchemeSetting.cardColors)
            {
                string n = cardColorSetting.name.WsNormalizedOrNullIfEmpty();
                if (n is null)
                {
                    _logger.LogCritical("Database contains no color schemas and card color name is empty for new schema {name}.", schemaName);
                    return true;
                }
                if (!(ColorModel.CssColor.TryParse(cardColorSetting.fill, out fill) && ColorModel.CssColor.TryParse(cardColorSetting.stroke, out stroke) && ColorModel.CssColor.TryParse(cardColorSetting.text, out text)))
                {
                    _logger.LogCritical("Database contains no color schemas and card color values couldn't be parsed for schema {name}.", schemaName);
                    return true;
                }
                cc.AddLast(new CardColor()
                {
                    Id = Guid.NewGuid(),
                    SchemaId = id,
                    Name = n,
                    Fill = fill,
                    Stroke = stroke,
                    Text = text
                });
            }
            ColorSchemas.Add(new()
            {
                Id = id,
                Name = schemaName,
                VotingFill = fill,
                VotingStroke = stroke,
                VotingText = text
            });
            SaveChanges(true);
            CardColors.AddRange(cc);
            SaveChanges(true);
        }
        return false;
    }

    private bool EnsureDeckTypes(SettingDeck[] defaultDecks, SettingCard[] defaultCards)
    {
        if (Decks.Any())
            return true;

        return false;
    }

    public ScrumPokerContext(DbContextOptions<ScrumPokerContext> options, IOptions<ScrumPokerAppSettings> settingOption)
        : base(options)
    {
        _logger = Program.CurrentApp.Services.GetRequiredService<ILogger<ScrumPokerContext>>();
        ScrumPokerAppSettings settings = settingOption.Value;
        _adminUserName = settings.adminUserName.TrimmedOrNullIfEmpty();
        _adminDisplayName = settings.adminDisplayName.WsNormalizedOrNullIfEmpty();
        // FIXME: The rest of the code should not be be in constructor
        UserProfile? adminUser;
        if (Profiles.Any(p => p.IsAdmin))
        {
            if (_adminUserName is null)
                return;
            if ((adminUser = Profiles.FirstOrDefault(p => p.UserName == _adminUserName)) is null)
                _logger.LogWarning("Default administrative user not found.");
        }
        else
        {
            _adminUserName ??= DEFAULT_ADMIN_LOGIN;
            if ((adminUser = Profiles.FirstOrDefault(p => p.UserName == _adminUserName)) is null && Profiles.Any())
                _logger.LogWarning("No administrative user found.");
        }
        if ((adminUser = Profiles.FirstOrDefault(p => p.UserName == _adminUserName)) is null)
        {
            string displayName = _adminDisplayName;
            if (displayName is null)
            {
                int index = _adminUserName.IndexOfAny(new[] { '@', '/', '\\', ':' });
                displayName = (index < 0) ? _adminUserName : _adminUserName[(index + 1)..].TrimmedOrNullIfEmpty() ?? _adminUserName;
            }
            Guid id = Guid.NewGuid();
            _logger.LogInformation("Adding administrative user: {{ Id = '{Id}', UserName = '{UserName}', DisplayName = {DisplayName} }}", id, _adminUserName, displayName);
            Profiles.Add(new()
            {
                Id = id,
                UserName = _adminUserName,
                DisplayName = displayName,
                IsAdmin = true
            });
            EnsureColorSchemas(settings.defaultColorSchemes);
            EnsureDeckTypes(settings.defaultDecks, settings.defaultCards);
        }
        else if (adminUser.IsAdmin)
        {
            if (EnsureColorSchemas(settings.defaultColorSchemes))
            {
                if (EnsureDeckTypes(settings.defaultDecks, settings.defaultCards))
                    return;
            }
            else
                EnsureDeckTypes(settings.defaultDecks, settings.defaultCards);
        }
        else
        {
            _logger.LogInformation("Changing UserProfile to administator: {{ Id = '{Id}', UserName = '{UserName}', DisplayName = {DisplayName} }}", adminUser.Id, adminUser.UserName, adminUser.DisplayName);
            adminUser.IsAdmin = true;
            Profiles.Update(adminUser);
            EnsureColorSchemas(settings.defaultColorSchemes);
            EnsureDeckTypes(settings.defaultDecks, settings.defaultCards);
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

    public DbSet<CardDeck> Decks { get; set; } = null!;

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
            // FIXME: Comparison should be case insensitive
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
            // FIXME: Comparison should be case insensitive
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
            // FIXME: Comparison should be case insensitive
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
