import { CardDefinition } from './card-definition';
import { SheetDefinition } from './sheet-definition';
import { ImageFileDimensions } from './image-file-dimensions';
import { getMinimalFibonacciCardDefinitions, getFibonacciCardDefinitions,
  getExtFibonacciCardDefinitions, getPseudoFibonacciCardDefinitions, getIntegralCardDefinitions } from './card-definition';
import { getMinimalFibonacciSheetDefinitions, getFibonacciSheetDefinitions,
  getExtFibonacciSheetDefinitions, getPseudoFibonacciSheetDefinitions, getIntegralFibonacciSheetDefinitions } from './sheet-definition';

export function getDeckTypes(): DeckType[] {
  return [
    {
      name: "Minimal Fibonacci",
      preview: { uri: "MinimalFibonacci-Preview.svg", width: 210, height: 166 },
      description: "Fibonacci sequence values up to 34.",
      cards: getMinimalFibonacciCardDefinitions(),
      sheets: getMinimalFibonacciSheetDefinitions()
    },
    {
      name: "Fibonacci",
      preview: { uri: "Fibonacci-Preview.svg", width: 210, height: 176 },
      description: "Fibonacci sequence values up to 89.",
      cards: getFibonacciCardDefinitions(),
      sheets: getFibonacciSheetDefinitions()
    },
    {
        name: "Extended Fibonacci",
        preview: { uri: "ExtFibonacci-Preview.svg", width: 210, height: 187 },
        description: "Fibonacci sequence values up to 233.",
        cards: getExtFibonacciCardDefinitions(),
        sheets: getExtFibonacciSheetDefinitions()
    },
    {
        name: "Pseudo-Fibonacci",
        preview: { uri: "PseudoFibonacci-Preview.svg", width: 210, height: 165 },
        description: "Estimation values up to 100, similar to Fibonacci sequence.",
        cards: getPseudoFibonacciCardDefinitions(),
        sheets: getPseudoFibonacciSheetDefinitions()
    },
    {
        name: "Integral",
        preview: { uri: "Integral-Preview.svg", width: 210, height: 176 },
        description: "Estimation scale values from 1 to 10.",
        cards: getIntegralCardDefinitions(),
        sheets: getIntegralFibonacciSheetDefinitions()
    }
  ];
}

export interface DeckType {
  name: string;
  description: string;
  preview: ImageFileDimensions;
  cards: CardDefinition[];
  sheets: SheetDefinition[];
}
