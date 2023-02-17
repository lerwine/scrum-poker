using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using ScrumPoker.WebApp.Models;
using ScrumPoker.WebApp.Models.DTO;
using ScrumPoker.WebApp.Services;

namespace ScrumPoker.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeckTypeController : ControllerBase
{
    private readonly DeckService _context;

    public DeckTypeController(DeckService context)
    {
        _context = context;
    }

    // GET: api/DeckType
    [HttpGet]
    public ActionResult<GetDeckTypesResponseDTO> GetDeckTypes() => new GetDeckTypesResponseDTO
    {
        DeckTypes = new(_context.DeckTypes.Select(dt => new DeckTypeItemDTO {
            ID = dt.ID,
            Name = dt.Name,
            Description = dt.Description,
            Preview = (dt.Preview is null) ? null : new()
            {
                URL = dt.Preview.URL,
                Width = dt.Preview.Width,
                Height = dt.Preview.Height
            }
        }).ToList())
    };

    // GET: api/DeckType/{id}
    [HttpGet("{id}")]
    public ActionResult<DeckTypeDetailDTO> GetDeckType(int id)
    {
        if (id < 0 || id >= _context.DeckTypes.Count)
            return NotFound();
        DeckType dt = _context.DeckTypes[id];
        Collection<CardDefinitionDTO> cards = new();
        Collection<SheetDefinitionDTO> sheets = new();
        if (dt.Cards is not null)
            foreach (CardDefinition dc in dt.Cards)
                cards.Add(new()
                {
                    ID = dc.ID,
                    Value = dc.Value,
                    Symbol = dc.Symbol,
                    Type = dc.Type,
                    BaseName = dc.BaseName
                });
        if (dt.Sheets is not null)
            foreach (OrderedSheetDefinition sd in dt.Sheets)
                sheets.Add(new()
                {
                    ID = sd.ID,
                    URL = sd.URL,
                    MaxValue = sd.MaxValue,
                    SheetNumber = sd.SheetNumber
                });
        return new DeckTypeDetailDTO()
        {
            ID = dt.ID,
            Name = dt.Name,
            Description = dt.Description,
            Preview = (dt.Preview is null) ? null : new()
            {
                URL = dt.Preview.URL,
                Width = dt.Preview.Width,
                Height = dt.Preview.Height,
            },
            Cards = cards,
            Sheets = sheets
        };
    }
}