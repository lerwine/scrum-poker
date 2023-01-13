const COMMON_CARD_SHEETS: SheetDefinition[] = [
  { fileName: "LargeCards-1.svg", maxValue: 5 },
  { fileName: "LargeCards-2.svg", maxValue: 5 },
  { fileName: "LargeCards-3.svg", maxValue: 5 },
  { fileName: "LargeCards-4.svg", maxValue: 5 },
];

const FIBBONACCI_SHEET_1: SheetDefinition = { fileName: "LargeCards-Fibonacci-1.svg", maxValue: 13 };

export function getMinimalFibonacciSheetDefinitions() {
  return COMMON_CARD_SHEETS.concat([
    FIBBONACCI_SHEET_1,
    { fileName: "LargeCards-Fibonacci-2.svg", maxValue: 34 }
  ]);
}

export function getFibonacciSheetDefinitions() {
  return getMinimalFibonacciSheetDefinitions().concat([
    { fileName: "LargeCards-Fibonacci-3.svg", maxValue: 34 }
  ]);
}

export function getExtFibonacciSheetDefinitions() {
  return getFibonacciSheetDefinitions().concat([
    { fileName: "LargeCards-Fibonacci-4.svg", maxValue: 233 }
  ]);
}

export function getPseudoFibonacciSheetDefinitions() {
  return COMMON_CARD_SHEETS.concat([
    FIBBONACCI_SHEET_1,
      { fileName: "LargeCards-SimplifiedAgile-1.svg", maxValue: 100 },
      { fileName: "LargeCards-SimplifiedAgile-2.svg", maxValue: 100 }
    ]);
}

export function getIntegralFibonacciSheetDefinitions() {
  return COMMON_CARD_SHEETS.concat([
    { fileName: "LargeCards-TenScale-1.svg", maxValue: 10 },
    { fileName: "LargeCards-TenScale-2.svg", maxValue: 10 },
    { fileName: "LargeCards-TenScale-3.svg", maxValue: 10 }
  ]);
}

export interface SheetDefinition {
  fileName: string;
  maxValue: number;
}
