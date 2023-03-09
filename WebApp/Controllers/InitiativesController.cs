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
public class InitiativesController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    // private readonly ILogger<InitiativesController> _logger;

    // public InitiativesController(ScrumPokerContext context, ILogger<InitiativesController> logger)
    public InitiativesController(ScrumPokerContext context)
    {
        _context = context;
        // _logger = logger;
    }

    // GET: api/Initiatives/New
    [HttpGet("New")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> AddNewInitiative(DataContracts.Initiative.NewItemRequest request, CancellationToken token = default)
    {
        string title = request.Title;
        if (title.Length == 0 || (request.StartDate.HasValue && request.PlannedEndDate.HasValue && request.StartDate.Value > request.PlannedEndDate.Value))
            return BadRequest();
        if (!UserProfile.TryGetCurrentIdentityName(out string? userName))
            return Unauthorized();
        Guid teamId = request.TeamId;
        Team? team = await _context.Teams.Where(t => t.Id == teamId).Include(t => t.Facilitator).FirstOrDefaultAsync(token);
        if (team is null)
            return NotFound($"A team with the id '{teamId:n}' was not found.");
        if (!team.Facilitator!.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
        {
            UserProfile? userProfile = await _context.GetUserProfileAsync(token);
            if (userProfile is null || !userProfile.IsAdmin)
                return Unauthorized();
        }
        if (await _context.Initiatives.CountAsync(e => e.TeamId == teamId && e.Title == title, token) > 0)
            return Conflict("An initiative with that title already exists.");
        Guid id = Guid.NewGuid();
        _ = await _context.Initiatives.AddAsync(new()
        {
            Id = id,
            Title = title,
            Description = request.Description,
            StartDate = request.StartDate,
            PlannedEndDate = request.PlannedEndDate,
            TeamId = teamId
        }, token);
        _ = await _context.SaveChangesAsync(token);
        return Ok(id.ToJsonString());
    }
}
