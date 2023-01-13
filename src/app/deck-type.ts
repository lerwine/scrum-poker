import { CardDefinition } from './card-definition';
import { SheetDefinition } from './sheet-definition';
import { getMinimalFibonacciCardDefinitions, getFibonacciCardDefinitions,
  getExtFibonacciCardDefinitions, getPseudoFibonacciCardDefinitions, getIntegralCardDefinitions } from './card-definition';
import { getMinimalFibonacciSheetDefinitions, getFibonacciSheetDefinitions,
  getExtFibonacciSheetDefinitions, getPseudoFibonacciSheetDefinitions, getIntegralFibonacciSheetDefinitions } from './sheet-definition';

export function getDeckTypes(): DeckType[] {
  return [
    {
      name: "Minimal Fibonacci",
      preview: "MinimalFibonacci-Preview.svg",
      description: "Fibonacci sequence values up to 34.",
      cards: getMinimalFibonacciCardDefinitions(),
      sheets: getMinimalFibonacciSheetDefinitions()
    },
    {
      name: "Fibonacci",
      preview: "Fibonacci-Preview.svg",
      description: "Fibonacci sequence values up to 89.",
      cards: getFibonacciCardDefinitions(),
      sheets: getFibonacciSheetDefinitions()
    },
    {
        name: "Extended Fibonacci",
        preview: "ExtFibonacci-Preview.svg",
        description: "Fibonacci sequence values up to 233.",
        cards: getExtFibonacciCardDefinitions(),
        sheets: getExtFibonacciSheetDefinitions()
    },
    {
        name: "Pseudo-Fibonacci",
        preview: "PseudoFibonacci-Preview.svg",
        description: "Estimation values up to 100, similar to Fibonacci sequence.",
        cards: getPseudoFibonacciCardDefinitions(),
        sheets: getPseudoFibonacciSheetDefinitions()
    },
    {
        name: "Integral",
        preview: "Integral-Preview.svg",
        description: "Estimation scale values from 1 to 10.",
        cards: getIntegralCardDefinitions(),
        sheets: getIntegralFibonacciSheetDefinitions()
    }
  ];
}

export interface DeckType {
  name: string;
  description: string;
  preview: string;
  cards: CardDefinition[];
  sheets: SheetDefinition[];
}
