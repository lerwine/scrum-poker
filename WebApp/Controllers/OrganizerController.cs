using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrumPoker.WebApp.Models;
using ScrumPoker.WebApp.Models.DTO;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizerController : ControllerBase
{
    private readonly ScrumPokerContext _context;
    private readonly DeckService _deckService;
    private readonly ILogger<OrganizerController> _logger;
    private readonly SessionTokenService _tokenService;

    public OrganizerController(ScrumPokerContext context, DeckService deckService, ILogger<OrganizerController> logger, SessionTokenService tokenService)
    {
        _context = context;
        _deckService = deckService;
        _logger = logger;
        _tokenService = tokenService;
    }

    // GET: api/Organizer/{token}
    /// <summary>
    /// Get all organizers
    /// </summary>
    /// <param name="token">The administrative token.</param>
    /// <returns></returns>
    [HttpGet("{token:length(352)}")]
    public async Task<ActionResult<GetOrganizersResponseDTO>> GetOrganizers(string token)
    {
        if (!_tokenService.ValidateAdminTokenString(token))
            return Unauthorized();
        await using IAsyncEnumerator<SessionOrganizer> asyncEnumerator = _context.Organizers.GetAsyncEnumerator();
        Collection<OrganizerAdminItemDTO> organizers = new();
        while (await asyncEnumerator.MoveNextAsync())
        {
            SessionOrganizer o = asyncEnumerator.Current;
            organizers.Add(new(){
                ID = o.ID,
                Token = _tokenService.ToProtectedToken(o.Token),
                Name = o.Name,
                EmailAddress = o.EmailAddress,
                LastActivity = o.LastActivity
            });
        }
        return new GetOrganizersResponseDTO { Organizers = organizers };
    }

    // GET: api/Organizer/{token}/{id}
    /// <summary>
    /// Get organizer by ID
    /// </summary>
    /// <param name="token">The administrative token.</param>
    /// <param name="id">The unique identifier of the organizer record.</param>
    /// <returns></returns>
    [HttpGet("{token:length(352)}/{id}")]
    public async Task<ActionResult<OrganizerDetailsDTO>> GetOrganizer(string token, Guid id)
    {
        if (!_tokenService.ValidateAdminTokenString(token))
            return Unauthorized();
        SessionOrganizer? org = await _context.Organizers.Include(o => o.Sessions).FirstOrDefaultAsync(o => o.ID == id);
        if (org is null)
            return NotFound();
        Collection<ScrumSessionItemDTO> sessions = new();
        if (org.Sessions is not null)
            foreach (ScrumSession s in org.Sessions)
                sessions.Add(new()
                {
                    ID = s.ID,
                    Token = _tokenService.ToProtectedToken(s.Token),
                    Title = s.Title,
                    Stage = s.Stage,
                    Instructions = s.Instructions,
                    DeckTypeID = s.DeckTypeID,
                    NoHalfPoint = s.NoHalfPoint,
                    NoZeroPoint = s.NoZeroPoint,
                    LastActivity = s.LastActivity
                });
        return new OrganizerDetailsDTO
        {
            ID = org.ID,
            Token = _tokenService.ToProtectedToken(org.Token),
            Name = org.Name,
            EmailAddress = org.EmailAddress,
            LastActivity = org.LastActivity,
            Sessions = sessions
        };
    }

    // POST: api/Organizer/{token}
    /// <summary>
    /// Adds a new organizer
    /// </summary>
    /// <param name="token">The administrative token.</param>
    /// <param name="organizer">Organizer record values.</param>
    /// <returns></returns>
    [HttpPost("{token:length(352)")]
    public async Task<ActionResult<NewOrganizerResponseDTO>> NewOrganizer(string token, NewOrganizerRequestDTO organizer)
    {
        if (!_tokenService.ValidateAdminTokenString(token))
            return Unauthorized();
        SessionOrganizer so = new()
        {
            ID = Guid.NewGuid(),
            Token = _tokenService.GenerateSessionToken(),
            Name = organizer.Name ?? organizer.EmailAddress ?? "(unnamed)",
            EmailAddress = organizer.EmailAddress ?? "",
            LastActivity = DateTime.Now
        };
        _context.Organizers.Add(so);
        await _context.SaveChangesAsync();
        return new NewOrganizerResponseDTO
        {
            ID = so.ID,
            Token = _tokenService.ToProtectedToken(so.Token),
            Name = so.Name,
            EmailAddress = so.EmailAddress,
            LastActivity = so.LastActivity
        };
    }

    // PUT: api/Organizer/{token}
    /// <summary>
    /// Update organizer values
    /// </summary>
    /// <param name="token">The administrative token.</param>
    /// <param name="organizer">Organizer record values.</param>
    /// <returns></returns>
    [HttpPut("{token:length(352)")]
    public async Task<ActionResult> UpdateOrganizer(string token, UpdateOrganizerRequestDTO organizer)
    {
        if (!_tokenService.ValidateAdminTokenString(token))
            return Unauthorized();
        Guid id = organizer.ID;
        SessionOrganizer? org = await _context.Organizers.Include(o => o.Sessions).FirstOrDefaultAsync(o => o.ID == id);
        if (org is null)
            return NotFound();
        if (organizer.EmailAddress is not null && organizer.EmailAddress != org.EmailAddress)
        {
            org.EmailAddress = organizer.EmailAddress;
            if (organizer.Name is not null)
                org.Name = organizer.Name;
        }
        else if (organizer.Name is not null && organizer.Name != org.Name)
            org.Name = organizer.Name;
        else
            return NoContent();
        _context.Organizers.Update(org);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Organizer
    /// <summary>
    /// Delete organizer record
    /// </summary>
    /// <param name="token">The administrative token.</param>
    /// <param name="id">The unique identifier of the organizer record.</param>
    /// <returns></returns>
    [HttpDelete("{token:length(352)}/{id}")]
    public async Task<ActionResult> DeleteOrganizer(string token, Guid id)
    {
        if (!_tokenService.ValidateAdminTokenString(token))
            return Unauthorized();
        SessionOrganizer? org = await _context.Organizers.Include(o => o.Sessions).FirstOrDefaultAsync(o => o.ID == id);
        if (org is null)
            return NotFound();
        _context.Organizers.Remove(org);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
