using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ScrumPoker.WebApp.Models;

namespace ScrumPoker.WebApp.Services
{
    public class DeckService
    {
        public ReadOnlyCollection<DeckType> DeckTypes { get; }
        public ReadOnlyCollection<CardDefinition> Cards { get; }
        public ReadOnlyCollection<SheetDefinition> Sheets { get; }
        
        public DeckService(IOptions<ScrumPokerAppSettings> settings, ILogger<DeckService> logger)
        {
            Collection<SheetDefinition> sheetDefinitions = new();
            Collection<CardDefinition> cardDefinitions = new();
            Collection<DeckType> deckTypes = new();
            if (settings.Value.DeckTypes is null || settings.Value.DeckTypes.Count == 0)
                logger.LogCritical("No deck types specified in settings");
            else if (settings.Value.Cards is null || settings.Value.Cards.Count == 0)
                logger.LogCritical("No cards specified in settings");
            else
            {
                if (settings.Value.Sheets is not null)
                    for (int id = 0; id < settings.Value.Sheets.Count; id++)
                    {
                        SheetSettings ss = settings.Value.Sheets[id];
                        sheetDefinitions.Add(new()
                        {
                            ID = id,
                            URL = ss.URL,
                            MaxValue = ss.MaxValue
                        });
                    }
                for (int id = 0; id < settings.Value.Cards.Count; id++)
                {
                    CardSettings cs = settings.Value.Cards[id];
                    if (Enum.TryParse<CardType>(cs.Type, true, out CardType cardType))
                        cardDefinitions.Add(new()
                        {
                            ID = id,
                            Value = cs.Value,
                            Symbol = cs.Symbol,
                            Type = cardType,
                            BaseName = cs.BaseName
                        });
                    else
                        logger.LogCritical("Unknown card type {type} at index {index}.", cs.Type, id);
                }
                for (int id = 0; id < settings.Value.DeckTypes.Count; id++)
                {
                    DeckSettings ds = settings.Value.DeckTypes[id];
                    if (ds.Preview is null)
                        logger.LogCritical("Deck type has no preview at index {index}.", id);
                    else if (ds.Cards is null || ds.Cards.Length == 0)
                        logger.LogCritical("Deck type has no cards at index {index}.", id);
                    else
                    {
                        Collection<CardDefinition> cards = new();
                        Collection<OrderedSheetDefinition> sheets = new();
                        foreach (int i in ds.Cards)
                        {
                            if (i < 0 || i >= cardDefinitions.Count)
                                logger.LogCritical("Card with id {id} not found for deck type at index {index}.", id, i);
                            else
                                cards.Add(cardDefinitions[i]);
                        }
                        if (ds.Sheets is not null)
                            foreach (int i in ds.Sheets)
                            {
                                if (i < 0 || i >= sheetDefinitions.Count)
                                    logger.LogCritical("Sheet with id {id} not found for deck type at index {index}.", id, i);
                                else
                                {
                                    SheetDefinition sd = sheetDefinitions[i];
                                    sheets.Add(new()
                                    {
                                        ID = sd.ID,
                                        URL = sd.URL,
                                        MaxValue = sd.MaxValue,
                                        SheetNumber = sheets.Count + 1
                                    });
                                }
                            }
                        deckTypes.Add(new()
                        {
                            ID = id,
                            Name = ds.Name,
                            Description = ds.Description,
                            Preview = new()
                            {
                                URL = ds.Preview.URL,
                                Height = ds.Preview.Height,
                                Width = ds.Preview.Width
                            },
                            Cards = cards,
                            Sheets = sheets
                        });
                    }
                }
            }
            Sheets = new(sheetDefinitions);
            Cards = new(cardDefinitions);
            DeckTypes = new(deckTypes);
        }
    }
}