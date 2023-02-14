declare namespace dataEntities {
    /**
     * Defines card types.
     */
    export type CardType = "Ambiguous" | "Points" | "Unattainable" | "Abstain";

    /**
     * Defines the preview image for the current deck type.
     */
    export interface IDeckPreviewImage extends ISize {
        /**
         * The file name of the SVG file, relative to the assets subdirectory.
         */
        fileName: string;
    }
    
    /**
     * Defines a card within a Scrum Poker deck.
     */
    export interface ICardEntity {
        /**
         * The value of the card in points.
         */
        value: number;

        /**
         * The character symbol of the card or the string representation of its numeric value.
         */
        symbol: string;

        /**
         * The generalized card type.
         */
        type: CardType;

        /**
         * The base file name for the card. For each color of each card base name, there should be an SVG file in the assets folder with the file name baseName + '-' + color + '.svg'.
         */
        baseName: string;
    }
    
    /**
     * Defines a printable sheet of Scrum Poker cards for the current deck type.
     */
    export interface IPrintableSheetEntity {
        /**
         * The name of the SVG file, relative to the assets subdirectory.
         */
        fileName: string;

        /**
         * The maximum points value for the cards represented in the printable sheet.
         */
        maxValue: number;
    }
    
    /**
     * Represents a deck of Scrum Poker cards.
     */
    export interface IDeckTypeEntity extends INameAndDescription {
        /**
         * Defines the preview image for the current deck type.
         */
        previewImage: IDeckPreviewImage;

        /**
         * Defines the individual cards that make up the deck.
         */
        cards: ICardEntity[];

        /**
         * Defines the printable sheets of cards for the current deck type.
         */
        printableSheets: IPrintableSheetEntity[];
    }

    /**
     * Defines the Scum Poker deck and card types that are available.
     */
    export interface IDeckDefinitions {
        /**
         * Defines card colors. For each color of each card base name, there should be an SVG file in the assets folder with the file name baseName + '-' + color + '.svg'
         */
        deckColors: string[];

        /**
         * Defines card deck types.
         */
        deckTypes: IDeckTypeEntity[];
    }
}