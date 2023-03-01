using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumPoker.WebApp.Models;

/// <summary>
/// 
/// </summary>
public class TeamMember
{
    private readonly FKNavProperty<Team> _team = new(e => e.Id);
    /// <summary>
    /// The unique identifier for the planning meeting's team.
    /// </summary>
    public Guid TeamId
    {
        get => _team.ForeignKey;
        set => _team.ForeignKey = value;
    }

    /// <summary>
    /// The planning meeting's team.
    /// </summary>
    public Team? Team
    {
        get => _team.Model;
        set => _team.Model = value;
    }
    
    private readonly FKNavProperty<UserProfile> _user = new(e => e.Id);
    /// <summary>
    /// 
    /// </summary>
    public Guid UserId
    {
        get => _user.ForeignKey;
        set => _user.ForeignKey = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public UserProfile? User
    {
        get => _user.Model;
        set => _user.Model = value;
    }
}