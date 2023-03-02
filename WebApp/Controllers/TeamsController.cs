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
public class TeamsController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    // private readonly ILogger<TeamsController> _logger;

    // public TeamsController(ScrumPokerContext context, ILogger<TeamsController> logger)
    public TeamsController(ScrumPokerContext context)
    {
        _context = context;
        // _logger = logger;
    }

    // GET: api/Teams/New
    [HttpGet("New")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> AddNewTeam(DataContracts.Team.NewItemRequest request, CancellationToken token = default)
    {
        UserProfile? userProfile = await _context.GetUserProfileAsync(token);
        if (userProfile is null || !userProfile.IsAdmin)
            return Unauthorized();
        Guid facilitatorId;
        if (request.FacilitatorId.HasValue && !userProfile.Id.Equals(facilitatorId = request.FacilitatorId.Value))
        {
            if ((userProfile = await _context.Profles.FirstOrDefaultAsync(p => p.Id == facilitatorId, token)) is null)
                return NotFound($"A user with the id '{facilitatorId:n}' was not found.");
        }
        else
            facilitatorId = userProfile.Id;
        string title = request.Title;
        if (title.Length == 0)
            return BadRequest("Title cannot be empty.");
        if (await _context.Teams.CountAsync(t => t.Title == title, token) > 0)
            return Conflict("A team with that title already exists.");
        Guid id = Guid.NewGuid();
        _ = await _context.Teams.AddAsync(new()
        {
            Id = id,
            Title = title,
            Description = request.Description,
            FacilitatorId = facilitatorId
        }, token);
        _ = await _context.SaveChangesAsync(token);
        return Ok(id.ToJsonString());
    }
}
