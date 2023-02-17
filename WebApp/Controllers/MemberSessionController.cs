using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ScrumPoker.WebApp.Models;
using ScrumPoker.WebApp.Models.DTO;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemberSessionController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    private readonly DeckService _deckService;
    private readonly ILogger<MemberSessionController> _logger;
    private readonly SessionTokenService _tokenService;

    public MemberSessionController(ScrumPokerContext context, DeckService deckService, ILogger<MemberSessionController> logger, SessionTokenService tokenService)
    {
        _context = context;
        _deckService = deckService;
        _logger = logger;
        _tokenService = tokenService;
    }

    // GET: api/MemberSession/{token}
    /// <summary>
    /// Gets a session by the token string.
    /// </summary>
    /// <param name="token">The team member token.</param>
    /// <returns></returns>
    [HttpGet("{token:length(352)}")]
    public async Task<ActionResult<GetMemberSessionResponseDTO>> GetSession(string token)
    {
        if (!_tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return Unauthorized();
        ScrumSession? ss = (await _context.TeamMembers.Include(m => m.Session).FirstOrDefaultAsync(m => m.Token == decryptedToken))?.Session;
        if (ss is null)
            return NotFound();

        throw new NotImplementedException();
    }

    // POST: api/MemberSession/Status
    /// <summary>
    /// Gets updated session information.
    /// </summary>
    /// <param name="statusRequest">Status request parameters.</param>
    /// <returns></returns>
    [HttpPost("Status")]
    public async Task<ActionResult<MemberSessionUpdatesResponseDTO>> SessionUpdates(MemberSessionUpdatesRequestDTO statusRequest)
    {
        if (!_tokenService.TryUnrotectToken(statusRequest.Token, out string? decryptedToken))
            return Unauthorized();
        ScrumSession? ss = (await _context.TeamMembers.Include(m => m.Session).FirstOrDefaultAsync(m => m.Token == decryptedToken))?.Session;
        if (ss is null)
            return NotFound();
        throw new NotImplementedException();
    }

    // PUT: api/MemberSession/{token}
    /// <summary>
    /// Changes to the <see href="SessionStage.Instruction" /> stage and sets the instruction text.
    /// </summary>
    /// <param name="token">The team member token.</param>
    /// <param name="selection">The selected card ID and optional explanation.</param>
    /// <returns></returns>
    [HttpPut("{token:length(352)}")]
    public async Task<ActionResult<SelectCardResponseDTO>> SelectCard(string token, SelectCardRequestDTO selection)
    {
        if (!_tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return Unauthorized();
        TeamMember? tm = await _context.TeamMembers.Include(m => m.Session).FirstOrDefaultAsync(m => m.Token == decryptedToken);
        if (tm is null)
            return NotFound();
        ScrumSession? ss = tm.Session;
        if (ss is null)
            return NotFound();
        if (selection.DrawnCardID != tm.DrawnCardID)
        {
            tm.DrawnCardID = selection.DrawnCardID;
            tm.Explanation = selection.Explanation;
            tm.LastActivity = DateTime.Now;
            _context.Update(tm);
            await _context.SaveChangesAsync();
        }
        else if (tm.Explanation != selection.Explanation)
        {
            tm.Explanation = selection.Explanation;
            tm.LastActivity = DateTime.Now;
            _context.Update(tm);
            await _context.SaveChangesAsync();
        }
        throw new NotImplementedException();
    }
}