import { CardDefinition } from './card-definition';
import { ImageFileDimensions } from './image-file-dimensions';
import { OrderedSheetDefinition } from './ordered-sheet-definition';

export interface DeckType {
  id: number;
  name: string;
  description: string;
  preview: ImageFileDimensions;
  cards: CardDefinition[];
  sheets: OrderedSheetDefinition[];
}
