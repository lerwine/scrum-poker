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
    
    export interface ICardDescription {
        /**
         * The description for the card.
         */
        text: string;

        /**
         * The truncated description for the text.
         */
        truncatedText: string;

        /**
         * The brief details in italics which follow the description text without a line break.
         */
        briefDetails?: string;

        /**
         * THe full details in italics which appear under the description text.
         */
        fullDetails?: string;
    }

    export interface ICardBase {
        /**
         * The character symbol of the card or the string representation of its numeric value.
         */
        symbol: string;

        /**
         * The generalized card type.
         */
        type: CardType;

        /**
         * Title text for the card.
         * @TODO: Add to definition
         */
        title: string;

        /**
         * Shortened title text for the card.
         * @TODO: Add to definition
         */
        shortTitle?: string;
    }

    /**
     * Defines a card within a Scrum Poker deck.
     */
    export interface ICardEntity extends ICardBase {

        /**
         * Descriptive information for the card.
         */
        description?: ICardDescription;

        /**
         * @TODO: Make this obsolete
         */
        baseName: string;
    }

    /**
     * Cards whose symbol can be represented with text.
     * Infinity, Q
     */
    export interface ISimpleCardEntity extends ICardEntity {
        /**
         * The value of the card in points or undefined for zero.
         */
        value?: number;

        /**
         * The y-coordinate for the middle symbol or undefined for 115.
         */
        middleSymbolTop?: number;

        /**
         * The font size in pixels for the top and bottom symbol text or undefined for 20.
         */
        smallSymbolFontSize?: number;

        /**
         * The font size in pixels for the large center symbol text or undefined for 92.
         */
        largeSymbolFontSize?: number;
    }

    /**
     * Cards whose symbol is rendered with a path.
     * Abstain
     */
    export interface IPathCardEntity extends ICardEntity {
        /**
         * The graphics path data for the upper symbol.
         */
        upperSymbolPath: string;
        
        /**
         * The graphics path data for the center symbol.
         */
        middleSymbolPath: string;
        
        /**
         * The graphics path data for the lower symbol.
         */
        lowerSymbolPath: string;
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
        cards: (ISimpleCardEntity | IPathCardEntity)[];

        /**
         * Defines the printable sheets of cards for the current deck type.
         */
        printableSheets: IPrintableSheetEntity[];
    }

    export interface IDeckColor {
        name: string;
        fill: string;
        stroke: string;
    }
    /**
     * Defines the Scum Poker deck and card types that are available.
     */
    export interface IDeckDefinitions {
        /**
         * Defines card colors. For each color of each card base name, there should be an SVG file in the assets folder with the file name baseName + '-' + color + '.svg'
         */
        deckColors: IDeckColor[];

        /**
         * Defines card deck types.
         */
        deckTypes: IDeckTypeEntity[];
    }
}