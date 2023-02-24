declare namespace dataEntities {
    /**
     * Defines card types.
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/cardType
     */
    export type CardType = "Ambiguous" | "Points" | "Unattainable" | "Abstain";

    /**
     * Defines the preview image for the current deck type.
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/deckPreviewImage
     */
    export interface IDeckPreviewImage extends ISize {
        /**
         * The file name of the SVG file, relative to the assets subdirectory.
         */
        fileName: string;
    }
    
    /**
     * Card description information.
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/cardDescription
     */
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

    /**
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/textAndFont
     */
    export interface ITextAndFont {
        text: string;
        font: string;
    }

    /**
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/textAndTruncated
     */
    export interface ITextAndTruncated {
        text: string;
        truncated: string;
    }

    /**
     * Defines a card within a Scrum Poker deck.
     */
    export interface ICardEntity {
        /**
         * The character symbol of the card or the string representation of its numeric value.
         */
        symbol: string | ITextAndFont;

        /**
         * The generalized card type.
         */
        type: CardType;

        /**
         * Title text for the card.
         * @TODO: Add to definition
         */
        title: string | ITextAndTruncated;

        /**
         * Descriptive information for the card.
         */
        description?: ICardDescription;

        /**
         * @TODO: Make this obsolete
         * @todo: This is no longer used
         */
        baseName?: string;
    }

    /**
     * Cards whose symbol can be represented with text.
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/simpleCardEntity
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
         * The font size in pixels for the large center symbol text or undefined for 96.
         */
        largeSymbolFontSize?: number;
    }

    /**
     * Cards whose symbol is rendered with a path.
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/pathCardEntity
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
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/printableSheetEntity
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
     * @todo Convert to schema at wwwRoot\assets\card-configuration.schema.json
     * JSON Schema: deck-definitions.schema.json#/$defs/deckTypeEntity
     */
    export interface IDeckTypeEntity extends INameAndDescription {
        /**
         * Defines the preview image for the current deck type.
         */
        previewImage: IDeckPreviewImage;

        /**
         * Symbols for the individual cards that make up the deck.
         */
        cards: string[];

        /**
         * Defines the printable sheets of cards for the current deck type.
         */
        printableSheets: IPrintableSheetEntity[];
    }

    /**
     * Defines a deck color scheme.
     * JSON Schema: card-configuration.schema.json#/$defs/cardColorEntity
     */
    export interface IDeckColor {
        /**
         * The identifier for the color scheme.
         */
        name: string;

        /**
         * The fill color.
         */
        fill: string;
        
        /**
         * The stroke color.
         */
        stroke: string;
        
        /**
         * The text color.
         */
        text: string;
    }

    export interface IColorSchema {
        /**
         * The identifier for the color scheme.
         */
        name: string;
        
        /**
         * The fill color for the "voting" card.
         */
        votingFill: string;
        
        /**
         * The stroke color for the "voting" card.
         */
        votingStroke: string;
        
        /**
         * The text color for the "voting" card.
         */
        votingText: string;

        cardColors: IDeckColor[];
    }
    
    /**
     * Defines the Scum Poker deck and card types that are available.
     * JSON Schema: deck-definitions.schema.json
     */
    export interface IDeckDefinitions {
        /**
         * Color scheme for "voting" pseudo-card.
         * @todo Obsolete: Use colorSchemas
         */
        votingCard: {
            fill: string;
            stroke: string;
            text: string;
        };

        /**
         * Color schemes for cards.
         * JSON Schema: deck-definitions.schema.json#/$defs/colorSchemaEntity
         */
        colorSchemas: IColorSchema[];

        /**
         * Defines card colors schemes.
         * @todo Obsolete: Use colorSchemas
         */
        deckColors: IDeckColor[];

        /**
         * Defines card deck types.
         */
        deckTypes: IDeckTypeEntity[];

        /**
         * Defines the individual cards that can be used in decks.
         */
        cards: (ISimpleCardEntity | IPathCardEntity)[];
    }
}