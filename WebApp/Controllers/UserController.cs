using System.Collections.ObjectModel;
// using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ScrumPoker.WebApp.Models;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Controllers;

// [Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route(Routings.User_Route)]
public class UserController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    // private readonly ILogger<UserController> _logger;

    // public UserController(ScrumPokerContext context, ILogger<UserController> logger)
    public UserController(ScrumPokerContext context)
    {
        _context = context;
        // _logger = logger;
    }

    // GET: api/User/New
    [HttpGet("New")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> AddNewUser(DataContracts.User.NewItemRequest request, CancellationToken token = default)
    {
        string newUserName = request.UserName;
        string displayName = request.DisplayName;
        if (newUserName.Length == 0)
            return BadRequest("userName cannot be empty.");
        if (displayName.Length == 0)
            return BadRequest("displayName cannot be empty.");
        if (!_context.TryGetUserProfile(out UserProfile? profile))
            return Unauthorized();
        Guid[] teamIds = request.TeamIds?.Distinct().ToArray() ?? Array.Empty<Guid>();
        Guid userId;
        if (!profile.IsAdmin)
        {
            if (request.IsAdmin)
                return Unauthorized("Admin role required to add other admin users.");
            userId = profile.Id;
            if (teamIds.Length == 0)
                return Unauthorized("Admin role required to add new users without team membership.");
            foreach (Guid id in teamIds)
                if (await _context.Teams.CountAsync(t => t.Id == id && t.FacilitatorId == userId, token) == 0)
                    return Unauthorized("Only admins may add users to other teams.");
        }
        if (await _context.Profiles.CountAsync(p => p.UserName == newUserName, token) > 0)
            return Conflict("Another profile with that user name already exists.");
        if (await _context.Profiles.CountAsync(p => p.DisplayName == displayName, token) > 0)
            return Conflict("Another profile with that display name already exists.");
        Collection<Team> teams = new();
        foreach (Guid id in teamIds)
        {
            Team? item = await _context.Teams.Include(t => t.Members).FirstOrDefaultAsync(t => t.Id == id, token);
            if (item is null)
                return NotFound($"A team with the id {id:n} does not exist.");
            teams.Add(item);
        }
        userId = Guid.NewGuid();
        EntityEntry<UserProfile> userEntry = await _context.Profiles.AddAsync(new()
        {
            Id = userId,
            DisplayName = displayName,
            IsAdmin = request.IsAdmin,
            UserName = newUserName
        }, token);
        _ = await _context.SaveChangesAsync(token);
        if (teams.Count > 0)
        {
            profile = userEntry.Entity;

            foreach (Team item in teams)
            {
                item.Members.Add(profile);
                _context.Update(item);
            }
            _ = await _context.SaveChangesAsync(token);
        }
        return Ok(userId.ToJsonString());
    }

    // GET: api/User/AppState
    /// <summary>
    /// Gets initial application state for the current user.
    /// </summary>
    /// <returns>A <see cref="DataContracts.User.AppState" /> object getting the current user's application state.</returns>
    [HttpGet(DataContracts.User.AppState.SUB_ROUTE)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DataContracts.User.AppState>> GetAppState(CancellationToken token = default)
    {
        UserProfile? userProfile = await _context.GetUserProfileAsync(profiles => profiles.Include(p => p.Teams).ThenInclude(t => t.Facilitator), token);
        if (userProfile is null)
             return Unauthorized();
        Guid id = userProfile.Id;
        Collection<DataContracts.Team.RecordEntry> resultTeams = new();
        foreach (Team t in userProfile.Teams)
            resultTeams.Add(new DataContracts.Team.RecordEntry
            {
                Id = t.Id,
                Description = t.Description,
                FacilitatorId = t.FacilitatorId,
                Title = t.Title
            });
        Collection<DataContracts.User.BaseEntry> facilitators = new();
        foreach (UserProfile u in userProfile.Teams.Select(t => t.Facilitator).GroupBy(f => f!.Id).Select(g => g.First()!))
            facilitators.Add(new DataContracts.User.BaseEntry
            {
                UserId = u!.Id,
                DisplayName = u.DisplayName,
                UserName = u.UserName
            });
        return Ok(new DataContracts.User.AppState
        {
            UserId = userProfile.Id,
            DisplayName = userProfile.DisplayName,
            UserName = userProfile.UserName,
            IsAdmin = userProfile.IsAdmin,
            Teams = resultTeams,
            Facilitators = facilitators
        });
    }

    // GET: api/User/TeamState/{id}
    /// <summary>
    /// Gets initial team state for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet(DataContracts.User.TeamState.SUB_ROUTE + "/{" + DataContracts.User.TeamState.PARAM_NAME + ":guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DataContracts.User.TeamState>> GetTeamState(Guid id, CancellationToken token = default)
    {
        Team? team = await _context.Teams.Where(t => t.Id == id).Include(t => t.Facilitator).Include(t => t.Members).FirstOrDefaultAsync(token);
        if (team is null)
        {
            if (await _context.GetUserProfileAsync(token) is null)
                return Unauthorized();
            return NotFound();
        }

        if (!_context.TryFindUserProfile(team.Facilitator, team.Members, out UserProfile? userProfile) && ((userProfile = await _context.GetUserProfileAsync(profiles => profiles.Include(p => p.Memberships), token)) is null || !userProfile.IsAdmin))
            return Unauthorized();
        Guid userId = userProfile.Id;
        DataContracts.User.TeamState response = new()
        {
            TeamId = team.Id,
            Facilitator = new DataContracts.User.BaseEntry
            {
                UserId = team.Facilitator!.Id,
                DisplayName = team.Facilitator.DisplayName,
                UserName = team.Facilitator.UserName
            },
            Title = team.Title,
            Description = team.Description
        };
        if (userProfile.IsAdmin)
            foreach (PlanningMeeting pm in team.Meetings)
                response.Meetings.Add(new DataContracts.PlanningMeeting.RecordEntry
                {
                    Id = pm.Id,
                    Title = pm.Title,
                    Description = pm.Description,
                    MeetingDate = pm.MeetingDate
                });
        else
            foreach (PlanningMeeting pm in team.Meetings.Where(m => m.Participants.Any(p => p.UserId == userId)))
                response.Meetings.Add(new DataContracts.PlanningMeeting.RecordEntry
                {
                    Id = pm.Id,
                    Title = pm.Title,
                    Description = pm.Description,
                    MeetingDate = pm.MeetingDate
                });
        return Ok(response);
    }

    private static DataContracts.Initiative.RecordEntry? ToInitiativeListItem(Initiative? source)
    {
        if (source is null)
            return null;
        return new DataContracts.Initiative.RecordEntry()
        {
            Title = source.Title,
            Description = source.Description,
            StartDate = source.StartDate,
            PlannedEndDate = source.PlannedEndDate
        };
    }

    private static DataContracts.Epic.RecordEntry? ToEpicListItem(Epic? source)
    {
        if (source is null)
            return null;
        return new DataContracts.Epic.RecordEntry()
        {
            Title = source.Title,
            Description = source.Description,
            StartDate = source.StartDate,
            PlannedEndDate = source.PlannedEndDate
        };
    }
    
    private static DataContracts.Milestone.RecordEntry? ToMilestoneListItem(Milestone? source)
    {
        if (source is null)
            return null;
        return new DataContracts.Milestone.RecordEntry()
        {
            Title = source.Title,
            Description = source.Description,
            StartDate = source.StartDate,
            PlannedEndDate = source.PlannedEndDate
        };
    }
    
    // GET: api/User/ScrumMeeting/{id}
    /// <summary>
    /// Gets initial scrum meeting state for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet(DataContracts.User.ScrumState.SUB_ROUTE + "/{" + DataContracts.User.ScrumState.PARAM_NAME + ":guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DataContracts.User.ScrumState>> GetScrumState(Guid id, CancellationToken token = default)
    {
        PlanningMeeting? planningMeeting = await _context.Meetings.Where(m => m.Id == id).Include(m => m.Initiative).Include(m => m.Epic).Include(m => m.Milestone)
            .Include(m => m.Team).ThenInclude(t => t!.Facilitator)
            .Include(m => m.Participants).ThenInclude(p => p.User)
            .Include(p => p.Deck).ThenInclude(d => d!.Cards).FirstOrDefaultAsync(token);
        if (planningMeeting is null)
        {
            if (await _context.GetUserProfileAsync(token) is null)
                return Unauthorized();
            return NotFound();
        }
        UserProfile facilitator = planningMeeting.Team!.Facilitator!;
        if (!_context.TryFindUserProfile(facilitator, planningMeeting.Participants.Select(p => p.User!), out UserProfile? userProfile) &&
                ((userProfile = await _context.GetUserProfileAsync(profiles => profiles.Include(p => p.Memberships), token)) is null || !userProfile.IsAdmin))
            return Unauthorized();
        Team team = planningMeeting!.Team!;
        CardDeck deck = planningMeeting.Deck!;
        DataContracts.User.ScrumState response = new()
        {
            PlannedStartDate = planningMeeting.PlannedStartDate,
            PlannedEndDate = planningMeeting.PlannedEndDate,
            Initiative = ToInitiativeListItem(planningMeeting.Initiative),
            Epic = ToEpicListItem(planningMeeting.Epic),
            Milestone = ToMilestoneListItem(planningMeeting.Milestone),
            CurrentScopePoints = planningMeeting.CurrentScopePoints,
            Team = new DataContracts.Team.RecordEntry()
            {
                Id = team.Id,
                Description = team.Description,
                FacilitatorId = team.FacilitatorId,
                Title = team.Title
            },
            Facilitator = new()
            {
                UserId = facilitator.Id,
                DisplayName = facilitator.DisplayName,
                UserName = facilitator.UserName
            },
            Description = planningMeeting.Description,
            ColorScheme = null,
            MeetingDate = planningMeeting.MeetingDate,
            Id = planningMeeting.Id,
            SprintCapacity = planningMeeting.SprintCapacity,
            Title = planningMeeting.Title,
            Deck = new()
            {
                Id = deck.Id,
                Name = deck.Name,
                Description = deck.Description
            }
        };
        foreach (DeckCard dc in deck.Cards)
        {
            CardDefinition card = dc.Definition!;
            response.Cards.Add(new()
            {
                Id = dc.CardId,
                Order = dc.Order,
                Title = card.Title,
                Description = card.Description,
                TruncatedDescription = card.TruncatedDescription,
                SymbolText = card.SymbolText,
                SymbolFont = card.SymbolFont,
                UpperSymbolPath = card.UpperSymbolPath,
                MiddleSymbolPath = card.MiddleSymbolPath,
                LowerSymbolPath = card.LowerSymbolPath,
                Value = card.Value,
                MiddleSymbolTop = card.MiddleSymbolTop,
                SmallSymbolFontSize = card.SmallSymbolFontSize,
                LargeSymbolFontSize = card.LargeSymbolFontSize,
                Type = card.Type
            });
        }
        foreach (Participant participant in planningMeeting.Participants)
            response.Participants.Add(new()
            {
                UserId = participant.UserId,
                DisplayName = participant.User!.DisplayName,
                UserName = participant.User.UserName,
                SelectedCardId = participant.DrawnCardId,
                CardColorId = participant.CardColorId,
                AssignedPoints = participant.PointsAssigned,
                SprintCapacity = participant.ScrumCapacity
            });
        return Ok(response);
    }
}