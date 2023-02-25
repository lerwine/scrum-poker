using System.Collections.ObjectModel;
// using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    // GET: api/User/AppState
    /// <summary>
    /// Gets initial application state for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet(DataContracts.User.AppState.SUB_ROUTE)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DataContracts.User.AppState>> GetAppState(CancellationToken token = default)
    {
        UserProfile? userProfile = await _context.GetUserProfileAsync(token);
        if (userProfile is null)
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

    // GET: api/User/Team/{id}
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
        UserProfile? userProfile = await _context.GetUserProfileAsync(token);
        if (userProfile is null)
             return Unauthorized();
        Team? team = await _context.Teams.Include(t => t.Facilitator).FirstOrDefaultAsync(t => t.Id == id, token);
        if (team is null)
            return NotFound();
        Guid userId = userProfile.Id;
        if (!userProfile.IsAdmin && team.FacilitatorId != userId)
        {
#if NET462
            TeamMember tm = await _context.TeamMembers.Where(m => m.TeamId == id && m.UserId == userId).FirstOrDefaultAsync(token);
#else
            TeamMember? tm = await _context.TeamMembers.Where(m => m.TeamId == id && m.UserId == userId).FirstOrDefaultAsync(token);
#endif
            if (tm is null)
                return Unauthorized();
        }
        DataContracts.User.TeamState response = new()
        {
            TeamId = team.Id,
            Facilitator = new DataContracts.User.UserListItem
            {
                UserId = team.Facilitator.Id,
                DisplayName = team.Facilitator.DisplayName,
                UserName = team.Facilitator.UserName
            },
            Title = team.Title,
            Description = team.Description
        };
        List<PlanningMeeting> meetings = await _context.Meetings.Where(m => m.TeamId == id).Include(m => m.Participants).ToListAsync(token);
        if (userProfile.IsAdmin)
            foreach (PlanningMeeting pm in meetings)
                response.Meetings.Add(new DataContracts.User.PlanningMeetingListItem
                {
                    MeetingId = pm.Id,
                    Title = pm.Title,
                    Description = pm.Description,
                    MeetingDate = pm.MeetingDate
                });
        else
            foreach (PlanningMeeting pm in meetings)
                if (pm.Participants.Any(p => p.UserId == userId))
                    response.Meetings.Add(new DataContracts.User.PlanningMeetingListItem
                    {
                        MeetingId = pm.Id,
                        Title = pm.Title,
                        Description = pm.Description,
                        MeetingDate = pm.MeetingDate
                    });
        return Ok(response);
    }

    // GET: api/User/ScrumMeeting/{id}
    /// <summary>
    /// Gets initial scrum meeting state for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet(DataContracts.User.ScrumState.SUB_ROUTE + "/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DataContracts.User.ScrumState>> GetScrumState(Guid id, CancellationToken token = default)
    {
        UserProfile? userProfile = await _context.GetUserProfileAsync(token);
        if (userProfile is null)
             return Unauthorized();
        Guid userId = userProfile.Id;
            
        PlanningMeeting? planningMeeting = await _context.Meetings.Where(m => m.Id == id).Include(m => m.Team).Include(m => m.Initiative).Include(m => m.Epic).Include(m => m.Milestone).Include(m => m.Participants).FirstOrDefaultAsync(token);
        if (planningMeeting is null)
            return NotFound();
        List<Participant> participants = await _context.Participants.Where(p => p.MeetingId == id).Include(p => p.User).ToListAsync(token);
        Team team = planningMeeting.Team;
        if (!(userProfile.IsAdmin || team.FacilitatorId == userId || participants.Any(p => p.UserId == userId)))
            return Unauthorized();
        UserProfile facilitator;
        Dictionary<Guid, DataContracts.User.UserListItem> usersLookedUp = new()
        {
            {
                userId,
                new()
                {
                    UserId = userProfile.Id,
                    DisplayName = userProfile.DisplayName,
                    UserName = userProfile.UserName
                }
            }
        };
        if (userId == team.FacilitatorId)
            facilitator = userProfile;
        else
            try { facilitator = await _context.Profles.FirstAsync(p => p.Id == team.FacilitatorId, token); }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Possible database corruption: Unable to find UserProfile {{ Id = \"{team.FacilitatorId}\"}}, which is associated with Team {{ Id = \"{team.Id}\"}}.");
            }
        facilitator = (userId == team.FacilitatorId) ? userProfile : await _context.Profles.FirstAsync(p => p.Id == team.FacilitatorId, token);
        DataContracts.User.ScrumState response = new()
        {
            PlannedStartDate = planningMeeting.PlannedStartDate,
            PlannedEndDate = planningMeeting.PlannedEndDate,
            Initiative = (planningMeeting.Initiative is null) ? null : new DataContracts.SprintGroupingResponse()
            {
                Title = planningMeeting.Initiative.Title,
                Description = planningMeeting.Initiative.Description,
                StartDate = planningMeeting.Initiative.StartDate,
                PlannedEndDate = planningMeeting.Initiative.PlannedEndDate
            },
            Epic = (planningMeeting.Epic is null) ? null : new  DataContracts.SprintGroupingResponse()
            {
                Title = planningMeeting.Epic.Title,
                Description = planningMeeting.Epic.Description,
                StartDate = planningMeeting.Epic.StartDate,
                PlannedEndDate = planningMeeting.Epic.PlannedEndDate
            },
            Milestone = (planningMeeting.Milestone is null) ? null : new  DataContracts.SprintGroupingResponse()
            {
                Title = planningMeeting.Milestone.Title,
                Description = planningMeeting.Milestone.Description,
                StartDate = planningMeeting.Milestone.StartDate,
                PlannedEndDate = planningMeeting.Milestone.PlannedEndDate
            },
            CurrentScopePoints = planningMeeting.CurrentScopePoints,
            Team = new DataContracts.User.TeamListItem()
            {
                TeamId = team.Id,
                Description = team.Description,
                FacilitatorId = team.FacilitatorId,
                Title = team.Title
            },
            Facilitator = new()
            {
                UserId = facilitator.Id,
                DisplayName = facilitator.DisplayName,
                UserName = facilitator.UserName
            }
        };
        // TODO: Add Deck Information
        foreach (Participant participant in planningMeeting.Participants)
            response.Participants.Add(new()
            {
                UserId = participant.UserId,
                DisplayName = participant.User.DisplayName,
                UserName = participant.User.UserName,
                SelectedCardId = participant.DrawnCardId,
                CardColorId = participant.CardColorId,
                AssignedPoints = participant.PointsAssigned,
                SprintCapacity = participant.ScrumCapacity
            });
        return Ok(response);
    }
}