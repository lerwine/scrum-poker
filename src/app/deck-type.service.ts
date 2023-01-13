import { Injectable } from '@angular/core';
import { DeckType } from './deck-type';
import { CardDefinition } from './card-definition';
import { CardType } from './card-type';
import { SheetDefinition } from './sheet-definition';

const CARD_5_DEFINITION: CardDefinition = { value: 5, symbol: "5", type: CardType.points, baseName: "Card-5" };
const CARD_8_DEFINITION: CardDefinition = { value: 8, symbol: "8", type: CardType.points, baseName: "Card-8" };
const COMMON_CARDS1: CardDefinition[] = [
  { value: 0, symbol: "?", type: CardType.ambiguous, baseName: "Card-Q" },
  { value: 0, symbol: "0", type: CardType.points, baseName: "Card-0" },
  { value: 0.5, symbol: "½", type: CardType.points, baseName: "Card-Half" },
  { value: 1, symbol: "1", type: CardType.points, baseName: "Card-1" },
  { value: 2, symbol: "2", type: CardType.points, baseName: "Card-2" },
  { value: 3, symbol: "3", type: CardType.points, baseName: "Card-3" }
];
const CARD_13_DEFINITION: CardDefinition = { value: 13, symbol: "13", type: CardType.points, baseName: "Card-13" };
const MIN_Fibonacci_CARDS: CardDefinition[] = COMMON_CARDS1.concat([
  CARD_5_DEFINITION,
  CARD_8_DEFINITION,
  CARD_13_DEFINITION,
  { value: 21, symbol: "21", type: CardType.points, baseName: "Card-21" },
  { value: 34, symbol: "34", type: CardType.points, baseName: "Card-34" },
]);
const COMMON_CARDS2: CardDefinition[] = [
  { value: 0, symbol: "∞", type: CardType.unattainable, baseName: "Card-Infinity" },
  { value: 0, symbol: "!", type: CardType.abstain, baseName: "Card-Abstain" }
];
const Fibonacci_CARDS: CardDefinition[] = MIN_Fibonacci_CARDS.concat([
  { value: 55, symbol: "55", type: CardType.points, baseName: "Card-55" },
  { value: 89, symbol: "89", type: CardType.points, baseName: "Card-89" }
]);
const EXT_Fibonacci_CARDS: CardDefinition[] = MIN_Fibonacci_CARDS.concat([
  { value: 144, symbol: "144", type: CardType.points, baseName: "Card-144" },
  { value: 233, symbol: "233", type: CardType.points, baseName: "Card-233" }
]);
const COMMON_CARD_SHEETS: SheetDefinition[] = [
  { uri: "LargeCards-1.svg", maxValue: 5 },
  { uri: "LargeCards-2.svg", maxValue: 5 },
  { uri: "LargeCards-3.svg", maxValue: 5 },
  { uri: "LargeCards-4.svg", maxValue: 5 },
];

const FIBBONACCI_SHEET_1: SheetDefinition = { uri: "LargeCards-Fibonacci-1.svg", maxValue: 13 };

const MINIMAL_FIBONACCI_SHEET_DEFINITIONS: SheetDefinition[] = COMMON_CARD_SHEETS.concat([
  FIBBONACCI_SHEET_1,
  { uri: "LargeCards-Fibonacci-2.svg", maxValue: 34 }
]);

const FIBONACCI_SHEET_DEFINITIONS: SheetDefinition[] = MINIMAL_FIBONACCI_SHEET_DEFINITIONS.concat([
  { uri: "LargeCards-Fibonacci-3.svg", maxValue: 34 }
]);

const DECK_TYPES: DeckType[] = [
  {
    id: 1,
    name: "Minimal Fibonacci",
    preview: { uri: "MinimalFibonacci-Preview.svg", width: 210, height: 166 },
    description: "Fibonacci sequence values up to 34.",
    cards:MIN_Fibonacci_CARDS.concat(COMMON_CARDS2),
    sheets: MINIMAL_FIBONACCI_SHEET_DEFINITIONS
  },
  {
    id: 2,
    name: "Fibonacci",
    preview: { uri: "Fibonacci-Preview.svg", width: 210, height: 176 },
    description: "Fibonacci sequence values up to 89.",
    cards: Fibonacci_CARDS.concat(COMMON_CARDS2),
    sheets: FIBONACCI_SHEET_DEFINITIONS
  },
  {
    id: 3,
    name: "Extended Fibonacci",
    preview: { uri: "ExtFibonacci-Preview.svg", width: 210, height: 187 },
    description: "Fibonacci sequence values up to 233.",
    cards: EXT_Fibonacci_CARDS.concat(COMMON_CARDS2),
    sheets: FIBONACCI_SHEET_DEFINITIONS.concat([
      { uri: "LargeCards-Fibonacci-4.svg", maxValue: 233 }
    ])
  },
  {
    id: 4,
    name: "Alternative Exponential",
    preview: { uri: "PseudoFibonacci-Preview.svg", width: 210, height: 165 },
    description: "Estimation values up to 100 using exponential sequence values.",
    cards: COMMON_CARDS1.concat([
      CARD_5_DEFINITION,
      CARD_8_DEFINITION,
      CARD_13_DEFINITION,
        { value: 20, symbol: "20", type: CardType.points, baseName: "Card-20" },
        { value: 40, symbol: "40", type: CardType.points, baseName: "Card-40" },
        { value: 100, symbol: "100", type: CardType.points, baseName: "Card-100" }
    ]).concat(COMMON_CARDS2),
    sheets: COMMON_CARD_SHEETS.concat([
      FIBBONACCI_SHEET_1,
        { uri: "LargeCards-SimplifiedAgile-1.svg", maxValue: 100 },
        { uri: "LargeCards-SimplifiedAgile-2.svg", maxValue: 100 }
      ])
  },
  {
    id: 5,
    name: "Integral",
    preview: { uri: "Integral-Preview.svg", width: 210, height: 176 },
    description: "Absolute estimation values from up to 10.",
    cards: COMMON_CARDS1.concat([
      { value: 4, symbol: "4", type: CardType.points, baseName: "Card-4" },
      CARD_5_DEFINITION,
      { value: 6, symbol: "6", type: CardType.points, baseName: "Card-6" },
      { value: 7, symbol: "7", type: CardType.points, baseName: "Card-7" },
      CARD_8_DEFINITION,
      { value: 9, symbol: "9", type: CardType.points, baseName: "Card-9" },
      { value: 10, symbol: "10", type: CardType.points, baseName: "Card-10" }
    ]).concat(COMMON_CARDS2),
    sheets: COMMON_CARD_SHEETS.concat([
      { uri: "LargeCards-TenScale-1.svg", maxValue: 10 },
      { uri: "LargeCards-TenScale-2.svg", maxValue: 10 },
      { uri: "LargeCards-TenScale-3.svg", maxValue: 10 }
    ])
  }
];

@Injectable({
  providedIn: 'root'
})
export class DeckTypeService {

  constructor() { }

  getDeckType(id: number): DeckType | undefined {
    var deckTypes = DECK_TYPES.filter(deckType => deckType.id == id);
    if (deckTypes.length == 0)
      return;
    return deckTypes[0];
  }

  getDeckTypes(): DeckType[] {
    return DECK_TYPES;
  }
}
