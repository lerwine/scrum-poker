import { CardDefinition } from './card-definition';
import { SheetDefinition } from './sheet-definition';
import { ImageFileDimensions } from './image-file-dimensions';

export interface DeckType {
  id: number;
  name: string;
  description: string;
  preview: ImageFileDimensions;
  cards: CardDefinition[];
  sheets: SheetDefinition[];
}
