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
public class ScrumSessionController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    private readonly DeckService _deckService;
    private readonly ILogger<ScrumSessionController> _logger;
    private readonly SessionTokenService _tokenService;

    public ScrumSessionController(ScrumPokerContext context, DeckService deckService, ILogger<ScrumSessionController> logger, SessionTokenService tokenService)
    {
        _context = context;
        _deckService = deckService;
        _logger = logger;
        _tokenService = tokenService;
    }

    // GET: api/ScrumSession/All/{token}
    /// <summary>
    /// Gets all sessions.
    /// </summary>
    /// <param name="token">The administrative token.</param>
    /// <returns></returns>
    [HttpGet("All/{token:length(352)}")]
    public async Task<ActionResult<GetAllSessionsResponseDTO>> GetAllSessions(string token)
    {
        if (!_tokenService.ValidateAdminTokenString(token))
            return Unauthorized();
        await using IAsyncEnumerator<ScrumSession> asyncEnumerator = _context.Sessions.Include(s => s.Organizer).AsAsyncEnumerable().GetAsyncEnumerator();
        Collection<SessionAdminItemDTO> organizers = new();
        while (await asyncEnumerator.MoveNextAsync())
        {
            ScrumSession s = asyncEnumerator.Current;
            organizers.Add(new()
            {
                ID = s.ID,
                Token = _tokenService.ToProtectedToken(s.Token),
                Title = s.Title,
                Stage = s.Stage,
                Instructions = s.Instructions,
                DeckTypeID = s.DeckTypeID,
                NoHalfPoint = s.NoHalfPoint,
                NoZeroPoint = s.NoZeroPoint,
                LastActivity = s.LastActivity,
                Organizer = (s.Organizer is null) ? null : new()
                {
                    ID = s.Organizer.ID,
                    Name = s.Organizer.Name,
                    EmailAddress = s.Organizer.EmailAddress,
                    LastActivity = s.Organizer.LastActivity
                }
            });
        }
        return new GetAllSessionsResponseDTO { Organizers = organizers };
    }

    // GET: api/ScrumSession/Item/{token}
    /// <summary>
    /// Gets a session by the token string.
    /// </summary>
    /// <param name="token">The session token.</param>
    /// <returns></returns>
    [HttpGet("Item/{token:length(352)}")]
    public async Task<ActionResult<GetSessionResponseDTO>> GetSession(string token)
    {
        if (_tokenService.TryUnrotectToken(token, out string? decryptedToken))
        {
            ScrumSession? ss = await _context.Sessions.Include(s => s.TeamMembers).FirstOrDefaultAsync(s => s.Token == decryptedToken);
            if (ss is not null)
            {
                Collection<TeamMemberSessionItemDTO> teamMembers = new();
                if (ss.TeamMembers is not null)
                    foreach (TeamMember tm in ss.TeamMembers)
                        teamMembers.Add(new()
                        {
                            ID = tm.ID,
                            Name = tm.Name,
                            EmailAddress = tm.EmailAddress,
                            Token = _tokenService.ToProtectedToken(tm.Token),
                            DrawnCardID = tm.DrawnCardID,
                            LastActivity = tm.LastActivity
                        });
                DeckType deckType = _deckService.DeckTypes.FirstOrDefault(d => d.ID == ss.DeckTypeID) ?? _deckService.DeckTypes.First();
                Collection<CardDefinitionDTO> cards = new();
                if (deckType.Cards is not null)
                    foreach (CardDefinition cd in deckType.Cards)
                        cards.Add(new()
                        {
                            ID = cd.ID,
                            Value = cd.Value,
                            Symbol = cd.Symbol,
                            Type = cd.Type,
                            BaseName = cd.BaseName
                        });
                Collection<SheetDefinitionDTO> sheets = new();
                if (deckType.Sheets is not null)
                    foreach (SheetDefinition sd in deckType.Sheets)
                        sheets.Add(new()
                        {
                            ID = sd.ID,
                            URL = sd.URL,
                            MaxValue = sd.MaxValue
                        });
                return new GetSessionResponseDTO
                {
                    ID = ss.ID,
                    Title = ss.Title,
                    Stage = ss.Stage,
                    Instructions = ss.Instructions,
                    DeckTypeID = ss.DeckTypeID,
                    DeckType = new()
                    {
                        ID = deckType.ID,
                        Name = deckType.Name,
                        Description = deckType.Description,
                        Preview = (deckType.Preview is null) ? null : new()
                        {
                            URL = deckType.Preview.URL,
                            Height = deckType.Preview.Height,
                            Width = deckType.Preview.Width
                        },
                        Cards = cards,
                        Sheets = sheets
                    },
                    NoHalfPoint = ss.NoHalfPoint,
                    NoZeroPoint = ss.NoZeroPoint,
                    LastActivity = ss.LastActivity,
                    TeamMembers = teamMembers
                };
            }
        }
        return NotFound();
    }

    // POST: api/ScrumSession/New/{token}
    /// <summary>
    /// Creates a new session.
    /// </summary>
    /// <param name="token">The organizer's token.</param>
    /// <param name="session">Session creation parameters.</param>
    /// <returns></returns>
    [HttpPost("New/{token:length(352)}")]
    public async Task<ActionResult<NewScrumSessionResponseDTO>> NewScrumSession(string token, object NewScrumSessionRequestDTO)
    {
        if (!_tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return Unauthorized();
        SessionOrganizer? so = await _context.Organizers.FirstOrDefaultAsync(s => s.Token == decryptedToken);
        if (so is null)
            return Unauthorized();
        throw new NotImplementedException();
    }

    // PUT: api/ScrumSession/SetInstruction/{token}
    /// <summary>
    /// Changes to the <see href="SessionStage.Instruction" /> stage and sets the instruction text.
    /// </summary>
    /// <param name="token">The session token.</param>
    /// <param name="instructions">Instruction text.</param>
    /// <returns></returns>
    [HttpPut("SetInstructions/{token:length(352)}")]
    public async Task<ActionResult<SetInstructionsResponseDTO>> SetInstructions(string token, string instructions)
    {
        if (!_tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return Unauthorized();
        ScrumSession? ss = await _context.Sessions.Include(s => s.TeamMembers).Include(s => s.Organizer).FirstOrDefaultAsync(s => s.Token == decryptedToken);
        if (ss is null)
            return NotFound();
        throw new NotImplementedException();
    }

    // PUT: api/ScrumSession/ShowCards
    /// <summary>
    /// Changes to the <see href="SessionStage.Show" /> stage.
    /// </summary>
    /// <param name="token">The session token.</param>
    /// <returns></returns>
    [HttpPut("ShowCards")]
    public async Task<ActionResult<ShowCardsResponseDTO>> ShowCards(string token)
    {
        if (!_tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return Unauthorized();
        ScrumSession? ss = await _context.Sessions.Include(s => s.TeamMembers).Include(s => s.Organizer).FirstOrDefaultAsync(s => s.Token == decryptedToken);
        if (ss is null)
            return NotFound();
        throw new NotImplementedException();
    }

    // POST: api/ScrumSession/Status
    /// <summary>
    /// Gets updated session information.
    /// </summary>
    /// <param name="statusRequest">Status request parameters.</param>
    /// <returns></returns>
    [HttpPost("Status")]
    public async Task<ActionResult<GetSessionUpdatesResponseDTO>> GetSessionUpdates(GetSessionUpdatesRequestDTO statusRequest)
    {
        if (!_tokenService.TryUnrotectToken(statusRequest.Token, out string? decryptedToken))
            return Unauthorized();
        ScrumSession? ss = await _context.Sessions.Include(s => s.TeamMembers).Include(s => s.Organizer).FirstOrDefaultAsync(s => s.Token == decryptedToken);
        if (ss is null)
            return NotFound();
        throw new NotImplementedException();
    }

    // DELETE: api/ScrumSession/{token}
    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="token">The session token.</param>
    /// <returns></returns>
    [HttpDelete("{token:length(352)}")]
    public async Task<IActionResult> DeleteSession(string token)
    {
        if (!_tokenService.TryUnrotectToken(token, out string? decryptedToken))
            return Unauthorized();
        ScrumSession? ss = await _context.Sessions.Include(s => s.TeamMembers).FirstOrDefaultAsync(s => s.Token == decryptedToken);
        if (ss is null)
            return NotFound();
        throw new NotImplementedException();
    }
}