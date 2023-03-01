using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrumPoker.WebApp.Models;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MilestonesController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    // private readonly ILogger<MilestonesController> _logger;

    // public MilestonesController(ScrumPokerContext context, ILogger<MilestonesController> logger)
    public MilestonesController(ScrumPokerContext context)
    {
        _context = context;
        // _logger = logger;
    }

    // GET: api/Milestones/New
    [HttpGet("New")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> AddNewMilestone(DataContracts.Milestone.NewMilestoneRequest request, CancellationToken token = default)
    {
        string title = request.Title;
        if (title.Length == 0 || (request.StartDate.HasValue && request.PlannedEndDate.HasValue && request.StartDate.Value > request.PlannedEndDate.Value))
            return BadRequest();
        if (!_context.TryGetCurrentIdentityName(out string? userName))
            return Unauthorized();
        Guid teamid = request.Id;
        Team? team;
        Guid? epicId;
        if (request.IsTeamMilestone)
        {
            epicId = null;
            if ((team = await _context.Teams.Where(t => t.Id == teamid).Include(t => t.Facilitator).FirstOrDefaultAsync(token)) is null)
                return NotFound();
        }
        else
        {
            epicId = teamid;
            Epic? epic = await _context.Epics.Where(e => e.Id == teamid).Include(e => e.Team).ThenInclude(t => t!.Facilitator).FirstOrDefaultAsync(token);
            if (epic is null)
                return NotFound();
            teamid = (team = epic.Team!).Id;
        }
        if (await _context.Milestones.CountAsync(e => e.TeamId == teamid && e.Title == title, token) > 0)
            return Conflict();
        if (!team.Facilitator!.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            UserProfile? userProfile = await _context.GetUserProfileAsync(token);
            if (userProfile is null || !userProfile.IsAdmin)
                return Unauthorized();
        }
        Guid id = Guid.NewGuid();
        _ = await _context.Milestones.AddAsync(new()
        {
            Id = id,
            Title = title,
            Description = request.Description,
            StartDate = request.StartDate,
            PlannedEndDate = request.PlannedEndDate,
            TeamId = teamid,
            EpicId = epicId
        }, token);
        _ = await _context.SaveChangesAsync(token);
        return id.ToJsonString();
    }
}