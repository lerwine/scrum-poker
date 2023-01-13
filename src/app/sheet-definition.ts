const COMMON_CARD_SHEETS: SheetDefinition[] = [
  { uri: "LargeCards-1.svg", maxValue: 5 },
  { uri: "LargeCards-2.svg", maxValue: 5 },
  { uri: "LargeCards-3.svg", maxValue: 5 },
  { uri: "LargeCards-4.svg", maxValue: 5 },
];

const FIBBONACCI_SHEET_1: SheetDefinition = { uri: "LargeCards-Fibonacci-1.svg", maxValue: 13 };

export function getMinimalFibonacciSheetDefinitions() {
  return COMMON_CARD_SHEETS.concat([
    FIBBONACCI_SHEET_1,
    { uri: "LargeCards-Fibonacci-2.svg", maxValue: 34 }
  ]);
}

export function getFibonacciSheetDefinitions() {
  return getMinimalFibonacciSheetDefinitions().concat([
    { uri: "LargeCards-Fibonacci-3.svg", maxValue: 34 }
  ]);
}

export function getExtFibonacciSheetDefinitions() {
  return getFibonacciSheetDefinitions().concat([
    { uri: "LargeCards-Fibonacci-4.svg", maxValue: 233 }
  ]);
}

export function getPseudoFibonacciSheetDefinitions() {
  return COMMON_CARD_SHEETS.concat([
    FIBBONACCI_SHEET_1,
      { uri: "LargeCards-SimplifiedAgile-1.svg", maxValue: 100 },
      { uri: "LargeCards-SimplifiedAgile-2.svg", maxValue: 100 }
    ]);
}

export function getIntegralFibonacciSheetDefinitions() {
  return COMMON_CARD_SHEETS.concat([
    { uri: "LargeCards-TenScale-1.svg", maxValue: 10 },
    { uri: "LargeCards-TenScale-2.svg", maxValue: 10 },
    { uri: "LargeCards-TenScale-3.svg", maxValue: 10 }
  ]);
}

export interface SheetDefinition {
  uri: string;
  maxValue: number;
}
