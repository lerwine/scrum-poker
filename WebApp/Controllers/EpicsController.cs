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
public class EpicsController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    // private readonly ILogger<EpicsController> _logger;

    // public EpicsController(ScrumPokerContext context, ILogger<EpicsController> logger)
    public EpicsController(ScrumPokerContext context)
    {
        _context = context;
        // _logger = logger;
    }

    // GET: api/Epics/New
    [HttpGet("New")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> AddNewEpic(DataContracts.Epic.NewEpicRequest request, CancellationToken token = default)
    {
        string title = request.Title;
        if (title.Length == 0 || (request.StartDate.HasValue && request.PlannedEndDate.HasValue && request.StartDate.Value > request.PlannedEndDate.Value))
            return BadRequest();
        if (!_context.TryGetCurrentIdentityName(out string? userName))
            return Unauthorized();
        Guid teamid = request.TeamId;
        Team? team = await _context.Teams.Where(t => t.Id == teamid).Include(t => t.Facilitator).FirstOrDefaultAsync(token);
        if (team is null)
            return NotFound();
        if (!team.Facilitator!.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            UserProfile? userProfile = await _context.GetUserProfileAsync(token);
            if (userProfile is null || !userProfile.IsAdmin)
                return Unauthorized();
        }
        if (await _context.Epics.CountAsync(e => e.TeamId == teamid && e.Title == title, token) > 0)
            return Conflict();
        Guid id = Guid.NewGuid();
        _ = await _context.Epics.AddAsync(new()
        {
            Id = id,
            Title = title,
            Description = request.Description,
            StartDate = request.StartDate,
            PlannedEndDate = request.PlannedEndDate,
            TeamId = teamid
        }, token);
        _ = await _context.SaveChangesAsync(token);
        return id.ToJsonString();
    }
}