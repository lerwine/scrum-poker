import { CardType } from './card-type';

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

export function getMinimalFibonacciCardDefinitions(): CardDefinition[]
{
  return MIN_Fibonacci_CARDS.concat(COMMON_CARDS2);
}

export function getFibonacciCardDefinitions(): CardDefinition[]
{
  return Fibonacci_CARDS.concat(COMMON_CARDS2);
}

export function getExtFibonacciCardDefinitions(): CardDefinition[]
{
  return EXT_Fibonacci_CARDS.concat(COMMON_CARDS2);
}

export function getPseudoFibonacciCardDefinitions(): CardDefinition[]
{
  return COMMON_CARDS1.concat([
    CARD_5_DEFINITION,
    CARD_8_DEFINITION,
    CARD_13_DEFINITION,
      { value: 20, symbol: "20", type: CardType.points, baseName: "Card-20" },
      { value: 40, symbol: "40", type: CardType.points, baseName: "Card-40" },
      { value: 100, symbol: "100", type: CardType.points, baseName: "Card-100" }
  ]).concat(COMMON_CARDS2);
}

export function getIntegralCardDefinitions(): CardDefinition[]
{
  return COMMON_CARDS1.concat([
    { value: 4, symbol: "4", type: CardType.points, baseName: "Card-4" },
    CARD_5_DEFINITION,
    { value: 6, symbol: "6", type: CardType.points, baseName: "Card-6" },
    { value: 7, symbol: "7", type: CardType.points, baseName: "Card-7" },
    CARD_8_DEFINITION,
    { value: 9, symbol: "9", type: CardType.points, baseName: "Card-9" },
    { value: 10, symbol: "10", type: CardType.points, baseName: "Card-10" }
  ]).concat(COMMON_CARDS2);
}

export interface CardDefinition {
  value: number;
  symbol: string;
  type: CardType;
  baseName: string;
}
