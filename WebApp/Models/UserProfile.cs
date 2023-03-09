using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using ScrumPoker.WebApp.Services;
using ScrumPoker.WebApp.Services.Settings;

namespace ScrumPoker.WebApp.Models;

/// <summary>
///
/// </summary>
public class UserProfile
{
    /// <summary>
    ///
    /// </summary>
    public Guid Id { get; set; }

    private string _displayName = "";
    /// <summary>
    /// The display name of the currently authenticated user.
    /// </summary>
    public string DisplayName
    {
        get { return _displayName; }
        set { _displayName = value.WsNormalizedOrEmptyIfNull(); }
    }

    private string _userName = "";
    /// <summary>
    /// The login name of the currently authenticated user.
    /// </summary>
    public string UserName
    {
        get { return _userName; }
        set { _userName = value.WsNormalizedOrEmptyIfNull(); }
    }

    /// <summary>
    ///
    /// </summary>
    public bool IsAdmin { get; set; }

    private Collection<Team> _facilitated = new();
    /// <summary>
    /// Gets the teams that the current user belongs to.
    /// </summary>
    public Collection<Team> Facilitated
    {
        get { return _facilitated; }
        set { _facilitated = value ?? new Collection<Team>(); }
    }

    private Collection<Team> _teams = new();
    /// <summary>
    /// Gets the teams that the current user belongs to.
    /// </summary>
    public Collection<Team> Teams
    {
        get { return _teams; }
        set { _teams = value ?? new Collection<Team>(); }
    }

    private Collection<TeamMember> _memberships = new();
    /// <summary>
    /// Gets the teams that the current user belongs to.
    /// </summary>
    public Collection<TeamMember> Memberships
    {
        get { return _memberships; }
        set { _memberships = value ?? new Collection<TeamMember>(); }
    }

    private Collection<Participant> _participation = new();
    /// <summary>
    /// Gets the teams that the current user belongs to.
    /// </summary>
    public Collection<Participant> Participation
    {
        get { return _participation; }
        set { _participation = value ?? new Collection<Participant>(); }
    }

    public static bool TryGetCurrentIdentityName([MaybeNullWhen(false)] out string result)
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

    internal static void OnBuildEntity(EntityTypeBuilder<UserProfile> builder)
    {
        _ = builder.HasKey(nameof(Id));
        _ = builder.Property(c => c.UserName).UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = builder.Property(c => c.DisplayName).UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }

    internal static bool SeedData(ScrumPokerContext context, IOptions<ScrumPokerAppSettings> settingOption, ILogger<UserProfile> logger)
    {
        ScrumPokerAppSettings settings = settingOption.Value;
        string? adminUserName = settings.adminUserName.TrimmedOrNullIfEmpty();
        if (adminUserName is null)
        {
            if (!context.Profiles.Any(p => p.IsAdmin))
                logger.LogWarning("Default administrative user not found.");
            return false;
        }
        UserProfile? adminUser = context.Profiles.FirstOrDefault(p => p.UserName == adminUserName);
        if (adminUser is null)
            context.Profiles.Add(new()
            {
                Id = Guid.NewGuid(),
                UserName = adminUserName,
                DisplayName = settings.adminDisplayName.WsNormalizedOrDefaultIfEmpty(adminUserName),
                IsAdmin = true
            });
        else
        {
            if (adminUser.IsAdmin)
                return false;
            adminUser.IsAdmin = true;
            context.Profiles.Update(adminUser);
        }
        return true;
    }
}
