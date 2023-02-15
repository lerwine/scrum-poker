declare namespace deckTypesService {
    export interface IDeckTypeListItem extends INameAndDescription {
        id: number;
        previewUrl: string;
        width: number;
        height: number;
    }
    
    export interface ICardItem extends dataEntities.ICardBase {
        /**
         * The unique identifier of the card within the parent deck.
         */
        id: number;

        /**
         * The value of the card in points.
         */
        value: number;

        /**
         * The y-coordinate for the middle symbol.
         */
        middleSymbolTop?: number;

        /**
         * The font size in pixels for the top and bottom symbol text.
         */
        smallSymbolFontSize?: number;

        /**
         * The font size in pixels for the large center symbol text.
         */
        largeSymbolFontSize?: number;

        /**
         * The graphics path data for the upper symbol.
         */
        upperSymbolPath?: string;
        
        /**
         * The graphics path data for the center symbol.
         */
        middleSymbolPath?: string;
        
        /**
         * The graphics path data for the lower symbol.
         */
        lowerSymbolPath?: string;

        /**
         * The description for the card.
         */
        description?: string;

        /**
         * The truncated description for the text.
         */
        truncatedDescription?: string;

        /**
         * The brief details in italics which follow the description text without a line break.
         */
        briefDetails?: string;

        /**
         * THe full details in italics which appear under the description text.
         */
        fullDetails?: string;
    }

    export interface IDeckDetails {
        name: string;
        description: string;
        previewImage: ISize & {
            url: string;
        };
        cards: ICardItem[];
    }

    export interface IDeckTypesServiceResult {
        promise: angular.IPromise<void>;
        getAllDeckTypes(): IDeckTypeListItem[];
        getDeck(id: number): IDeckDetails | undefined;
        getCards(deckId: number, color: string): IGetCardsResult | undefined;
        getDeckColors(): string[];
    }

    export interface IParticpantCard extends ICardItem {
        fillColor: string;
        strokeColor: string;
    }

    export interface IGetCardsResult {
        votingCardUrl: string;
        cards: IParticpantCard[];
    }
}