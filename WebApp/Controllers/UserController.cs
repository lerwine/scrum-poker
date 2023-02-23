using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ScrumPoker.WebApp.Models;
using ScrumPoker.WebApp.Models.DTO;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    private readonly DeckService _deckService;
    private readonly ILogger<UserController> _logger;

    public UserController(ScrumPokerContext context, DeckService deckService, ILogger<UserController> logger)
    {
        _context = context;
        _deckService = deckService;
        _logger = logger;
    }

    // GET: api/User/AppState
    /// <summary>
    /// Gets initial application state for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("AppState")]
    public async Task<ActionResult<DataContracts.User.AppState.Response>> GetAppState(CancellationToken token = default)
    {
        UserProfile? userProfile = await _context.GetUserProfileAsync(token);
        if (userProfile == null)
             return Unauthorized();
        Guid id = userProfile.Id;
        List<Team> teams = await _context.TeamMembers.Where(m => m.UserId == id).Include(m => m.Team).Select(m => m.Team).Include(t => t.Facilitator).ToListAsync(token);
        Collection<DataContracts.User.TeamListItem> resultTeams = new();
        foreach (Team t in teams)
            resultTeams.Add(new DataContracts.User.TeamListItem
            {
                TeamId = t.Id,
                Description = t.Description,
                FacilitatorId = t.FacilitatorId,
                Title = t.Title
            });
        Collection<DataContracts.User.UserListItem> facilitators = new();
        foreach (UserProfile u in teams.Select(t => t.Facilitator).GroupBy(f => f.Id).Select(g => g.First()))
            facilitators.Add(new DataContracts.User.UserListItem
            {
                UserId = u.Id,
                DisplayName = u.DisplayName,
                UserName = u.UserName
            });
        return new DataContracts.User.AppState.Response
        {
            UserId = userProfile.Id,
            DisplayName = userProfile.DisplayName,
            UserName = userProfile.UserName,
            IsAdmin = userProfile.IsAdmin,
            Teams = resultTeams,
            Facilitators = facilitators
        };
    }

    // GET: api/User/Team/{id}
    /// <summary>
    /// Gets initial team state for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("TeamState/{id}")]
    public async Task<ActionResult<DataContracts.User.TeamState.Response>> GetTeamState(Guid id)
    {
        // if (!_tokenService.ValidateAdminTokenString(token))
        //     return Unauthorized();
        throw new NotImplementedException();
    }

    // GET: api/User/ScrumMeeting/{id}
    /// <summary>
    /// Gets initial scrum meeting state for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("ScrumMeeting/{id}")]
    public async Task<ActionResult<DataContracts.User.ScrumState.Response>> GetScrumState(Guid id)
    {
        // if (!_tokenService.ValidateAdminTokenString(token))
        //     return Unauthorized();
        throw new NotImplementedException();
    }
}